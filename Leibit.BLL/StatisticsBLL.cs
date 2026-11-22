using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Leibit.BLL
{
    public class StatisticsBLL : BLLBase
    {
        #region - Public methods -

        #region [GetCurrentStatistics]
        public OperationResult<CurrentStatistics> GetCurrentStatistics(ESTW estw)
        {
            try
            {
                var result = new CurrentStatistics();
                result.ESTW = estw;

                var currentTrains = estw.Area.LiveTrains.Values.Where(t => t.LastModification.ContainsKey(estw.Id) && t.LastModification[estw.Id] == estw.Time).ToList();
                var passengerTrains = currentTrains.Where(t => t.Train.Type.IsPassengerTrain()).ToList();
                var freightTrains = currentTrains.Where(t => !t.Train.Type.IsPassengerTrain()).ToList();

                result.Total = __CalculateTrainTypeStatistics(currentTrains);
                result.PassengerTrains = __CalculateTrainTypeStatistics(passengerTrains);
                result.FreightTrains = __CalculateTrainTypeStatistics(freightTrains);

                return OperationResult<CurrentStatistics>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<CurrentStatistics>.Fail(ex.ToString());
            }
        }
        #endregion

        #region [GetHistoricalStatistics]
        public OperationResult<HistoricalStatistics> GetHistoricalStatistics(ESTW estw, int frameSize, eHistoricalStatisticsType type)
        {
            try
            {
                var result = new HistoricalStatistics();
                result.ESTW = estw;

                var trains = estw.Area.LiveTrains.Values.Where(t => t.LastModification.ContainsKey(estw.Id)).ToList();

                // To determine whether a train is a passenger or freight train, check all its schedules.
                // Some trains change their type during their journey (e.g. RB -> Lt or vice versa, when coming from or going to the depot).
                // If an Lt train is late from the depot, it has an impact for passengers.
                // That's why we want to consider these cases as passenger trains as well, even if the Lt train itself has no passengers.
                if (type == eHistoricalStatisticsType.OnlyPassengerTrains)
                    trains = trains.Where(t => t.Train.Schedules.Any(s => s.TrainType.IsPassengerTrain())).ToList();
                if (type == eHistoricalStatisticsType.OnlyFreightFrains)
                    trains = trains.Where(t => !t.Train.Schedules.Any(s => s.TrainType.IsPassengerTrain())).ToList();

                var trainDelayStatistics = new List<TrainDelayStatistic>();

                foreach (var train in trains)
                {
                    var schedules = train.Schedules.Where(s => s.Schedule.Station.ESTW == estw);
                    var firstSchedule = schedules.FirstOrDefault(s => s.LiveArrival != null);
                    var lastSchedule = schedules.LastOrDefault(s => s.LiveArrival != null);
                    int entryDelay, exitDelay;

                    /* Calculation of entry delay
                     * ==========================
                     * The entry delay is the delay at the first station the train has reached in the current ESTW.
                     * If the train hasn't yet reached any station in the current ESTW, we just take the current delay as entry delay.
                     * When the train starts in the current ESTW, we take the delay from the previous service as entry delay.
                     * If no previous service exists, the entry delay is always 0 per definition.
                     * If the train did't start in the current ESTW, we calculate the arrival delay for the first schedule in the current ESTW.
                     * In case of passing trains, no arrival time exists, so use the departure time as replacement.
                     */
                    if (firstSchedule == null)
                        entryDelay = train.Delay;
                    else if (firstSchedule.Schedule.Handling == eHandling.Start)
                    {
                        if (train.PreviousService.HasValue && estw.Area.LiveTrains.TryGetValue(train.PreviousService.Value, out var previousService))
                            entryDelay = previousService.Delay;
                        else
                            entryDelay = 0;
                    }
                    else
                    {
                        var referenceTime = firstSchedule.Schedule.Arrival ?? firstSchedule.Schedule.Departure;
                        entryDelay = (firstSchedule.LiveArrival - referenceTime).TotalMinutes;
                    }

                    /* Calculation of exit delay
                     * =========================
                     * The exit delay is the delay at the last station in the current ESTW, or - if the train hasn't yet left the current ESTW - the current delay.
                     */
                    if (lastSchedule != null && lastSchedule.LiveDeparture != null && lastSchedule.Schedule.Departure != null)
                        exitDelay = (lastSchedule.LiveDeparture - lastSchedule.Schedule.Departure).TotalMinutes;
                    else
                        exitDelay = train.Delay;

                    if (exitDelay < -5)
                        result.DelayStatistics.NumberOfEarlyTrains++;
                    else if (exitDelay <= 5)
                        result.DelayStatistics.NumberOfTrainsOnTime++;
                    else if (exitDelay <= 10)
                        result.DelayStatistics.NumberOfTrainsWithShortDelay++;
                    else
                        result.DelayStatistics.NumberOfTrainsWithLongDelay++;

                    var netDelay = Math.Max(0, exitDelay) - Math.Max(0, entryDelay);
                    result.DelayTotalNet += netDelay;

                    if (netDelay < 0)
                        result.DelayReduced += netDelay;
                    else
                        result.DelayAdded += netDelay;

                    trainDelayStatistics.Add(new TrainDelayStatistic
                    {
                        Train = train,
                        EntryDelay = entryDelay,
                        ExitDelay = exitDelay,
                        NetDelay = netDelay,
                    });
                }

                result.NumberOfTrains = trains.Count;
                result.TrainsWithGreatestDelays = trainDelayStatistics.Where(s => s.NetDelay > 0).OrderByDescending(s => s.NetDelay).Take(10).ToList();

                var allSchedules = trains.SelectMany(t => t.Schedules.Where(s => s.Schedule.Station.ESTW == estw)).ToList();

                var arrivals = allSchedules.Where(s => s.LiveArrival != null && s.Schedule.Arrival != null)
                                           .Select(s => new
                                           {
                                               FrameStartTime = new LeibitTime(s.LiveArrival.Day, s.LiveArrival.Hour, (s.LiveArrival.Minute / frameSize) * frameSize),
                                               TrainNumber = s.Schedule.Train.Number,
                                               Delay = Math.Max(0, (s.LiveArrival - s.Schedule.Arrival).TotalMinutes),
                                           });

                var departures = allSchedules.Where(s => s.LiveDeparture != null && s.Schedule.Departure != null)
                                             .Select(s => new
                                             {
                                                 FrameStartTime = new LeibitTime(s.LiveDeparture.Day, s.LiveDeparture.Hour, (s.LiveDeparture.Minute / frameSize) * frameSize),
                                                 TrainNumber = s.Schedule.Train.Number,
                                                 Delay = Math.Max(0, (s.LiveDeparture - s.Schedule.Departure).TotalMinutes),
                                             });

                var frames = arrivals.Union(departures).GroupBy(x => x.FrameStartTime).OrderBy(x => x.Key).ToList();

                foreach (var frame in frames)
                {
                    var resultFrame = new StatisticsTimeFrame();
                    resultFrame.StartTime = frame.Key;
                    resultFrame.EndTime = frame.Key.AddMinutes(frameSize);

                    if (resultFrame.EndTime > estw.Time)
                        continue;

                    while (result.TimeFrames.Any() && resultFrame.StartTime != result.TimeFrames.Last().EndTime)
                    {
                        var emptyFrame = new StatisticsTimeFrame();
                        emptyFrame.StartTime = result.TimeFrames.Last().EndTime;
                        emptyFrame.EndTime = emptyFrame.StartTime.AddMinutes(frameSize);
                        emptyFrame.OnTimeRate = 100;
                        result.TimeFrames.Add(emptyFrame);
                    }

                    var trainsInFrame = frame.GroupBy(x => x.TrainNumber).Select(x => new
                    {
                        TrainNumber = x.Key,
                        AverageDelay = x.Average(y => y.Delay),
                    }).ToList();

                    resultFrame.NumberOfTrains = trainsInFrame.Count;
                    resultFrame.DelaySum = Math.Round(trainsInFrame.Sum(x => x.AverageDelay), 2);

                    if (trainsInFrame.Count > 0)
                        resultFrame.OnTimeRate = 100 * trainsInFrame.Count(x => x.AverageDelay <= 5) / trainsInFrame.Count;

                    result.TimeFrames.Add(resultFrame);
                }

                result.StationsWithGreatestDelays = allSchedules.SelectMany(s => s.Delays)
                                                                .GroupBy(d => d.Schedule.Schedule.Station)
                                                                .Select(x => new StationDelayStatistic { Station = x.Key, DelaySum = x.Sum(y => y.Minutes) })
                                                                .OrderByDescending(x => x.DelaySum)
                                                                .ToList();

                return OperationResult<HistoricalStatistics>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<HistoricalStatistics>.Fail(ex.ToString());
            }
        }
        #endregion

        #region [GetDelaysForStation]
        public OperationResult<List<DelayInfo>> GetDelaysForStation(Station station)
        {
            try
            {
                var result = station.ESTW.Area.LiveTrains.Values.SelectMany(s => s.Schedules.Where(s => s.Schedule.Station == station))
                                                                .SelectMany(s => s.Delays)
                                                                .OrderByDescending(d => d.Minutes)
                                                                .ToList();
                return OperationResult<List<DelayInfo>>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<List<DelayInfo>>.Fail(ex.ToString());
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__CalculateTrainTypeStatistics]
        private TrainTypeStatistics __CalculateTrainTypeStatistics(List<TrainInformation> trains)
        {
            var result = new TrainTypeStatistics();
            result.NumberOfTrains = trains.Count;
            result.DelaySum = trains.Sum(t => Math.Max(0, t.Delay));
            result.DelayStatistics.NumberOfEarlyTrains = trains.Count(t => t.Delay < -5);
            result.DelayStatistics.NumberOfTrainsOnTime = trains.Count(t => t.Delay >= -5 && t.Delay <= 5);
            result.DelayStatistics.NumberOfTrainsWithShortDelay = trains.Count(t => t.Delay > 5 && t.Delay <= 10);
            result.DelayStatistics.NumberOfTrainsWithLongDelay = trains.Count(t => t.Delay > 10);
            result.TrainWithGreatestDelay = trains.OrderByDescending(t => t.Delay).FirstOrDefault(t => t.Delay > 0);
            return result;
        }
        #endregion

        #endregion
    }
}
