using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Serialization;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Leibit.BLL
{
    public class SerializationBLL : BLLBase
    {

        #region - Needs -
        private IFormatter m_Formatter;
        private CalculationBLL m_CalculationBll;
        private InitializationBLL m_InitializationBll;
        private SettingsBLL m_SettingsBll;
        #endregion

        #region - Ctor -
        public SerializationBLL()
            : base()
        {
            m_Formatter = new BinaryFormatter();
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
                }));

                foreach (var Train in Request.Area.LiveTrains.Values)
                {
                    var SerializedTrain = new SerializedTrain();
                    SerializedTrain.TrainNumber = Train.Train.Number;
                    SerializedTrain.Delay = Train.Delay;
                    SerializedTrain.LastModified = Train.LastModified;
                    SerializedTrain.TrainDirection = Train.Direction;

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
                        SerializedSchedule.ExpectedDelay = Schedule.ExpectedDelay;
                        SerializedSchedule.IsComposed = Schedule.IsComposed;
                        SerializedSchedule.IsPrepared = Schedule.IsPrepared;

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

                using (var Stream = new FileStream(Filename, FileMode.Create, FileAccess.Write))
                {
                    m_Formatter.Serialize(Stream, Root);
                }

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

                var File = new FileInfo(Filename);

                if (!File.Exists)
                    throw new OperationFailedException(String.Format("File '{0}' does not exist.", Filename));

                SerializedRoot Root;

                using (var Stream = File.OpenRead())
                {
                    Root = m_Formatter.Deserialize(Stream) as SerializedRoot;
                }

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
                }

                foreach (var SerializedTrain in Root.LiveTrains)
                {
                    var Estw = Area.ESTWs.SingleOrDefault(e => e.Id == SerializedTrain.CurrentEstwId);

                    if (Estw == null || !Estw.IsLoaded || !Area.Trains.ContainsKey(SerializedTrain.TrainNumber))
                        continue;

                    var Train = Area.Trains[SerializedTrain.TrainNumber];
                    var LiveTrain = new TrainInformation(Train);
                    LiveTrain.Delay = SerializedTrain.Delay;
                    LiveTrain.LastModified = SerializedTrain.LastModified;
                    LiveTrain.Direction = SerializedTrain.TrainDirection;

                    if (Estw.Blocks.ContainsKey(SerializedTrain.Block))
                        LiveTrain.Block = Estw.Blocks[SerializedTrain.Block].FirstOrDefault(b => b.Direction == SerializedTrain.BlockDirection);

                    var SchedulesResult = CalculationBll.GetSchedulesByTime(Train.Schedules, Estw.Time);
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
                        LiveSchedule.ExpectedDelay = SerializedSchedule.ExpectedDelay;
                        LiveSchedule.IsComposed = SerializedSchedule.IsComposed;
                        LiveSchedule.IsPrepared = SerializedSchedule.IsPrepared;

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
                Container.Windows = Root.Windows;

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
