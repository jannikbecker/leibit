using Leibit.BLL;
using Leibit.Entities.Serialization;
using Leibit.Tests.Comparer;
using Leibit.Tests.ExpectedData;
using Leibit.Tests.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Leibit.Tests
{
    [TestClass]
    public class SerializationBLLTest
    {

        #region - Needs -
        private SerializationBLL m_Bll;
        #endregion

        #region - Ctor -
        public SerializationBLLTest()
        {

        }
        #endregion

        #region - Singletons -

        #region [BLL]
        private SerializationBLL BLL
        {
            get
            {
                if (m_Bll == null)
                    m_Bll = new SerializationBLL();

                return m_Bll;
            }
        }
        #endregion

        #endregion

        #region - Test methods -

        #region [SerializationBLLTest_Serialize_Deserialize]
        [TestMethod]
        public void SerializationBLLTest_Serialize_Deserialize()
        {
            string Filename = "test.leibit";

            var Expected = ExpectedValuesOfSerializationBLLTest.Serialize_Deserialize();

            var SaveResult = BLL.Save(Filename, Expected);
            DefaultChecks.IsOperationSucceeded(SaveResult);

            SerializationContainer Actual;

            using (var scope = new ESTWTestDataScope(BLL))
            {
                var OpenResult = BLL.Open(Filename);
                DefaultChecks.IsOperationSucceeded(OpenResult);
                Actual = OpenResult.Result;
            }

            AreaComparer.Instance.Compare(Expected.Area, Actual.Area);
            AreaComparer.CompareDictionary(Expected.Area.LiveTrains, Actual.Area.LiveTrains, TrainInformationComparer.Instance, "LiveTrains");
        }
        #endregion

        #endregion

    }
}
