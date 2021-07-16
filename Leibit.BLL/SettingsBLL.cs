using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Entities.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
                __SaveSettings(Setting);

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

        #region [AreSettingsComplete]
        public OperationResult<bool> AreSettingsComplete()
        {
            try
            {
                var settings = __GetSettings();
                __ValidateSettings(settings);

                var result = new OperationResult<bool>();
                result.Result = settings.Paths.Any();
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

        #region [GetWindowSettings]
        public OperationResult<WindowSettings> GetWindowSettings()
        {
            try
            {
                var settings = __GetSettings();
                var result = new OperationResult<WindowSettings>();
                result.Result = settings.WindowSettings;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult<WindowSettings> { Message = ex.Message };
            }
        }
        #endregion

        #region [SaveWindowSettings]
        public OperationResult<WindowSettings> SaveWindowSettings(WindowSettings windowSettings)
        {
            try
            {
                var settings = __GetSettings();
                settings.WindowSettings = windowSettings;
                __SaveSettings(settings);
                settings = __GetSettings();

                var result = new OperationResult<WindowSettings>();
                result.Result = settings.WindowSettings;
                result.Succeeded = true;
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult<WindowSettings> { Message = ex.Message };
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
                __FillDefaultValuesIfNeeded(m_Settings);
            }
            else
                m_Settings = __GetDefaultSettings();

            return m_Settings;
        }

        private void __SaveSettings(Settings settings)
        {
            var settingsFile = new FileInfo(m_SettingsFile);

            if (!settingsFile.Directory.Exists)
                settingsFile.Directory.Create();

            var json = JsonConvert.SerializeObject(settings);
            File.WriteAllText(m_SettingsFile, json);

            m_Settings = null;
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
                    Messages.Add($"Das Verzeichnis '{Estw.Value}' existiert nicht.");
                else if (!Directory.Exists(Path.Combine(Estw.Value, Constants.SCHEDULE_FOLDER)) || !Directory.Exists(Path.Combine(Estw.Value, Constants.LOCAL_ORDERS_FOLDER)))
                    Messages.Add($"Das Verzeichnis '{Estw.Value}' ist kein gültiges ESTWsim-Verzeichnis.");
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

        private void __FillDefaultValuesIfNeeded(Settings settings)
        {
            var defaultSettings = __GetDefaultSettings();

            if (!settings.FollowUpTime.HasValue)
                settings.FollowUpTime = defaultSettings.FollowUpTime;
            if (!settings.AutomaticallyCheckForUpdates.HasValue)
                settings.AutomaticallyCheckForUpdates = defaultSettings.AutomaticallyCheckForUpdates;
        }

        private Settings __GetDefaultSettings()
        {
            var settings = new Settings();
            settings.DelayJustificationEnabled = true;
            settings.DelayJustificationMinutes = 3;
            settings.CheckPlausibility = true;
            settings.DisplayCompleteTrainSchedule = true;
            settings.EstwTimeout = 30;
            settings.LoadInactiveEstws = true;
            settings.AutomaticReadyMessageEnabled = true;
            settings.AutomaticReadyMessageTime = 2;
            settings.WindowColor = -16728065;
            settings.WindowSettings = new WindowSettings { Width = 900, Height = 600, Maximized = true };
            settings.EstwOnlinePath = @".\ESTWonline\";
            settings.FollowUpTime = 5;
            settings.AutomaticallyCheckForUpdates = true;
            return settings;
        }

        #endregion

    }
}
