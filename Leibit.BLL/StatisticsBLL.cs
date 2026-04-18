using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Statistics;
using System;
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

        #endregion
    }
}
