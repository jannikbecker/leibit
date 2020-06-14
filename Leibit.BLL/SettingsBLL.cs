using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Entities.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Leibit.BLL
{
    public class SettingsBLL : BLLBase
    {

        #region - Needs -
        private static Settings m_Settings;
        private readonly string m_SettingsFile;
        #endregion

        #region - Ctor -
        public SettingsBLL()
        {
            m_SettingsFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LeiBIT", "settings.json");
        }
        #endregion

        #region - Public methods -

        #region [GetSettings]
        public OperationResult<Settings> GetSettings()
        {
            try
            {
                var Result = new OperationResult<Settings>();
                Result.Result = __GetSettings();
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<Settings> { Message = ex.Message };
            }
        }
        #endregion

        #region [GetPath]
        public OperationResult<string> GetPath(string EstwId)
        {
            try
            {
                var Result = new OperationResult<string>();
                var Settings = __GetSettings();

                if (Settings.Paths.ContainsKey(EstwId))
                    Result.Result = Settings.Paths[EstwId];

                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<string> { Message = ex.Message };
            }
        }
        #endregion

        #region [SaveSettings]
        public OperationResult<Settings> SaveSettings(Settings Setting)
        {
            try
            {
                __ValidateSettings(Setting);

                var settingsFile = new FileInfo(m_SettingsFile);

                if (!settingsFile.Directory.Exists)
                    settingsFile.Directory.Create();

                var json = JsonConvert.SerializeObject(Setting);
                File.WriteAllText(m_SettingsFile, json);

                m_Settings = null;

                var Result = new OperationResult<Settings>();
                Result.Result = __GetSettings();
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<Settings> { Message = ex.Message };
            }
        }
        #endregion

        #region [AreSettingsValid]
        public OperationResult<bool> AreSettingsValid()
        {
            try
            {
                var settings = __GetSettings();
                __ValidateSettings(settings);

                var result = new OperationResult<bool>();
                result.Result = true;
                result.Succeeded = true;
                return result;
            }
            catch (ValidationFailedException)
            {
                var result = new OperationResult<bool>();
                result.Result = false;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
            }
        }
        #endregion

        #endregion

        #region - Helper methods -

        private Settings __GetSettings()
        {
            if (m_Settings != null)
                return m_Settings;

            if (File.Exists(m_SettingsFile))
            {
                var json = File.ReadAllText(m_SettingsFile);
                m_Settings = JsonConvert.DeserializeObject<Settings>(json);
            }
            else
                m_Settings = __GetDefaultSettings();

            return m_Settings;
        }

        private void __ValidateSettings(Settings Setting)
        {
            var Messages = new List<string>();

            foreach (var Estw in Setting.Paths)
            {
                if (Estw.Value.IsNullOrWhiteSpace())
                {
                    Messages.Add("Der Pfad zum ESTWsim Verzeichnis darf nicht leer sein.");
                    break;
                }

                if (!Directory.Exists(Estw.Value))
                    Messages.Add(String.Format("Das Verzeichnis '{0}' existiert nicht.", Estw.Value));
                else if (!File.Exists(Path.Combine(Estw.Value, "estw_sim.exe")))
                    Messages.Add(String.Format("Das Verzeichnis '{0}' ist kein gültiges ESTWsim-Verzeichnis.", Estw.Value));
            }

            if (Setting.DelayJustificationMinutes <= 0)
                Messages.Add($"{Setting.DelayJustificationMinutes} ist keine gültige Verspätung");

            if (Setting.EstwTimeout < 6)
                Messages.Add("Die Zeit, nach der ein ESTW inaktiv wird, muss mindestens 6 Sekunden betragen.");

            if (Setting.EstwOnlinePath.IsNullOrWhiteSpace())
                Messages.Add("Der Pfad zu ESTWonline darf nicht leer sein.");

            if (!Directory.Exists(Setting.EstwOnlinePath))
                Messages.Add(String.Format("Das Verzeichnis '{0}' existiert nicht.", Setting.EstwOnlinePath));
            else if (!File.Exists(Path.Combine(Setting.EstwOnlinePath, "ESTWonline.exe")))
                Messages.Add(String.Format("Das Verzeichnis '{0}' ist kein gültiges ESTWonline-Verzeichnis.", Setting.EstwOnlinePath));

            if (Messages.Count > 0)
                throw new ValidationFailedException(String.Join(Environment.NewLine, Messages));
        }

        private Settings __GetDefaultSettings()
        {
            var settings = new Settings();
            settings.DelayJustificationEnabled = true;
            settings.DelayJustificationMinutes = 3;
            settings.WriteDelayJustificationFile = false;
            settings.CheckPlausibility = true;
            settings.DisplayCompleteTrainSchedule = true;
            settings.EstwTimeout = 30;
            settings.LoadInactiveEstws = true;
            settings.AutomaticReadyMessageEnabled = true;
            settings.AutomaticReadyMessageTime = 2;
            return settings;
        }

        #endregion

    }
}
