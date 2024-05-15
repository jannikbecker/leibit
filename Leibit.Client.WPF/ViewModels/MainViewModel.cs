using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.Dialogs.Error;
using Leibit.Client.WPF.Dialogs.OpenReminders;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.Windows.About.ViewModels;
using Leibit.Client.WPF.Windows.About.Views;
using Leibit.Client.WPF.Windows.DelayJustification.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.Views;
using Leibit.Client.WPF.Windows.Display.ViewModels;
using Leibit.Client.WPF.Windows.Display.Views;
using Leibit.Client.WPF.Windows.ESTWSelection.ViewModels;
using Leibit.Client.WPF.Windows.ESTWSelection.Views;
using Leibit.Client.WPF.Windows.LocalOrders.ViewModels;
using Leibit.Client.WPF.Windows.LocalOrders.Views;
using Leibit.Client.WPF.Windows.Settings.ViewModels;
using Leibit.Client.WPF.Windows.Settings.Views;
using Leibit.Client.WPF.Windows.SystemState.ViewModels;
using Leibit.Client.WPF.Windows.SystemState.Views;
using Leibit.Client.WPF.Windows.TimeTable.ViewModels;
using Leibit.Client.WPF.Windows.TimeTable.Views;
using Leibit.Client.WPF.Windows.TrainComposition.ViewModels;
using Leibit.Client.WPF.Windows.TrainComposition.Views;
using Leibit.Client.WPF.Windows.TrainProgressInformation.ViewModels;
using Leibit.Client.WPF.Windows.TrainProgressInformation.Views;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Controls;
using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.Serialization;
using Leibit.Entities.Settings;
using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.ViewModels
{
    public class MainViewModel : WindowViewModelBase
    {

        #region - Needs -
        private readonly InitializationBLL m_InitializationBll;
        private readonly LiveDataBLL m_LiveDataBll;
        private readonly SettingsBLL m_SettingsBll;
        private readonly SerializationBLL m_SerializationBll;
        private readonly SoftwareInfoBLL m_SoftwareInfoBll;
        private UpdateBLL m_UpdateBll;

        private Area m_CurrentArea;
        private Thread m_RefreshingThread;
        private CancellationTokenSource m_CancellationTokenSource;
        private string m_CurrentFilename;
        private bool m_ForceClose;
        private SoftwareInfo m_BildFplInfo;
        private System.Timers.Timer m_StatusBarMessageTimer;

        private CommandHandler m_NewCommand;
        private CommandHandler m_OpenCommand;
        private CommandHandler m_SaveCommand;
        private CommandHandler m_SaveAsCommand;
        private CommandHandler m_SettingsCommand;
        private CommandHandler m_EstwOnlineCommand;
        private CommandHandler m_BildFplCommand;
        private CommandHandler m_ExitCommand;
        private CommandHandler m_EstwSelectionCommand;
        private CommandHandler m_TrainProgressInformationCommand;
        private CommandHandler m_TimeTableCommand;
        private CommandHandler m_DisplayCommand;
        private CommandHandler m_TrainScheduleCommand;
        private CommandHandler m_SystemStateCommand;
        private CommandHandler m_RemindersCommand;
        private CommandHandler m_SaveLayoutCommand;
        private CommandHandler m_ClearChildWindowsCommand;
        private CommandHandler m_ShowHelpCommand;
        private CommandHandler m_ShowQuickStartHelpCommand;
        private CommandHandler m_AboutCommand;
        private CommandHandler m_DebugModeCommand;
        #endregion

        #region - Const -
        private const string NOTIFICATION_TYPE = "Type";
        private const string NOTIFICATION_TYPE_UPDATE_AVAILABLE = "UpdateAvailable";
        private const string NOTIFICATION_TYPE_UPDATE_INSTALLED = "UpdateInstalled";
        private const string NOTIFICATION_TYPE_ERROR = "Error";
        private const string NOTIFICATION_ACTION = "Action";
        #endregion

        #region - Ctor -
        public MainViewModel()
        {
            m_InitializationBll = new InitializationBLL();
            m_LiveDataBll = new LiveDataBLL();
            m_SettingsBll = new SettingsBLL();
            m_SerializationBll = new SerializationBLL();
            m_SoftwareInfoBll = new SoftwareInfoBLL();

            var BildFplResult = m_SoftwareInfoBll.GetSoftwareInfo("Bildfahrplan");

            if (BildFplResult.Succeeded)
                m_BildFplInfo = BildFplResult.Result;
            else
            {
                ShowMessage(BildFplResult);
                m_BildFplInfo = new SoftwareInfo { IsInstalled = false };
            }

            m_NewCommand = new CommandHandler<string>(__New, true);
            m_OpenCommand = new CommandHandler(__Open, true);
            m_SaveCommand = new CommandHandler(__Save, false);
            m_SaveAsCommand = new CommandHandler(() => __SaveAs(null), false);
            m_SettingsCommand = new CommandHandler(__Settings, true);
            m_EstwOnlineCommand = new CommandHandler(__StartEstwOnline, true);
            m_BildFplCommand = new CommandHandler(__StartBildFpl, IsBildFplInstalled);
            m_ExitCommand = new CommandHandler(__Exit, true);
            m_EstwSelectionCommand = new CommandHandler(__ShowEstwSelectionWindow, false);
            m_TrainProgressInformationCommand = new CommandHandler(__ShowTrainProgressInformationWindow, false);
            m_TimeTableCommand = new CommandHandler<Station>(__ShowTimeTableWindow, true);
            m_DisplayCommand = new CommandHandler(__ShowDisplay, false);
            m_TrainScheduleCommand = new CommandHandler(__ShowTrainScheduleWindow, false);
            m_SystemStateCommand = new CommandHandler(__ShowSystemStateWindow, false);
            m_RemindersCommand = new CommandHandler(OpenRemindersDialog.Open, false);
            m_SaveLayoutCommand = new CommandHandler(__SaveLayout, true);
            m_ClearChildWindowsCommand = new CommandHandler(__ClearChildWindows, true);
            m_ShowHelpCommand = new CommandHandler(__ShowHelp, true);
            m_ShowQuickStartHelpCommand = new CommandHandler(__ShowQuickStartHelp, true);
            m_AboutCommand = new CommandHandler(__ShowAboutWindow, true);
            m_DebugModeCommand = new CommandHandler(__ToggleDebugMode, true);

            ChildWindows = new ObservableCollection<LeibitWindow>();

            Runtime.VisibleStationsChanged += __VisibleStationsChanged;

            var AreaResult = m_InitializationBll.GetAreaInformation();

            if (AreaResult.Succeeded)
                Areas = AreaResult.Result.OrderBy(a => a.Name).ToObservableCollection();
            else
            {
                ShowMessage(AreaResult);
                Areas = new ObservableCollection<Area>();
            }

            ProgressBarVisibility = Visibility.Collapsed;

            m_StatusBarMessageTimer = new System.Timers.Timer(10000);
            m_StatusBarMessageTimer.AutoReset = false;
            m_StatusBarMessageTimer.Elapsed += (sender, e) => StatusBarMessage = null;

            ToastNotificationManagerCompat.History.Clear();
            ToastNotificationManagerCompat.OnActivated += __ToastNotificationClicked;
            __CheckForUpdatesIfNeeded();

            var settingsResult = m_SettingsBll.GetSettings();

            if (settingsResult.Succeeded && settingsResult.Result != null)
            {
                (App.Current as App)?.ChangeSkin(settingsResult.Result.Skin);
                (App.Current as App)?.ChangeScaleFactor(settingsResult.Result.ScaleFactor.Value);
            }
        }
        #endregion

        #region - Properties -

        #region [Areas]
        public ObservableCollection<Area> Areas
        {
            get
            {
                return Get<ObservableCollection<Area>>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsNewDropDownOpen]
        public bool IsNewDropDownOpen
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsTimeTableDropDownOpen]
        public bool IsTimeTableDropDownOpen
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsTrainScheduleDropDownOpen]
        public bool IsTrainScheduleDropDownOpen
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);

                if (!value)
                    TrainScheduleNumber = null;
            }
        }
        #endregion

        #region [ChildWindows]
        public ObservableCollection<LeibitWindow> ChildWindows
        {
            get
            {
                return Get<ObservableCollection<LeibitWindow>>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [StatusBarAreaText]
        public string StatusBarAreaText
        {
            get
            {
                if (m_CurrentArea == null)
                    return "Kein Bereich geladen";
                else
                    return $"Bereich {m_CurrentArea.Name}";
            }
        }
        #endregion

        #region [ConnectedESTWs]
        public string ConnectedESTWs
        {
            get
            {
                if (m_CurrentArea == null)
                    return null;

                var result = new StringBuilder("Verbunden: ");
                var loadedEstw = m_CurrentArea.ESTWs.Where(estw => estw.IsLoaded);

                if (loadedEstw.Any())
                    result.Append(string.Join(", ", loadedEstw.Select(estw => estw.Id).OrderBy(x => x)));
                else
                    result.Append("-");

                return result.ToString();
            }
        }
        #endregion

        #region [CurrentFile]
        public string CurrentFile
        {
            get
            {
                if (m_CurrentArea == null)
                    return null;
                else if (m_CurrentFilename.IsNullOrEmpty())
                    return "ungespeichert";
                else
                    return m_CurrentFilename;
            }
        }
        #endregion

        #region [IsAreaSelected]
        public bool IsAreaSelected
        {
            get => m_CurrentArea != null;
        }
        #endregion

        #region [IsDebugModeActive]
        public bool IsDebugModeActive
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [StatusBarMessage]
        public string StatusBarMessage
        {
            get => Get<string>();
            private set
            {
                Set(value);
                OnPropertyChanged(nameof(HasStatusBarMessage));
            }
        }
        #endregion

        #region [HasStatusBarMessage]
        public bool HasStatusBarMessage
        {
            get => StatusBarMessage.IsNotNullOrEmpty();
        }
        #endregion

        #region [ProgressBarText]
        public string ProgressBarText
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

        #region [ProgressBarPercentage]
        public double ProgressBarPercentage
        {
            get
            {
                return Get<double>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [ProgressBarVisibility]
        public Visibility ProgressBarVisibility
        {
            get => Get<Visibility>();
            set => Set(value);
        }
        #endregion

        #region [Stations]
        public ObservableCollection<Station> Stations
        {
            get
            {
                return Get<ObservableCollection<Station>>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsTimeTableEnabled]
        public bool IsTimeTableEnabled
        {
            get
            {
                return Get<bool>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsTrainScheduleEnabled]
        public bool IsTrainScheduleEnabled
        {
            get
            {
                return Get<bool>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [TrainScheduleNumber]
        public int? TrainScheduleNumber
        {
            get
            {
                return Get<int?>();
            }
            set
            {
                Set(value);
                m_TrainScheduleCommand.SetCanExecute(value.HasValue);
            }
        }
        #endregion

        #region [IsBildFplInstalled]
        public bool IsBildFplInstalled
        {
            get => m_BildFplInfo?.IsInstalled == true;
        }
        #endregion

        #region - Commands -

        #region [NewCommand]
        public ICommand NewCommand
        {
            get
            {
                return m_NewCommand;
            }
        }
        #endregion

        #region [OpenCommand]
        public ICommand OpenCommand
        {
            get
            {
                return m_OpenCommand;
            }
        }
        #endregion

        #region [SaveCommand]
        public ICommand SaveCommand
        {
            get
            {
                return m_SaveCommand;
            }
        }
        #endregion

        #region [SaveAsCommand]
        public ICommand SaveAsCommand
        {
            get
            {
                return m_SaveAsCommand;
            }
        }
        #endregion

        #region [SettingsCommand]
        public ICommand SettingsCommand
        {
            get
            {
                return m_SettingsCommand;
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

        #region [BildFplCommand]
        public ICommand BildFplCommand
        {
            get
            {
                return m_BildFplCommand;
            }
        }
        #endregion

        #region [ExitCommand]
        public ICommand ExitCommand
        {
            get
            {
                return m_ExitCommand;
            }
        }
        #endregion

        #region [EstwSelectionCommand]
        public ICommand EstwSelectionCommand
        {
            get
            {
                return m_EstwSelectionCommand;
            }
        }
        #endregion

        #region [TrainProgressInformationCommand]
        public ICommand TrainProgressInformationCommand
        {
            get
            {
                return m_TrainProgressInformationCommand;
            }
        }
        #endregion

        #region [TimeTableCommand]
        public ICommand TimeTableCommand
        {
            get
            {
                return m_TimeTableCommand;
            }
        }
        #endregion

        #region [DisplayCommand]
        public ICommand DisplayCommand
        {
            get
            {
                return m_DisplayCommand;
            }
        }
        #endregion

        #region [TrainScheduleCommand]
        public ICommand TrainScheduleCommand
        {
            get
            {
                return m_TrainScheduleCommand;
            }
        }
        #endregion

        #region [SystemStateCommand]
        public ICommand SystemStateCommand
        {
            get
            {
                return m_SystemStateCommand;
            }
        }
        #endregion

        #region [RemindersCommand]
        public ICommand RemindersCommand
        {
            get
            {
                return m_RemindersCommand;
            }
        }
        #endregion

        #region [SaveLayoutCommand]
        public ICommand SaveLayoutCommand
        {
            get
            {
                return m_SaveLayoutCommand;
            }
        }
        #endregion

        #region [ClearChildWindowsCommand]
        public ICommand ClearChildWindowsCommand
        {
            get
            {
                return m_ClearChildWindowsCommand;
            }
        }
        #endregion

        #region [ShowHelpCommand]
        public ICommand ShowHelpCommand
        {
            get
            {
                return m_ShowHelpCommand;
            }
        }
        #endregion

        #region [ShowQuickStartHelpCommand]
        public ICommand ShowQuickStartHelpCommand
        {
            get
            {
                return m_ShowQuickStartHelpCommand;
            }
        }
        #endregion

        #region [AboutCommand]
        public ICommand AboutCommand
        {
            get
            {
                return m_AboutCommand;
            }
        }
        #endregion

        #region [DebugModeCommand]
        public ICommand DebugModeCommand
        {
            get
            {
                return m_DebugModeCommand;
            }
        }
        #endregion

        #endregion

        #endregion

        #region - Overrides -

        #region [OnSourceInitialized]
        protected override void OnSourceInitialized(object sender, EventArgs e)
        {
            base.OnSourceInitialized(sender, e);

            if (sender is Window window)
            {
                var windowSettingsResult = m_SettingsBll.GetWindowSettings();

                if (!windowSettingsResult.Succeeded)
                    return;

                var windowSettings = windowSettingsResult.Result;

                if (windowSettings == null)
                    return;

                if (!System.Windows.Forms.Screen.AllScreens.Any(s => __IsWindowVisibleOnScreen(windowSettings, s)))
                    return;

                window.Left = windowSettings.Left;
                window.Top = windowSettings.Top;
                window.Width = windowSettings.Width;
                window.Height = windowSettings.Height;

                if (windowSettings.Maximized)
                    window.WindowState = WindowState.Maximized;
            }

            var SettingsResult = m_SettingsBll.AreSettingsComplete();

            if (SettingsResult.Succeeded)
            {
                if (!SettingsResult.Result)
                {
                    Task.Run(async () =>
                    {
                        await Task.Delay(500); // Dirty hack. We must ensure that layout is updated. Calling UpdateLayout() does not work :(
                        Application.Current?.Dispatcher?.Invoke(__Settings);
                    });
                }
            }
            else
                ShowMessage(SettingsResult);
        }
        #endregion

        #region [OnWindowClosing]
        public override void OnWindowClosing(object sender, CancelEventArgs e)
        {
            base.OnWindowClosing(sender, e);
            e.Cancel |= !__CheckForSaving();

            if (!e.Cancel && sender is Window window)
            {
                var windowSettings = new WindowSettings();

                if (window.WindowState == WindowState.Maximized)
                {
                    windowSettings.Left = window.RestoreBounds.Left;
                    windowSettings.Top = window.RestoreBounds.Top;
                    windowSettings.Width = window.RestoreBounds.Width;
                    windowSettings.Height = window.RestoreBounds.Height;
                    windowSettings.Maximized = true;
                }
                else
                {
                    windowSettings.Left = window.Left;
                    windowSettings.Top = window.Top;
                    windowSettings.Width = window.Width;
                    windowSettings.Height = window.Height;
                    windowSettings.Maximized = false;
                }

                m_SettingsBll.SaveWindowSettings(windowSettings);
                OpenRemindersDialog.CloseAndReset();
            }
        }
        #endregion

        #region [OnWindowClosed]
        public override void OnWindowClosed()
        {
            base.OnWindowClosed();
            __CleanUp();
            Runtime.VisibleStationsChanged -= __VisibleStationsChanged;
        }
        #endregion

        #endregion

        #region - Private methods -

        #region - Commands -

        #region [__New]
        private void __New(string AreaId)
        {
            IsNewDropDownOpen = false;

            if (!__CheckForSaving())
                return;

            __CleanUp();
            __Initialize(Areas.FirstOrDefault(a => a.Id == AreaId));

            if (m_CurrentArea == null)
                return;

            m_CurrentFilename = null;
            OnPropertyChanged(nameof(CurrentFile));

            __ShowEstwSelectionWindow();
        }
        #endregion

        #region [__Open]
        private void __Open()
        {
            if (!__CheckForSaving())
                return;

            var Dialog = new OpenFileDialog();
            Dialog.Filter = "LeiBIT-Dateien|*.leibit2|Alle Dateien|*.*";

            if (Dialog.ShowDialog() == true)
            {
                var Filename = Dialog.FileName;

                if (Filename.IsNullOrWhiteSpace())
                {
                    MessageBox.Show("Bitte wählen Sie eine Datei aus.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var OpenResult = m_SerializationBll.Open(Filename);

                if (!OpenResult.Succeeded)
                {
                    ShowMessage(OpenResult);
                    return;
                }

                if (OpenResult.Result.IsOldVersion)
                {
                    var msgBoxResult = MessageBox.Show("Dieser Zustand wurde mit einer älteren Version von LeiBIT gespeichert. Es kann zu Fehlern in der Darstellung von Informationen kommen. Möchten Sie fortfahren?", "Frage", MessageBoxButton.YesNo, MessageBoxImage.Question);

                    if (msgBoxResult != MessageBoxResult.Yes)
                        return;
                }

                __CleanUp();
                __Initialize(OpenResult.Result.Area);

                var Stations = OpenResult.Result.Area.ESTWs.SelectMany(e => e.Stations);

                foreach (var SerializedStation in OpenResult.Result.VisibleStations)
                {
                    var Station = Stations.FirstOrDefault(s => s.ESTW.Id == SerializedStation.EstwId && s.ShortSymbol == SerializedStation.ShortSymbol);
                    Runtime.VisibleStations.AddIfNotNull(Station);
                }

                __DeserializeVisibleTrains(OpenResult.Result.VisibleTrains);
                __DeserializeHiddenSchedules(OpenResult.Result.HiddenSchedules);

                foreach (var SerializedWindow in OpenResult.Result.Windows)
                {
                    LeibitWindow Window;
                    ChildWindowViewModelBase ViewModel;

                    int TrainNumber;
                    string StationShortSymbol;

                    switch (SerializedWindow.Type)
                    {
                        case eChildWindowType.DelayJustification:
                            TrainNumber = (int)SerializedWindow.Tag;

                            if (m_CurrentArea.LiveTrains.ContainsKey(TrainNumber))
                            {
                                Window = new DelayJustificationView(TrainNumber);
                                ViewModel = new DelayJustificationViewModel(m_CurrentArea.LiveTrains[TrainNumber]);
                                break;
                            }
                            else
                                continue;

                        case eChildWindowType.ESTWSelection:
                            Window = new ESTWSelectionView();
                            ViewModel = new ESTWSelectionViewModel(Window.Dispatcher);
                            break;

                        case eChildWindowType.Settings:
                            Window = new SettingsView();
                            ViewModel = new SettingsViewModel(Areas);
                            break;

                        case eChildWindowType.TimeTable:
                            StationShortSymbol = SerializedWindow.Tag as string;

                            if (StationShortSymbol.IsNullOrWhiteSpace())
                                continue;

                            var Station = Stations.FirstOrDefault(s => s.ShortSymbol == StationShortSymbol && s.HasScheduleFile);

                            if (Station == null)
                                continue;

                            Window = new TimeTableView(StationShortSymbol);
                            ViewModel = new TimeTableViewModel(Window.Dispatcher, Station);
                            break;

                        case eChildWindowType.TrainProgressInformation:
                            Window = new TrainProgressInformationView();
                            ViewModel = new TrainProgressInformationViewModel(Window.Dispatcher, m_CurrentArea);
                            break;

                        case eChildWindowType.TrainSchedule:
                            TrainNumber = (int)SerializedWindow.Tag;

                            if (m_CurrentArea.Trains.ContainsKey(TrainNumber))
                            {
                                Window = new TrainScheduleView(TrainNumber);
                                ViewModel = new TrainScheduleViewModel(Window.Dispatcher, m_CurrentArea.Trains[TrainNumber], m_CurrentArea);
                                break;
                            }
                            else
                                continue;

                        case eChildWindowType.LocalOrders:
                            if (SerializedWindow.Tag is KeyValuePair<int, string> kvp)
                            {
                                TrainNumber = kvp.Key;
                                StationShortSymbol = kvp.Value;
                            }
                            else if (SerializedWindow.Tag is string tag)
                            {
                                var tagParts = tag.Split(';');

                                if (tagParts != null && tagParts.Length >= 2 && int.TryParse(tagParts[0], out TrainNumber))
                                    StationShortSymbol = tagParts[1];
                                else
                                    continue;
                            }
                            else
                                continue;

                            if (m_CurrentArea.Trains.ContainsKey(TrainNumber))
                            {
                                var Schedule = m_CurrentArea.Trains[TrainNumber].Schedules.FirstOrDefault(s => s.Station.ShortSymbol == StationShortSymbol);

                                if (Schedule == null)
                                    continue;

                                Window = new LocalOrdersView(TrainNumber, StationShortSymbol);
                                ViewModel = new LocalOrdersViewModel(Window.Dispatcher, Schedule, m_CurrentArea);
                                break;
                            }
                            else
                                continue;

                        case eChildWindowType.SystemState:
                            Window = new SystemStateView();
                            ViewModel = new SystemStateViewModel(Window.Dispatcher);
                            break;

                        case eChildWindowType.TrainComposition:
                            var trainNumber = (int)SerializedWindow.Tag;

                            if (m_CurrentArea.Trains.ContainsKey(trainNumber))
                            {
                                var train = m_CurrentArea.Trains[trainNumber];

                                if (train.Composition.IsNullOrWhiteSpace())
                                    continue;

                                Window = new TrainCompositionView(trainNumber);
                                ViewModel = new TrainCompositionViewModel(train);
                                break;
                            }
                            else
                                continue;

                        case eChildWindowType.Display:
                            var parts = SerializedWindow.Tag?.ToString().Split(';');

                            if (parts != null && parts.Length >= 3 && Enum.TryParse(parts[0], out eDisplayType type))
                            {
                                Window = new DisplayView();

                                var vm = new DisplayViewModel(Window.Dispatcher, m_CurrentArea);
                                vm.SelectedDisplayType = vm.DisplayTypes.FirstOrDefault(x => x.Type == type);
                                vm.SelectedStation = vm.StationList.FirstOrDefault(s => s.ShortSymbol == parts[1]);
                                vm.SelectedTrack = vm.TrackList.FirstOrDefault(t => t.Name == parts[2]);

                                ViewModel = vm;
                            }
                            else
                                continue;

                            break;

                        default:
                            continue;
                    }

                    Window.Width = SerializedWindow.Width;
                    Window.Height = SerializedWindow.Height;
                    Window.PositionX = SerializedWindow.PositionX;
                    Window.PositionY = SerializedWindow.PositionY;
                    Window.IsDockedOut = SerializedWindow.IsDockedOut;
                    Window.DataContext = ViewModel;

                    __OpenChildWindow(Window);
                }

                m_CurrentFilename = Filename;
                OnPropertyChanged(nameof(CurrentFile));
            }
        }
        #endregion

        #region [__Save]
        private void __Save()
        {
            if (m_CurrentFilename.IsNullOrEmpty())
            {
                __SaveAs(null);
                return;
            }

            var Container = new SerializationContainer();
            Container.Area = m_CurrentArea;
            Container.VisibleStations = Runtime.VisibleStations.Select(s => new SerializedStation { EstwId = s.ESTW.Id, ShortSymbol = s.ShortSymbol }).ToList();
            Container.VisibleTrains = __SerializeVisibleTrains();
            Container.HiddenSchedules = __SerializeHiddenSchedules();

            foreach (var Window in ChildWindows)
            {
                var SerializedWindow = new SerializedWindowInformation();

                if (Window.DataContext is DelayJustificationViewModel delayJustificationViewModel)
                {
                    SerializedWindow.Type = eChildWindowType.DelayJustification;
                    SerializedWindow.Tag = delayJustificationViewModel.CurrentTrain.Train.Number;
                }
                else if (Window.DataContext is ESTWSelectionViewModel)
                    SerializedWindow.Type = eChildWindowType.ESTWSelection;
                else if (Window.DataContext is SettingsViewModel)
                    SerializedWindow.Type = eChildWindowType.Settings;
                else if (Window.DataContext is TimeTableViewModel timeTableViewModel)
                {
                    SerializedWindow.Type = eChildWindowType.TimeTable;
                    SerializedWindow.Tag = timeTableViewModel.CurrentStation.ShortSymbol;
                }
                else if (Window.DataContext is TrainProgressInformationViewModel)
                    SerializedWindow.Type = eChildWindowType.TrainProgressInformation;
                else if (Window.DataContext is TrainScheduleViewModel trainScheduleViewModel && !trainScheduleViewModel.IsInEditMode)
                {
                    SerializedWindow.Type = eChildWindowType.TrainSchedule;
                    SerializedWindow.Tag = (Window.DataContext as TrainScheduleViewModel).CurrentTrain.Number;
                }
                else if (Window.DataContext is LocalOrdersViewModel localOrdersViewModel)
                {
                    SerializedWindow.Type = eChildWindowType.LocalOrders;
                    SerializedWindow.Tag = $"{localOrdersViewModel.CurrentSchedule.Train.Number};{localOrdersViewModel.CurrentSchedule.Station.ShortSymbol}";
                }
                else if (Window.DataContext is SystemStateViewModel)
                    SerializedWindow.Type = eChildWindowType.SystemState;
                else if (Window.DataContext is TrainCompositionViewModel trainCompositionViewModel)
                {
                    SerializedWindow.Type = eChildWindowType.TrainComposition;
                    SerializedWindow.Tag = trainCompositionViewModel.TrainNumber;
                }
                else if (Window.DataContext is DisplayViewModel displayViewModel)
                {
                    SerializedWindow.Type = eChildWindowType.Display;
                    SerializedWindow.Tag = $"{(int)displayViewModel.SelectedDisplayType.Type};{displayViewModel.SelectedStation?.ShortSymbol};{displayViewModel.SelectedTrack?.Name}";
                }
                else
                    continue;

                SerializedWindow.Width = Window.Width;
                SerializedWindow.Height = Window.Height;
                SerializedWindow.PositionX = Window.PositionX;
                SerializedWindow.PositionY = Window.PositionY;
                SerializedWindow.IsDockedOut = Window.IsDockedOut;

                Container.Windows.Add(SerializedWindow);
            }

            var SaveResult = m_SerializationBll.Save(m_CurrentFilename, Container);

            if (SaveResult.Succeeded)
                __ShowStatusBarInfo("Aktueller Zustand gespeichert");
            else
                ShowMessage(SaveResult);
        }
        #endregion

        #region [__SaveAs]
        private void __SaveAs(string proposedFileName)
        {
            var Dialog = new SaveFileDialog();
            Dialog.Filter = "LeiBIT-Dateien|*.leibit2";
            Dialog.FileName = proposedFileName;

            if (Dialog.ShowDialog() == true)
            {
                var Filename = Dialog.FileName;

                if (Filename.IsNullOrWhiteSpace())
                {
                    MessageBox.Show("Bitte geben Sie eine Datei an.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                m_CurrentFilename = Filename;
                OnPropertyChanged(nameof(CurrentFile));
                __Save();
            }
        }
        #endregion

        #region [__Settings]
        private void __Settings()
        {
            var VM = new SettingsViewModel(Areas);
            var Window = new SettingsView();
            Window.DataContext = VM;

            VM.SettingsChanged += __SettingsChanged;

            __OpenChildWindow(Window);
        }
        #endregion

        #region [__StartEstwOnline]
        private void __StartEstwOnline()
        {
            var SettingsResult = m_SettingsBll.GetSettings();

            if (SettingsResult.Succeeded && SettingsResult.Result != null)
            {
                if (SettingsResult.Result.EstwOnlinePath.IsNullOrWhiteSpace())
                {
                    MessageBox.Show("ESTWonline Pfad nicht gesetzt", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    string EstwOnlinePath = Path.Combine(SettingsResult.Result.EstwOnlinePath, "ESTWonline.exe");

                    if (!File.Exists(EstwOnlinePath))
                    {
                        MessageBox.Show("ESTWonline Pfad ungültig", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        try
                        {
                            Process.Start(EstwOnlinePath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(String.Format("Konnte ESTWonline nicht starten: {0}", ex.Message), "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                }
            }
        }
        #endregion

        #region [__StartBildFpl]
        private void __StartBildFpl()
        {
            if (m_BildFplInfo == null || !m_BildFplInfo.IsInstalled || m_BildFplInfo.InstallLocation.IsNullOrWhiteSpace())
                return;

            var exePath = Path.Combine(m_BildFplInfo.InstallLocation, "BildFpl_V2.exe");

            if (File.Exists(exePath))
            {
                var processStartInfo = new ProcessStartInfo(exePath);
                processStartInfo.WorkingDirectory = m_BildFplInfo.InstallLocation;
                Process.Start(processStartInfo);
            }
        }
        #endregion

        #region [__Exit]
        private void __Exit()
        {
            OnCloseWindow();
        }
        #endregion

        #region [__ShowEstwSelectionWindow]
        private void __ShowEstwSelectionWindow()
        {
            var Window = new ESTWSelectionView();
            Window.DataContext = new ESTWSelectionViewModel(Window.Dispatcher);
            __OpenChildWindow(Window);
        }
        #endregion

        #region [__ShowTrainProgressInformationWindow]
        private void __ShowTrainProgressInformationWindow()
        {
            var Window = new TrainProgressInformationView();
            Window.DataContext = new TrainProgressInformationViewModel(Window.Dispatcher, m_CurrentArea);
            __OpenChildWindow(Window);
        }
        #endregion

        #region [__ShowTimeTableWindow]
        private void __ShowTimeTableWindow(Station Station)
        {
            IsTimeTableDropDownOpen = false;

            var Window = new TimeTableView(Station.ShortSymbol);
            Window.DataContext = new TimeTableViewModel(Window.Dispatcher, Station);
            __OpenChildWindow(Window);
        }
        #endregion

        #region [__ShowDisplay]
        private void __ShowDisplay()
        {
            var window = new DisplayView();
            window.DataContext = new DisplayViewModel(window.Dispatcher, m_CurrentArea);

            __OpenChildWindow(window);
        }
        #endregion

        #region [__ShowTrainScheduleWindow]
        private void __ShowTrainScheduleWindow()
        {
            if (!TrainScheduleNumber.HasValue)
                return;

            if (m_CurrentArea.Trains.ContainsKey(TrainScheduleNumber.Value))
            {
                var Train = m_CurrentArea.Trains[TrainScheduleNumber.Value];
                var Window = new TrainScheduleView(TrainScheduleNumber.Value);
                Window.DataContext = new TrainScheduleViewModel(Window.Dispatcher, Train, m_CurrentArea);
                __OpenChildWindow(Window);
            }
            else if (m_CurrentArea.LiveTrains.ContainsKey(TrainScheduleNumber.Value))
            {
                var Train = m_CurrentArea.LiveTrains[TrainScheduleNumber.Value];
                var Window = new TrainScheduleView(TrainScheduleNumber.Value);
                Window.DataContext = new TrainScheduleViewModel(Window.Dispatcher, Train.Train, m_CurrentArea);
                __OpenChildWindow(Window);
            }
            else
            {
                MessageBox.Show(String.Format("Unbekannte Zugnummer: {0}", TrainScheduleNumber), "Zuglauf", MessageBoxButton.OK, MessageBoxImage.Warning);
            }

            IsTrainScheduleDropDownOpen = false;
        }
        #endregion

        #region [__ShowSystemStateWindow]
        private void __ShowSystemStateWindow()
        {
            var Window = new SystemStateView();
            Window.DataContext = new SystemStateViewModel(Window.Dispatcher);
            __OpenChildWindow(Window);
        }
        #endregion

        #region [__SaveLayout]
        private void __SaveLayout()
        {
            ChildWindows.Select(w => w.DataContext).Where(vm => vm is ILayoutSavable).Cast<ILayoutSavable>().ForEach(vm => vm.SaveLayout());
            __ShowStatusBarInfo("Layout wurde gespeichert");
        }
        #endregion

        #region [__ClearChildWindows]
        private void __ClearChildWindows()
        {
            ChildWindows.Clear();
        }
        #endregion

        #region [__ShowHelp]
        private void __ShowHelp()
        {
            var startInfo = new ProcessStartInfo("https://github.com/jannikbecker/leibit/wiki");
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
        }
        #endregion

        #region [__ShowQuickStartHelp]
        private void __ShowQuickStartHelp()
        {
            var startInfo = new ProcessStartInfo("https://github.com/jannikbecker/leibit/wiki/Schnellstartanleitung");
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
        }
        #endregion

        #region [__ShowAboutWindow]
        private void __ShowAboutWindow()
        {
            var window = new AboutView();
            window.DataContext = new AboutViewModel();

            __OpenChildWindow(window);
        }
        #endregion

        #region [__ToggleDebugMode]
        private void __ToggleDebugMode()
        {
            if (!Debugger.IsAttached)
                return;

            m_LiveDataBll.DebugMode = !m_LiveDataBll.DebugMode;
            IsDebugModeActive = m_LiveDataBll.DebugMode;
        }
        #endregion

        #endregion

        #region [__OpenChildWindow]
        private void __OpenChildWindow(LeibitWindow window)
        {
            var existingWindow = ChildWindows.FirstOrDefault(c => c.Identifier != null && c.Identifier == window.Identifier);

            if (existingWindow != null)
            {
                existingWindow.BringToFront();
                return;
            }

            window.ChildWindowStyle = Application.Current.Resources["LeibitChildWindowStyle"] as Style;
            ChildWindows.Add(window);

            if (window.DataContext is ChildWindowViewModelBase vm)
            {
                vm.OpenWindow += (sender, e) =>
                {
                    __OpenChildWindow(e);
                };

                vm.StatusBarTextChanged += (sender, text) => __ShowStatusBarInfo(text);
                vm.ReportProgress += __ReportProgress;

                vm.ShutdownRequested += (sender, force) =>
                {
                    m_ForceClose = force;
                    __Exit();
                };
            }
        }
        #endregion

        #region [__ShowStatusBarInfo]
        private void __ShowStatusBarInfo(string message)
        {
            StatusBarMessage = message;

            m_StatusBarMessageTimer.Stop();
            m_StatusBarMessageTimer.Start();
        }
        #endregion

        #region [__ReportProgress]
        private void __ReportProgress(object sender, ReportProgressEventArgs e)
        {
            if (e.Completed)
            {
                ProgressBarVisibility = Visibility.Collapsed;
            }
            else
            {
                ProgressBarText = e.ProgressText;
                ProgressBarPercentage = e.ProgressValue;
                ProgressBarVisibility = Visibility.Visible;
            }
        }
        #endregion

        #region [__Initialize]
        private void __Initialize(Area Area)
        {
            m_CurrentArea = Area;
            OnPropertyChanged(nameof(StatusBarAreaText));
            OnPropertyChanged(nameof(IsAreaSelected));
            OnPropertyChanged(nameof(ConnectedESTWs));
            OnPropertyChanged(nameof(CurrentFile));

            if (m_CurrentArea == null)
                return;

            m_SaveCommand.SetCanExecute(true);
            m_SaveAsCommand.SetCanExecute(true);
            m_EstwSelectionCommand.SetCanExecute(true);
            m_TrainProgressInformationCommand.SetCanExecute(true);
            m_DisplayCommand.SetCanExecute(true);
            m_SystemStateCommand.SetCanExecute(true);
            m_RemindersCommand.SetCanExecute(true);
            IsTrainScheduleEnabled = true;


            m_CancellationTokenSource = new CancellationTokenSource();
            m_RefreshingThread = new Thread(() => __Refresh(m_CurrentArea, m_CancellationTokenSource.Token));
            m_RefreshingThread.Start();
        }
        #endregion

        #region [__CleanUp]
        private void __CleanUp()
        {
            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource = null;
            }

            if (m_CurrentArea != null)
                m_CurrentArea.LiveTrains.Clear();

            ChildWindows.Clear();
            OpenRemindersDialog.CloseAndReset();
            Runtime.VisibleStations.Clear();
            ToastNotificationManagerCompat.History.Clear();
        }
        #endregion

        #region [__VisibleStationsChanged]
        private void __VisibleStationsChanged()
        {
            Stations = Runtime.VisibleStations;
            IsTimeTableEnabled = Stations.Count > 0;
        }
        #endregion

        #region [__Refresh]
        private void __Refresh(Area Area, CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var Result = m_LiveDataBll.RefreshLiveData(Area);

                if (Result.Succeeded && Result.Result)
                {
                    List<IRefreshable> vmList = null;
                    Application.Current?.Dispatcher?.Invoke(() => vmList = ChildWindows.Select(w => w.DataContext).Where(vm => vm is IRefreshable).Cast<IRefreshable>().ToList());

                    if (cancellationToken.IsCancellationRequested)
                        break;

                    foreach (var vm in vmList)
                    {
                        try
                        {
                            vm.Refresh(Area);
                        }
                        catch (Exception ex)
                        {
                            Application.Current?.Dispatcher?.Invoke(() => __ShowErrorWindow(ex.ToString()));
                        }
                    }

                    OnPropertyChanged(nameof(ConnectedESTWs));
                    __ProcessReminders(Area);
                }
                else
                    Application.Current?.Dispatcher?.Invoke(() => __ShowErrorWindow(Result.Message));

                Thread.Sleep(500);
            }
        }
        #endregion

        #region [__ProcessReminders]
        private void __ProcessReminders(Area area)
        {
            foreach (var estw in area.ESTWs.Where(e => e.IsLoaded))
            {
                var reminders = estw.Reminders.Where(r => r.DueTime <= estw.Time).ToList();
                Application.Current?.Dispatcher?.Invoke(() => OpenRemindersDialog.ShowReminders(estw, reminders));

                foreach (var reminder in reminders)
                    estw.Reminders.Remove(reminder);
            }

        }
        #endregion

        #region [__CheckForSaving]
        private bool __CheckForSaving()
        {
            if (m_CurrentArea != null)
            {
                var buttons = m_ForceClose ? MessageBoxButton.YesNo : MessageBoxButton.YesNoCancel;
                var MBResult = MessageBox.Show("Soll der aktuelle Zustand zunächst gespeichert werden?", "Frage", buttons, MessageBoxImage.Question);

                switch (MBResult)
                {
                    case MessageBoxResult.Yes:
                        __Save();
                        break;
                    case MessageBoxResult.No:
                        // Nothing
                        break;
                    default:
                        return false;
                }
            }

            return true;
        }
        #endregion

        #region [__SerializeVisibleTrains]
        private List<SerializedVisibleTrainInfo> __SerializeVisibleTrains()
        {
            var result = new List<SerializedVisibleTrainInfo>();

            foreach (var train in Runtime.VisibleTrains)
            {
                var serializedTrain = new SerializedVisibleTrainInfo();
                serializedTrain.TrainNumber = train.TrainNumber;
                serializedTrain.HadLiveData = train.HadLiveData;
                result.Add(serializedTrain);
            }

            return result;
        }
        #endregion

        #region [__DeserializeVisibleTrains]
        private void __DeserializeVisibleTrains(List<SerializedVisibleTrainInfo> serializedTrains)
        {
            Runtime.VisibleTrains.Clear();

            if (serializedTrains == null)
                return;

            foreach (var serializedTrain in serializedTrains)
            {
                var train = new VisibleTrainInfo();
                train.TrainNumber = serializedTrain.TrainNumber;
                train.HadLiveData = serializedTrain.HadLiveData;
                Runtime.VisibleTrains.Add(train);
            }
        }
        #endregion

        #region [__SerializeHiddenSchedules]
        private List<SerializedHiddenScheduleInfo> __SerializeHiddenSchedules()
        {
            var result = new List<SerializedHiddenScheduleInfo>();

            foreach (var schedule in Runtime.HiddenSchedules)
            {
                var serializedSchedule = new SerializedHiddenScheduleInfo();
                serializedSchedule.TrainNumber = schedule.Schedule.Train.Number;
                serializedSchedule.EstwId = schedule.Schedule.Station.ESTW.Id;
                serializedSchedule.Station = schedule.Schedule.Station.ShortSymbol;
                serializedSchedule.Time = schedule.Schedule.Time;
                serializedSchedule.CreatedOn = schedule.CreatedOn;
                result.Add(serializedSchedule);
            }

            return result;
        }
        #endregion

        #region [__DeserializeHiddenSchedules]
        private void __DeserializeHiddenSchedules(List<SerializedHiddenScheduleInfo> serializedSchedules)
        {
            Runtime.HiddenSchedules.Clear();

            if (serializedSchedules == null)
                return;

            foreach (var serializedSchedule in serializedSchedules)
            {
                if (!m_CurrentArea.Trains.ContainsKey(serializedSchedule.TrainNumber))
                    continue;

                var train = m_CurrentArea.Trains[serializedSchedule.TrainNumber];
                var schedule = train.Schedules.FirstOrDefault(s => s.Station.ESTW.Id == serializedSchedule.EstwId
                                                                && s.Station.ShortSymbol == serializedSchedule.Station
                                                                && s.Time == serializedSchedule.Time);

                if (schedule == null)
                    continue;

                var hiddenSchedule = new HiddenScheduleInfo();
                hiddenSchedule.Schedule = schedule;
                hiddenSchedule.CreatedOn = serializedSchedule.CreatedOn;
                Runtime.HiddenSchedules.Add(hiddenSchedule);
            }
        }
        #endregion

        #region [__ShowErrorWindow]
        private void __ShowErrorWindow(string errorMessage)
        {
            SystemSounds.Hand.Play();
            var close = ErrorDialog.ShowModal(errorMessage);

            if (!close)
                return;

            if (m_CancellationTokenSource != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource = null;
            }

            m_ForceClose = true;
            __Exit();
        }
        #endregion

        #region [__CheckForUpdatesIfNeeded]
        private void __CheckForUpdatesIfNeeded()
        {
            var settingsResult = m_SettingsBll.GetSettings();
            var settings = settingsResult.Result;

            if (settings?.AutomaticallyCheckForUpdates == false)
                return;

            Task.Run(async () =>
            {
                m_UpdateBll = await UpdateHelper.GetUpdateBLL();
                var checkForUpdateResult = await m_UpdateBll.CheckForUpdates();

                if (!checkForUpdateResult.Succeeded)
                    return;

                if (checkForUpdateResult.Result.ReleasesToApply.Any() && checkForUpdateResult.Result.FutureVersion != settings?.SkipVersion)
                {
                    if (settings?.AutomaticallyInstallUpdates == true)
                    {
                        __DoUpdate();
                        return;
                    }

                    var installButton = new ToastButton()
                        .SetContent("Installieren")
                        .AddArgument(NOTIFICATION_ACTION, "install");

                    var remindButton = new ToastButton()
                        .SetContent("Später")
                        .AddArgument(NOTIFICATION_ACTION, "later");

                    var skipButton = new ToastButton()
                        .SetContent("Überspringen")
                        .AddArgument(NOTIFICATION_ACTION, "skip")
                        .AddArgument("versionToSkip", checkForUpdateResult.Result.FutureVersion);

                    new ToastContentBuilder()
                        .AddArgument(NOTIFICATION_TYPE, NOTIFICATION_TYPE_UPDATE_AVAILABLE)
                        .AddText("Es steht eine neue Version von LeiBIT zur Verfügung")
                        .AddText($"Aktuell verwendete Version: {checkForUpdateResult.Result.CurrentVersion}")
                        .AddText($"Neue Version: {checkForUpdateResult.Result.FutureVersion}")
                        .AddButton(installButton)
                        .AddButton(remindButton)
                        .AddButton(skipButton)
                        .Show();
                }
            });
        }
        #endregion

        #region [__DoUpdate]
        private void __DoUpdate()
        {
            Task.Run(async () =>
            {
                __UpdateProgress(this, 0);

                m_UpdateBll.UpdateProgress += __UpdateProgress;
                var updateResult = await m_UpdateBll.Update();
                m_UpdateBll.UpdateProgress -= __UpdateProgress;

                __ReportProgress(this, new ReportProgressEventArgs(true));

                if (!updateResult.Succeeded)
                {
                    var detailsButton = new ToastButton()
                       .SetContent("Details...")
                       .AddArgument(NOTIFICATION_ACTION, "showDetails")
                       .AddArgument("error", "Installation fehlgeschlagen")
                       .AddArgument("errorMessage", updateResult.Message);

                    new ToastContentBuilder()
                        .AddArgument(NOTIFICATION_TYPE, NOTIFICATION_TYPE_ERROR)
                        .AddText("Installation fehlgeschlagen")
                        .AddButton(detailsButton)
                        .Show();

                    return;
                }

                if (updateResult.Result)
                {
                    var checkResult = await m_UpdateBll.CheckForUpdates();
                    var currentVersion = checkResult?.Result?.CurrentVersion ?? "???";

                    var restartButton = new ToastButton()
                        .SetContent("Jetzt neu starten")
                        .AddArgument(NOTIFICATION_ACTION, "restart");

                    var laterButton = new ToastButton()
                        .SetContent("Später")
                        .AddArgument(NOTIFICATION_ACTION, "later");

                    new ToastContentBuilder()
                        .AddArgument(NOTIFICATION_TYPE, NOTIFICATION_TYPE_UPDATE_INSTALLED)
                        .AddText("Update erfolgreich installiert")
                        .AddText($"LeiBIT wurde auf Version {currentVersion} aktualisiert. Beim nächsten Start steht die neue Version zur Verfügung.")
                        .AddButton(restartButton)
                        .AddButton(laterButton)
                        .Show();
                }
            });
        }
        #endregion

        #region [__Restart]
        private void __Restart()
        {
            var restartResult = m_UpdateBll.RestartApp();

            if (restartResult.Succeeded && restartResult.Result)
            {
                m_ForceClose = true;
                Application.Current?.Dispatcher?.BeginInvoke(new Action(__Exit));
            }
        }
        #endregion

        #region [__UpdateProgress]
        private void __UpdateProgress(object sender, int e)
        {
            __ReportProgress(this, new ReportProgressEventArgs("Update wird installiert", e));
        }
        #endregion

        #region [__ToastNotificationClicked]
        private void __ToastNotificationClicked(ToastNotificationActivatedEventArgsCompat e)
        {
            var args = ToastArguments.Parse(e.Argument);

            if (!args.Contains(NOTIFICATION_TYPE) || !args.Contains(NOTIFICATION_ACTION))
                return;

            if (args[NOTIFICATION_TYPE] == NOTIFICATION_TYPE_UPDATE_AVAILABLE)
            {
                if (args[NOTIFICATION_ACTION] == "install")
                    __DoUpdate();
                else if (args[NOTIFICATION_ACTION] == "skip" && args.Contains("versionToSkip"))
                {
                    var settingsResult = m_SettingsBll.GetSettings();

                    if (!settingsResult.Succeeded)
                        return;

                    var settings = settingsResult.Result;
                    settings.SkipVersion = args["versionToSkip"];
                    m_SettingsBll.SaveSettings(settings);
                }
            }

            if (args[NOTIFICATION_TYPE] == NOTIFICATION_TYPE_UPDATE_INSTALLED)
            {
                if (args[NOTIFICATION_ACTION] == "restart")
                    __Restart();
            }

            if (args[NOTIFICATION_TYPE] == NOTIFICATION_TYPE_ERROR)
            {
                if (args[NOTIFICATION_ACTION] == "showDetails" && args.Contains("error") && args.Contains("errorMessage"))
                    Application.Current?.Dispatcher?.Invoke(() => MessageBox.Show(args["errorMessage"], args["error"], MessageBoxButton.OK, MessageBoxImage.Error));
            }
        }
        #endregion

        #region [__SettingsChanged]
        private void __SettingsChanged(object sender, EventArgs e)
        {
            foreach (var window in ChildWindows)
                window.ChildWindow?.SetWindowColor();
        }
        #endregion

        #region [__IsWindowVisibleOnScreen]
        private bool __IsWindowVisibleOnScreen(WindowSettings windowSettings, System.Windows.Forms.Screen screen)
        {
            return windowSettings.Left >= screen.WorkingArea.Left
                && windowSettings.Left <= screen.WorkingArea.Right
                && windowSettings.Top >= screen.WorkingArea.Top
                && windowSettings.Top <= screen.WorkingArea.Bottom;
        }
        #endregion

        #endregion

    }
}
