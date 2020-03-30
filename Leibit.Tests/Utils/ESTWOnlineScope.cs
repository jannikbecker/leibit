using Leibit.BLL;
using Leibit.Entities.Common;
using System;
using System.IO;
using System.Reflection;

namespace Leibit.Tests.Utils
{
    public class ESTWOnlineScope : IDisposable
    {
        private SettingsBLL m_SettingsBll = new SettingsBLL();
        private ESTW m_Estw;
        private string m_OldDataFilePath;
        private string m_OldEstwOnlinePath;

        private static FieldInfo m_Field;

        private const string DATA_FILE_FIELD_NAME = "m_DataFile";

        private FieldInfo Field
        {
            get
            {
                if (m_Field == null)
                    m_Field = m_Estw.GetType().GetField(DATA_FILE_FIELD_NAME, BindingFlags.NonPublic | BindingFlags.Instance);

                return m_Field;
            }
        }

        public ESTWOnlineScope(ESTW estw, string dataFilePath)
            : this(estw, @"TestData\ESTWOnline", dataFilePath)
        {
        }

        public ESTWOnlineScope(ESTW estw, string estwOnlineDirectory, string dataFilePath)
        {
            m_Estw = estw;

            m_OldDataFilePath = Field.GetValue(m_Estw) as string;
            Field.SetValue(m_Estw, dataFilePath);

            var SettingsResult = m_SettingsBll.GetSettings();

            if (SettingsResult.Succeeded)
            {
                m_OldEstwOnlinePath = SettingsResult.Result.EstwOnlinePath;
                SettingsResult.Result.EstwOnlinePath = Path.Combine(Environment.CurrentDirectory, estwOnlineDirectory);
            }
        }

        public void Dispose()
        {
            Field.SetValue(m_Estw, m_OldDataFilePath);

            var SettingsResult = m_SettingsBll.GetSettings();

            if (SettingsResult.Succeeded)
                SettingsResult.Result.EstwOnlinePath = m_OldEstwOnlinePath;
        }

    }
}
