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

                if (type == eHistoricalStatisticsType.OnlyPassengerTrains)
                    trains = trains.Where(t => t.Train.Type.IsPassengerTrain()).ToList();
                if (type == eHistoricalStatisticsType.OnlyFreightFrains)
                    trains = trains.Where(t => !t.Train.Type.IsPassengerTrain()).ToList();

                result.NumberOfTrains = trains.Count;
                result.NumberOfEarlyTrains = trains.Count(t => t.Delay < -5);
                result.NumberOfTrainsOnTime = trains.Count(t => t.Delay >= -5 && t.Delay <= 5);
                result.NumberOfTrainsWithShortDelay = trains.Count(t => t.Delay > 5 && t.Delay <= 10);
                result.NumberOfTrainsWithLongDelay = trains.Count(t => t.Delay > 10);

                if (result.NumberOfTrains > 0)
                    result.OnTimeRate = 100 * (result.NumberOfTrainsOnTime + result.NumberOfEarlyTrains) / result.NumberOfTrains;

                foreach (var train in trains)
                {
                    var schedules = train.Schedules.Where(s => s.Schedule.Station.ESTW == estw).ToList();
                    var firstSchedule = schedules.FirstOrDefault();
                    var lastSchedule = schedules.LastOrDefault();

                    if (firstSchedule == null || lastSchedule == null)
                        continue;

                    int entryDelay, exitDelay;

                    if (firstSchedule.Schedule.Handling == eHandling.Start)
                        entryDelay = 0;

                    var referenceTime = firstSchedule.Schedule.Arrival ?? firstSchedule.Schedule.Departure;

                    if (firstSchedule.LiveArrival != null && referenceTime != null)
                        entryDelay = Math.Max(0, (firstSchedule.LiveArrival - referenceTime).TotalMinutes);
                    else
                        continue;

                    if (lastSchedule.Schedule.Handling == eHandling.Destination)
                    {
                        if (lastSchedule.LiveArrival != null && lastSchedule.Schedule.Arrival != null)
                            exitDelay = Math.Max(0, (firstSchedule.LiveArrival - lastSchedule.Schedule.Arrival).TotalMinutes);
                        else
                            continue;
                    }
                    else
                    {
                        if (lastSchedule.LiveDeparture != null && lastSchedule.Schedule.Departure != null)
                            exitDelay = Math.Max(0, (firstSchedule.LiveDeparture - lastSchedule.Schedule.Departure).TotalMinutes);
                        else
                            continue;
                    }

                    var netDelay = exitDelay - entryDelay;
                    result.DelayTotalNet += netDelay;

                    if (netDelay < 0)
                        result.DelayReduced += netDelay;
                    else
                        result.DelayAdded += netDelay;
                }

                var allSchedules = trains.SelectMany(t => t.Schedules.Where(s => s.Schedule.Station.ESTW == estw)).ToList();

                var arrivals = allSchedules.Where(s => s.LiveArrival != null && s.Schedule.Arrival != null)
                                           .Select(s => new
                                           {
                                               FrameStartTime = new LeibitTime(s.LiveArrival.Hour, (s.LiveArrival.Minute / frameSize) * frameSize),
                                               TrainNumber = s.Schedule.Train.Number,
                                               Delay = Math.Max(0, (s.LiveArrival - s.Schedule.Arrival).TotalMinutes),
                                           });

                var departures = allSchedules.Where(s => s.LiveDeparture != null && s.Schedule.Departure != null)
                                             .Select(s => new
                                             {
                                                 FrameStartTime = new LeibitTime(s.LiveDeparture.Hour, (s.LiveDeparture.Minute / frameSize) * frameSize),
                                                 TrainNumber = s.Schedule.Train.Number,
                                                 Delay = Math.Max(0, (s.LiveDeparture - s.Schedule.Departure).TotalMinutes),
                                             });

                var frames = arrivals.Union(departures).GroupBy(x => x.FrameStartTime).OrderBy(x => x.Key).ToList();

                foreach (var frame in frames)
                {
                    var resultFrame = new StatisticsTimeFrame();
                    resultFrame.StartTime = frame.Key;
                    resultFrame.EndTime = frame.Key.AddMinutes(frameSize);

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

                result.TrainsWithGreatestDelays = trains.Where(t => t.Delay > 0).OrderByDescending(t => t.Delay).Take(10).ToList();

                result.StationsWithGreatestDelays = allSchedules.SelectMany(s => s.Delays)
                                                                .GroupBy(d => d.Schedule.Schedule.Station)
                                                                .Select(x => new StationDelayStatistic { Station = x.Key, DelaySum = x.Sum(y => y.Minutes) })
                                                                .OrderByDescending(x => x.DelaySum)
                                                                .Take(10)
                                                                .ToList();

                return OperationResult<HistoricalStatistics>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<HistoricalStatistics>.Fail(ex.ToString());
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
            result.NumberOfEarlyTrains = trains.Count(t => t.Delay < -5);
            result.NumberOfTrainsOnTime = trains.Count(t => t.Delay >= -5 && t.Delay <= 5);
            result.NumberOfTrainsWithShortDelay = trains.Count(t => t.Delay > 5 && t.Delay <= 10);
            result.NumberOfTrainsWithLongDelay = trains.Count(t => t.Delay > 10);
            result.TrainWithGreatestDelay = trains.OrderByDescending(t => t.Delay).FirstOrDefault(t => t.Delay > 0);

            if (result.NumberOfTrains > 0)
                result.OnTimeRate = 100 * (result.NumberOfTrainsOnTime + result.NumberOfEarlyTrains) / result.NumberOfTrains;

            return result;
        }
        #endregion

        #endregion
    }
}
