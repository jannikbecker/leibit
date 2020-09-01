using Leibit.BLL;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Leibit.Tests
{
    [TestClass]
    public class CalculationBLLTest
    {

        #region [CalculationBLLTest_CalculateDelay_Unscheduled]
        [TestMethod]
        public void CalculationBLLTest_CalculateDelay_Unscheduled()
        {
            var bll = new CalculationBLL();
            var estw = new ESTW("meep", "bla", string.Empty, null);

            var train = new Train(4711);
            var liveTrain = new TrainInformation(train);

            var station1 = new Station("Bahnhof A", "A", 1, string.Empty, string.Empty, estw);
            var station2 = new Station("Bahnhof B", "B", 2, string.Empty, string.Empty, estw);
            var station3 = new Station("Bahnhof C", "C", 3, string.Empty, string.Empty, estw);
            var track1 = new Track("dummy", true, true, station1, null);
            var track2 = new Track("dummy", true, true, station2, null);
            var track3 = new Track("dummy", true, true, station3, null);

            var schedule1 = new Schedule(train, null, new LeibitTime(8, 20), track1, new List<eDaysOfService> { eDaysOfService.Tuesday }, eScheduleDirection.LeftToRight, eHandling.Transit, string.Empty, null);
            var schedule2 = new Schedule(train, null, new LeibitTime(8, 30), track2, new List<eDaysOfService> { eDaysOfService.Tuesday }, eScheduleDirection.LeftToRight, eHandling.Transit, string.Empty, null);
            train.AddSchedule(schedule1);
            train.AddSchedule(schedule2);

            var liveSchedule1 = new LiveSchedule(liveTrain, schedule1);
            var liveSchedule2 = new LiveSchedule(liveTrain, schedule2);
            liveTrain.AddSchedule(liveSchedule1);
            liveTrain.AddSchedule(liveSchedule2);

            liveSchedule1.LiveArrival = new LeibitTime(8, 21);
            liveSchedule1.LiveDeparture = new LeibitTime(8, 22);

            estw.Time = new LeibitTime(eDaysOfService.Tuesday, 8, 25);
            var result = bll.CalculateDelay(liveTrain, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, result.Result);

            liveTrain.Delay = result.Result.Value;
            estw.Time = new LeibitTime(eDaysOfService.Tuesday, 8, 39);

            var liveSchedule3 = new LiveSchedule(liveTrain, station3);
            liveTrain.AddSchedule(liveSchedule3);
            train.AddSchedule(liveSchedule3.Schedule);

            liveSchedule3.LiveArrival = estw.Time;
            liveSchedule3.LiveDeparture = estw.Time;

            Assert.AreEqual(new LeibitTime(eDaysOfService.Tuesday, 8, 37), liveSchedule3.Schedule.Arrival);
            Assert.AreEqual(new LeibitTime(eDaysOfService.Tuesday, 8, 37), liveSchedule3.Schedule.Departure);

            result = bll.CalculateDelay(liveTrain, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, result.Result);

            estw.Time = new LeibitTime(eDaysOfService.Tuesday, 8, 45);
            liveSchedule2.LiveArrival = estw.Time;
            liveSchedule2.LiveDeparture = estw.Time;

            result = bll.CalculateDelay(liveTrain, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(15, result.Result);
        }
        #endregion

    }
}
