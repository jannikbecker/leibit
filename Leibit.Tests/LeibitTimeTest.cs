using Leibit.Core.Scheduling;
using Leibit.Tests.Comparer;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Leibit.Tests
{
    [TestClass]
    public class LeibitTimeTest
    {

        #region - AddDays -

        [TestMethod]
        public void LeibitTimeTest_AddDays1()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 13, 7);
            time = time.AddDays(3);

            Assert.AreEqual(eDaysOfService.Friday, time.Day);
            Assert.AreEqual(13, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddDays2()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            time = time.AddDays(10);

            Assert.AreEqual(eDaysOfService.Sunday, time.Day);
            Assert.AreEqual(13, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddDays3()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            time = time.AddDays(-3);

            Assert.AreEqual(eDaysOfService.Monday, time.Day);
            Assert.AreEqual(13, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddDays4()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Thursday, 13, 7);
            time = time.AddDays(-9);

            Assert.AreEqual(eDaysOfService.Tuesday, time.Day);
            Assert.AreEqual(13, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        //[TestMethod]
        //public void LeibitTimeTest_AddDays5()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Wednesday | eDaysOfService.Thursday, 13, 7);
        //    time = time.AddDays(-2);

        //    Assert.AreEqual(eDaysOfService.Saturday | eDaysOfService.Monday | eDaysOfService.Tuesday, time.Day);
        //    Assert.AreEqual(13, time.Hour);
        //    Assert.AreEqual(7, time.Minute);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_AddDays6()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Tuesday | eDaysOfService.Wednesday | eDaysOfService.Thursday, 13, 7);
        //    time = time.AddDays(-16);

        //    Assert.AreEqual(eDaysOfService.Saturday | eDaysOfService.Sunday | eDaysOfService.Monday | eDaysOfService.Tuesday, time.Day);
        //    Assert.AreEqual(13, time.Hour);
        //    Assert.AreEqual(7, time.Minute);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_AddDays7()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Tuesday | eDaysOfService.Sunday, 13, 7);
        //    time = time.AddDays(2);

        //    Assert.AreEqual(eDaysOfService.Tuesday | eDaysOfService.Wednesday | eDaysOfService.Thursday, time.Day);
        //    Assert.AreEqual(13, time.Hour);
        //    Assert.AreEqual(7, time.Minute);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_AddDays8()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Tuesday | eDaysOfService.Saturday | eDaysOfService.Sunday, 13, 7);
        //    time = time.AddDays(9);

        //    Assert.AreEqual(eDaysOfService.Monday | eDaysOfService.Tuesday | eDaysOfService.Wednesday | eDaysOfService.Thursday, time.Day);
        //    Assert.AreEqual(13, time.Hour);
        //    Assert.AreEqual(7, time.Minute);
        //}

        #endregion

        #region - AddHours -

        [TestMethod]
        public void LeibitTimeTest_AddHours1()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 13, 7);
            time = time.AddHours(5);

            Assert.AreEqual(eDaysOfService.Tuesday, time.Day);
            Assert.AreEqual(18, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddHours2()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 13, 7);
            time = time.AddHours(50);

            Assert.AreEqual(eDaysOfService.Thursday, time.Day);
            Assert.AreEqual(15, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddHours3()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 13, 7);
            time = time.AddHours(-6);

            Assert.AreEqual(eDaysOfService.Tuesday, time.Day);
            Assert.AreEqual(7, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddHours4()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 13, 7);
            time = time.AddHours(-75);

            Assert.AreEqual(eDaysOfService.Saturday, time.Day);
            Assert.AreEqual(10, time.Hour);
            Assert.AreEqual(7, time.Minute);
        }

        //[TestMethod]
        //public void LeibitTimeTest_AddHours5()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Tuesday | eDaysOfService.Friday | eDaysOfService.Sunday, 13, 7);
        //    time = time.AddHours(14);

        //    Assert.AreEqual(eDaysOfService.Wednesday | eDaysOfService.Saturday | eDaysOfService.Monday, time.Day);
        //    Assert.AreEqual(3, time.Hour);
        //    Assert.AreEqual(7, time.Minute);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_AddHours6()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Tuesday | eDaysOfService.Friday | eDaysOfService.Sunday, 13, 7);
        //    time = time.AddHours(-80);

        //    Assert.AreEqual(eDaysOfService.Saturday | eDaysOfService.Tuesday | eDaysOfService.Thursday, time.Day);
        //    Assert.AreEqual(5, time.Hour);
        //    Assert.AreEqual(7, time.Minute);
        //}

        #endregion

        #region - AddMinutes -

        [TestMethod]
        public void LeibitTimeTest_AddMinutes1()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 13, 7);
            time = time.AddMinutes(45);

            Assert.AreEqual(eDaysOfService.Tuesday, time.Day);
            Assert.AreEqual(13, time.Hour);
            Assert.AreEqual(52, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddMinutes2()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 13, 7);
            time = time.AddMinutes(156);

            Assert.AreEqual(eDaysOfService.Tuesday, time.Day);
            Assert.AreEqual(15, time.Hour);
            Assert.AreEqual(43, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddMinutes3()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Tuesday, 23, 7);
            time = time.AddMinutes(105);

            Assert.AreEqual(eDaysOfService.Wednesday, time.Day);
            Assert.AreEqual(0, time.Hour);
            Assert.AreEqual(52, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddMinutes4()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Sunday, 23, 7);
            time = time.AddMinutes(55);

            Assert.AreEqual(eDaysOfService.Monday, time.Day);
            Assert.AreEqual(0, time.Hour);
            Assert.AreEqual(2, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddMinutes5()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Sunday, 23, 50);
            time = time.AddMinutes(-27);

            Assert.AreEqual(eDaysOfService.Sunday, time.Day);
            Assert.AreEqual(23, time.Hour);
            Assert.AreEqual(23, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddMinutes6()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Friday, 23, 7);
            time = time.AddMinutes(-63);

            Assert.AreEqual(eDaysOfService.Friday, time.Day);
            Assert.AreEqual(22, time.Hour);
            Assert.AreEqual(4, time.Minute);
        }

        [TestMethod]
        public void LeibitTimeTest_AddMinutes7()
        {
            LeibitTime time = new LeibitTime(eDaysOfService.Monday, 1, 7);
            time = time.AddMinutes(-153);

            Assert.AreEqual(eDaysOfService.Sunday, time.Day);
            Assert.AreEqual(22, time.Hour);
            Assert.AreEqual(34, time.Minute);
        }

        //[TestMethod]
        //public void LeibitTimeTest_AddMinutes8()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Thursday, 1, 7);
        //    time = time.AddMinutes(-153);

        //    Assert.AreEqual(eDaysOfService.Sunday | eDaysOfService.Wednesday, time.Day);
        //    Assert.AreEqual(22, time.Hour);
        //    Assert.AreEqual(34, time.Minute);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_AddMinutes9()
        //{
        //    LeibitTime time = new LeibitTime(eDaysOfService.Wednesday | eDaysOfService.Saturday, 22, 54);
        //    time = time.AddMinutes(1680);

        //    Assert.AreEqual(eDaysOfService.Monday | eDaysOfService.Friday, time.Day);
        //    Assert.AreEqual(2, time.Hour);
        //    Assert.AreEqual(54, time.Minute);
        //}

        #endregion

        #region - Compare -

        [TestMethod]
        public void LeibitTimeTest_Compare1()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);

            Assert.IsTrue(time1 == time2);
            Assert.IsFalse(time1 != time2);
            Assert.IsFalse(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsTrue(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare2()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Tuesday, 16, 21);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsTrue(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsFalse(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare3()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Tuesday, 16, 33);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsFalse(time1 < time2);
            Assert.IsTrue(time1 > time2);
            Assert.IsFalse(time1 <= time2);
            Assert.IsTrue(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare4()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Tuesday, 14, 24);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsTrue(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsFalse(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare5()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Tuesday, 17, 24);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsFalse(time1 < time2);
            Assert.IsTrue(time1 > time2);
            Assert.IsFalse(time1 <= time2);
            Assert.IsTrue(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare6()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Monday, 16, 24);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsTrue(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsFalse(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare7()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Thursday, 16, 24);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsFalse(time1 < time2);
            Assert.IsTrue(time1 > time2);
            Assert.IsFalse(time1 <= time2);
            Assert.IsTrue(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare8()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Monday, 16, 24);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Sunday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsFalse(time1 < time2);
            Assert.IsTrue(time1 > time2);
            Assert.IsFalse(time1 <= time2);
            Assert.IsTrue(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare9()
        {
            LeibitTime time1 = new LeibitTime(15, 19);
            LeibitTime time2 = new LeibitTime(16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsTrue(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsFalse(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare10()
        {
            LeibitTime time1 = new LeibitTime(23, 57);
            LeibitTime time2 = new LeibitTime(0, 5);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsTrue(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsFalse(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare11()
        {
            LeibitTime time1 = new LeibitTime(10, 17);
            LeibitTime time2 = new LeibitTime(22, 16);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsTrue(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsFalse(time1 >= time2);
        }

        [TestMethod]
        public void LeibitTimeTest_Compare12()
        {
            LeibitTime time1 = new LeibitTime(eDaysOfService.Thursday, 2, 43);
            LeibitTime time2 = new LeibitTime(eDaysOfService.Thursday, 16, 24);

            Assert.IsFalse(time1 == time2);
            Assert.IsTrue(time1 != time2);
            Assert.IsTrue(time1 < time2);
            Assert.IsFalse(time1 > time2);
            Assert.IsTrue(time1 <= time2);
            Assert.IsFalse(time1 >= time2);
        }

        //[TestMethod]
        //public void LeibitTimeTest_Compare9()
        //{
        //    LeibitTime time1 = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Sunday, 16, 24);
        //    LeibitTime time2 = new LeibitTime(eDaysOfService.Sunday | eDaysOfService.Saturday, 16, 24);

        //    Assert.IsFalse(time1 == time2);
        //    Assert.IsTrue(time1 != time2);
        //    Assert.IsFalse(time1 < time2);
        //    Assert.IsTrue(time1 > time2);
        //    Assert.IsFalse(time1 <= time2);
        //    Assert.IsTrue(time1 >= time2);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_Compare10()
        //{
        //    LeibitTime time1 = new LeibitTime(eDaysOfService.Sunday | eDaysOfService.Monday, 23, 24);
        //    LeibitTime time2 = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Tuesday, 2, 24);

        //    Assert.IsFalse(time1 == time2);
        //    Assert.IsTrue(time1 != time2);
        //    Assert.IsTrue(time1 < time2);
        //    Assert.IsFalse(time1 > time2);
        //    Assert.IsTrue(time1 <= time2);
        //    Assert.IsFalse(time1 >= time2);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_Compare11()
        //{
        //    LeibitTime time1 = new LeibitTime(eDaysOfService.Tuesday | eDaysOfService.Wednesday, 23, 24);
        //    LeibitTime time2 = new LeibitTime(eDaysOfService.Wednesday | eDaysOfService.Thursday, 2, 24);

        //    Assert.IsFalse(time1 == time2);
        //    Assert.IsTrue(time1 != time2);
        //    Assert.IsTrue(time1 < time2);
        //    Assert.IsFalse(time1 > time2);
        //    Assert.IsTrue(time1 <= time2);
        //    Assert.IsFalse(time1 >= time2);
        //}

        //[TestMethod]
        //public void LeibitTimeTest_Compare12()
        //{
        //    LeibitTime time1 = new LeibitTime(eDaysOfService.Monday | eDaysOfService.Wednesday | eDaysOfService.Friday, 12, 05);
        //    LeibitTime time2 = new LeibitTime(eDaysOfService.Tuesday | eDaysOfService.Thursday | eDaysOfService.Saturday, 12, 05);

        //    Assert.IsFalse(time1 == time2);
        //    Assert.IsTrue(time1 != time2);
        //    Assert.IsTrue(time1 < time2);
        //    Assert.IsFalse(time1 > time2);
        //    Assert.IsTrue(time1 <= time2);
        //    Assert.IsFalse(time1 >= time2);
        //}

        #endregion

        #region - Subtract -

        [TestMethod]
        public void LeibitTimeTest_Subtract1()
        {
            LeibitTime time1 = new LeibitTime(13, 52);
            LeibitTime time2 = new LeibitTime(13, 24);
            LeibitTime result = time1 - time2;

            Assert.AreEqual(0, result.Hour);
            Assert.AreEqual(28, result.Minute);
            Assert.AreEqual(28, result.TotalMinutes);
        }

        [TestMethod]
        public void LeibitTimeTest_Subtract2()
        {
            LeibitTime time1 = new LeibitTime(13, 52);
            LeibitTime time2 = new LeibitTime(10, 36);
            LeibitTime result = time1 - time2;

            Assert.AreEqual(3, result.Hour);
            Assert.AreEqual(16, result.Minute);
            Assert.AreEqual(196, result.TotalMinutes);
        }

        #endregion

        #region - Parsing -

        [TestMethod]
        public void LeibitTimeTest_ParseDays1()
        {
            var Expected = new List<eDaysOfService> { eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Thursday, eDaysOfService.Saturday };
            var Actual = LeibitTime.ParseDays("Di-Do+Sa");
            LeibitComparer<eDaysOfService>.CompareList(Expected, Actual, DayComparer.Instance, "Days");
        }

        [TestMethod]
        public void LeibitTimeTest_ParseDays2()
        {
            var Expected = new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday, eDaysOfService.Saturday, eDaysOfService.Sunday };
            var Actual = LeibitTime.ParseDays("Sa-Mi");
            LeibitComparer<eDaysOfService>.CompareList(Expected, Actual, DayComparer.Instance, "Days");
        }

        [TestMethod]
        public void LeibitTimeTest_ParseDays3()
        {
            var Expected = new List<eDaysOfService> { eDaysOfService.Monday, eDaysOfService.Tuesday, eDaysOfService.Wednesday };
            var Actual = LeibitTime.ParseDays("Mo-Mi+Ma");
            LeibitComparer<eDaysOfService>.CompareList(Expected, Actual, DayComparer.Instance, "Days");
        }

        #endregion

    }
}
