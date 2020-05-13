using Leibit.BLL;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Forms;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.Settings.ViewModels
{
    public class SettingsViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private SettingsBLL m_SettingsBll;
        private IEnumerable<Area> m_Areas;
        private Entities.Settings.Settings m_Settings;

        private CommandHandler m_SaveCommand;
        private CommandHandler m_CancelCommand;
        private CommandHandler m_EstwOnlineCommand;
        #endregion

        #region - Ctor -
        public SettingsViewModel(ObservableCollection<Area> areas)
        {
            m_Areas = areas;
            m_SettingsBll = new SettingsBLL();
            Areas = new ObservableCollection<AreaViewModel>();

            var SettingsResult = m_SettingsBll.GetSettings();

            if (SettingsResult.Succeeded)
            {
                m_Settings = SettingsResult.Result.Clone();
                __Initialize();
            }
            else
                ShowMessage(SettingsResult);

            m_SaveCommand = new CommandHandler(__Save, false);
            m_CancelCommand = new CommandHandler(__Cancel, true);
            m_EstwOnlineCommand = new CommandHandler(__BrowseEstwOnline, true);
        }
        #endregion

        #region - Properties -

        #region [Areas]
        public ObservableCollection<AreaViewModel> Areas
        {
            get
            {
                return Get<ObservableCollection<AreaViewModel>>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [DelayJustificationEnabled]
        public bool DelayJustificationEnabled
        {
            get
            {
                return m_Settings.DelayJustificationEnabled;
            }
            set
            {
                m_Settings.DelayJustificationEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [DelayJustificationMinutes]
        public int DelayJustificationMinutes
        {
            get
            {
                return m_Settings.DelayJustificationMinutes;
            }
            set
            {
                m_Settings.DelayJustificationMinutes = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [WriteDelayJustificationFile]
        public bool WriteDelayJustificationFile
        {
            get
            {
                return m_Settings.WriteDelayJustificationFile;
            }
            set
            {
                m_Settings.WriteDelayJustificationFile = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [CheckPlausibility]
        public bool CheckPlausibility
        {
            get
            {
                return m_Settings.CheckPlausibility;
            }
            set
            {
                m_Settings.CheckPlausibility = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [DisplayCompleteTrainSchedule]
        public bool DisplayCompleteTrainSchedule
        {
            get
            {
                return m_Settings.DisplayCompleteTrainSchedule;
            }
            set
            {
                m_Settings.DisplayCompleteTrainSchedule = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [EstwTimeout]
        public int EstwTimeout
        {
            get
            {
                return m_Settings.EstwTimeout;
            }
            set
            {
                m_Settings.EstwTimeout = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [LoadInactiveEstws]
        public bool LoadInactiveEstws
        {
            get
            {
                return m_Settings.LoadInactiveEstws;
            }
            set
            {
                m_Settings.LoadInactiveEstws = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [AutomaticReadyMessageEnabled]
        public bool AutomaticReadyMessageEnabled
        {
            get
            {
                return m_Settings.AutomaticReadyMessageEnabled;
            }
            set
            {
                m_Settings.AutomaticReadyMessageEnabled = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [AutomaticReadyMessageTime]
        public int AutomaticReadyMessageTime
        {
            get
            {
                return m_Settings.AutomaticReadyMessageTime;
            }
            set
            {
                m_Settings.AutomaticReadyMessageTime = value;
                OnPropertyChanged();
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

            if (PropertyName == nameof(DelayJustificationEnabled)
                || PropertyName == nameof(DelayJustificationMinutes)
                || PropertyName == nameof(WriteDelayJustificationFile)
                || PropertyName == nameof(CheckPlausibility)
                || PropertyName == nameof(DisplayCompleteTrainSchedule)
                || PropertyName == nameof(EstwTimeout)
                || PropertyName == nameof(LoadInactiveEstws)
                || PropertyName == nameof(AutomaticReadyMessageEnabled)
                || PropertyName == nameof(AutomaticReadyMessageTime)
                || PropertyName == nameof(EstwOnlinePath)
                || PropertyName == nameof(WindowColor))
            {
                m_SaveCommand.SetCanExecute(true);
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        private void __Initialize()
        {
            foreach (var area in m_Areas)
            {
                var VM = new AreaViewModel(area, m_Settings);
                VM.PropertyChanged += AreaVM_PropertyChanged;
                Areas.Add(VM);
            }
        }

        private void AreaVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var VM = sender as AreaViewModel;

            if (VM == null)
                return;

            if (e.PropertyName == "Path")
            {
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
