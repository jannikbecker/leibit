using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Leibit.BLL
{
    public class LiveDataBLL : BLLBase
    {

        #region - Needs -
        private SettingsBLL m_SettingsBll;
        private CalculationBLL m_CalculationBll;
        private InitializationBLL m_InitializationBll;

        private string m_EstwOnlinePath;
        private object m_LockRefresh = new object();
        private object m_LockDataFilesPath = new object();
        #endregion

        #region - Ctor -
        public LiveDataBLL()
            : base()
        {
        }
        #endregion

        #region - Properties -

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

        #region [DebugMode]
        public bool DebugMode { get; set; }
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

                    var EstwTime = area.ESTWs.Max(e => e.Time);

                    if (EstwTime != null)
                    {
                        Parallel.ForEach(area.LiveTrains, Options, train =>
                            {
                                // Set departure times for trains that are gone.
                                if (train.Value.LastModified < EstwTime.AddMinutes(-2))
                                {
                                    var CurrentSchedules = train.Value.Schedules.Where(s => s.IsArrived && !s.IsDeparted);

                                    foreach (var Schedule in CurrentSchedules)
                                    {
                                        Schedule.IsDeparted = true;

                                        if (Schedule.LiveArrival != null)
                                            Schedule.LiveDeparture = train.Value.LastModified;
                                    }
                                }

                                // Delete train information that are older than 12 hours.
                                if (train.Value.LastModified < EstwTime.AddHours(-12))
                                    area.LiveTrains.TryRemove(train.Value.Train.Number, out _);
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
        public OperationResult<bool> JustifyDelay(DelayInfo delay, string reason, int? causedBy)
        {
            try
            {
                if (reason.IsNullOrWhiteSpace())
                    return new OperationResult<bool> { Message = "Bitte Grund angeben!" };

                var SettingsResult = SettingsBLL.GetSettings();
                ValidateResult(SettingsResult);
                var Settings = SettingsResult.Result;

                if (Settings.CheckPlausibility)
                    __CheckDelayJustificationPlausibility(delay, causedBy);

                delay.Reason = reason;
                delay.CausedBy = causedBy;

                var Result = new OperationResult<bool>();
                Result.Result = true;
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
            }
        }
        #endregion

        #region [SetExpectedDelay]
        public OperationResult<bool> SetExpectedDelay(LiveSchedule schedule, int expectedDelay)
        {
            try
            {
                // Validation
                var validationMessages = new List<string>();

                if (schedule.IsDeparted)
                    validationMessages.Add($"Der Zug {schedule.Train.Train.Number} hat die Betriebsstelle {schedule.Schedule.Station.ShortSymbol} bereits verlassen.");
                if (schedule.Schedule.Handling == eHandling.Destination)
                    validationMessages.Add($"Der Zug {schedule.Train.Train.Number} endet in {schedule.Schedule.Station.ShortSymbol}.");

                if (validationMessages.Any())
                {
                    validationMessages.Insert(0, "Verspätung kann nicht eingetragen werden");
                    var message = string.Join(Environment.NewLine, validationMessages);
                    throw new InvalidOperationException(message);
                }

                // Let's do it
                schedule.ExpectedDelay = expectedDelay;

                var calculationResult = CalculationBLL.CalculateExpectedTimes(schedule.Train, schedule.Schedule.Station.ESTW);
                ValidateResult(calculationResult);

                var result = new OperationResult<bool>();
                result.Result = true;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
            }
        }
        #endregion

        #region [ChangeTrack]
        public OperationResult<bool> ChangeTrack(LiveSchedule schedule, Track track)
        {
            try
            {
                // Validation
                var validationMessages = new List<string>();

                if (schedule.IsArrived)
                    validationMessages.Add($"Der Zug {schedule.Train.Train.Number} hat die Betriebsstelle {schedule.Schedule.Station.ShortSymbol} bereits erreicht.");

                if (schedule.Schedule.Track != null && !schedule.Schedule.Track.IsPlatform)
                    validationMessages.Add($"Für die Betriebsstelle {schedule.Schedule.Station.ShortSymbol} kann kein Gleiswechsel vorgenommen werden.");

                if (schedule.Schedule.Track != null && (schedule.Schedule.Track.Alternatives.Any() || schedule.Schedule.Track.Parent.Alternatives.Any()))
                {
                    if (!schedule.Schedule.Track.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase) // 1 -> 1
                        && !schedule.Schedule.Track.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase) // 1 -> 1A
                        && !schedule.Schedule.Track.Parent.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase) // 1A -> 1
                        && !schedule.Schedule.Track.Parent.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase) // 1A -> 1B
                        && !schedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase)) // 1 -> 2
                        && !schedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase)) // 1 -> 2A
                        && !schedule.Schedule.Track.Parent.Alternatives.Any(a => a.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase)) // 1A -> 2
                        && !schedule.Schedule.Track.Parent.Alternatives.Any(a => a.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase))) // 1A -> 2A
                    {
                        validationMessages.Add($"Das Gleis {track.Name} liegt in einem anderen Bahnhofsteil als das planmäßige Gleis {schedule.Schedule.Track.Name}.");
                    }
                }

                if (validationMessages.Any())
                {
                    validationMessages.Insert(0, "Gleiswechsel nicht möglich");
                    var message = string.Join(Environment.NewLine, validationMessages);
                    throw new InvalidOperationException(message);
                }

                // Let's go
                if (schedule.Schedule.Track != null && schedule.Schedule.Track.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase))
                    schedule.LiveTrack = null;
                else
                    schedule.LiveTrack = track;

                var result = new OperationResult<bool>();
                result.Result = true;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
            }
        }
        #endregion

        #region [SetTrainState]
        public OperationResult<bool> SetTrainState(LiveSchedule schedule, eTrainState state)
        {
            try
            {
                // Check if we are lucky
                if ((state == eTrainState.None && !schedule.IsComposed && !schedule.IsPrepared)
                 || (state == eTrainState.Composed && schedule.IsComposed)
                 || (state == eTrainState.Prepared && schedule.IsPrepared))
                {
                    return new OperationResult<bool> { Result = false, Succeeded = true };
                }

                // Validation
                var validationMessages = new List<string>();

                if (!schedule.IsArrived)
                    validationMessages.Add($"Der Zug {schedule.Train.Train.Number} hat die Betriebsstelle {schedule.Schedule.Station.ShortSymbol} noch nicht erreicht.");

                if (schedule.IsDeparted)
                    validationMessages.Add($"Der Zug {schedule.Train.Train.Number} hat die Betriebsstelle {schedule.Schedule.Station.ShortSymbol} bereits verlassen.");

                if (validationMessages.Any())
                {
                    validationMessages.Insert(0, "Eingabe des Zugstatus nicht möglich");
                    var message = string.Join(Environment.NewLine, validationMessages);
                    throw new InvalidOperationException(message);
                }

                // Here we go
                switch (state)
                {
                    case eTrainState.None:
                        schedule.IsComposed = false;
                        schedule.IsPrepared = false;
                        break;
                    case eTrainState.Composed:
                        schedule.IsComposed = true;
                        break;
                    case eTrainState.Prepared:
                        schedule.IsComposed = true; // Set both flags is this case
                        schedule.IsPrepared = true;
                        break;
                }

                var result = new OperationResult<bool>();
                result.Result = true;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
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
                            {
                                var NewTime = new LeibitTime(Day, Hour, Minute);

                                /* Check if the new time is greater than the older one.
                                 * This is important because at midnight there can be a file with time 00:00 but still the old day, i.e.
                                 * 
                                 * 1. DI 23:59
                                 * 2. DI 00:00 --> Ignore this!
                                 * 3. MI 00:00
                                 */
                                if (NewTime > estw.Time)
                                    estw.Time = NewTime;
                            }
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
                        string sSpeed = Parts[4];
                        string sDirection = Parts[5];

                        if (sTrainNumber.Length > 5)
                            sTrainNumber = sTrainNumber.Substring(sTrainNumber.Length - 5);

                        int TrainNumber, Delay;

                        if (!Int32.TryParse(sTrainNumber, out TrainNumber) || !Int32.TryParse(sDelay, out Delay) || (sDirection != "L" && sDirection != "R"))
                            continue;

                        TrainInformation Train;

                        if (estw.Area.LiveTrains.ContainsKey(TrainNumber))
                            Train = estw.Area.LiveTrains[TrainNumber];
                        else
                        {
                            Train = __CreateLiveTrainInformation(TrainNumber, estw);
                            Train.CreatedOn = estw.Time;
                            Train = estw.Area.LiveTrains.GetOrAdd(TrainNumber, Train);
                        }

                        if (Train.Schedules.All(s => s.Schedule.IsUnscheduled || s.LiveArrival == null) || !estw.SchedulesLoaded)
                            Train.Delay = Delay;
                        else
                        {
                            // In this case delay is calculated by LeiBIT. Ignore the delay information from ESTWsim!
                        }

                        Train.Direction = sDirection == "L" ? eBlockDirection.Left : eBlockDirection.Right;

                        var Block = estw.Blocks.ContainsKey(BlockName) ? estw.Blocks[BlockName].FirstOrDefault(b => b.Direction == eBlockDirection.Both || b.Direction == Train.Direction) : null;

                        // When speed is not set, the direction in the file is random.
                        // When direction plays a role it's safer to not assign the block in that case.
                        if (Block != null && Block.Direction != eBlockDirection.Both && sSpeed == "0")
                            Block = null;

                        __RefreshTrainInformation(Train, Block, estw);
                    }
                }
            }

            estw.LastUpdatedOn = DateTime.Now;

            if (Debugger.IsAttached && DebugMode)
            {
                var fileInfo = new FileInfo(FilePath);
                var backupDirectory = Path.Combine(fileInfo.DirectoryName, "debug");

                if (!Directory.Exists(backupDirectory))
                    Directory.CreateDirectory(backupDirectory);

                var fileName = $"{fileInfo.Name.Substring(0, fileInfo.Name.Length - fileInfo.Extension.Length)}_{DateTime.Now:yyyyMMdd_HHmmss}{fileInfo.Extension}";
                var backupFile = Path.Combine(backupDirectory, fileName);
                File.Move(FilePath, backupFile);
            }
            else
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

                // Don't validate result here. When this fails, it's not so dramatic...
                var prevResult = CalculationBLL.GetPreviousService(Train, estw);
                if (prevResult.Succeeded)
                    Result.PreviousService = prevResult.Result;

                var followUpResult = CalculationBLL.GetFollowUpService(Train, estw);
                if (followUpResult.Succeeded)
                    Result.FollowUpService = followUpResult.Result;

                return Result;
            }
            else
            {
                var Train = estw.Area.Trains.GetOrAdd(trainNumber, new Train(trainNumber));
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
                if (!Estw.SchedulesLoaded || Block.Track.Name.IsNullOrEmpty())
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
                    if (CurrentSchedule == null && Block.Track.Station.HasScheduleFile && Block.Track.CalculateDelay)
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

                        if (LiveTrack != null)
                            Train.Block = Block;

                        if (LiveTrack == null)
                            CurrentSchedule = null;
                        else
                        {
                            CurrentSchedule.IsArrived = true;

                            if ((CurrentSchedule.Schedule.Track == null || CurrentSchedule.Schedule.Track.CalculateDelay) && CurrentSchedule.LiveArrival == null)
                                CurrentSchedule.LiveArrival = Estw.Time;

                            if (CurrentSchedule.Schedule.Track == null || CurrentSchedule.Schedule.Track.IsPlatform)
                            {
                                CurrentSchedule.LiveTrack = LiveTrack;

                                if (LiveTrack.IsPlatform)
                                {
                                    // When train is in station, it cannot be departed.
                                    // This fixes issues that can occur in mirror fields when the train has arrived at the station in one ESTW, but not yet in the other.
                                    CurrentSchedule.IsDeparted = false;
                                    CurrentSchedule.LiveDeparture = null;
                                }
                            }
                        }
                    }
                }
            }

            foreach (var Schedule in Train.Schedules)
            {
                if (Schedule != CurrentSchedule || Schedule.LiveTrack == null || !Schedule.LiveTrack.IsPlatform)
                {
                    if (Schedule.IsArrived)
                        Schedule.IsDeparted = true;

                    if (Schedule.LiveArrival != null && Schedule.LiveDeparture == null)
                        Schedule.LiveDeparture = Estw.Time;
                }
            }

            // For stations that are located in mirror fields, two schedules might exist.
            // The times etc. must be identical to both schedules to ensure that delay and expected times are calculated correctly.
            // Example: HBON is located in the district of AROG, but also in the mirror fields of HB. For the local trains, two schedules exist (one from ESTW HB and one from AROG).

            foreach (var ScheduleGroup in Train.Schedules.GroupBy(s => new { s.Schedule.Station.ShortSymbol, s.Schedule.Time }))
            {
                var ReferenceSchedule = ScheduleGroup.FirstOrDefault(s => s.IsArrived);

                if (ReferenceSchedule != null)
                {
                    foreach (var Schedule in ScheduleGroup)
                    {
                        Schedule.IsArrived = ReferenceSchedule.IsArrived;
                        Schedule.IsDeparted = ReferenceSchedule.IsDeparted;
                        Schedule.LiveArrival = ReferenceSchedule.LiveArrival;
                        Schedule.LiveDeparture = ReferenceSchedule.LiveDeparture;
                        Schedule.LiveTrack = ReferenceSchedule.LiveTrack;
                    }
                }
            }

            if (Estw.SchedulesLoaded)
            {
                var DelayResult = CalculationBLL.CalculateDelay(Train, Estw);
                ValidateResult(DelayResult);

                if (DelayResult.Result.HasValue)
                    Train.Delay = DelayResult.Result.Value;
            }

            var ExpectedResult = CalculationBLL.CalculateExpectedTimes(Train, Estw);
            ValidateResult(ExpectedResult);

            Train.LastModified = Estw.Time;
            Train.RealBlock = Block;

            if (Train.PreviousService.HasValue && Estw.Area.LiveTrains.ContainsKey(Train.PreviousService.Value) && CurrentSchedule != null && CurrentSchedule.Schedule.Handling == eHandling.Start)
            {
                var previousTrain = Estw.Area.LiveTrains[Train.PreviousService.Value];

                if (previousTrain.Schedules.FirstOrDefault(s => s.Schedule.Handling == eHandling.Destination)?.IsArrived == false)
                {
                    __RefreshTrainInformation(previousTrain, Block, Estw);
                }
            }
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
                    var LiveSchedule = OldSchedules.FirstOrDefault(s => s.Schedule.Station.ESTW.Id == Schedule.Station.ESTW.Id
                                                                     && s.Schedule.Station.ShortSymbol == Schedule.Station.ShortSymbol
                                                                     && s.Schedule.Time == Schedule.Time);

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

        private void __CheckDelayJustificationPlausibility(DelayInfo delay, int? causedBy)
        {
            var validationMessages = new List<string>();

            if (causedBy.HasValue)
            {
                var area = delay.Schedule.Schedule.Station.ESTW.Area;

                if (area.LiveTrains.ContainsKey(causedBy.Value))
                {
                    var train = area.LiveTrains[causedBy.Value];
                    var schedules = train.Schedules.Where(s => s.Schedule.Station.ShortSymbol == delay.Schedule.Schedule.Station.ShortSymbol);

                    if (!schedules.Any())
                    {
                        validationMessages.Add($"Zug {causedBy.Value} hat die Betriebsstelle '{delay.Schedule.Schedule.Station.Name}' nicht durchfahren");
                    }
                    else if (delay.Type == eDelayType.Arrival && !schedules.Any(s => __AreTimesClose(delay.Schedule.LiveArrival, s.LiveArrival)
                                                                                  || __AreTimesClose(delay.Schedule.LiveArrival, s.ExpectedArrival)
                                                                                  || __AreTimesClose(delay.Schedule.LiveArrival, s.LiveDeparture)))
                    {
                        validationMessages.Add($"Zug {causedBy.Value} hat die Betriebsstelle '{delay.Schedule.Schedule.Station.Name}' zu einer anderen Zeit durchfahren als {delay.Schedule.Train.Train.Number}");
                    }
                    else if (delay.Type == eDelayType.Departure && !schedules.Any(s => __AreTimesClose(delay.Schedule.LiveDeparture, s.LiveArrival)
                                                                                    || __AreTimesClose(delay.Schedule.LiveDeparture, s.ExpectedArrival)
                                                                                    || __AreTimesClose(delay.Schedule.LiveDeparture, s.LiveDeparture)))
                    {
                        validationMessages.Add($"Zug {causedBy.Value} hat die Betriebsstelle '{delay.Schedule.Schedule.Station.Name}' zu einer anderen Zeit durchfahren als {delay.Schedule.Train.Train.Number}");
                    }
                }
                else
                    validationMessages.Add($"Zug {causedBy.Value} nicht gefunden");
            }

            if (validationMessages.Any())
            {
                validationMessages.Insert(0, "Plausibilitätsprüfung fehlgeschlagen");
                var message = string.Join(Environment.NewLine, validationMessages);
                throw new InvalidOperationException(message);
            }
        }

        private bool __AreTimesClose(LeibitTime time1, LeibitTime time2)
        {
            if (time1 == null || time2 == null)
                return false;

            var diff = time1 - time2;
            return Math.Abs(diff.TotalMinutes) < 15;
        }

        #endregion

    }
}
