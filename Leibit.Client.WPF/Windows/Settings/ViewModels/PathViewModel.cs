using Leibit.BLL;
using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.Settings.ViewModels
{
    public class PathViewModel : ViewModelBase
    {

        #region - Needs -
        private ESTW m_Estw;
        private CommandHandler m_BrowseCommand;
        #endregion

        #region - Ctor -
        public PathViewModel(ESTW estw, string path)
        {
            if (estw == null)
                throw new ArgumentNullException("estw must not be null");

            m_Estw = estw;
            Path = path;

            m_BrowseCommand = new CommandHandler(__Browse, true);
        }
        #endregion

        #region - Properties -

        #region [EstwId]
        internal string EstwId => m_Estw.Id;
        #endregion

        #region [EstwName]
        public string EstwName => m_Estw.Name;
        #endregion

        #region [AreaId]
        internal string AreaId => m_Estw.Area.Id;
        #endregion

        #region [AreaName]
        public string AreaName => m_Estw.Area.Name;
        #endregion

        #region [Path]
        public string Path
        {
            get
            {
                return Get<string>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [BrowseCommand]
        public ICommand BrowseCommand
        {
            get
            {
                return m_BrowseCommand;
            }
        }
        #endregion

        #region [IsAutoDetermined]
        public bool IsAutoDetermined
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [DeterminePath]
        public bool DeterminePath()
        {
            if (Path.IsNotNullOrWhiteSpace() || m_Estw.ProductName.IsNullOrWhiteSpace())
                return false;

            var bll = new SoftwareInfoBLL();
            var result = bll.GetSoftwareInfo(m_Estw.ProductName);

            if (!result.Succeeded || !result.Result.IsInstalled || result.Result.InstallLocation.IsNullOrWhiteSpace() || !Directory.Exists(result.Result.InstallLocation))
                return false;

            string pattern;

            if (m_Estw.InfrastructureManager == eInfrastructureManager.DB)
                pattern = "ESTW *";
            else if (m_Estw.InfrastructureManager == eInfrastructureManager.OEBB)
                pattern = "BFZ *";
            else
                return false;

            var directories = Directory.EnumerateDirectories(result.Result.InstallLocation, pattern);
            var candidates = directories.Where(d => __Normalize(System.IO.Path.GetFileName(d).Substring(pattern.Length - 1)).Equals(__Normalize(m_Estw.Name), StringComparison.OrdinalIgnoreCase));

            if (candidates.Count() != 1)
                return false;

            Path = candidates.Single();
            IsAutoDetermined = true;
            return true;
        }
        #endregion

        #endregion

        #region - Private methods -

        private void __Browse()
        {
            var Dialog = new FolderBrowserDialog();
            Dialog.SelectedPath = Path;

            if (Dialog.ShowDialog() == DialogResult.OK)
                Path = Dialog.SelectedPath;
        }

        private string __Normalize(string path)
        {
            return path.Replace(" ", string.Empty).Replace("-", string.Empty);
        }

        #endregion

    }
}
