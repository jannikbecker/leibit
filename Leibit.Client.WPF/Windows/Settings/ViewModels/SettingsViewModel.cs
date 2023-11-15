using Leibit.BLL;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
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
        private Entities.Settings.Settings m_Settings;
        private bool m_IsScaleFactorDragging;
        private ObservableCollection<PathViewModel> m_AllPathViewModels;

        private CommandHandler m_SaveCommand;
        private CommandHandler m_CancelCommand;
        private CommandHandler m_EstwOnlineCommand;
        private CommandHandler m_DeterminePathsCommand;
        #endregion

        #region - Events -
        public event EventHandler SettingsChanged;
        #endregion

        #region - Ctor -
        public SettingsViewModel(ObservableCollection<Area> areas)
        {
            m_SettingsBll = new SettingsBLL();
            m_AllPathViewModels = new ObservableCollection<PathViewModel>();

            Areas = new ObservableCollection<Area>(areas.OrderBy(x => x.Name));
            Areas.Insert(0, new Area(null, "<Alle>"));

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
            m_DeterminePathsCommand = new CommandHandler(__DeterminePaths, true);
        }
        #endregion

        #region - Properties -

        #region [SelectedTabIndex]
        public int SelectedTabIndex
        {
            get => Get<int>();
            set => Set(value);
        }
        #endregion

        #region [PathViewModels]
        public ObservableCollection<PathViewModel> PathViewModels
        {
            get => Get<ObservableCollection<PathViewModel>>();
            private set => Set(value);
        }
        #endregion

        #region [Areas]
        public ObservableCollection<Area> Areas
        {
            get => Get<ObservableCollection<Area>>();
            private set => Set(value);
        }
        #endregion

        #region [EstwNameFilter]
        public string EstwNameFilter
        {
            get => Get<string>();
            set
            {
                Set(value);
                __Filter();
            }
        }
        #endregion

        #region [AreaIdFilter]
        public string AreaIdFilter
        {
            get => Get<string>();
            set
            {
                Set(value);
                __Filter();
            }
        }
        #endregion

        #region [FilterMissingPaths]
        public bool FilterMissingPaths
        {
            get => Get<bool>();
            set
            {
                Set(value);
                __Filter();
            }
        }
        #endregion

        #region [ShowPathsWarning]
        public bool ShowPathsWarning
        {
            get => Get<bool>();
            private set => Set(value);
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

        #region [AutomaticReadyMessageTime]
        public eAutomaticReadyMessageBehaviour? AutomaticReadyMessageBehaviour
        {
            get
            {
                return m_Settings.AutomaticReadyMessageBehaviour;
            }
            set
            {
                m_Settings.AutomaticReadyMessageBehaviour = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(AutomaticReadyMessageIsDisabled));
                OnPropertyChanged(nameof(AutomaticReadyMessageIsFix));
                OnPropertyChanged(nameof(AutomaticReadyMessageIsRandom));
            }
        }
        #endregion

        #region [AutomaticReadyMessageIsDisabled]
        public bool AutomaticReadyMessageIsDisabled
        {
            get => AutomaticReadyMessageBehaviour == eAutomaticReadyMessageBehaviour.Disabled;
            set
            {
                if (value)
                    AutomaticReadyMessageBehaviour = eAutomaticReadyMessageBehaviour.Disabled;
            }
        }
        #endregion

        #region [AutomaticReadyMessageIsFix]
        public bool AutomaticReadyMessageIsFix
        {
            get => AutomaticReadyMessageBehaviour == eAutomaticReadyMessageBehaviour.Fix;
            set
            {
                if (value)
                    AutomaticReadyMessageBehaviour = eAutomaticReadyMessageBehaviour.Fix;
            }
        }
        #endregion

        #region [AutomaticReadyMessageIsRandom]
        public bool AutomaticReadyMessageIsRandom
        {
            get => AutomaticReadyMessageBehaviour == eAutomaticReadyMessageBehaviour.Random;
            set
            {
                if (value)
                    AutomaticReadyMessageBehaviour = eAutomaticReadyMessageBehaviour.Random;
            }
        }
        #endregion

        #region [AutomaticReadyMessageTime]
        public int? AutomaticReadyMessageTime
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

        #region [AutomaticReadyMessageBeginTime]
        public int? AutomaticReadyMessageBeginTime
        {
            get
            {
                return m_Settings.AutomaticReadyMessageBeginTime;
            }
            set
            {
                m_Settings.AutomaticReadyMessageBeginTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [AutomaticReadyMessageEndTime]
        public int? AutomaticReadyMessageEndTime
        {
            get
            {
                return m_Settings.AutomaticReadyMessageEndTime;
            }
            set
            {
                m_Settings.AutomaticReadyMessageEndTime = value;
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

        #region [LeadTime]
        public int? LeadTime
        {
            get
            {
                return m_Settings.LeadTime;
            }
            set
            {
                m_Settings.LeadTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [FollowUpTime]
        public int? FollowUpTime
        {
            get
            {
                return m_Settings.FollowUpTime;
            }
            set
            {
                m_Settings.FollowUpTime = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [AutomaticallyCheckForUpdates]
        public bool AutomaticallyCheckForUpdates
        {
            get
            {
                return m_Settings.AutomaticallyCheckForUpdates.Value;
            }
            set
            {
                m_Settings.AutomaticallyCheckForUpdates = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [AutomaticallyInstallUpdates]
        public bool AutomaticallyInstallUpdates
        {
            get
            {
                return m_Settings.AutomaticallyInstallUpdates;
            }
            set
            {
                m_Settings.AutomaticallyInstallUpdates = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [Skin]
        public eSkin Skin
        {
            get
            {
                return m_Settings.Skin;
            }
            set
            {
                m_Settings.Skin = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [ScaleFactor]
        public int? ScaleFactor
        {
            get
            {
                return m_Settings.ScaleFactor;
            }
            set
            {
                m_Settings.ScaleFactor = value;
                OnPropertyChanged();

                if (!m_IsScaleFactorDragging)
                    (App.Current as App)?.ChangeScaleFactor(ScaleFactor.Value);
            }
        }
        #endregion

        #region [IsScaleFactorDragging]
        internal bool IsScaleFactorDragging
        {
            get => m_IsScaleFactorDragging;
            set
            {
                if (m_IsScaleFactorDragging && !value)
                    (App.Current as App)?.ChangeScaleFactor(ScaleFactor.Value);

                m_IsScaleFactorDragging = value;
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

        #region [DeterminePathsCommand]
        public ICommand DeterminePathsCommand
        {
            get => m_DeterminePathsCommand;
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
                || PropertyName == nameof(CheckPlausibility)
                || PropertyName == nameof(DisplayCompleteTrainSchedule)
                || PropertyName == nameof(EstwTimeout)
                || PropertyName == nameof(LoadInactiveEstws)
                || PropertyName == nameof(AutomaticReadyMessageBehaviour)
                || PropertyName == nameof(AutomaticReadyMessageTime)
                || PropertyName == nameof(AutomaticReadyMessageBeginTime)
                || PropertyName == nameof(AutomaticReadyMessageEndTime)
                || PropertyName == nameof(EstwOnlinePath)
                || PropertyName == nameof(WindowColor)
                || PropertyName == nameof(LeadTime)
                || PropertyName == nameof(FollowUpTime)
                || PropertyName == nameof(AutomaticallyCheckForUpdates)
                || PropertyName == nameof(AutomaticallyInstallUpdates)
                || PropertyName == nameof(Skin)
                || PropertyName == nameof(ScaleFactor))
            {
                m_SaveCommand.SetCanExecute(true);
            }
        }
        #endregion

        #region [OnWindowClosing]
        public override void OnWindowClosing(object sender, CancelEventArgs e)
        {
            var settingsResult = m_SettingsBll.GetSettings();

            if (settingsResult.Succeeded)
            {
                var settings = settingsResult.Result;
                (App.Current as App)?.ChangeScaleFactor(settings.ScaleFactor.Value);
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        private void __Initialize()
        {
            foreach (var area in Areas)
            {
                foreach (var estw in area.ESTWs)
                {
                    string path = m_Settings.Paths.ContainsKey(estw.Id) ? m_Settings.Paths[estw.Id] : null;

                    var pathVM = new PathViewModel(estw, path);
                    pathVM.PropertyChanged += __PathVM_PropertyChanged;
                    m_AllPathViewModels.Add(pathVM);
                }
            }

            PathViewModels = new ObservableCollection<PathViewModel>(m_AllPathViewModels.OrderBy(x => x.EstwName));
            ShowPathsWarning = !m_Settings.Paths.Any();

            if (ShowPathsWarning)
                SelectedTabIndex = 1;
        }

        private void __PathVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var senderVM = sender as PathViewModel;

            if (senderVM == null)
                return;

            if (e.PropertyName == nameof(PathViewModel.Path))
            {
                if (senderVM.Path.IsNullOrWhiteSpace())
                    m_Settings.Paths.Remove(senderVM.EstwId);
                else
                    m_Settings.Paths[senderVM.EstwId] = senderVM.Path;

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
                (App.Current as App)?.ChangeSkin(m_Settings.Skin);
                SettingsChanged?.Invoke(this, EventArgs.Empty);
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
            Dialog.SelectedPath = Path.GetFullPath(EstwOnlinePath);

            if (Dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                EstwOnlinePath = Dialog.SelectedPath;
        }

        private void __DeterminePaths()
        {
            SelectedTabIndex = 1;

            var changedPaths = 0;

            foreach (var path in PathViewModels)
                if (path.DeterminePath())
                    changedPaths++;

            if (changedPaths == 0)
                MessageBox.Show("Es konnten keine Pfade ermittelt werden.", "Warnung", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            else
                MessageBox.Show($"Es wurden {changedPaths} neue Pfade ermittelt.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);

            if (changedPaths > 0)
                ShowPathsWarning = false;

            __Filter();
        }

        private void __Filter()
        {
            IEnumerable<PathViewModel> paths = m_AllPathViewModels;

            if (EstwNameFilter.IsNotNullOrWhiteSpace())
                paths = paths.Where(x => x.EstwName.Contains(EstwNameFilter, StringComparison.CurrentCultureIgnoreCase));
            if (AreaIdFilter.IsNotNullOrWhiteSpace())
                paths = paths.Where(x => x.AreaId == AreaIdFilter);
            if (FilterMissingPaths)
                paths = paths.Where(x => x.Path.IsNullOrWhiteSpace());

            PathViewModels = new ObservableCollection<PathViewModel>(paths.OrderBy(x => x.EstwName));
        }

        #endregion

    }
}
