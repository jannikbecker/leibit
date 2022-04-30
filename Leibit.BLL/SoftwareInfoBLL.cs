using Leibit.Core.Common;
using Leibit.Entities;
using Microsoft.Win32;
using System;
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

                var key = __Search(Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"), softwareName);

                if (key == null)
                    key = __Search(Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"), softwareName);

                if (key == null)
                    return OperationResult<SoftwareInfo>.Ok(new SoftwareInfo { IsInstalled = false });

                var result = new SoftwareInfo();
                result.IsInstalled = true;
                result.DisplayName = key.GetValue("DisplayName")?.ToString();
                result.DisplayVersion = key.GetValue("DisplayVersion")?.ToString();
                result.InstallLocation = key.GetValue("InstallLocation")?.ToString();
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
        private RegistryKey __Search(RegistryKey key, string softwareName)
        {
            foreach (var subKeyName in key.GetSubKeyNames())
            {
                var candidate = key.OpenSubKey(subKeyName);
                var displayName = candidate.GetValue("DisplayName")?.ToString();

                if (displayName == null)
                    continue;

                displayName = displayName.Replace(" ", string.Empty).Replace("-", string.Empty);
                softwareName = softwareName.Replace(" ", string.Empty).Replace("-", string.Empty);

                if (displayName.Contains(softwareName))
                    return candidate;
            }

            return null;
        }
        #endregion

        #endregion

    }
}
