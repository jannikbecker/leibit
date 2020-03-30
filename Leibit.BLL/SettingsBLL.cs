using Leibit.Core.Common;
using Leibit.Core.Exceptions;
using Leibit.Entities.Common;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;

namespace Leibit.BLL
{
    public class SettingsBLL : BLLBase
    {

        #region - Needs -
        private RegistryKey m_RootKey;
        private static Settings m_Settings;
        #endregion

        #region - Constants -
        private const string LEIBIT_ROOT_KEY_NAME = @"Software\Leibit for ESTWsim";
        private const string RULES_KEY_NAME = "Rules";
        #endregion

        #region - Ctor -
        public SettingsBLL()
        {
            m_RootKey = Registry.CurrentUser;
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
                __SavePaths(Setting);
                __SaveOtherSettings(Setting);

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

        #region [LoadRule]
        public T LoadRule<T>(string KeyName, string RuleName)
        {
            return LoadRule(KeyName, RuleName, default(T));
        }

        public T LoadRule<T>(string KeyName, string RuleName, T DefaultValue)
        {
            try
            {
                var RuleKey = m_RootKey.OpenSubKey(Path.Combine(LEIBIT_ROOT_KEY_NAME, RULES_KEY_NAME, KeyName));

                if (RuleKey != null)
                {
                    object Value = RuleKey.GetValue(RuleName, DefaultValue);

                    if (Value is T)
                        return (T)Value;

                    Type ValueType = typeof(T);

                    if (ValueType.IsEnum && Value != null && Enum.IsDefined(ValueType, Value))
                        return (T)Enum.Parse(ValueType, Value.ToString());
                }

                return DefaultValue;
            }
            catch
            {
                return DefaultValue;
            }
        }
        #endregion

        #region [SaveRule]
        public OperationResult<bool> SaveRule<T>(string KeyName, string RuleName, T Value)
        {
            try
            {
                var RuleKey = m_RootKey.CreateSubKey(Path.Combine(LEIBIT_ROOT_KEY_NAME, RULES_KEY_NAME, KeyName));
                RuleKey.SetValue(RuleName, Value);

                var Result = new OperationResult<bool>();
                Result.Result = true;
                Result.Succeeded = true;
                return Result;
            }
            catch (Exception ex)
            {
                return new OperationResult<bool> { Message = ex.Message };
            }
        }
        #endregion

        #region [ClearRules]
        public OperationResult<bool> ClearRules(string KeyName)
        {
            try
            {
                var RuleKey = m_RootKey.CreateSubKey(Path.Combine(LEIBIT_ROOT_KEY_NAME, RULES_KEY_NAME));
                RuleKey.DeleteSubKeyTree(KeyName);

                var Result = new OperationResult<bool>();
                Result.Result = true;
                Result.Succeeded = true;
                return Result;
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

            var Result = new Settings();
            __LoadPaths(Result);
            __LoadOtherSettings(Result);
            m_Settings = Result;
            return Result;
        }

        private void __LoadPaths(Settings Setting)
        {
            var PathKey = m_RootKey.OpenSubKey(Path.Combine(LEIBIT_ROOT_KEY_NAME, "Paths"));

            if (PathKey == null)
                return;

            var Paths = PathKey.GetValueNames();

            foreach (var name in Paths)
                Setting.Paths.AddIfNotNull(name, PathKey.GetValue(name) as String);
        }

        private void __LoadOtherSettings(Settings Setting)
        {
            var OtherKey = m_RootKey.OpenSubKey(Path.Combine(LEIBIT_ROOT_KEY_NAME, "Other"));

            if (OtherKey == null)
                return;

            Setting.EstwOnlinePath = OtherKey.GetValue("ESTWonline") as String;
            Setting.WindowColor = OtherKey.GetValue("WindowColor") as int?;
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

            if (Setting.EstwOnlinePath.IsNullOrWhiteSpace())
                Messages.Add("Der Pfad zu ESTWonline darf nicht leer sein.");

            if (!Directory.Exists(Setting.EstwOnlinePath))
                Messages.Add(String.Format("Das Verzeichnis '{0}' existiert nicht.", Setting.EstwOnlinePath));
            else if (!File.Exists(Path.Combine(Setting.EstwOnlinePath, "ESTWonline.exe")))
                Messages.Add(String.Format("Das Verzeichnis '{0}' ist kein gültiges ESTWonline-Verzeichnis.", Setting.EstwOnlinePath));

            if (Messages.Count > 0)
                throw new ValidationFailedException(String.Join(Environment.NewLine, Messages));
        }

        private void __SavePaths(Settings Setting)
        {
            var PathKey = m_RootKey.CreateSubKey(Path.Combine(LEIBIT_ROOT_KEY_NAME, "Paths"));

            foreach (var Estw in Setting.Paths)
                PathKey.SetValue(Estw.Key, Estw.Value);
        }

        private void __SaveOtherSettings(Settings Setting)
        {
            var OtherKey = m_RootKey.CreateSubKey(Path.Combine(LEIBIT_ROOT_KEY_NAME, "Other"));
            OtherKey.SetValue("ESTWonline", Setting.EstwOnlinePath);

            if (Setting.WindowColor.HasValue)
                OtherKey.SetValue("WindowColor", Setting.WindowColor.Value);
        }

        #endregion

    }
}
