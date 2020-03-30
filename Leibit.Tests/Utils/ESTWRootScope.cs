using Leibit.BLL;
using Leibit.Core.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Leibit.Tests.Utils
{
    public class ESTWRootScope : IDisposable
    {
        private SettingsBLL m_SettingsBll = new SettingsBLL();
        private Dictionary<string, string> m_OldPaths;
        private string m_OldEstwOnlinePath;

        public ESTWRootScope()
        {
            var SettingsResult = m_SettingsBll.GetSettings();

            if (SettingsResult.Succeeded)
            {
                var Paths = SettingsResult.Result.Paths;
                m_OldPaths = Paths.Clone();
                m_OldEstwOnlinePath = SettingsResult.Result.EstwOnlinePath;

                var ResourceSet = Properties.Resources.ResourceManager.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

                foreach (DictionaryEntry Resource in ResourceSet)
                {
                    if (Resource.Key is string && (Resource.Key as string).StartsWith("path_") && Resource.Value is string)
                    {
                        string EstwId = (Resource.Key as string).Substring(5);
                        Paths[EstwId] = Path.Combine(Environment.CurrentDirectory, Resource.Value as string);
                    }
                }

                SettingsResult.Result.EstwOnlinePath = Path.Combine(Environment.CurrentDirectory, @"TestData\ESTWOnline");
            }
        }

        public void Dispose()
        {
            var SettingsResult = m_SettingsBll.GetSettings();

            if (SettingsResult.Succeeded)
            {
                var Paths = SettingsResult.Result.Paths;
                Paths.Clear();

                foreach (var EstwId in m_OldPaths.Keys)
                    Paths.Add(EstwId, m_OldPaths[EstwId]);

                SettingsResult.Result.EstwOnlinePath = m_OldEstwOnlinePath;
            }
        }

    }
}
