using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Common;
using System;
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
        public string EstwId
        {
            get
            {
                return m_Estw.Id;
            }
        }
        #endregion

        #region [EstwName]
        public string EstwName
        {
            get
            {
                return m_Estw.Name;
            }
        }
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

        #endregion

        #region - Private methods -

        private void __Browse()
        {
            var Dialog = new FolderBrowserDialog();
            Dialog.SelectedPath = Path;

            if (Dialog.ShowDialog() == DialogResult.OK)
                Path = Dialog.SelectedPath;
        }

        #endregion

    }
}
