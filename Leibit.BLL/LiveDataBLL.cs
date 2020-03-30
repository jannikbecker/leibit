﻿using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Leibit.BLL
{
    public class LiveDataBLL : BLLBase
    {

        #region - Needs -
        private SettingsBLL m_SettingsBll;
        private CalculationBLL m_CalculationBll;
        private InitializationBLL m_InitializationBll;
        private XmlSerializer m_DelaySerializer;

        private string m_EstwOnlinePath;
        private object m_LockRefresh = new object();
        private object m_LockDataFilesPath = new object();
        #endregion

        #region - Ctor -
        public LiveDataBLL()
            : base()
        {
            m_DelaySerializer = new XmlSerializer(typeof(SharedDelay));
        }
        #endregion

        #region - Singletons -

        #region [SettingsBLL]
        private SettingsBLL SettingsBLL
        {
            get
            {
                if (m_SettingsBll == null)
                    m_SettingsBll = new SettingsBLL();

                return m_SettingsBll;
            }
        }
        #endregion

        #region [CalculationBLL]
        private CalculationBLL CalculationBLL
        {
            get
            {
                if (m_CalculationBll == null)
                    m_CalculationBll = new CalculationBLL();

                return m_CalculationBll;
            }
        }
        #endregion

        #region [InitializationBLL]
        private InitializationBLL InitializationBLL
        {
            get
            {
                if (m_InitializationBll == null)
                    m_InitializationBll = new InitializationBLL();

                return m_InitializationBll;
            }
        }
        #endregion

        #region [DataFilesPath]
        private string DataFilesPath
        {
            get
            {
                lock (m_LockDataFilesPath)
                {
                    if (m_EstwOnlinePath.IsNullOrEmpty())
                        m_EstwOnlinePath = __GetDataFilesPath();

                    return m_EstwOnlinePath;
                }
            }
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [RefreshLiveData]
        public OperationResult<bool> RefreshLiveData(Area area)
        {
            lock (m_LockRefresh)
            {
                try
                {
                    var Result = new OperationResult<bool>();
                    Result.Result = true;

                    var Options = new ParallelOptions();
                    Options.MaxDegreeOfParallelism = Environment.ProcessorCount;

                    Parallel.ForEach(area.ESTWs, Options, estw =>
                    {
                        try
                        {
                            __LoadLiveDataFromEstw(estw);
                            estw.IOExceptionCount = 0;
                        }
                        catch (IOException ex) when (estw.IOExceptionCount < 3)
                        {
                            estw.IOExceptionCount++;
                            Result.Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            Result.Result = false;
                            Result.Message = ex.Message;
                        }
                    });

                    // Load shared delay files that have been downloaded by ESTWonline.
                    __LoadSharedDelays(area);

                    var Estw = area.ESTWs.FirstOrDefault(e => e.Time != null);

                    if (Estw != null)
                    {
                        Parallel.ForEach(area.LiveTrains, Options, train =>
                            {
                                // Set departure times for trains that are gone.
                                if (train.Value.LastModified < Estw.Time.AddMinutes(-2))
                                {
                                    var CurrentSchedules = train.Value.Schedules.Where(s => s.LiveArrival != null && s.LiveDeparture == null);
                                    CurrentSchedules.ForEach(s => s.LiveDeparture = train.Value.LastModified);
                                }

                                // Delete train information that are older than 12 hours.
                                var LastSchedule = train.Value.Schedules.LastOrDefault(s => s.LiveArrival != null || s.LiveDeparture != null);

                                if (LastSchedule != null)
                                {
                                    var LastTime = LastSchedule.LiveDeparture == null ? LastSchedule.LiveArrival : LastSchedule.LiveDeparture;

                                    if (LastTime < Estw.Time.AddHours(-12))
                                    {
                                        TrainInformation tmp;
                                        area.LiveTrains.TryRemove(train.Value.Train.Number, out tmp);
                                    }
                                }
                            });
                    }

                    Result.Succeeded = true;
                    return Result;
                }
                catch (Exception ex)
                {
                    return new OperationResult<bool> { Message = ex.Message };
                }
            }
        }
        #endregion

        #region [JustifyDelay]
        public OperationResult<SharedDelay> JustifyDelay(DelayInfo delay)
        {
            try
            {
                if (delay.Reason.IsNullOrWhiteSpace())
                    return new OperationResult<SharedDelay> { Message = "Bitte Grund angeben!" };

                var SettingsResult = SettingsBLL.GetSettings();
                ValidateResult(SettingsResult);

                string EstwId = delay.Schedule.Schedule.Station.ESTW.Id;

                if (!SettingsResult.Result.Paths.ContainsKey(EstwId))
                    return new OperationResult<SharedDelay> { Message = String.Format("Pfad zu ESTW '{0}' nicht gefunden.", delay.Schedule.Schedule.Station.ESTW.Name), Succeeded = true };

                var Result = new OperationResult<SharedDelay>();

                var Shared = new SharedDelay(delay.Schedule.Train.Train.Number,
                    delay.Schedule.Schedule.Station.ShortSymbol,
                    delay.Schedule.Train.Schedules.Where(s => s.Schedule.Station.ShortSymbol == delay.Schedule.Schedule.Station.ShortSymbol).ToList().IndexOf(delay.Schedule),
                    delay.Minutes,
                    delay.Type,
                    delay.Reason);

                Shared.CausedBy = delay.CausedBy;

                var FilePath = Path.Combine(SettingsResult.Result.Paths[EstwId], Constants.SHARED_DELAY_FOLDER, String.Format(Constants.SHARED_DELAY_FILE_TEMPLATE, Shared.TrainNumber, Shared.StationShortSymbol));
                var FileInfo = new FileInfo(FilePath);

                if (!FileInfo.Directory.Exists)
                    return new OperationResult<SharedDelay> { Message = String.Format("ESTWonline-Verzeichnis von ESTW '{0}' nicht gefunden.", delay.Schedule.Schedule.Station.ESTW.Name) };

                using (var FileStream = FileInfo.Open(FileMode.Create))
                {
                    m_DelaySerializer.Serialize(FileStream, Shared);
                }

                Result.Result = Shared;
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<SharedDelay> { Message = ex.Message };
            }
        }
        #endregion

        #endregion

        #region - Private helpers -

        private void __LoadLiveDataFromEstw(ESTW estw)
        {
            if (estw == null || estw.DataFile.IsNullOrWhiteSpace())
                return;

            var FilePath = Path.Combine(DataFilesPath, estw.DataFile);

            if (!File.Exists(FilePath))
                return;

            using (var stream = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (var reader = new StreamReader(stream))
                {
                    // Parse time out of first line.
                    if (!reader.EndOfStream)
                    {
                        var FirstLine = reader.ReadLine();

                        if (FirstLine.Length >= 11)
                        {
                            string sHour = FirstLine.Substring(0, 2);
                            string sMinute = FirstLine.Substring(3, 2);
                            string sDay = FirstLine.Substring(9, 2);

                            int Hour, Minute;
                            eDaysOfService Day = LeibitTime.ParseSingleDay(sDay);

                            if (Day != eDaysOfService.None && Int32.TryParse(sHour, out Hour) && Int32.TryParse(sMinute, out Minute))
                                estw.Time = new LeibitTime(Day, Hour, Minute);
                        }
                    }

                    if (!estw.IsLoaded)
                    {
                        InitializationBLL.LoadESTW(estw);

                        foreach (var Train in estw.Area.LiveTrains.Values)
                            __RefreshLiveSchedules(Train, estw);
                    }

                    while (!reader.EndOfStream)
                    {
                        var Line = reader.ReadLine();
                        var Parts = Line.Split(' ');

                        if (Parts.Length != 6)
                            continue;

                        string BlockName = Parts[1].Trim('/');
                        string sTrainNumber = Parts[2].Trim('_');
                        string sDelay = Parts[3];
                        string sDirection = Parts[5];

                        if (sTrainNumber.Length > 5)
                            continue;

                        int TrainNumber, Delay;

                        if (!Int32.TryParse(sTrainNumber, out TrainNumber) || !Int32.TryParse(sDelay, out Delay) || (sDirection != "L" && sDirection != "R"))
                            continue;

                        TrainInformation Train;

                        if (estw.Area.LiveTrains.ContainsKey(TrainNumber))
                            Train = estw.Area.LiveTrains[TrainNumber];
                        else
                        {
                            Train = __CreateLiveTrainInformation(TrainNumber, estw);
                            Train = estw.Area.LiveTrains.GetOrAdd(TrainNumber, Train);
                        }

                        Train.Delay = Delay;
                        Train.Direction = sDirection == "L" ? eBlockDirection.Left : eBlockDirection.Right;

                        var Block = estw.Blocks.ContainsKey(BlockName) ? estw.Blocks[BlockName].FirstOrDefault(b => b.Direction == eBlockDirection.Both || b.Direction == Train.Direction) : null;
                        __RefreshTrainInformation(Train, Block, estw);
                    }
                }
            }

            //if (!Debugger.IsAttached)
            File.Delete(FilePath);
        }

        private string __GetDataFilesPath()
        {
            var SettingsResult = SettingsBLL.GetSettings();
            ValidateResult(SettingsResult);

            if (SettingsResult.Result.EstwOnlinePath == null)
                return String.Empty;

            var IniPath = Path.Combine(SettingsResult.Result.EstwOnlinePath, Constants.ESTWONLINE_SETTINGS_FILE);

            if (!File.Exists(IniPath))
                throw new OperationFailedException("Ini-Datei von ESTWonline nicht gefunden!");

            using (var reader = new StreamReader(IniPath))
            {
                while (!reader.EndOfStream)
                {
                    var Line = reader.ReadLine();

                    if (Line.StartsWith(String.Format("{0}=", Constants.ESTWONLINE_DIRECTORY_KEY), StringComparison.InvariantCultureIgnoreCase))
                    {
                        var Parts = Line.Split('=');
                        return Path.Combine(SettingsResult.Result.EstwOnlinePath, Parts[1].Trim());
                    }
                }
            }

            throw new OperationFailedException(String.Format("Ungültige Ini-Datei '{0}'!", IniPath));
        }

        private TrainInformation __CreateLiveTrainInformation(int trainNumber, ESTW estw)
        {
            if (estw.Area.Trains.ContainsKey(trainNumber))
            {
                var Train = estw.Area.Trains[trainNumber];
                var Result = new TrainInformation(Train);

                var SchedulesResult = CalculationBLL.GetSchedulesByTime(Train.Schedules, estw.Time);
                ValidateResult(SchedulesResult);

                foreach (var Schedule in SchedulesResult.Result)
                {
                    var LiveSchedule = new LiveSchedule(Result, Schedule);
                    Result.AddSchedule(LiveSchedule);
                }

                return Result;
            }
            else
            {
                var Train = new Train(trainNumber);
                return new TrainInformation(Train);
            }
        }

        private void __RefreshTrainInformation(TrainInformation Train, Block Block, ESTW Estw)
        {
            LiveSchedule CurrentSchedule = null;

            // Train is in station and drives in the correct direction
            if (Block != null)
            {
                // Dummy track for stations without platforms (e.g. Üst)
                if (Block.Track.Name.IsNullOrEmpty())
                {
                    Train.Block = Block;
                }
                else
                {
                    var Schedules = Train.Schedules.Where(s => s.Schedule.Station.ShortSymbol == Block.Track.Station.ShortSymbol);

                    // Find schedule that fits to the current direction of travel
                    foreach (var Schedule in Schedules)
                    {
                        if ((Schedule.Schedule.Direction == eScheduleDirection.LeftToRight && Train.Direction == eBlockDirection.Right)
                            || (Schedule.Schedule.Direction == eScheduleDirection.RightToLeft && Train.Direction == eBlockDirection.Left))
                        {
                            CurrentSchedule = Schedule;
                            break;
                        }
                    }

                    // When no schedule has been found according to the direction, take the first one.
                    if (CurrentSchedule == null)
                        CurrentSchedule = Schedules.FirstOrDefault();

                    // The train has no schedule for the current station (e.g. special or misdirected trains).
                    if (CurrentSchedule == null && Block.Track.Station.ScheduleFile.IsNotNullOrWhiteSpace() && Block.Track.CalculateDelay)
                    {
                        CurrentSchedule = new LiveSchedule(Train, Block.Track.Station);
                        Train.AddSchedule(CurrentSchedule);
                        Train.Train.AddSchedule(CurrentSchedule.Schedule);
                    }

                    if (CurrentSchedule == null)
                        Train.Block = Block;
                    else
                    {
                        Track LiveTrack = null;

                        // Too difficult to explain -> LTA...
                        if (CurrentSchedule.Schedule.Track == null
                            || !CurrentSchedule.Schedule.Track.IsPlatform
                            || CurrentSchedule.Schedule.Track.Name.Equals(Block.Track.Name, StringComparison.InvariantCultureIgnoreCase)
                            || (CurrentSchedule.Schedule.Track.Alternatives.Count == 0 && CurrentSchedule.Schedule.Track.Parent.Alternatives.Count == 0)
                            || CurrentSchedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(Block.Track.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LiveTrack = Block.Track;
                        }
                        else if (CurrentSchedule.Schedule.Track.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase)
                            || CurrentSchedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LiveTrack = Block.Track.Parent;
                        }
                        else if (CurrentSchedule.Schedule.Track.Parent.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase)
                            || CurrentSchedule.Schedule.Track.Parent.Alternatives.Any(a => a.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase)))
                        {
                            LiveTrack = Block.Track;
                        }

                        if (LiveTrack != null || Train.Block == null)
                            Train.Block = Block;

                        if (LiveTrack == null)
                            CurrentSchedule = null;
                        else
                        {
                            if ((CurrentSchedule.Schedule.Track == null || CurrentSchedule.Schedule.Track.CalculateDelay) && CurrentSchedule.LiveArrival == null)
                                CurrentSchedule.LiveArrival = Estw.Time;

                            if (CurrentSchedule.Schedule.Track == null || CurrentSchedule.Schedule.Track.IsPlatform)
                                CurrentSchedule.LiveTrack = LiveTrack;
                        }
                    }
                }
            }

            foreach (var Schedule in Train.Schedules)
                if ((Schedule != CurrentSchedule) && Schedule.LiveArrival != null && Schedule.LiveDeparture == null && Schedule.LiveTrack != null && Schedule.LiveTrack.CalculateDelay)
                    Schedule.LiveDeparture = Estw.Time;

            var DelayResult = CalculationBLL.CalculateDelay(Train, Estw);
            ValidateResult(DelayResult);

            if (DelayResult.Result.HasValue)
                Train.Delay = DelayResult.Result.Value;

            var ExpectedResult = CalculationBLL.CalculateExpectedTimes(Train, Estw);
            ValidateResult(ExpectedResult);

            Train.LastModified = Estw.Time;
        }

        private void __RefreshLiveSchedules(TrainInformation Train, ESTW estw)
        {
            lock (Train.LockSchedules)
            {
                var OldSchedules = Train.Schedules.ToList();
                Train.TruncateSchedules();

                var SchedulesResult = CalculationBLL.GetSchedulesByTime(Train.Train.Schedules, estw.Time);
                ValidateResult(SchedulesResult);

                foreach (var Schedule in SchedulesResult.Result)
                {
                    var LiveSchedule = OldSchedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == Schedule.Station.ShortSymbol && s.Schedule.Time == Schedule.Time);

                    if (LiveSchedule == null)
                        LiveSchedule = new LiveSchedule(Train, Schedule);
                    else
                        OldSchedules.Remove(LiveSchedule);

                    Train.AddSchedule(LiveSchedule);
                }

                foreach (var OldSchedule in OldSchedules)
                    Train.AddSchedule(OldSchedule);
            }
        }

        private void __LoadSharedDelays(Area area)
        {
            if (DataFilesPath.IsNullOrEmpty())
                return;

            var Files = Directory.EnumerateFiles(DataFilesPath, String.Format("{0}*_*.dat", Constants.SHARED_DELAY_PREFIX)).Select(file => new FileInfo(file));

            foreach (var File in Files)
            {
                using (var FileStream = File.OpenRead())
                {
                    var Reader = XmlReader.Create(FileStream);

                    if (!m_DelaySerializer.CanDeserialize(Reader))
                        continue;

                    var SharedDelay = m_DelaySerializer.Deserialize(Reader) as SharedDelay;

                    if (SharedDelay == null)
                        continue;

                    TrainInformation Train;

                    if (!area.LiveTrains.TryGetValue(SharedDelay.TrainNumber, out Train))
                        continue;

                    var Schedules = Train.Schedules.Where(s => s.Schedule.Station.ShortSymbol == SharedDelay.StationShortSymbol);
                    var Schedule = Schedules.ElementAtOrDefault(SharedDelay.ScheduleIndex);

                    if (Schedule == null)
                        Schedule = Schedules.FirstOrDefault();

                    if (Schedule == null)
                        continue;

                    var Delay = Schedule.Delays.FirstOrDefault(d => d.Type == SharedDelay.Type);

                    if (Delay == null)
                        Delay = Schedule.AddDelay(SharedDelay.Minutes, SharedDelay.Type);

                    Delay.Reason = SharedDelay.Reason;
                    Delay.CausedBy = SharedDelay.CausedBy;
                }

                File.Delete();
            }
        }

        #endregion

    }
}