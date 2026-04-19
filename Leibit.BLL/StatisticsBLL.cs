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
                result.NumberOfTrains = currentTrains.Count;

                var trainsOnWay = currentTrains.Where(t => __IsDeparted(t) && !__IsArrived(t));
                result.AverageDelay = Math.Round(trainsOnWay.Average(t => t.Delay), 2);
                result.TrainWithSmallestDelay = trainsOnWay.OrderBy(t => t.Delay).FirstOrDefault();
                result.TrainWithGreatestDelay = trainsOnWay.OrderByDescending(t => t.Delay).FirstOrDefault();
                result.NumberOfEarlyTrains = trainsOnWay.Count(t => t.Delay < -5);
                result.NumberOfTrainsOnTime = trainsOnWay.Count(t => t.Delay >= -5 && t.Delay <= 5);
                result.NumberOfTrainsWithShortDelay = trainsOnWay.Count(t => t.Delay > 5 && t.Delay <= 10);
                result.NumberOfTrainsWithLongDelay = trainsOnWay.Count(t => t.Delay > 10);

                return OperationResult<CurrentStatistics>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<CurrentStatistics>.Fail(ex.ToString());
            }
        }
        #endregion

        #region [GetHistoricalStatistics]
        public OperationResult<HistoricalStatistics> GetHistoricalStatistics(ESTW estw, int frameSize)
        {
            try
            {
                var result = new HistoricalStatistics();
                result.ESTW = estw;

                var trains = estw.Area.LiveTrains.Values.Where(t => t.LastModification.ContainsKey(estw.Id)).ToList();
                result.NumberOfTrains = trains.Count;

                var departedTrains = trains.Where(t => __IsDeparted(t));
                result.AverageDelay = Math.Round(departedTrains.Average(t => t.Delay), 2);
                result.TrainWithSmallestDelay = departedTrains.OrderBy(t => t.Delay).FirstOrDefault();
                result.TrainWithGreatestDelay = departedTrains.OrderByDescending(t => t.Delay).FirstOrDefault();
                result.NumberOfEarlyTrains = departedTrains.Count(t => t.Delay < -5);
                result.NumberOfTrainsOnTime = departedTrains.Count(t => t.Delay >= -5 && t.Delay <= 5);
                result.NumberOfTrainsWithShortDelay = departedTrains.Count(t => t.Delay > 5 && t.Delay <= 10);
                result.NumberOfTrainsWithLongDelay = departedTrains.Count(t => t.Delay > 10);

                var trainFrames = new Dictionary<LeibitTime, List<int>>();
                var delayFrames = new Dictionary<LeibitTime, List<int>>();

                foreach (var train in trains)
                {
                    foreach (var schedule in train.Schedules)
                    {
                        if (schedule.LiveDeparture != null && schedule.Schedule.Departure != null)
                            __AddToTimeFrame(train, schedule.LiveDeparture, schedule.Schedule.Departure, frameSize, trainFrames, delayFrames);
                        else if (schedule.LiveArrival != null && schedule.Schedule.Arrival != null)
                            __AddToTimeFrame(train, schedule.LiveArrival, schedule.Schedule.Arrival, frameSize, trainFrames, delayFrames);
                    }
                }

                foreach (var trainFrame in trainFrames.OrderBy(x => x.Key))
                {
                    var frame = new StatisticsTimeFrame();
                    frame.StartTime = trainFrame.Key;
                    frame.EndTime = trainFrame.Key.AddMinutes(frameSize);
                    frame.NumberOfTrains = trainFrame.Value.Count;
                    frame.AverageDelay = Math.Round(delayFrames[trainFrame.Key].Average(), 2);
                    result.TimeFrames.Add(frame);
                }

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

        #region [__IsArrived]
        private bool __IsArrived(TrainInformation train)
        {
            var arrivalSchedule = train.Schedules.FirstOrDefault(s => s.Schedule.Handling == eHandling.Destination);
            return arrivalSchedule != null && arrivalSchedule.IsArrived;
        }
        #endregion

        #region [__IsDeparted]
        private bool __IsDeparted(TrainInformation train)
        {
            var departureSchedule = train.Schedules.FirstOrDefault(s => s.Schedule.Handling == eHandling.Start);
            return departureSchedule == null || departureSchedule.IsDeparted;
        }
        #endregion

        #region [__AddToTimeFrame]
        private void __AddToTimeFrame(TrainInformation train, LeibitTime liveTime, LeibitTime scheduledTime, int frameSize, Dictionary<LeibitTime, List<int>> trainFrames, Dictionary<LeibitTime, List<int>> delayFrames)
        {
            var frameStartTime = new LeibitTime(liveTime.Hour, (liveTime.Minute / frameSize) * frameSize);

            if (!trainFrames.TryGetValue(frameStartTime, out var trainFrame))
                trainFrames[frameStartTime] = [train.Train.Number];
            else if (!trainFrame.Contains(train.Train.Number))
                trainFrame.Add(train.Train.Number);

            var delay = (liveTime - scheduledTime).TotalMinutes;

            if (delayFrames.TryGetValue(frameStartTime, out var delayFrame))
                delayFrame.Add(delay);
            else
                delayFrames[frameStartTime] = [delay];
        }
        #endregion

        #endregion
    }
}
