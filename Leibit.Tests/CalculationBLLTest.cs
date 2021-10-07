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

        #region [CalculationBLLTest_SortSchedules_NormalCase]
        [TestMethod]
        public void CalculationBLLTest_SortSchedules_NormalCase()
        {
            var estw = new ESTW("meep", "bla", string.Empty, null);

            var station1 = new Station("Bahnhof A", "A", 1, string.Empty, string.Empty, estw);
            var station2 = new Station("Bahnhof B", "B", 2, string.Empty, string.Empty, estw);
            var station3 = new Station("Bahnhof C", "C", 3, string.Empty, string.Empty, estw);

            var track1 = new Track("dummy", true, true, station1, null);
            var track2 = new Track("dummy", true, true, station2, null);
            var track3 = new Track("dummy", true, true, station3, null);

            var train = new Train(4711);

            var schedule1 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 1),
                                         track: track1,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule2 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 5),
                                         track: track2,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule3 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 9),
                                         track: track3,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var liveTrain = new TrainInformation(train);

            var liveSchedule1 = new LiveSchedule(liveTrain, schedule1);
            var liveSchedule2 = new LiveSchedule(liveTrain, schedule2);
            var liveSchedule3 = new LiveSchedule(liveTrain, schedule3);

            liveTrain.AddSchedule(liveSchedule1);
            liveTrain.AddSchedule(liveSchedule2);
            liveTrain.AddSchedule(liveSchedule3);

            liveSchedule1.LiveArrival = new LeibitTime(eDaysOfService.Monday, 16, 2);
            liveSchedule1.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 16, 3);

            Assert.AreEqual(0, liveTrain.Schedules.IndexOf(liveSchedule1));
            Assert.AreEqual(1, liveTrain.Schedules.IndexOf(liveSchedule2));
            Assert.AreEqual(2, liveTrain.Schedules.IndexOf(liveSchedule3));
        }
        #endregion

        #region [CalculationBLLTest_SortSchedules_DelayedTrain]
        [TestMethod]
        public void CalculationBLLTest_SortSchedules_DelayedTrain()
        {
            var estw = new ESTW("meep", "bla", string.Empty, null);

            var station1 = new Station("Bahnhof A", "A", 1, string.Empty, string.Empty, estw);
            var station2 = new Station("Bahnhof B", "B", 2, string.Empty, string.Empty, estw);
            var station3 = new Station("Bahnhof C", "C", 3, string.Empty, string.Empty, estw);

            var track1 = new Track("dummy", true, true, station1, null);
            var track2 = new Track("dummy", true, true, station2, null);
            var track3 = new Track("dummy", true, true, station3, null);

            var train = new Train(4711);

            var schedule1 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 1),
                                         track: track1,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule2 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 5),
                                         track: track2,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule3 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 9),
                                         track: track3,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var liveTrain = new TrainInformation(train);

            var liveSchedule1 = new LiveSchedule(liveTrain, schedule1);
            var liveSchedule2 = new LiveSchedule(liveTrain, schedule2);
            var liveSchedule3 = new LiveSchedule(liveTrain, schedule3);

            liveTrain.AddSchedule(liveSchedule1);
            liveTrain.AddSchedule(liveSchedule2);
            liveTrain.AddSchedule(liveSchedule3);

            liveSchedule1.LiveArrival = new LeibitTime(eDaysOfService.Monday, 16, 6);
            liveSchedule1.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 16, 7);

            Assert.AreEqual(0, liveTrain.Schedules.IndexOf(liveSchedule1));
            Assert.AreEqual(1, liveTrain.Schedules.IndexOf(liveSchedule2));
            Assert.AreEqual(2, liveTrain.Schedules.IndexOf(liveSchedule3));
        }
        #endregion

        #region [CalculationBLLTest_SortSchedules_FirstStationMissing]
        [TestMethod]
        public void CalculationBLLTest_SortSchedules_FirstStationMissing()
        {
            var estw = new ESTW("meep", "bla", string.Empty, null);

            var station1 = new Station("Bahnhof A", "A", 1, string.Empty, string.Empty, estw);
            var station2 = new Station("Bahnhof B", "B", 2, string.Empty, string.Empty, estw);
            var station3 = new Station("Bahnhof C", "C", 3, string.Empty, string.Empty, estw);

            var track1 = new Track("dummy", true, true, station1, null);
            var track2 = new Track("dummy", true, true, station2, null);
            var track3 = new Track("dummy", true, true, station3, null);

            var train = new Train(4711);

            var schedule1 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 1),
                                         track: track1,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule2 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 5),
                                         track: track2,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule3 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 9),
                                         track: track3,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var liveTrain = new TrainInformation(train);

            var liveSchedule1 = new LiveSchedule(liveTrain, schedule1);
            var liveSchedule2 = new LiveSchedule(liveTrain, schedule2);
            var liveSchedule3 = new LiveSchedule(liveTrain, schedule3);

            liveTrain.AddSchedule(liveSchedule1);
            liveTrain.AddSchedule(liveSchedule2);
            liveTrain.AddSchedule(liveSchedule3);

            liveSchedule2.LiveArrival = new LeibitTime(eDaysOfService.Monday, 16, 6);
            liveSchedule2.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 16, 7);

            Assert.AreEqual(0, liveTrain.Schedules.IndexOf(liveSchedule1));
            Assert.AreEqual(1, liveTrain.Schedules.IndexOf(liveSchedule2));
            Assert.AreEqual(2, liveTrain.Schedules.IndexOf(liveSchedule3));
        }
        #endregion

        #region [CalculationBLLTest_SortSchedules_DifferentOrder]
        [TestMethod]
        public void CalculationBLLTest_SortSchedules_DifferentOrder()
        {
            var estw = new ESTW("meep", "bla", string.Empty, null);

            var station1 = new Station("Bahnhof A", "A", 1, string.Empty, string.Empty, estw);
            var station2 = new Station("Bahnhof B", "B", 2, string.Empty, string.Empty, estw);
            var station3 = new Station("Bahnhof C", "C", 3, string.Empty, string.Empty, estw);

            var track1 = new Track("dummy", true, true, station1, null);
            var track2 = new Track("dummy", true, true, station2, null);
            var track3 = new Track("dummy", true, true, station3, null);

            var train = new Train(4711);

            var schedule1 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 1),
                                         track: track1,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule2 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 5),
                                         track: track2,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule3 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 9),
                                         track: track3,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var liveTrain = new TrainInformation(train);

            var liveSchedule1 = new LiveSchedule(liveTrain, schedule1);
            var liveSchedule2 = new LiveSchedule(liveTrain, schedule2);
            var liveSchedule3 = new LiveSchedule(liveTrain, schedule3);

            liveTrain.AddSchedule(liveSchedule1);
            liveTrain.AddSchedule(liveSchedule2);
            liveTrain.AddSchedule(liveSchedule3);

            liveSchedule1.LiveArrival = new LeibitTime(eDaysOfService.Monday, 16, 2);
            liveSchedule1.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 16, 3);
            liveSchedule3.LiveArrival = new LeibitTime(eDaysOfService.Monday, 16, 10);
            liveSchedule3.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 16, 11);
            liveSchedule2.LiveArrival = new LeibitTime(eDaysOfService.Monday, 16, 14);
            liveSchedule2.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 16, 15);

            Assert.AreEqual(0, liveTrain.Schedules.IndexOf(liveSchedule1));
            Assert.AreEqual(1, liveTrain.Schedules.IndexOf(liveSchedule3));
            Assert.AreEqual(2, liveTrain.Schedules.IndexOf(liveSchedule2));
        }
        #endregion

        #region [CalculationBLLTest_SortSchedules_StationSkipped]
        [TestMethod]
        public void CalculationBLLTest_SortSchedules_StationSkipped()
        {
            var estw = new ESTW("meep", "bla", string.Empty, null);

            var station1 = new Station("Bahnhof A", "A", 1, string.Empty, string.Empty, estw);
            var station2 = new Station("Bahnhof B", "B", 2, string.Empty, string.Empty, estw);
            var station3 = new Station("Bahnhof C", "C", 3, string.Empty, string.Empty, estw);

            var track1 = new Track("dummy", true, true, station1, null);
            var track2 = new Track("dummy", true, true, station2, null);
            var track3 = new Track("dummy", true, true, station3, null);

            var train = new Train(4711);

            var schedule1 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 1),
                                         track: track1,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule2 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 5),
                                         track: track2,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var schedule3 = new Schedule(train: train,
                                         arrival: null,
                                         departure: new LeibitTime(eDaysOfService.Monday, 16, 9),
                                         track: track3,
                                         days: new List<eDaysOfService> { eDaysOfService.Monday },
                                         direction: eScheduleDirection.LeftToRight,
                                         handling: eHandling.Transit,
                                         remark: string.Empty);

            var liveTrain = new TrainInformation(train);

            var liveSchedule1 = new LiveSchedule(liveTrain, schedule1);
            var liveSchedule2 = new LiveSchedule(liveTrain, schedule2);
            var liveSchedule3 = new LiveSchedule(liveTrain, schedule3);

            liveTrain.AddSchedule(liveSchedule1);
            liveTrain.AddSchedule(liveSchedule2);
            liveTrain.AddSchedule(liveSchedule3);

            liveSchedule1.LiveArrival = new LeibitTime(eDaysOfService.Monday, 15, 55);
            liveSchedule1.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 15, 56);
            liveSchedule3.LiveArrival = new LeibitTime(eDaysOfService.Monday, 16, 2);
            liveSchedule3.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 16, 3);

            Assert.AreEqual(0, liveTrain.Schedules.IndexOf(liveSchedule1));
            Assert.AreEqual(1, liveTrain.Schedules.IndexOf(liveSchedule2));
            Assert.AreEqual(2, liveTrain.Schedules.IndexOf(liveSchedule3));
        }
        #endregion

        #region [CalculationBLLTest_GetFollowUpService_SimpleCase]
        [TestMethod]
        public void CalculationBLLTest_GetFollowUpService_SimpleCase()
        {
            var estw = new ESTW("ID", "Name", string.Empty, null);
            var train = new Train(1);

            var relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Monday);
            relation.Days.Add(eDaysOfService.Tuesday);
            relation.Days.Add(eDaysOfService.Wednesday);
            relation.Days.Add(eDaysOfService.Thursday);
            relation.Days.Add(eDaysOfService.Friday);
            relation.Days.Add(eDaysOfService.Saturday);
            relation.Days.Add(eDaysOfService.Sunday);
            train.FollowUpServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetFollowUpService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetFollowUpService_SameDay]
        [TestMethod]
        public void CalculationBLLTest_GetFollowUpService_SameDay()
        {
            var estw = new ESTW("ID", "Name", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Thursday, 14, 45);

            var train = new Train(0);
            new Schedule(train, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);

            var relation = new TrainRelation(1);
            relation.Days.Add(eDaysOfService.Monday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Tuesday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(3);
            relation.Days.Add(eDaysOfService.Wednesday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(4);
            relation.Days.Add(eDaysOfService.Thursday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(5);
            relation.Days.Add(eDaysOfService.Friday);
            train.FollowUpServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetFollowUpService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(4, result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetFollowUpService_WrongDay]
        [TestMethod]
        public void CalculationBLLTest_GetFollowUpService_WrongDay()
        {
            var estw = new ESTW("ID", "Name", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Saturday, 14, 45);

            var train = new Train(1);
            new Schedule(train, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);

            var relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Monday);
            relation.Days.Add(eDaysOfService.Tuesday);
            relation.Days.Add(eDaysOfService.Wednesday);
            relation.Days.Add(eDaysOfService.Thursday);
            relation.Days.Add(eDaysOfService.Friday);
            train.FollowUpServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetFollowUpService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.IsNull(result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetFollowUpService_AfterMidnight]
        [TestMethod]
        public void CalculationBLLTest_GetFollowUpService_AfterMidnight()
        {
            var estw = new ESTW("ID", "Name", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Thursday, 0, 5);

            var train = new Train(0);
            new Schedule(train, new LeibitTime(23, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);

            var relation = new TrainRelation(1);
            relation.Days.Add(eDaysOfService.Monday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Tuesday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(3);
            relation.Days.Add(eDaysOfService.Wednesday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(4);
            relation.Days.Add(eDaysOfService.Thursday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(5);
            relation.Days.Add(eDaysOfService.Friday);
            train.FollowUpServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetFollowUpService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(3, result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetFollowUpService_BeforeMidnight]
        [TestMethod]
        public void CalculationBLLTest_GetFollowUpService_BeforeMidnight()
        {
            var estw = new ESTW("ID", "Name", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Thursday, 23, 55);

            var train = new Train(0);
            new Schedule(train, new LeibitTime(0, 5), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);

            var relation = new TrainRelation(1);
            relation.Days.Add(eDaysOfService.Monday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Tuesday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(3);
            relation.Days.Add(eDaysOfService.Wednesday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(4);
            relation.Days.Add(eDaysOfService.Thursday);
            train.FollowUpServices.Add(relation);

            relation = new TrainRelation(5);
            relation.Days.Add(eDaysOfService.Friday);
            train.FollowUpServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetFollowUpService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(5, result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetPreviousService_SimpleCase]
        [TestMethod]
        public void CalculationBLLTest_GetPreviousService_SimpleCase()
        {
            var estw = new ESTW("ID", "Name", string.Empty, null);
            var train = new Train(1);

            var relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Monday);
            relation.Days.Add(eDaysOfService.Tuesday);
            relation.Days.Add(eDaysOfService.Wednesday);
            relation.Days.Add(eDaysOfService.Thursday);
            relation.Days.Add(eDaysOfService.Friday);
            relation.Days.Add(eDaysOfService.Saturday);
            relation.Days.Add(eDaysOfService.Sunday);
            train.PreviousServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetPreviousService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(2, result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetPreviousService_SameDay]
        [TestMethod]
        public void CalculationBLLTest_GetPreviousService_SameDay()
        {
            var area = new Area("ID", "Name");
            var estw = new ESTW("ID", "Name", string.Empty, area);
            estw.Time = new LeibitTime(eDaysOfService.Thursday, 15, 5);

            var train = new Train(0);

            var train1 = new Train(1);
            new Schedule(train1, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(1, train1);

            var train2 = new Train(2);
            new Schedule(train2, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(2, train2);

            var train3 = new Train(3);
            new Schedule(train3, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(3, train3);

            var train4 = new Train(4);
            new Schedule(train4, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(4, train4);

            var train5 = new Train(5);
            new Schedule(train5, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(5, train5);

            var relation = new TrainRelation(1);
            relation.Days.Add(eDaysOfService.Monday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Tuesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(3);
            relation.Days.Add(eDaysOfService.Wednesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(4);
            relation.Days.Add(eDaysOfService.Thursday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(5);
            relation.Days.Add(eDaysOfService.Friday);
            train.PreviousServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetPreviousService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(4, result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetPreviousService_WrongDay]
        [TestMethod]
        public void CalculationBLLTest_GetPreviousService_WrongDay()
        {
            var area = new Area("ID", "Name");
            var estw = new ESTW("ID", "Name", string.Empty, area);
            estw.Time = new LeibitTime(eDaysOfService.Saturday, 15, 5);

            var train = new Train(0);

            var train1 = new Train(1);
            new Schedule(train1, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(1, train1);

            var train2 = new Train(2);
            new Schedule(train2, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(2, train2);

            var train3 = new Train(3);
            new Schedule(train3, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(3, train3);

            var train4 = new Train(4);
            new Schedule(train4, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(4, train4);

            var train5 = new Train(5);
            new Schedule(train5, new LeibitTime(14, 56), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(5, train5);

            var relation = new TrainRelation(1);
            relation.Days.Add(eDaysOfService.Monday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Tuesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(3);
            relation.Days.Add(eDaysOfService.Wednesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(4);
            relation.Days.Add(eDaysOfService.Thursday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(5);
            relation.Days.Add(eDaysOfService.Friday);
            train.PreviousServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetPreviousService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.IsNull(result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetPreviousService_AfterMidnight]
        [TestMethod]
        public void CalculationBLLTest_GetPreviousService_AfterMidnight()
        {
            var area = new Area("ID", "Name");
            var estw = new ESTW("ID", "Name", string.Empty, area);
            estw.Time = new LeibitTime(eDaysOfService.Thursday, 0, 5);

            var train = new Train(0);

            var train1 = new Train(1);
            new Schedule(train1, new LeibitTime(23, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(1, train1);

            var train2 = new Train(2);
            new Schedule(train2, new LeibitTime(23, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(2, train2);

            var train3 = new Train(3);
            new Schedule(train3, new LeibitTime(23, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(3, train3);

            var train4 = new Train(4);
            new Schedule(train4, new LeibitTime(23, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(4, train4);

            var train5 = new Train(5);
            new Schedule(train5, new LeibitTime(23, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(5, train5);

            var relation = new TrainRelation(1);
            relation.Days.Add(eDaysOfService.Monday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Tuesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(3);
            relation.Days.Add(eDaysOfService.Wednesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(4);
            relation.Days.Add(eDaysOfService.Thursday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(5);
            relation.Days.Add(eDaysOfService.Friday);
            train.PreviousServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetPreviousService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(3, result.Result);
        }
        #endregion

        #region [CalculationBLLTest_GetPreviousService_BeforeMidnight]
        [TestMethod]
        public void CalculationBLLTest_GetPreviousService_BeforeMidnight()
        {
            var area = new Area("ID", "Name");
            var estw = new ESTW("ID", "Name", string.Empty, area);
            estw.Time = new LeibitTime(eDaysOfService.Thursday, 23, 45);

            var train = new Train(0);

            var train1 = new Train(1);
            new Schedule(train1, new LeibitTime(0, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(1, train1);

            var train2 = new Train(2);
            new Schedule(train2, new LeibitTime(0, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(2, train2);

            var train3 = new Train(3);
            new Schedule(train3, new LeibitTime(0, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(3, train3);

            var train4 = new Train(4);
            new Schedule(train4, new LeibitTime(0, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(4, train4);

            var train5 = new Train(5);
            new Schedule(train5, new LeibitTime(0, 55), null, null, new List<eDaysOfService>(), eScheduleDirection.Unknown, eHandling.Destination, string.Empty);
            area.Trains.TryAdd(5, train5);

            var relation = new TrainRelation(1);
            relation.Days.Add(eDaysOfService.Monday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(2);
            relation.Days.Add(eDaysOfService.Tuesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(3);
            relation.Days.Add(eDaysOfService.Wednesday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(4);
            relation.Days.Add(eDaysOfService.Thursday);
            train.PreviousServices.Add(relation);

            relation = new TrainRelation(5);
            relation.Days.Add(eDaysOfService.Friday);
            train.PreviousServices.Add(relation);

            var bll = new CalculationBLL();
            var result = bll.GetPreviousService(train, estw);
            Assert.IsTrue(result.Succeeded);
            Assert.AreEqual(5, result.Result);
        }
        #endregion

    }
}
