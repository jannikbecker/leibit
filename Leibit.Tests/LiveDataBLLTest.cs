using Leibit.BLL;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.LiveData;
using Leibit.Tests.Comparer;
using Leibit.Tests.ExpectedData;
using Leibit.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace Leibit.Tests
{
    [TestClass]
    public class LiveDataBLLTest
    {

        #region - Needs -
        private LiveDataBLL m_Bll;
        #endregion

        #region - Singletons -

        #region [BLL]
        private LiveDataBLL BLL
        {
            get
            {
                if (m_Bll == null)
                    m_Bll = new LiveDataBLL();

                return m_Bll;
            }
        }
        #endregion

        #endregion

        #region - Test methods -

        #region [LiveDataBLLTest_TestParseTime]
        [TestMethod]
        public void LiveDataBLLTest_TestParseTime()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "ParseTime.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);

                DefaultChecks.IsOperationSucceeded(Result);
                Assert.AreEqual(new LeibitTime(eDaysOfService.Thursday, 3, 1), Estw.Time);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestPuntualTrain]
        [TestMethod]
        public void LiveDataBLLTest_TestPuntualTrain()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPunctualTrain/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestPunctualTrain(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestTrainDelayArrival]
        [TestMethod]
        public void LiveDataBLLTest_TestTrainDelayArrival()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(5, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(5, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayArrival/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestTrainDelayArrival(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestTrainDelayDeparture]
        [TestMethod]
        public void LiveDataBLLTest_TestTrainDelayDeparture()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(4, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrainDelayDeparture/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestTrainDelayDeparture(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestTrain2MinutesDelayDeparture]
        [TestMethod]
        public void LiveDataBLLTest_TestTrain2MinutesDelayDeparture()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayDeparture/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestTrain2MinutesDelayDeparture(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestTrain2MinutesDelayArrival]
        [TestMethod]
        public void LiveDataBLLTest_TestTrain2MinutesDelayArrival()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(0, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(1, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(3, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(3, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestTrain2MinutesDelayArrival/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestTrain2MinutesDelayArrival(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestPrematureTrain]
        [TestMethod]
        public void LiveDataBLLTest_TestPrematureTrain()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-28, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-28, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-29, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-28, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-28, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-28, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-28, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-28, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-20, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(-20, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestPrematureTrain/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestPrematureTrain(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestChangedTrack]
        [TestMethod]
        public void LiveDataBLLTest_TestChangedTrack()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/TestdorfArrival2A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G21"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/TestdorfArrival2B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G22"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChangedTrack/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestChangedTrack(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestSpecialTrain]
        [TestMethod]
        public void LiveDataBLLTest_TestSpecialTrain()
        {
            using (var testDataScope = new ESTWTestDataScope(BLL))
            {
                var Area = ExpectedValuesOfInitializationBLLTest.GetAreaInformation().FirstOrDefault(a => a.Id == "myTestArea");
                Assert.IsNotNull(Area, "Area is null");

                var Testdorf = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Testdorf, "Estw Testdorf is null");

                var Rechtsheim = Area.ESTWs.FirstOrDefault(e => e.Id == "TREH");
                Assert.IsNotNull(Rechtsheim, "Estw Rechtsheim is null");

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/AdvanceNoticeRechtsheim.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["33BP"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/BetweenRechtsheimAndTestdorf.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["33BP"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/TestdorfArrival3B.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["32G32"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(3, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/TestdorfArrival3A.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(3, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/TestdorfDeparture.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/BetweenUestMitteAndTestdorf.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/UestMitteWrongDirection.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/UestMitte.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["32B816"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/BetweenProbeAndUestMitte.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["32B816"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/ProbeArrival.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    Assert.AreEqual(Testdorf.Blocks["31G1"].First(), Area.LiveTrains[98765].Block);
                    Assert.AreEqual(3, Area.LiveTrains[98765].Delay);
                }

                Assert.IsFalse(Rechtsheim.IsLoaded, "ESTW Rechtsheim is loaded");

                using (var scope = new ESTWOnlineScope(Testdorf, "TestSpecialTrain/ProbeDeparture.dat"))
                {
                    using (var scopeRechtsheim = new ESTWOnlineScope(Rechtsheim, "TestSpecialTrain/Rechtsheim.dat"))
                    {
                        var Result = BLL.RefreshLiveData(Area);
                        DefaultChecks.IsOperationSucceeded(Result);
                        Assert.IsTrue(Result.Result, "Result is false");

                        Assert.AreEqual(1, Area.LiveTrains.Count);
                        Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                    }
                }

                Assert.IsTrue(Rechtsheim.IsLoaded, "ESTW Rechtsheim is not loaded");

                var Train = Area.LiveTrains[98765];
                TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestSpecialTrain(Testdorf), Train);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestSpecialTrainDelay]
        [TestMethod]
        public void LiveDataBLLTest_TestSpecialTrainDelay()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/AdvanceNoticeRechtsheim.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["33BP"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/BetweenRechtsheimAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["33BP"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(4, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/TestdorfArrival3B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["32G32"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(3, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/TestdorfArrival3A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(3, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(7, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(7, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["32G31"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(7, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["32B816"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(7, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["32B816"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(7, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
                Assert.AreEqual(Estw.Blocks["31G1"].First(), Area.LiveTrains[98765].Block);
                Assert.AreEqual(10, Area.LiveTrains[98765].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSpecialTrainDelay/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(98765));
            }

            var Train = Area.LiveTrains[98765];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestSpecialTrainDelay(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestSequentialStations]
        [TestMethod]
        public void LiveDataBLLTest_TestSequentialStations()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestSequentialStations/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSequentialStations/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSequentialStations/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSequentialStations/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(3, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSequentialStations/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(3, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestSequentialStations/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestSequentialStations(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestStartDelayCountdown]
        [TestMethod]
        public void LiveDataBLLTest_TestStartDelayCountdown()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestStartDelayCountdown/3MinutesBefore.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(-3, Area.LiveTrains[12346].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestStartDelayCountdown/2MinutesBefore.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(-2, Area.LiveTrains[12346].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestStartDelayCountdown/1MinuteBefore.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(-1, Area.LiveTrains[12346].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestStartDelayCountdown/Punctual.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(0, Area.LiveTrains[12346].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestStartDelayCountdown/1MinuteDelayed.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(1, Area.LiveTrains[12346].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestStartDelayCountdown/2MinutesDelayed.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(2, Area.LiveTrains[12346].Delay);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestChildTracks]
        [TestMethod]
        public void LiveDataBLLTest_TestChildTracks()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestChildTracks/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(-2, Area.LiveTrains[12346].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChildTracks/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(-1, Area.LiveTrains[12346].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestChildTracks/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12346));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[12346].Block);
                Assert.AreEqual(1, Area.LiveTrains[12346].Delay);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestEndingTrainDelay]
        [TestMethod]
        public void LiveDataBLLTest_TestEndingTrainDelay()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestEndingTrainDelay/Arrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12345));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12345].Block);
                Assert.AreEqual(-1, Area.LiveTrains[12345].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestEndingTrainDelay/1MinuteAfterArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12345));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12345].Block);
                Assert.AreEqual(-1, Area.LiveTrains[12345].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestEndingTrainDelay/2MinutesAfterArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12345));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12345].Block);
                Assert.AreEqual(-1, Area.LiveTrains[12345].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestEndingTrainDelay/3MinutesAfterArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12345));
                Assert.AreEqual(Estw.Blocks["31G3"].First(), Area.LiveTrains[12345].Block);
                Assert.AreEqual(-1, Area.LiveTrains[12345].Delay);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestDeleteLiveData]
        [TestMethod]
        public void LiveDataBLLTest_TestDeleteLiveData()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/AdvanceNoticeLinksdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/BetweenLinksdorfAndProbe.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/BetweenProbeAndUestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/UestMitteWrongDirection.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/UestMitte.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/BetweenUestMitteAndTestdorf.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/TestdorfArrival1B.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/Exactly12Hours.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                Assert.AreEqual(Estw.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestDeleteLiveData/After12Hours.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(0, Area.LiveTrains.Count);
            }
        }
        #endregion

        #region [LiveDataBLLTest_JustifyDelay]
        [TestMethod]
        public void LiveDataBLLTest_JustifyDelay()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayArrival(Estw);
            var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TPRB");
            Assert.IsNotNull(Schedule, "Schedule is null");
            Assert.AreEqual(1, Schedule.Delays.Count);

            string Reason = "Kaputt";
            int CausedBy = 12345;

            var Delay = Schedule.Delays.First();
            Delay.Reason = Reason;
            Delay.CausedBy = CausedBy;

            var Expected = new SharedDelay(2007, "TPRB", 0, 4, eDelayType.Arrival, Reason);
            Expected.CausedBy = CausedBy;

            using (var scope = new ESTWRootScope())
            {
                var BllResult = BLL.JustifyDelay(Delay);
                DefaultChecks.IsOperationSucceeded(BllResult);
                Assert.IsNotNull(BllResult.Result, "BllResult is null");
                SharedDelayComparer.Instance.Compare(Expected, BllResult.Result);

                var SettingsBll = new SettingsBLL();
                var Settings = SettingsBll.GetSettings().Result;
                Assert.IsNotNull(Settings, "Settings is null");

                var FilePath = Path.Combine(Settings.Paths["TTST"], @"Kommunikation\leibit_delay_2007_TPRB.dat");
                Assert.IsTrue(File.Exists(FilePath));

                var Serializer = new XmlSerializer(typeof(SharedDelay));

                using (var FileStram = File.OpenRead(FilePath))
                {
                    var Reader = XmlReader.Create(FileStram);
                    Assert.IsTrue(Serializer.CanDeserialize(Reader));

                    object DeserializedResult = Serializer.Deserialize(Reader);
                    Assert.IsInstanceOfType(DeserializedResult, typeof(SharedDelay));

                    SharedDelay DeserializedDelay = DeserializedResult as SharedDelay;
                    SharedDelayComparer.Instance.Compare(Expected, DeserializedDelay);
                }
            }
        }
        #endregion

        #region [LiveDataBLLTest_JustifyDelay_NoReason]
        [TestMethod]
        public void LiveDataBLLTest_JustifyDelay_NoReason()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayArrival(Estw);
            var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TPRB");
            Assert.IsNotNull(Schedule, "Schedule is null");
            Assert.AreEqual(1, Schedule.Delays.Count);

            string Reason = "   ";

            var Delay = Schedule.Delays.First();
            Delay.Reason = Reason;

            using (var scope = new ESTWRootScope())
            {
                var BllResult = BLL.JustifyDelay(Delay);
                DefaultChecks.IsOperationNotSucceeded(BllResult);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestRefreshLiveSchedules]
        [TestMethod]
        public void LiveDataBLLTest_TestRefreshLiveSchedules()
        {
            using (var testDataScope = new ESTWTestDataScope(BLL))
            {
                var Area = ExpectedValuesOfInitializationBLLTest.GetAreaInformation().FirstOrDefault(a => a.Id == "myTestArea");
                Assert.IsNotNull(Area, "Area is null");

                var Testdorf = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Testdorf, "Estw Testdorf is null");

                var Rechtsheim = Area.ESTWs.FirstOrDefault(e => e.Id == "TREH");
                Assert.IsNotNull(Rechtsheim, "Estw Rechtsheim is null");

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/AdvanceNoticeLinksdorf.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/BetweenLinksdorfAndProbe.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["30BN"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/ProbeArrival.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/ProbeDeparture.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/BetweenProbeAndUestMitte.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/UestMitteWrongDirection.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["31G2"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/UestMitte.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/BetweenUestMitteAndTestdorf.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["32B815"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(7, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/TestdorfArrival1A.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["32G11"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
                }

                using (var scope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/TestdorfArrival1B.dat"))
                {
                    var Result = BLL.RefreshLiveData(Area);
                    DefaultChecks.IsOperationSucceeded(Result);
                    Assert.IsTrue(Result.Result, "Result is false");

                    Assert.AreEqual(1, Area.LiveTrains.Count);
                    Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    Assert.AreEqual(Testdorf.Blocks["32G12"].First(), Area.LiveTrains[2007].Block);
                    Assert.AreEqual(6, Area.LiveTrains[2007].Delay);
                }

                Assert.IsFalse(Rechtsheim.IsLoaded, "ESTW Rechtsheim is loaded");

                using (var TestdorfScope = new ESTWOnlineScope(Testdorf, "TestRefreshLiveSchedules/TestdorfDeparture.dat"))
                {
                    using (var RechtsheimScope = new ESTWOnlineScope(Rechtsheim, "TestRefreshLiveSchedules/Rechtsheim.dat"))
                    {
                        var Result = BLL.RefreshLiveData(Area);
                        DefaultChecks.IsOperationSucceeded(Result);
                        Assert.IsTrue(Result.Result, "Result is false");

                        Assert.AreEqual(1, Area.LiveTrains.Count);
                        Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
                    }
                }

                Assert.IsTrue(Rechtsheim.IsLoaded, "ESTW Rechtsheim is not loaded");

                var Train = Area.LiveTrains[2007];
                TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestRefreshLiveSchedules(Testdorf), Train);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestLoadSharedDelay_Existing]
        [TestMethod]
        public void LiveDataBLLTest_TestLoadSharedDelay_Existing()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            Area.LiveTrains.TryAdd(2007, ExpectedValuesOfLiveDataBLLTest.TestTrainDelayDeparture(Estw));

            using (var scope = new ESTWOnlineScope(Estw, @"TestData\ESTWOnline\TestLoadSharedDelay_Existing", "TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestLoadSharedDelay_Existing(Estw), Train);
        }

        //[TestMethod]
        public void LiveDataBLLTest_TestLoadSharedDelay_Existing_CreateTestData()
        {
            var TrainNumber = 2007;
            var StationShortSymbol = "TTST";
            var ScheduleIndex = 0;
            var Reason = "Keine Ahnung";
            var CausedBy = 4711;

            var SharedDelay = new SharedDelay(TrainNumber, StationShortSymbol, ScheduleIndex, 4, eDelayType.Departure, Reason);
            SharedDelay.CausedBy = CausedBy;

            var FilePath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestData\ESTWOnline\TestLoadSharedDelay_Existing", String.Format("leibit_delay_{0}_{1}.dat", TrainNumber, StationShortSymbol));

            using (var FileStream = File.Open(FilePath, FileMode.Create))
            {
                var Serializer = new XmlSerializer(typeof(SharedDelay));
                Serializer.Serialize(FileStream, SharedDelay);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestLoadSharedDelay_NonExisting]
        [TestMethod]
        public void LiveDataBLLTest_TestLoadSharedDelay_NonExisting()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            Area.LiveTrains.TryAdd(2007, ExpectedValuesOfLiveDataBLLTest.TestPunctualTrain(Estw));

            using (var scope = new ESTWOnlineScope(Estw, @"TestData\ESTWOnline\TestLoadSharedDelay_NonExisting", "TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestLoadSharedDelay_NonExisting(Estw), Train);
        }

        //[TestMethod]
        public void LiveDataBLLTest_TestLoadSharedDelay_NonExisting_CreateTestData()
        {
            var TrainNumber = 2007;
            var StationShortSymbol = "TTST";
            var ScheduleIndex = 0;
            var Reason = "Keine Ahnung";
            var CausedBy = 4711;

            var SharedDelay = new SharedDelay(TrainNumber, StationShortSymbol, ScheduleIndex, 4, eDelayType.Departure, Reason);
            SharedDelay.CausedBy = CausedBy;

            var FilePath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestData\ESTWOnline\TestLoadSharedDelay_NonExisting", String.Format("leibit_delay_{0}_{1}.dat", TrainNumber, StationShortSymbol));

            using (var FileStream = File.Open(FilePath, FileMode.Create))
            {
                var Serializer = new XmlSerializer(typeof(SharedDelay));
                Serializer.Serialize(FileStream, SharedDelay);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestLoadSharedDelay_InvalidScheduleIndex]
        [TestMethod]
        public void LiveDataBLLTest_TestLoadSharedDelay_InvalidScheduleIndex()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            Area.LiveTrains.TryAdd(2007, ExpectedValuesOfLiveDataBLLTest.TestTrainDelayDeparture(Estw));

            using (var scope = new ESTWOnlineScope(Estw, @"TestData\ESTWOnline\TestLoadSharedDelay_InvalidScheduleIndex", "TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(2007));
            }

            var Train = Area.LiveTrains[2007];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestLoadSharedDelay_Existing(Estw), Train);
        }

        //[TestMethod]
        public void LiveDataBLLTest_TestLoadSharedDelay_InvalidScheduleIndex_CreateTestData()
        {
            var TrainNumber = 2007;
            var StationShortSymbol = "TTST";
            var ScheduleIndex = 815;
            var Reason = "Keine Ahnung";
            var CausedBy = 4711;

            var SharedDelay = new SharedDelay(TrainNumber, StationShortSymbol, ScheduleIndex, 4, eDelayType.Departure, Reason);
            SharedDelay.CausedBy = CausedBy;

            var FilePath = Path.Combine(Environment.CurrentDirectory, @"..\..\TestData\ESTWOnline\TestLoadSharedDelay_InvalidScheduleIndex", String.Format("leibit_delay_{0}_{1}.dat", TrainNumber, StationShortSymbol));

            using (var FileStream = File.Open(FilePath, FileMode.Create))
            {
                var Serializer = new XmlSerializer(typeof(SharedDelay));
                Serializer.Serialize(FileStream, SharedDelay);
            }
        }
        #endregion

        #region [LiveDataBLLTest_TestExpectedTimesDelayed]
        [TestMethod]
        public void LiveDataBLLTest_TestExpectedTimesDelayed()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestExpectedTimesDelayed/AdvanceNoticeRechtsheim.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12345));
            }

            var Train = Area.LiveTrains[12345];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestExpectedTimesDelayed(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestExpectedTimesPremature]
        [TestMethod]
        public void LiveDataBLLTest_TestExpectedTimesPremature()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestExpectedTimesPremature/AdvanceNoticeRechtsheim.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(12345));
            }

            var Train = Area.LiveTrains[12345];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestExpectedTimesPremature(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestExpectedTimesDelayedPassing]
        [TestMethod]
        public void LiveDataBLLTest_TestExpectedTimesDelayedPassing()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestExpectedTimesDelayedPassing/AdvanceNoticeRechtsheim.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(86312));
            }

            var Train = Area.LiveTrains[86312];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestExpectedTimesDelayedPassing(Estw), Train);
        }
        #endregion

        #region [LiveDataBLLTest_TestExpectedTimesPrematurePassing]
        [TestMethod]
        public void LiveDataBLLTest_TestExpectedTimesPrematurePassing()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestExpectedTimesPrematurePassing/AdvanceNoticeRechtsheim.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(86312));
            }

            var Train = Area.LiveTrains[86312];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestExpectedTimesPrematurePassing(Estw), Train);
        }
        #endregion

        #endregion

    }
}
