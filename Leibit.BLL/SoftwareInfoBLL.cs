using Leibit.Core.Common;
using Leibit.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Leibit.BLL
{
    public class SoftwareInfoBLL : BLLBase
    {

        #region - Public methods -

        #region [GetSoftwareInfo]
        public OperationResult<SoftwareInfo> GetSoftwareInfo(string softwareName)
        {
            try
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    return OperationResult<SoftwareInfo>.Ok(new SoftwareInfo { IsInstalled = false });

                var candidates = __Search(Registry.CurrentUser, softwareName);
                candidates.AddRange(__Search(Registry.LocalMachine, softwareName));

                var result = candidates.OrderByDescending(x => x.DisplayVersion).FirstOrDefault() ?? new SoftwareInfo { IsInstalled = false };
                return OperationResult<SoftwareInfo>.Ok(result);
            }
            catch (Exception ex)
            {
                return OperationResult<SoftwareInfo>.Fail(ex.ToString());
            }
        }
        #endregion

        #endregion

        #region - Private helpers -

        #region [__Search]
        private List<SoftwareInfo> __Search(RegistryKey key, string softwareName)
        {
            var baseKey = key.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            var candidates = new List<SoftwareInfo>();

            foreach (var subKeyName in baseKey.GetSubKeyNames())
            {
                var candidate = baseKey.OpenSubKey(subKeyName);
                var displayName = candidate.GetValue("DisplayName")?.ToString();
                var displayVersion = candidate.GetValue("DisplayVersion")?.ToString();
                var installLocation = candidate.GetValue("InstallLocation")?.ToString();

                if (displayName == null || displayVersion == null || installLocation == null)
                    continue;

                if (!Directory.Exists(installLocation))
                    continue;

                var normalizedDisplayName = displayName.Replace(" ", string.Empty).Replace("-", string.Empty);
                softwareName = softwareName.Replace(" ", string.Empty).Replace("-", string.Empty);

                if (!normalizedDisplayName.Contains(softwareName))
                    continue;

                var softwareInfo = new SoftwareInfo();
                softwareInfo.IsInstalled = true;
                softwareInfo.DisplayName = displayName;
                softwareInfo.DisplayVersion = displayVersion;
                softwareInfo.InstallLocation = installLocation;
                candidates.Add(softwareInfo);
            }

            return candidates;
        }
        #endregion

        #endregion

    }
}
