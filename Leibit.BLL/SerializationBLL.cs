using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Serialization;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Leibit.BLL
{
    public class SerializationBLL : BLLBase
    {

        #region - Needs -
        private CalculationBLL m_CalculationBll;
        private InitializationBLL m_InitializationBll;
        private SettingsBLL m_SettingsBll;
        private JsonSerializerSettings m_SerializerSettings;
        #endregion

        #region - Ctor -
        public SerializationBLL()
            : base()
        {
            m_SerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto,
            };
        }
        #endregion

        #region - Properties -

        #region [CalculationBll]
        internal CalculationBLL CalculationBll
        {
            get
            {
                if (m_CalculationBll == null)
                    m_CalculationBll = new CalculationBLL();

                return m_CalculationBll;
            }
        }
        #endregion

        #region [InitializationBll]
        internal InitializationBLL InitializationBll
        {
            get
            {
                if (m_InitializationBll == null)
                    m_InitializationBll = new InitializationBLL();

                return m_InitializationBll;
            }
        }
        #endregion

        #region [SettingsBll]
        internal SettingsBLL SettingsBll
        {
            get
            {
                if (m_SettingsBll == null)
                    m_SettingsBll = new SettingsBLL();

                return m_SettingsBll;
            }
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Save]
        public OperationResult<SerializedRoot> Save(string Filename, SerializationContainer Request)
        {
            try
            {
                var SettingsResult = SettingsBll.GetSettings();
                ValidateResult(SettingsResult);
                var Settings = SettingsResult.Result;

                var Result = new OperationResult<SerializedRoot>();

                var Root = new SerializedRoot();
                Root.AreaId = Request.Area.Id;
                Root.LoadedESTWs.AddRange(Request.Area.ESTWs.Where(e => e.IsLoaded).Select(e => new SerializedESTW
                {
                    ESTWId = e.Id,
                    Time = e.Time,
                    StartTime = e.StartTime,
                    IsActive = (DateTime.Now - e.LastUpdatedOn).TotalSeconds < Settings.EstwTimeout,
                    Reminders = e.Reminders.Select(r => new SerializedReminder
                    {
                        TrainNumber = r.TrainNumber,
                        StationShort = r.StationShort,
                        DueTime = r.DueTime,
                        Text = r.Text,
                    }).ToList(),
                }));

                Root.Version = Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

                foreach (var Train in Request.Area.LiveTrains.Values)
                {
                    var SerializedTrain = new SerializedTrain();
                    SerializedTrain.TrainNumber = Train.Train.Number;
                    SerializedTrain.Delay = Train.Delay;
                    SerializedTrain.LastModification.AddRange(Train.LastModification);
                    SerializedTrain.CreatedOn = Train.CreatedOn;
                    SerializedTrain.TrainDirection = Train.Direction;
                    SerializedTrain.IsDestinationStationCancelled = Train.IsDestinationStationCancelled;

                    foreach (var block in Train.BlockHistory)
                    {
                        var serializedBlock = new SerializedBlock();
                        serializedBlock.EstwId = block.Track.Station.ESTW.Id;
                        serializedBlock.Name = block.Name;
                        serializedBlock.Direction = block.Direction;
                        SerializedTrain.BlockHistory.Add(serializedBlock);
                    }

                    if (Train.Block != null)
                    {
                        SerializedTrain.CurrentEstwId = Train.Block.Track.Station.ESTW.Id;
                        SerializedTrain.Block = Train.Block.Name;
                        SerializedTrain.BlockDirection = Train.Block.Direction;
                    }

                    foreach (var Schedule in Train.Schedules)
                    {
                        var SerializedSchedule = new SerializedSchedule();
                        SerializedSchedule.EstwId = Schedule.Schedule.Station.ESTW.Id;
                        SerializedSchedule.StationShortSymbol = Schedule.Schedule.Station.ShortSymbol;
                        SerializedSchedule.StationTime = Schedule.Schedule.Time;
                        SerializedSchedule.LiveArrival = Schedule.LiveArrival;
                        SerializedSchedule.LiveDeparture = Schedule.LiveDeparture;
                        SerializedSchedule.IsArrived = Schedule.IsArrived;
                        SerializedSchedule.IsDeparted = Schedule.IsDeparted;
                        SerializedSchedule.ExpectedArrival = Schedule.ExpectedArrival;
                        SerializedSchedule.ExpectedDeparture = Schedule.ExpectedDeparture;
                        SerializedSchedule.ExpectedDelayArrival = Schedule.ExpectedDelayArrival;
                        SerializedSchedule.ExpectedDelayDeparture = Schedule.ExpectedDelayDeparture;
                        SerializedSchedule.IsComposed = Schedule.IsComposed;
                        SerializedSchedule.IsPrepared = Schedule.IsPrepared;
                        SerializedSchedule.IsCancelled = Schedule.IsCancelled;
                        SerializedSchedule.IsManuallyModified = Schedule.IsManuallyModified;
                        SerializedSchedule.LocalOrders = Schedule.LocalOrders;

                        if (Schedule.LiveTrack != null)
                            SerializedSchedule.LiveTrack = Schedule.LiveTrack.Name;

                        foreach (var Delay in Schedule.Delays)
                        {
                            var SerializedDelay = new SerializedDelay();
                            SerializedDelay.Type = Delay.Type;
                            SerializedDelay.Minutes = Delay.Minutes;
                            SerializedDelay.Reason = Delay.Reason;
                            SerializedDelay.CausedBy = Delay.CausedBy;
                            SerializedSchedule.Delays.Add(SerializedDelay);
                        }

                        SerializedTrain.Schedules.Add(SerializedSchedule);
                    }

                    Root.LiveTrains.Add(SerializedTrain);
                }

                Root.Windows = Request.Windows;
                Root.VisibleStations = Request.VisibleStations;
                Root.VisibleTrains = Request.VisibleTrains;
                Root.HiddenSchedules = Request.HiddenSchedules;

                var json = JsonConvert.SerializeObject(Root, m_SerializerSettings);
                File.WriteAllText(Filename, json);

                Result.Result = Root;
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<SerializedRoot> { Message = ex.Message };
            }
        }
        #endregion

        #region [Open]
        public OperationResult<SerializationContainer> Open(string Filename)
        {
            try
            {
                var SettingsResult = SettingsBll.GetSettings();
                ValidateResult(SettingsResult);
                var Settings = SettingsResult.Result;

                var Container = new SerializationContainer();

                if (!File.Exists(Filename))
                    throw new OperationFailedException(String.Format("File '{0}' does not exist.", Filename));

                SerializedRoot Root;

                var json = File.ReadAllText(Filename);
                Root = JsonConvert.DeserializeObject<SerializedRoot>(json, m_SerializerSettings);

                if (Root == null)
                    throw new OperationFailedException("Invalid file.");

                var AreaResult = InitializationBll.GetAreaInformation();
                ValidateResult(AreaResult);

                var Area = AreaResult.Result.FirstOrDefault(a => a.Id == Root.AreaId);

                if (Area == null)
                    throw new OperationFailedException("Invalid area.");

                foreach (var SerializedEstw in Root.LoadedESTWs)
                {
                    if (!Settings.LoadInactiveEstws && !SerializedEstw.IsActive)
                        continue;

                    var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == SerializedEstw.ESTWId);

                    if (Estw == null)
                        throw new OperationFailedException("Invalid ESTW.");

                    var LoadResult = InitializationBll.LoadESTW(Estw);
                    ValidateResult(LoadResult);
                    Estw.Time = SerializedEstw.Time;
                    Estw.StartTime = SerializedEstw.StartTime;

                    if (SerializedEstw.Reminders != null)
                    {
                        foreach (var SerializedReminder in SerializedEstw.Reminders)
                        {
                            var Reminder = new Reminder();
                            Reminder.TrainNumber = SerializedReminder.TrainNumber;
                            Reminder.StationShort = SerializedReminder.StationShort;
                            Reminder.DueTime = SerializedReminder.DueTime;
                            Reminder.Text = SerializedReminder.Text;
                            Estw.Reminders.Add(Reminder);
                        }
                    }
                }

                foreach (var SerializedTrain in Root.LiveTrains)
                {
                    var Estw = Area.ESTWs.SingleOrDefault(e => e.Id == SerializedTrain.CurrentEstwId);

                    if (Estw?.IsLoaded == false || !Area.Trains.ContainsKey(SerializedTrain.TrainNumber))
                        continue;

                    var Train = Area.Trains[SerializedTrain.TrainNumber];
                    var LiveTrain = new TrainInformation(Train);
                    LiveTrain.Delay = SerializedTrain.Delay;
                    LiveTrain.LastModification.AddRange(SerializedTrain.LastModification);
                    LiveTrain.CreatedOn = SerializedTrain.CreatedOn;
                    LiveTrain.Direction = SerializedTrain.TrainDirection;
                    LiveTrain.IsDestinationStationCancelled = SerializedTrain.IsDestinationStationCancelled;

                    // Ensure compatibility
                    if (Estw != null && !LiveTrain.LastModification.ContainsKey(Estw.Id) && SerializedTrain.LastModified != null)
                        LiveTrain.LastModification[Estw.Id] = SerializedTrain.LastModified;

                    if (SerializedTrain.BlockHistory != null)
                    {
                        foreach (var block in SerializedTrain.BlockHistory)
                        {
                            var estw2 = Area.ESTWs.SingleOrDefault(e => e.Id == block.EstwId);

                            if (estw2 == null || !estw2.Blocks.ContainsKey(block.Name))
                                continue;

                            LiveTrain.BlockHistory.AddIfNotNull(estw2.Blocks[block.Name].FirstOrDefault(b => b.Direction == block.Direction));
                        }
                    }

                    if (SerializedTrain.Block != null && Estw.Blocks.ContainsKey(SerializedTrain.Block))
                        LiveTrain.Block = Estw.Blocks[SerializedTrain.Block].FirstOrDefault(b => b.Direction == SerializedTrain.BlockDirection);

                    var scheduleReferenceTime = Estw?.Time;

                    if (scheduleReferenceTime == null && SerializedTrain.LastModification != null && SerializedTrain.LastModification.Values.Any())
                        scheduleReferenceTime = SerializedTrain.LastModification.Values.Max();

                    if (scheduleReferenceTime == null)
                        scheduleReferenceTime = SerializedTrain.LastModified;

                    if (scheduleReferenceTime == null)
                    {
                        // We have no information of the location or time. This train is in the middle of nowhere.
                        continue;
                    }

                    var SchedulesResult = CalculationBll.GetSchedulesByTime(Train.Schedules, scheduleReferenceTime);
                    ValidateResult(SchedulesResult);

                    foreach (var SerializedSchedule in SerializedTrain.Schedules)
                    {
                        var Schedule = SchedulesResult.Result.FirstOrDefault(s => s.Station.ShortSymbol == SerializedSchedule.StationShortSymbol
                                                                               && s.Time == SerializedSchedule.StationTime
                                                                               && (s.Station.ESTW.Id == SerializedSchedule.EstwId || SerializedSchedule.EstwId == null));

                        if (Schedule == null)
                            continue;

                        var LiveSchedule = new LiveSchedule(LiveTrain, Schedule);
                        LiveSchedule.LiveArrival = SerializedSchedule.LiveArrival;
                        LiveSchedule.LiveDeparture = SerializedSchedule.LiveDeparture;
                        LiveSchedule.IsArrived = SerializedSchedule.IsArrived;
                        LiveSchedule.IsDeparted = SerializedSchedule.IsDeparted;
                        LiveSchedule.ExpectedArrival = SerializedSchedule.ExpectedArrival;
                        LiveSchedule.ExpectedDeparture = SerializedSchedule.ExpectedDeparture;
                        LiveSchedule.IsComposed = SerializedSchedule.IsComposed;
                        LiveSchedule.IsPrepared = SerializedSchedule.IsPrepared;
                        LiveSchedule.IsCancelled = SerializedSchedule.IsCancelled;
                        LiveSchedule.IsManuallyModified = SerializedSchedule.IsManuallyModified;

                        if (SerializedSchedule.LocalOrders == null)
                            LiveSchedule.LocalOrders = Schedule.LocalOrders; // For compatibility reasons
                        else
                            LiveSchedule.LocalOrders = SerializedSchedule.LocalOrders;

                        if (SerializedSchedule.ExpectedDelay.HasValue)
                        {
                            // Ensure compatibility
                            LiveSchedule.ExpectedDelayDeparture = SerializedSchedule.ExpectedDelay;
                        }
                        else
                        {
                            LiveSchedule.ExpectedDelayArrival = SerializedSchedule.ExpectedDelayArrival;
                            LiveSchedule.ExpectedDelayDeparture = SerializedSchedule.ExpectedDelayDeparture;

                        }

                        if (SerializedSchedule.LiveTrack.IsNotNullOrEmpty())
                            LiveSchedule.LiveTrack = Schedule.Station.Tracks.SingleOrDefault(t => t.Name == SerializedSchedule.LiveTrack);

                        if (LiveSchedule.LiveArrival != null)
                            LiveSchedule.IsArrived = true;
                        if (LiveSchedule.LiveDeparture != null)
                            LiveSchedule.IsDeparted = true;

                        foreach (var SerializedDelay in SerializedSchedule.Delays)
                        {
                            var Delay = LiveSchedule.AddDelay(SerializedDelay.Minutes, SerializedDelay.Type);
                            Delay.Reason = SerializedDelay.Reason;
                            Delay.CausedBy = SerializedDelay.CausedBy;
                        }

                        LiveTrain.AddSchedule(LiveSchedule);
                    }

                    if (Estw != null)
                    {
                        // Don't validate result here. When this fails, it's not so dramatic...
                        var prevResult = CalculationBll.GetPreviousService(Train, Estw);
                        if (prevResult.Succeeded)
                            LiveTrain.PreviousService = prevResult.Result;

                        var followUpResult = CalculationBll.GetFollowUpService(Train, Estw);
                        if (followUpResult.Succeeded)
                            LiveTrain.FollowUpService = followUpResult.Result;
                    }

                    if (LiveTrain.Schedules.Any())
                        Area.LiveTrains.TryAdd(Train.Number, LiveTrain);
                }

                foreach (var Estw in Area.ESTWs)
                {
                    if (Estw.StartTime == null)
                    {
                        var schedules = Area.LiveTrains.Values.SelectMany(t => t.Schedules).Where(s => s.Schedule.Station.ESTW == Estw);

                        if (schedules.Any())
                        {
                            Estw.StartTime = schedules.Min(s => s.LiveArrival);

                            var minStartTime = Estw.Time.AddHours(-12);

                            if (Estw.StartTime < minStartTime)
                                Estw.StartTime = minStartTime;
                        }
                        else
                            Estw.StartTime = Estw.Time;
                    }
                }

                Container.Area = Area;
                Container.VisibleStations = Root.VisibleStations;
                Container.VisibleTrains = Root.VisibleTrains;
                Container.HiddenSchedules = Root.HiddenSchedules;
                Container.Windows = Root.Windows;
                Container.IsOldVersion = Root.Version != Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

                var Result = new OperationResult<SerializationContainer>();
                Result.Result = Container;
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<SerializationContainer> { Message = ex.Message };
            }
        }
        #endregion

        #endregion

    }
}
