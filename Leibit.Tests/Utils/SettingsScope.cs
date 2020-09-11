using Leibit.BLL;
using Leibit.Entities.Settings;
using System;

namespace Leibit.Tests.Utils
{
    public class SettingsScope : IDisposable
    {

        #region - Needs -
        private SettingsBLL m_SettingsBll;
        private Settings m_Settings;
        private Settings m_OriginalSettings;
        #endregion

        #region - Ctor -
        public SettingsScope()
        {
            m_SettingsBll = new SettingsBLL();

            var settingsResult = m_SettingsBll.GetSettings();

            if (settingsResult.Succeeded)
            {
                m_Settings = settingsResult.Result;
                m_OriginalSettings = m_Settings.Clone();
                m_Settings.DelayJustificationEnabled = true;
                m_Settings.DelayJustificationMinutes = 3;
            }
        }
        #endregion

        #region - Properties -

        public bool DelayJustificationEnabled { set => m_Settings.DelayJustificationEnabled = value; }
        public int DelayJustificationMinutes { set => m_Settings.DelayJustificationMinutes = value; }
        public bool CheckPlausibility { set => m_Settings.CheckPlausibility = value; }
        public int EstwTimeout { set => m_Settings.EstwTimeout = value; }
        public bool LoadInactiveEstws { set => m_Settings.LoadInactiveEstws = value; }

        #endregion

        #region [Dispose]
        public void Dispose()
        {
            m_Settings.DelayJustificationEnabled = m_OriginalSettings.DelayJustificationEnabled;
            m_Settings.DelayJustificationMinutes = m_OriginalSettings.DelayJustificationMinutes;
            m_Settings.CheckPlausibility = m_OriginalSettings.CheckPlausibility;
            m_Settings.EstwTimeout = m_OriginalSettings.EstwTimeout;
            m_Settings.LoadInactiveEstws = m_OriginalSettings.LoadInactiveEstws;
        }
        #endregion

    }
}
