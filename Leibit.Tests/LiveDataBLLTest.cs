using Leibit.BLL;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using Leibit.Tests.Comparer;
using Leibit.Tests.ExpectedData;
using Leibit.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

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
            using (new SettingsScope())
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
        }
        #endregion

        #region [LiveDataBLLTest_TestTrainDelayDeparture]
        [TestMethod]
        public void LiveDataBLLTest_TestTrainDelayDeparture()
        {
            using (new SettingsScope())
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
        }
        #endregion

        #region [LiveDataBLLTest_TestTrain2MinutesDelayDeparture]
        [TestMethod]
        public void LiveDataBLLTest_TestTrain2MinutesDelayDeparture()
        {
            using (new SettingsScope())
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
        }
        #endregion

        #region [LiveDataBLLTest_TestTrain2MinutesDelayArrival]
        [TestMethod]
        public void LiveDataBLLTest_TestTrain2MinutesDelayArrival()
        {
            using (new SettingsScope())
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
            using (new SettingsScope())
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
                Assert.IsNull(Area.LiveTrains[12346].Block);
                Assert.AreEqual(7, Area.LiveTrains[12346].Delay);
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
            using (var settingsScope = new SettingsScope())
            {
                settingsScope.CheckPlausibility = false;

                var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
                var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Estw, "Estw is null");

                var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayArrival(Estw);
                var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TTST");
                Assert.IsNotNull(Schedule, "Schedule is null");
                Assert.AreEqual(1, Schedule.Delays.Count);

                var Delay = Schedule.Delays.First();
                string Reason = "Kaputt";
                int CausedBy = 12345;

                using (var scope = new ESTWRootScope())
                {
                    var BllResult = BLL.JustifyDelay(Delay, Reason, CausedBy);
                    DefaultChecks.IsOperationSucceeded(BllResult);
                    Assert.AreEqual(Reason, Delay.Reason);
                    Assert.AreEqual(CausedBy, Delay.CausedBy);
                }
            }
        }
        #endregion

        #region [LiveDataBLLTest_JustifyDelay_PlausibilityCheck_TrainNotFound]
        [TestMethod]
        public void LiveDataBLLTest_JustifyDelay_PlausibilityCheck_TrainNotFound()
        {
            using (var settingsScope = new SettingsScope())
            {
                settingsScope.CheckPlausibility = true;

                var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
                var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Estw, "Estw is null");

                var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayArrival(Estw);
                var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TTST");
                Assert.IsNotNull(Schedule, "Schedule is null");
                Assert.AreEqual(1, Schedule.Delays.Count);

                var Delay = Schedule.Delays.First();

                using (var scope = new ESTWRootScope())
                {
                    var BllResult = BLL.JustifyDelay(Delay, "Kaputt", 54321);
                    DefaultChecks.IsOperationNotSucceeded(BllResult);
                    Assert.IsTrue(BllResult.Message.Contains("Zug 54321 nicht gefunden"));
                }
            }
        }
        #endregion

        #region [LiveDataBLLTest_JustifyDelay_PlausibilityCheck_NoSchedule]
        [TestMethod]
        public void LiveDataBLLTest_JustifyDelay_PlausibilityCheck_NoSchedule()
        {
            using (var settingsScope = new SettingsScope())
            {
                settingsScope.CheckPlausibility = true;

                var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
                var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Estw, "Estw is null");

                var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayArrival(Estw);
                var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TTST");
                Assert.IsNotNull(Schedule, "Schedule is null");
                Assert.AreEqual(1, Schedule.Delays.Count);

                var Delay = Schedule.Delays.First();

                var Train2 = new TrainInformation(Area.Trains[12345]);
                Area.LiveTrains.TryAdd(12345, Train2);

                using (var scope = new ESTWRootScope())
                {
                    var BllResult = BLL.JustifyDelay(Delay, "Kaputt", 12345);
                    DefaultChecks.IsOperationNotSucceeded(BllResult);
                    Assert.IsTrue(BllResult.Message.Contains("Zug 12345 hat die Betriebsstelle 'Testdorf' nicht durchfahren"));
                }
            }
        }
        #endregion

        #region [LiveDataBLLTest_JustifyDelay_PlausibilityCheck_InvalidTime_Arrival]
        [TestMethod]
        public void LiveDataBLLTest_JustifyDelay_PlausibilityCheck_InvalidTime_Arrival()
        {
            using (var settingsScope = new SettingsScope())
            {
                settingsScope.CheckPlausibility = true;

                var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
                var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Estw, "Estw is null");

                var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayArrival(Estw);
                var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TTST");
                Assert.IsNotNull(Schedule, "Schedule is null");
                Assert.AreEqual(1, Schedule.Delays.Count);

                var Delay = Schedule.Delays.First();
                var Train2 = new TrainInformation(Area.Trains[12345]);
                var TestdorfSchedule = new LiveSchedule(Train2, Train2.Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
                TestdorfSchedule.LiveArrival = new LeibitTime(12, 50);
                TestdorfSchedule.LiveDeparture = new LeibitTime(12, 52);
                Train2.AddSchedule(TestdorfSchedule);
                Area.LiveTrains.TryAdd(12345, Train2);

                using (var scope = new ESTWRootScope())
                {
                    var BllResult = BLL.JustifyDelay(Delay, "Kaputt", 12345);
                    DefaultChecks.IsOperationNotSucceeded(BllResult);
                    Assert.IsTrue(BllResult.Message.Contains("Zug 12345 hat die Betriebsstelle 'Testdorf' zu einer anderen Zeit durchfahren als 2007"));
                }
            }
        }
        #endregion

        #region [LiveDataBLLTest_JustifyDelay_PlausibilityCheck_InvalidTime_Departure]
        [TestMethod]
        public void LiveDataBLLTest_JustifyDelay_PlausibilityCheck_InvalidTime_Departure()
        {
            using (var settingsScope = new SettingsScope())
            {
                settingsScope.CheckPlausibility = true;

                var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
                var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Estw, "Estw is null");

                var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayDeparture(Estw);
                var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TTST");
                Assert.IsNotNull(Schedule, "Schedule is null");
                Assert.AreEqual(1, Schedule.Delays.Count);

                var Delay = Schedule.Delays.First();
                var Train2 = new TrainInformation(Area.Trains[12345]);
                var TestdorfSchedule = new LiveSchedule(Train2, Train2.Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
                TestdorfSchedule.LiveArrival = new LeibitTime(12, 50);
                TestdorfSchedule.LiveDeparture = new LeibitTime(12, 52);
                Train2.AddSchedule(TestdorfSchedule);
                Area.LiveTrains.TryAdd(12345, Train2);

                using (var scope = new ESTWRootScope())
                {
                    var BllResult = BLL.JustifyDelay(Delay, "Kaputt", 12345);
                    DefaultChecks.IsOperationNotSucceeded(BllResult);
                    Assert.IsTrue(BllResult.Message.Contains("Zug 12345 hat die Betriebsstelle 'Testdorf' zu einer anderen Zeit durchfahren als 2007"));
                }
            }
        }
        #endregion

        #region [LiveDataBLLTest_JustifyDelay_PlausibilityCheck_Ok]
        [TestMethod]
        public void LiveDataBLLTest_JustifyDelay_PlausibilityCheck_Ok()
        {
            using (var settingsScope = new SettingsScope())
            {
                settingsScope.CheckPlausibility = true;

                var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
                var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
                Assert.IsNotNull(Estw, "Estw is null");

                var Train = ExpectedValuesOfLiveDataBLLTest.TestTrainDelayDeparture(Estw);
                var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TTST");
                Assert.IsNotNull(Schedule, "Schedule is null");
                Assert.AreEqual(1, Schedule.Delays.Count);

                var Delay = Schedule.Delays.First();
                var Train2 = new TrainInformation(Area.Trains[12345]);
                var TestdorfSchedule = new LiveSchedule(Train2, Train2.Train.Schedules.FirstOrDefault(s => s.Station.ShortSymbol == "TTST"));
                TestdorfSchedule.LiveArrival = new LeibitTime(13, 11);
                TestdorfSchedule.LiveDeparture = new LeibitTime(13, 34);
                Train2.AddSchedule(TestdorfSchedule);
                Area.LiveTrains.TryAdd(12345, Train2);

                using (var scope = new ESTWRootScope())
                {
                    var BllResult = BLL.JustifyDelay(Delay, "Kaputt", 12345);
                    DefaultChecks.IsOperationSucceeded(BllResult);
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
            var Schedule = Train.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == "TTST");
            Assert.IsNotNull(Schedule, "Schedule is null");
            Assert.AreEqual(1, Schedule.Delays.Count);

            var Delay = Schedule.Delays.First();

            using (var scope = new ESTWRootScope())
            {
                var BllResult = BLL.JustifyDelay(Delay, "   ", null);
                DefaultChecks.IsOperationNotSucceeded(BllResult);
            }
        }
        #endregion

        #region [LiveDataBLLTest_SetExpectedTimes_Ok]
        [TestMethod]
        public void LiveDataBLLTest_SetExpectedTimes_Ok()
        {
            var train = new Train(65432);
            var liveTrain = new TrainInformation(train);

            var estw = new ESTW("Test", "Test", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Monday, 20, 30);

            var station = new Station("Testbahnhof", "JTST", 333, string.Empty, string.Empty, estw);
            var track = new Track("1", true, true, station, null);

            var schedule = new Schedule(train,
                                        arrival: new LeibitTime(eDaysOfService.Monday, 20, 36),
                                        departure: new LeibitTime(eDaysOfService.Monday, 20, 38),
                                        track,
                                        days: new List<eDaysOfService> { eDaysOfService.Monday },
                                        direction: eScheduleDirection.LeftToRight,
                                        handling: eHandling.Transit,
                                        remark: string.Empty);

            var liveSchedule = new LiveSchedule(liveTrain, schedule);
            liveTrain.AddSchedule(liveSchedule);

            var bll = new LiveDataBLL();
            var result = bll.SetExpectedDelay(liveSchedule, 6, 5);
            DefaultChecks.IsOperationSucceeded(result);
            Assert.IsTrue(result.Result);
            Assert.AreEqual(new LeibitTime(eDaysOfService.Monday, 20, 42), liveSchedule.ExpectedArrival);
            Assert.AreEqual(new LeibitTime(eDaysOfService.Monday, 20, 43), liveSchedule.ExpectedDeparture);
            Assert.AreEqual(6, liveSchedule.ExpectedDelayArrival);
            Assert.AreEqual(5, liveSchedule.ExpectedDelayDeparture);
        }
        #endregion

        #region [LiveDataBLLTest_SetExpectedTimes_AlreadyArrived]
        [TestMethod]
        public void LiveDataBLLTest_SetExpectedTimes_AlreadyArrived()
        {
            var train = new Train(65432);
            var liveTrain = new TrainInformation(train);

            var estw = new ESTW("Test", "Test", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Monday, 20, 30);

            var station = new Station("Testbahnhof", "JTST", 333, string.Empty, string.Empty, estw);
            var track = new Track("1", true, true, station, null);

            var schedule = new Schedule(train,
                                        arrival: new LeibitTime(eDaysOfService.Monday, 20, 36),
                                        departure: new LeibitTime(eDaysOfService.Monday, 20, 38),
                                        track,
                                        days: new List<eDaysOfService> { eDaysOfService.Monday },
                                        direction: eScheduleDirection.LeftToRight,
                                        handling: eHandling.Transit,
                                        remark: string.Empty);

            var liveSchedule = new LiveSchedule(liveTrain, schedule);
            liveSchedule.IsArrived = true;
            liveSchedule.LiveArrival = new LeibitTime(eDaysOfService.Monday, 20, 35);
            liveTrain.AddSchedule(liveSchedule);

            var bll = new LiveDataBLL();
            var result = bll.SetExpectedDelay(liveSchedule, 6, 5);
            DefaultChecks.IsOperationNotSucceeded(result);
            Assert.IsTrue(result.Message.Contains("Die Ankunftsverspätung kann nicht gesetzt werden, da der Zug 65432 die Betriebsstelle JTST bereits erreicht hat."));
            Assert.IsNull(liveSchedule.ExpectedDelayArrival);
            Assert.IsNull(liveSchedule.ExpectedDelayDeparture);
        }
        #endregion

        #region [LiveDataBLLTest_SetExpectedTimes_AlreadyDeparted]
        [TestMethod]
        public void LiveDataBLLTest_SetExpectedTimes_AlreadyDeparted()
        {
            var train = new Train(65432);
            var liveTrain = new TrainInformation(train);

            var estw = new ESTW("Test", "Test", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Monday, 20, 30);

            var station = new Station("Testbahnhof", "JTST", 333, string.Empty, string.Empty, estw);
            var track = new Track("1", true, true, station, null);

            var schedule = new Schedule(train,
                                        arrival: new LeibitTime(eDaysOfService.Monday, 20, 36),
                                        departure: new LeibitTime(eDaysOfService.Monday, 20, 38),
                                        track,
                                        days: new List<eDaysOfService> { eDaysOfService.Monday },
                                        direction: eScheduleDirection.LeftToRight,
                                        handling: eHandling.Transit,
                                        remark: string.Empty);

            var liveSchedule = new LiveSchedule(liveTrain, schedule);
            liveSchedule.IsArrived = true;
            liveSchedule.LiveArrival = new LeibitTime(eDaysOfService.Monday, 20, 35);
            liveSchedule.IsDeparted = true;
            liveSchedule.LiveDeparture = new LeibitTime(eDaysOfService.Monday, 20, 39);
            liveTrain.AddSchedule(liveSchedule);

            var bll = new LiveDataBLL();
            var result = bll.SetExpectedDelay(liveSchedule, 6, 5);
            DefaultChecks.IsOperationNotSucceeded(result);
            Assert.IsTrue(result.Message.Contains("Der Zug 65432 hat die Betriebsstelle JTST bereits verlassen."));
            Assert.IsNull(liveSchedule.ExpectedDelayArrival);
            Assert.IsNull(liveSchedule.ExpectedDelayDeparture);
        }
        #endregion

        #region [LiveDataBLLTest_ChangeTrack_Ok]
        [TestMethod]
        public void LiveDataBLLTest_ChangeTrack_Ok()
        {
            var train = new Train(65432);
            var liveTrain = new TrainInformation(train);

            var estw = new ESTW("Test", "Test", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Monday, 20, 30);

            var station = new Station("Testbahnhof", "JTST", 333, string.Empty, string.Empty, estw);
            var track1 = new Track("1", true, true, station, null);
            var track2 = new Track("2", true, true, station, null);

            var schedule = new Schedule(train,
                                        arrival: new LeibitTime(eDaysOfService.Monday, 20, 36),
                                        departure: new LeibitTime(eDaysOfService.Monday, 20, 38),
                                        track1,
                                        days: new List<eDaysOfService> { eDaysOfService.Monday },
                                        direction: eScheduleDirection.LeftToRight,
                                        handling: eHandling.StopPassengerTrain,
                                        remark: string.Empty);

            var liveSchedule = new LiveSchedule(liveTrain, schedule);
            liveTrain.AddSchedule(liveSchedule);

            var bll = new LiveDataBLL();
            var result = bll.ChangeTrack(liveSchedule, track2);
            DefaultChecks.IsOperationSucceeded(result);
            Assert.IsTrue(result.Result);
            Assert.AreEqual(track2, liveSchedule.LiveTrack);
        }
        #endregion

        #region [LiveDataBLLTest_ChangeTrack_Reset]
        [TestMethod]
        public void LiveDataBLLTest_ChangeTrack_Reset()
        {
            var train = new Train(65432);
            var liveTrain = new TrainInformation(train);

            var estw = new ESTW("Test", "Test", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Monday, 20, 30);

            var station = new Station("Testbahnhof", "JTST", 333, string.Empty, string.Empty, estw);
            var track1 = new Track("1", true, true, station, null);
            var track2 = new Track("2", true, true, station, null);

            var schedule = new Schedule(train,
                                        arrival: new LeibitTime(eDaysOfService.Monday, 20, 36),
                                        departure: new LeibitTime(eDaysOfService.Monday, 20, 38),
                                        track1,
                                        days: new List<eDaysOfService> { eDaysOfService.Monday },
                                        direction: eScheduleDirection.LeftToRight,
                                        handling: eHandling.StopPassengerTrain,
                                        remark: string.Empty);

            var liveSchedule = new LiveSchedule(liveTrain, schedule);
            liveSchedule.LiveTrack = track2;
            liveTrain.AddSchedule(liveSchedule);

            var bll = new LiveDataBLL();
            var result = bll.ChangeTrack(liveSchedule, track1);
            DefaultChecks.IsOperationSucceeded(result);
            Assert.IsTrue(result.Result);
            Assert.IsNull(liveSchedule.LiveTrack);
        }
        #endregion

        #region [LiveDataBLLTest_ChangeTrack_AlreadyArrived]
        [TestMethod]
        public void LiveDataBLLTest_ChangeTrack_AlreadyArrived()
        {
            var train = new Train(65432);
            var liveTrain = new TrainInformation(train);

            var estw = new ESTW("Test", "Test", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Monday, 20, 30);

            var station = new Station("Testbahnhof", "JTST", 333, string.Empty, string.Empty, estw);
            var track1 = new Track("1", true, true, station, null);
            var track2 = new Track("2", true, true, station, null);

            var schedule = new Schedule(train,
                                        arrival: new LeibitTime(eDaysOfService.Monday, 20, 36),
                                        departure: new LeibitTime(eDaysOfService.Monday, 20, 38),
                                        track1,
                                        days: new List<eDaysOfService> { eDaysOfService.Monday },
                                        direction: eScheduleDirection.LeftToRight,
                                        handling: eHandling.StopPassengerTrain,
                                        remark: string.Empty);

            var liveSchedule = new LiveSchedule(liveTrain, schedule);
            liveSchedule.IsArrived = true;
            liveSchedule.LiveArrival = new LeibitTime(eDaysOfService.Monday, 20, 35);
            liveTrain.AddSchedule(liveSchedule);

            var bll = new LiveDataBLL();
            var result = bll.ChangeTrack(liveSchedule, track2);
            DefaultChecks.IsOperationNotSucceeded(result);
            Assert.IsTrue(result.Message.Contains("Der Zug 65432 hat die Betriebsstelle JTST bereits erreicht."));
        }
        #endregion

        #region [LiveDataBLLTest_ChangeTrack_NoPlatform]
        [TestMethod]
        public void LiveDataBLLTest_ChangeTrack_NoPlatform()
        {
            var train = new Train(65432);
            var liveTrain = new TrainInformation(train);

            var estw = new ESTW("Test", "Test", string.Empty, null);
            estw.Time = new LeibitTime(eDaysOfService.Monday, 20, 30);

            var station = new Station("Testbahnhof", "JTST", 333, string.Empty, string.Empty, estw);
            var track1 = new Track("1", false, true, station, null);
            var track2 = new Track("2", false, true, station, null);

            var schedule = new Schedule(train,
                                        arrival: null,
                                        departure: new LeibitTime(eDaysOfService.Monday, 20, 38),
                                        track1,
                                        days: new List<eDaysOfService> { eDaysOfService.Monday },
                                        direction: eScheduleDirection.LeftToRight,
                                        handling: eHandling.Transit,
                                        remark: string.Empty);

            var liveSchedule = new LiveSchedule(liveTrain, schedule);
            liveTrain.AddSchedule(liveSchedule);

            var bll = new LiveDataBLL();
            var result = bll.ChangeTrack(liveSchedule, track2);
            DefaultChecks.IsOperationNotSucceeded(result);
            Assert.IsTrue(result.Message.Contains("Für die Betriebsstelle JTST kann kein Gleiswechsel vorgenommen werden."));
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

        #region [LiveDataBLLTest_TestMisdirectedTrain]
        [TestMethod]
        public void LiveDataBLLTest_TestMisdirectedTrain()
        {
            var Area = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();
            var Estw = Area.ESTWs.FirstOrDefault(e => e.Id == "TTST");
            Assert.IsNotNull(Estw, "Estw is null");

            using (var scope = new ESTWOnlineScope(Estw, "TestMisdirectedTrain/ProbeArrival.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(4711));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[4711].Block);
                Assert.AreEqual(-1, Area.LiveTrains[4711].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestMisdirectedTrain/ProbeDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(4711));
                Assert.AreEqual(Estw.Blocks["31G2"].First(), Area.LiveTrains[4711].Block);
                Assert.AreEqual(0, Area.LiveTrains[4711].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestMisdirectedTrain/TestdorfArrival1A.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(4711));
                Assert.AreEqual(Estw.Blocks["32G11"].First(), Area.LiveTrains[4711].Block);
                Assert.AreEqual(0, Area.LiveTrains[4711].Delay);
            }

            using (var scope = new ESTWOnlineScope(Estw, "TestMisdirectedTrain/TestdorfDeparture.dat"))
            {
                var Result = BLL.RefreshLiveData(Area);
                DefaultChecks.IsOperationSucceeded(Result);
                Assert.IsTrue(Result.Result, "Result is false");

                Assert.AreEqual(1, Area.LiveTrains.Count);
                Assert.IsTrue(Area.LiveTrains.ContainsKey(4711));
                Assert.AreEqual(0, Area.LiveTrains[4711].Delay);
            }

            var Train = Area.LiveTrains[4711];
            TrainInformationComparer.Instance.Compare(ExpectedValuesOfLiveDataBLLTest.TestMisdirectedTrain(Estw), Train);
        }
        #endregion

        #endregion

    }
}
