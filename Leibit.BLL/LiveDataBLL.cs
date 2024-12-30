﻿using Leibit.Core.Common;
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
                            var timeDifferences = train.Value.LastModification.Select(x =>
                            {
                                var estwTime = area.ESTWs.FirstOrDefault(e => e.Id == x.Key)?.Time;
                                int? diff = null;

                                if (estwTime != null)
                                {
                                    // Don't use TotalMinutes here!
                                    var t = estwTime - x.Value;
                                    diff = t.Hour * 60 + t.Minute;
                                }

                                return new { EstwId = x.Key, TimeDiff = diff };
                            }).Where(x => x.TimeDiff.HasValue);

                            train.Value.IsActive = timeDifferences.Any(d => d.TimeDiff < 2);

                            // Set departure times for trains that are gone.
                            if (!train.Value.IsActive)
                            {
                                var CurrentSchedules = train.Value.Schedules.Where(s => s.IsArrived && !s.IsDeparted);

                                foreach (var Schedule in CurrentSchedules)
                                {
                                    Schedule.IsDeparted = true;

                                    if (Schedule.LiveArrival != null)
                                        Schedule.LiveDeparture = train.Value.LastModification[Schedule.Schedule.Station.ESTW.Id];
                                }
                            }

                            // Delete train information that are older than 12 hours.
                            if (timeDifferences.All(d => d.TimeDiff > 12 * 60))
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

                if (delay.Type == eDelayType.Arrival && delay.Schedule.Schedule.TwinScheduleArrival != null)
                    __SynchronizeDelayToTwinSchedule(delay, delay.Schedule.Schedule.TwinScheduleArrival);

                if (delay.Type == eDelayType.Departure && delay.Schedule.Schedule.TwinScheduleDeparture != null)
                    __SynchronizeDelayToTwinSchedule(delay, delay.Schedule.Schedule.TwinScheduleDeparture);

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
        public OperationResult<bool> SetExpectedDelay(LiveSchedule schedule, int? expectedDelayArrival, int? expectedDelayDeparture)
        {
            try
            {
                // Validation
                var validationMessages = new List<string>();

                if (expectedDelayArrival.HasValue && schedule.IsArrived)
                    validationMessages.Add($"Die Ankunftsverspätung kann nicht gesetzt werden, da der Zug {schedule.Train.Train.Number} die Betriebsstelle {schedule.Schedule.Station.ShortSymbol} bereits erreicht hat.");
                if (schedule.IsDeparted)
                    validationMessages.Add($"Der Zug {schedule.Train.Train.Number} hat die Betriebsstelle {schedule.Schedule.Station.ShortSymbol} bereits verlassen.");

                if (validationMessages.Any())
                {
                    validationMessages.Insert(0, "Verspätung kann nicht eingetragen werden");
                    var message = string.Join(Environment.NewLine, validationMessages);
                    throw new InvalidOperationException(message);
                }

                // Let's do it
                if (expectedDelayArrival.HasValue)
                    schedule.ExpectedDelayArrival = expectedDelayArrival;

                if (expectedDelayDeparture.HasValue)
                    schedule.ExpectedDelayDeparture = expectedDelayDeparture;

                schedule.IsManuallyModified = true;

                var calculationResult = CalculationBLL.CalculateExpectedTimes(schedule.Train, schedule.Schedule.Station.ESTW);
                ValidateResult(calculationResult);

                __SynchronizeTwinSchedules(schedule.Train, schedule.Schedule.Station.ESTW);

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

                schedule.IsManuallyModified = true;

                __SynchronizeTwinSchedules(schedule.Train, schedule.Schedule.Station.ESTW);

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

                __SynchronizeTwinSchedules(schedule.Train, schedule.Schedule.Station.ESTW);

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

        #region [CreateLiveTrainInformation]
        public OperationResult<TrainInformation> CreateLiveTrainInformation(int trainNumber, ESTW estw)
        {
            try
            {
                return OperationResult<TrainInformation>.Ok(__CreateLiveTrainInformation(trainNumber, estw));
            }
            catch (Exception ex)
            {
                return OperationResult<TrainInformation>.Fail(ex.ToString());
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
                        var LoadResult = InitializationBLL.LoadESTW(estw);
                        ValidateResult(LoadResult);

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

                        int TrainNumber, Delay;

                        if (!Int32.TryParse(sTrainNumber, out TrainNumber) || !Int32.TryParse(sDelay, out Delay) || (sDirection != "L" && sDirection != "R"))
                            continue;

                        // Special codes, i.e. 0F0F0 results in 1000002 --> ignore that
                        if (TrainNumber >= 1_000_000)
                            continue;

                        if (TrainNumber >= 100_000)
                        {
                            if (estw.IgnoreRoutingDigits)
                                continue;

                            // Cut the first digit, i.e. routing digit
                            TrainNumber = TrainNumber % 100_000;
                        }

                        if (Delay == 99)
                            Delay = 0;

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

                        // When speed is not set, the direction in the file is random. So it's safer to not change it.
                        if (sSpeed != "0")
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
                if (!Estw.SchedulesLoaded || Block.Track.Name == null)
                {
                    Train.Block = Block;
                }
                else
                {
                    CurrentSchedule = __GetCurrentSchedule(Train, Block);

                    if (CurrentSchedule == null)
                        Train.Block = Block;
                    else
                    {
                        Track LiveTrack = null;

                        /** Imagine this station layout for the following examples
                         * 
                         * -----<|-----1A-----<|>-----1B-----|>-----
                         * -----<|-----2A-----<|>-----2B-----|>-----
                         * -----<|-------------3-------------|>-----
                         * -----<|-------------4-------------|>-----
                         * 
                         */

                        if (CurrentSchedule.Schedule.Track == null // special or misdirected trains
                            || !CurrentSchedule.Schedule.Track.IsPlatform // Abzw/Üst
                            || CurrentSchedule.Schedule.Track.Name.Equals(Block.Track.Name, StringComparison.InvariantCultureIgnoreCase) // Normal case
                            || (CurrentSchedule.Schedule.Track.Alternatives.Count == 0 && CurrentSchedule.Schedule.Track.Parent.Alternatives.Count == 0) // No alternatives defined
                            || CurrentSchedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(Block.Track.Name, StringComparison.InvariantCultureIgnoreCase))) // Schedule = 3, Block = 4 => LiveTrack = 4
                        {
                            LiveTrack = Block.Track;
                        }
                        else if (CurrentSchedule.Schedule.Track.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase) // Schedule = 1, Block = 1A => LiveTrack = 1
                            || CurrentSchedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase))) // Schedule = 1, Block = 2A => LiveTrack = 2
                        {
                            LiveTrack = Block.Track.Parent;
                        }
                        // These conditions must be evaluated after the second block, otherwise those case won't work.
                        else if (/*CurrentSchedule.Schedule.Track.Parent.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase) || */ // Schedule = 1A, Block = 1B, => LiveTrack = 1B ==> That seems wrong
                            CurrentSchedule.Schedule.Track.Parent.Alternatives.Any(a => a.Name.Equals(Block.Track.Parent.Name, StringComparison.InvariantCultureIgnoreCase))) // Schedule = 1A, Block = 2A => LiveTrack = 2A   -OR-   Schedule = 1A, Block = 2B => LiveTrack = 2B
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
                            CurrentSchedule.IsCancelled = false;

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

            Train.LastModification[Estw.Id] = Estw.Time;
            Train.RealBlock = Block;

            __SynchronizeTwinSchedules(Train, Estw);

            if (Train.PreviousService.HasValue && Estw.Area.LiveTrains.ContainsKey(Train.PreviousService.Value) && CurrentSchedule != null && CurrentSchedule.Schedule.Handling == eHandling.Start)
            {
                var previousTrain = Estw.Area.LiveTrains[Train.PreviousService.Value];

                if (previousTrain.Schedules.FirstOrDefault(s => s.Schedule.Handling == eHandling.Destination)?.IsArrived == false)
                {
                    __RefreshTrainInformation(previousTrain, Block, Estw);
                }
            }
        }

        private LiveSchedule __GetCurrentSchedule(TrainInformation train, Block block)
        {
            var Schedules = train.Schedules.Where(s => s.Schedule.Station.ShortSymbol == block.Track.Station.ShortSymbol);

            // If there is only one schedule, it's easy. This will accommodate most cases.
            if (Schedules.Count() == 1)
                return Schedules.Single();

            // Find the schedule that fits to the train's direction (only if this is unique).
            // LeftToLeft and RightToRight can't be handled here as the train changes its direction.
            var scheduleDirection = eScheduleDirection.Unknown;

            if (train.Direction == eBlockDirection.Left)
                scheduleDirection = eScheduleDirection.RightToLeft;
            else if (train.Direction == eBlockDirection.Right)
                scheduleDirection = eScheduleDirection.LeftToRight;

            var candidates = Schedules.Where(s => s.Schedule.Direction == scheduleDirection);

            if (candidates.Count() == 1)
                return candidates.Single();

            // Next, check the schedules with the correct directions and take the first one that is not departed.
            if (candidates.Any(s => !s.IsDeparted))
                return candidates.First(s => !s.IsDeparted);

            // Okay, we were unlucky with the direction, so try again with all schedules.
            if (Schedules.Any(s => !s.IsDeparted))
                return Schedules.First(s => !s.IsDeparted);

            // As a last resort, assume the last schedule as the current schedule, even if it's already departed.
            if (Schedules.Any())
                return Schedules.Last();

            // The train has no schedule for the current station (e.g. special or misdirected trains).
            if (block.Track.Station.HasScheduleFile && block.Track.CalculateDelay)
            {
                var schedule = new LiveSchedule(train, block.Track.Station);
                train.AddSchedule(schedule);
                train.Train.AddSchedule(schedule.Schedule);
                return schedule;
            }

            return null;
        }

        private void __SynchronizeTwinSchedules(TrainInformation train, ESTW estw)
        {
            var twinTrains = new List<TrainInformation>();

            foreach (var schedule in train.Schedules)
            {
                if (schedule.Schedule.TwinScheduleArrival != null)
                {
                    var twinTrainNumber = schedule.Schedule.TwinScheduleArrival.Train.Number;
                    var twinTrain = __GetOrCreateLiveTrainInformation(twinTrainNumber, estw);
                    var twinSchedule = twinTrain.Schedules.FirstOrDefault(s => s.Schedule == schedule.Schedule.TwinScheduleArrival);

                    if (twinSchedule != null && !twinSchedule.IsCancelled)
                    {
                        twinSchedule.IsArrived = schedule.IsArrived;
                        twinSchedule.LiveArrival = schedule.LiveArrival;
                        twinSchedule.ExpectedArrival = schedule.ExpectedArrival;
                        twinSchedule.ExpectedDelayArrival = schedule.ExpectedDelayArrival;
                        twinSchedule.LiveTrack = schedule.LiveTrack;
                        twinTrain.LastModification[estw.Id] = estw.Time;
                        twinTrains.Add(twinTrain);
                    }
                }

                if (schedule.Schedule.TwinScheduleDeparture != null)
                {
                    var twinTrainNumber = schedule.Schedule.TwinScheduleDeparture.Train.Number;
                    var twinTrain = __GetOrCreateLiveTrainInformation(twinTrainNumber, estw);
                    var twinSchedule = twinTrain.Schedules.FirstOrDefault(s => s.Schedule == schedule.Schedule.TwinScheduleDeparture);

                    if (twinSchedule != null && !twinSchedule.IsCancelled)
                    {
                        twinSchedule.IsDeparted = schedule.IsDeparted;
                        twinSchedule.LiveDeparture = schedule.LiveDeparture;
                        twinSchedule.ExpectedDeparture = schedule.ExpectedDeparture;
                        twinSchedule.ExpectedDelayDeparture = schedule.ExpectedDelayDeparture;
                        twinSchedule.LiveTrack = schedule.LiveTrack;
                        twinSchedule.IsComposed = schedule.IsComposed;
                        twinSchedule.IsPrepared = schedule.IsPrepared;
                        twinTrain.LastModification[estw.Id] = estw.Time;
                        twinTrains.Add(twinTrain);
                    }
                }
            }

            var currentSchedule = train.Schedules.LastOrDefault(s => s.IsArrived);

            if (currentSchedule == null)
                currentSchedule = train.Schedules.FirstOrDefault();

            if (currentSchedule != null && !currentSchedule.IsDeparted && currentSchedule.Schedule.TwinScheduleArrival != null)
            {
                var twinTrainNumber = currentSchedule.Schedule.TwinScheduleArrival.Train.Number;
                var twinTrain = __GetOrCreateLiveTrainInformation(twinTrainNumber, estw);
                twinTrain.Block = train.Block;
                twinTrain.Delay = train.Delay;
                twinTrain.LastModification[estw.Id] = estw.Time;
                twinTrains.Add(twinTrain);
            }

            if (currentSchedule != null && currentSchedule.IsDeparted && currentSchedule.Schedule.TwinScheduleDeparture != null)
            {
                var twinTrainNumber = currentSchedule.Schedule.TwinScheduleDeparture.Train.Number;
                var twinTrain = __GetOrCreateLiveTrainInformation(twinTrainNumber, estw);
                twinTrain.Block = train.Block;
                twinTrain.Delay = train.Delay;
                twinTrain.LastModification[estw.Id] = estw.Time;
                twinTrains.Add(twinTrain);
            }

            foreach (var twinTrain in twinTrains.Distinct())
                CalculationBLL.CalculateExpectedTimes(twinTrain, estw);
        }

        private void __SynchronizeDelayToTwinSchedule(DelayInfo delay, Schedule twinSchedule)
        {
            var twinTrainNumber = twinSchedule.Train.Number;
            var twinTrain = __GetOrCreateLiveTrainInformation(twinTrainNumber, delay.Schedule.Schedule.Station.ESTW);
            var twinLiveSchedule = twinTrain.Schedules.FirstOrDefault(s => s.Schedule == twinSchedule);

            if (twinLiveSchedule != null && !twinLiveSchedule.IsCancelled)
            {
                var twinDelay = twinLiveSchedule.Delays.FirstOrDefault(d => d.Type == delay.Type);

                if (twinDelay == null)
                    twinDelay = twinLiveSchedule.AddDelay(delay.Minutes, delay.Type);

                twinDelay.Reason = delay.Reason;
                twinDelay.CausedBy = delay.CausedBy;
            }
        }

        private TrainInformation __GetOrCreateLiveTrainInformation(int trainNumber, ESTW estw)
        {
            if (estw.Area.LiveTrains.ContainsKey(trainNumber))
                return estw.Area.LiveTrains[trainNumber];

            var train = __CreateLiveTrainInformation(trainNumber, estw);
            train.CreatedOn = estw.Time;
            return estw.Area.LiveTrains.GetOrAdd(trainNumber, train);
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
