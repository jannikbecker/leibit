using Leibit.BLL;
using System;
using System.Reflection;
using System.Xml;

namespace Leibit.Tests.Utils
{
    public class ESTWTestDataScope : IDisposable
    {

        private InitializationBLL m_InitializationBll;
        private XmlDocument m_OldXml;
        private ESTWRootScope m_RootScope;

        private const string XML_DOC_FIELD_NAME = "m_AreasXml";

        public ESTWTestDataScope(SerializationBLL serializationBll)
        {
            var property = serializationBll.GetType().GetProperty("InitializationBll", BindingFlags.NonPublic | BindingFlags.Instance);
            var initializationBll = property.GetValue(serializationBll) as InitializationBLL;
            __Init(initializationBll);
        }

        public ESTWTestDataScope(LiveDataBLL liveDataBll)
        {
            var property = liveDataBll.GetType().GetProperty("InitializationBLL", BindingFlags.NonPublic | BindingFlags.Instance);
            var initializationBll = property.GetValue(liveDataBll) as InitializationBLL;
            __Init(initializationBll);
        }

        public ESTWTestDataScope(InitializationBLL initializationBll)
        {
            __Init(initializationBll);
        }

        private void __Init(InitializationBLL initializationBll)
        {
            m_InitializationBll = initializationBll;
            var field = m_InitializationBll.GetType().GetField(XML_DOC_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);
            m_OldXml = field.GetValue(m_InitializationBll) as XmlDocument;

            var newValue = new XmlDocument();
            newValue.LoadXml(Properties.Resources.Areas);
            field.SetValue(m_InitializationBll, newValue);

            initializationBll.CustomEstwXmlStream = estw =>
            {
                return Assembly.GetExecutingAssembly().GetManifestResourceStream($"Leibit.Tests.TestData.{estw.Id}.xml");
            };

            m_RootScope = new ESTWRootScope();
        }

        public void Dispose()
        {
            FieldInfo field = m_InitializationBll.GetType().GetField(XML_DOC_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(m_InitializationBll, m_OldXml);

            m_RootScope.Dispose();
        }

    }
}
