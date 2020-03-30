using Leibit.BLL;
using Leibit.Core.Common;
using Leibit.Entities.Common;
using Leibit.Tests.Comparer;
using Leibit.Tests.ExpectedData;
using Leibit.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Leibit.Tests
{
    [TestClass]
    public class InitializationBLLTest
    {

        #region - Needs -
        private InitializationBLL m_Bll;
        #endregion

        #region - Singletons -

        #region [BLL]
        private InitializationBLL BLL
        {
            get
            {
                if (m_Bll == null)
                    m_Bll = new InitializationBLL();

                return m_Bll;
            }
        }
        #endregion

        #endregion

        #region - Test methods -

        #region [InitializationBLLTest_GetAreaInformation]
        [TestMethod]
        public void InitializationBLLTest_GetAreaInformation()
        {
            using (var scope = new ESTWTestDataScope(BLL))
            {
                var Expected = ExpectedValuesOfInitializationBLLTest.GetAreaInformation();
                var BllResult = BLL.GetAreaInformation();

                DefaultChecks.IsOperationSucceeded(BllResult);
                DefaultChecks.HasResultValue(BllResult);

                LeibitComparer<Area>.CompareList(Expected, BllResult.Result, AreaComparer.Instance, "Result");
            }
        }
        #endregion

        #region [InitializationBLLTest_LoadESTW]
        [TestMethod]
        public void InitializationBLLTest_LoadESTW()
        {
            using (var scope = new ESTWTestDataScope(BLL))
            {
                var Area = new Area("myTestArea", "Testland");
                var Estw = new ESTW("TTST", "Testdorf", "leibit_TEST.dat", Area);
                new ESTW("TREH", "Rechtsheim", "leibit_RECHTSHEI.dat", Area);
                var Expected = ExpectedValuesOfInitializationBLLTest.LoadTestdorfESTW();

                //int progress = 0;
                //bool finished = false;
                //OperationResult<bool> BllResult = null;

                //var worker = new BackgroundWorker();
                //worker.WorkerReportsProgress = true;
                //worker.ProgressChanged += (sender, e) => progress = e.ProgressPercentage;
                //worker.DoWork += (sender, e) => BllResult = BLL.LoadESTW(Estw, sender as BackgroundWorker);
                //worker.RunWorkerCompleted += (sender, e) => finished = true;
                //worker.RunWorkerAsync();

                //while (!finished) ;

                OperationResult<bool> BllResult = BLL.LoadESTW(Estw);

                DefaultChecks.IsOperationSucceeded(BllResult);
                Assert.IsTrue(BllResult.Result, "Result is false.");
                AreaComparer.Instance.Compare(Expected, Area);
                //Assert.AreEqual(100, progress);
            }
        }
        #endregion

        #endregion

    }
}
