using Leibit.BLL;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.Settings.ViewModels
{
    public class SettingsViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private SettingsBLL m_SettingsBll;
        private List<ESTW> m_Estws;
        private Entities.Common.Settings m_Settings;

        private CommandHandler m_SaveCommand;
        private CommandHandler m_CancelCommand;
        private CommandHandler m_EstwOnlineCommand;
        #endregion

        #region - Ctor -
        public SettingsViewModel(ObservableCollection<Area> areas)
        {
            m_Estws = areas.SelectMany(a => a.ESTWs).ToList();
            m_SettingsBll = new SettingsBLL();
            Paths = new ObservableCollection<PathViewModel>();

            var SettingsResult = m_SettingsBll.GetSettings();

            if (SettingsResult.Succeeded)
            {
                m_Settings = SettingsResult.Result.Clone();
                __RefreshPaths();
            }
            else
                ShowMessage(SettingsResult);

            m_SaveCommand = new CommandHandler(__Save, false);
            m_CancelCommand = new CommandHandler(__Cancel, true);
            m_EstwOnlineCommand = new CommandHandler(__BrowseEstwOnline, true);
        }
        #endregion

        #region - Properties -

        #region [Paths]
        public ObservableCollection<PathViewModel> Paths
        {
            get
            {
                return Get<ObservableCollection<PathViewModel>>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [EstwOnlinePath]
        public string EstwOnlinePath
        {
            get
            {
                return m_Settings.EstwOnlinePath;
            }
            set
            {
                m_Settings.EstwOnlinePath = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [WindowColor]
        public int? WindowColor
        {
            get
            {
                return m_Settings.WindowColor;
            }
            set
            {
                m_Settings.WindowColor = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region - Commands -

        #region [SaveCommand]
        public ICommand SaveCommand
        {
            get
            {
                return m_SaveCommand;
            }
        }
        #endregion

        #region [CancelCommand]
        public ICommand CancelCommand
        {
            get
            {
                return m_CancelCommand;
            }
        }
        #endregion

        #region [EstwOnlineCommand]
        public ICommand EstwOnlineCommand
        {
            get
            {
                return m_EstwOnlineCommand;
            }
        }
        #endregion

        #endregion

        #endregion

        #region - Overrides -

        #region [OnPropertyChanged]
        protected override void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            base.OnPropertyChanged(PropertyName);

            if (PropertyName == "EstwOnlinePath" || PropertyName == "WindowColor")
                m_SaveCommand.SetCanExecute(true);
        }
        #endregion

        #endregion

        #region - Private methods -

        private void __RefreshPaths()
        {
            foreach (var vm in Paths)
                vm.PropertyChanged -= PathVM_PropertyChanged;

            Paths.Clear();

            foreach (var estw in m_Estws)
            {
                string Path = m_Settings.Paths.ContainsKey(estw.Id) ? m_Settings.Paths[estw.Id] : null;

                var VM = new PathViewModel(estw, Path);
                VM.PropertyChanged += PathVM_PropertyChanged;
                Paths.Add(VM);
            }
        }

        private void PathVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var VM = sender as PathViewModel;

            if (VM == null)
                return;

            if (e.PropertyName == "Path")
            {
                m_Settings.Paths[VM.EstwId] = VM.Path;
                m_SaveCommand.SetCanExecute(true);
            }
        }

        private void __Save()
        {
            var SaveResult = m_SettingsBll.SaveSettings(m_Settings);

            if (SaveResult.Succeeded)
            {
                OnStatusBarTextChanged("Einstellungen gespeichert");
                OnCloseWindow();
            }
            else
                ShowMessage(SaveResult);
        }

        private void __Cancel()
        {
            OnCloseWindow();
        }

        private void __BrowseEstwOnline()
        {
            var Dialog = new FolderBrowserDialog();
            Dialog.SelectedPath = EstwOnlinePath;

            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                EstwOnlinePath = Dialog.SelectedPath;
        }

        #endregion

    }
}
