using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.Windows.DelayJustification.ViewModels;
using Leibit.Client.WPF.Windows.DelayJustification.Views;
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
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
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

        private Area m_CurrentArea;
        private Thread m_RefreshingThread;
        private CancellationTokenSource m_CancellationTokenSource;
        private List<ViewModelBase> m_ChildViewModels;
        private string m_CurrentFilename;

        private CommandHandler m_NewCommand;
        private CommandHandler m_OpenCommand;
        private CommandHandler m_SaveCommand;
        private CommandHandler m_SaveAsCommand;
        private CommandHandler m_SettingsCommand;
        private CommandHandler m_EstwOnlineCommand;
        private CommandHandler m_ExitCommand;
        private CommandHandler m_EstwSelectionCommand;
        private CommandHandler m_TrainProgressInformationCommand;
        private CommandHandler m_TimeTableCommand;
        private CommandHandler m_TrainScheduleCommand;
        private CommandHandler m_SystemStateCommand;
        private CommandHandler m_SaveLayoutCommand;
        private CommandHandler m_ClearChildWindowsCommand;
        #endregion

        #region - Ctor -
        public MainViewModel()
        {
            m_NewCommand = new CommandHandler<string>(__New, true);
            m_OpenCommand = new CommandHandler(__Open, true);
            m_SaveCommand = new CommandHandler(__Save, false);
            m_SaveAsCommand = new CommandHandler(__SaveAs, false);
            m_SettingsCommand = new CommandHandler(__Settings, true);
            m_EstwOnlineCommand = new CommandHandler(__StartEstwOnline, true);
            m_ExitCommand = new CommandHandler(__Exit, true);
            m_EstwSelectionCommand = new CommandHandler(__ShowEstwSelectionWindow, false);
            m_TrainProgressInformationCommand = new CommandHandler(__ShowTrainProgressInformationWindow, false);
            m_TimeTableCommand = new CommandHandler<Station>(__ShowTimeTableWindow, true);
            m_TrainScheduleCommand = new CommandHandler(__ShowTrainScheduleWindow, false);
            m_SystemStateCommand = new CommandHandler(__ShowSystemStateWindow, false);
            m_SaveLayoutCommand = new CommandHandler(__SaveLayout, true);
            m_ClearChildWindowsCommand = new CommandHandler(__ClearChildWindows, true);

            m_InitializationBll = new InitializationBLL();
            m_LiveDataBll = new LiveDataBLL();
            m_SettingsBll = new SettingsBLL();
            m_SerializationBll = new SerializationBLL();

            m_ChildViewModels = new List<ViewModelBase>();
            ChildWindows = new ObservableCollection<ChildWindow>();

            Runtime.VisibleStationsChanged += __VisibleStationsChanged;

            var AreaResult = m_InitializationBll.GetAreaInformation();

            if (AreaResult.Succeeded)
                Areas = AreaResult.Result.ToObservableCollection();
            else
            {
                ShowMessage(AreaResult);
                Areas = new ObservableCollection<Area>();
            }

            var SettingsResult = m_SettingsBll.AreSettingsValid();

            if (SettingsResult.Succeeded)
            {
                if (!SettingsResult.Result)
                    __Settings();
            }
            else
                ShowMessage(SettingsResult);

            StatusBarText = "Herzlich willkommen!";
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
        public ObservableCollection<ChildWindow> ChildWindows
        {
            get
            {
                return Get<ObservableCollection<ChildWindow>>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [StatusBarText]
        public string StatusBarText
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

        #endregion

        #endregion

        #region - Overrides -

        #region [OnWindowClosing]
        protected override void OnWindowClosing(CancelEventArgs e)
        {
            base.OnWindowClosing(e);
            e.Cancel |= !__CheckForSaving();
        }
        #endregion

        #region [OnWindowClosed]
        protected override void OnWindowClosed()
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

            __ShowEstwSelectionWindow();
        }
        #endregion

        #region [__Open]
        private void __Open()
        {
            if (!__CheckForSaving())
                return;

            var Dialog = new OpenFileDialog();
            Dialog.Filter = "LeiBIT-Dateien|*.leibit|Alle Dateien|*.*";

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

                __CleanUp();
                __Initialize(OpenResult.Result.Area);

                var Stations = OpenResult.Result.Area.ESTWs.SelectMany(e => e.Stations);

                foreach (var SerializedStation in OpenResult.Result.VisibleStations)
                {
                    var Station = Stations.FirstOrDefault(s => s.ESTW.Id == SerializedStation.EstwId && s.ShortSymbol == SerializedStation.ShortSymbol);
                    Runtime.VisibleStations.AddIfNotNull(Station);
                }

                foreach (var SerializedWindow in OpenResult.Result.Windows)
                {
                    ChildWindow Window;
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

                            var Station = Stations.FirstOrDefault(s => s.ShortSymbol == StationShortSymbol);

                            if (Station == null)
                                continue;

                            Window = new TimeTableView(StationShortSymbol);
                            ViewModel = new TimeTableViewModel(Window.Dispatcher, Station);
                            break;

                        case eChildWindowType.TrainProgressInformation:
                            Window = new TrainProgressInformationView();
                            ViewModel = new TrainProgressInformationViewModel(Window.Dispatcher);
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
                            var Tag = (KeyValuePair<int, string>)SerializedWindow.Tag;

                            if (m_CurrentArea.Trains.ContainsKey(Tag.Key))
                            {
                                var Schedule = m_CurrentArea.Trains[Tag.Key].Schedules.FirstOrDefault(s => s.Station.ShortSymbol == Tag.Value);

                                if (Schedule == null)
                                    continue;

                                Window = new LocalOrdersView(Tag.Key, Tag.Value);
                                ViewModel = new LocalOrdersViewModel(Schedule);
                                break;
                            }
                            else
                                continue;

                        case eChildWindowType.SystemState:
                            Window = new SystemStateView();
                            ViewModel = new SystemStateViewModel(Window.Dispatcher);
                            break;

                        default:
                            continue;
                    }

                    Window.Width = SerializedWindow.Width;
                    Window.Height = SerializedWindow.Height;
                    Window.PositionX = SerializedWindow.PositionX;
                    Window.PositionY = SerializedWindow.PositionY;

                    __OpenChildWindow(Window, ViewModel);
                }

                m_CurrentFilename = Filename;
            }
        }
        #endregion

        #region [__Save]
        private void __Save()
        {
            if (m_CurrentFilename.IsNullOrEmpty())
            {
                __SaveAs();
                return;
            }

            var Container = new SerializationContainer();
            Container.Area = m_CurrentArea;
            Container.VisibleStations = Runtime.VisibleStations.Select(s => new SerializedStation { EstwId = s.ESTW.Id, ShortSymbol = s.ShortSymbol }).ToList();

            foreach (var Window in ChildWindows)
            {
                var SerializedWindow = new SerializedWindowInformation();

                if (Window is DelayJustificationView)
                {
                    SerializedWindow.Type = eChildWindowType.DelayJustification;
                    SerializedWindow.Tag = (Window.DataContext as DelayJustificationViewModel).CurrentTrain.Train.Number;
                }
                else if (Window is ESTWSelectionView)
                    SerializedWindow.Type = eChildWindowType.ESTWSelection;
                else if (Window is SettingsView)
                    SerializedWindow.Type = eChildWindowType.Settings;
                else if (Window is TimeTableView)
                {
                    SerializedWindow.Type = eChildWindowType.TimeTable;
                    SerializedWindow.Tag = (Window.DataContext as TimeTableViewModel).CurrentStation.ShortSymbol;
                }
                else if (Window is TrainProgressInformationView)
                    SerializedWindow.Type = eChildWindowType.TrainProgressInformation;
                else if (Window is TrainScheduleView)
                {
                    SerializedWindow.Type = eChildWindowType.TrainSchedule;
                    SerializedWindow.Tag = (Window.DataContext as TrainScheduleViewModel).CurrentTrain.Number;
                }
                else if (Window is LocalOrdersView)
                {
                    SerializedWindow.Type = eChildWindowType.LocalOrders;

                    var VM = Window.DataContext as LocalOrdersViewModel;
                    SerializedWindow.Tag = new KeyValuePair<int, string>(VM.CurrentSchedule.Train.Number, VM.CurrentSchedule.Station.ShortSymbol);
                }
                else if (Window is SystemStateView)
                    SerializedWindow.Type = eChildWindowType.SystemState;
                else
                    continue;

                SerializedWindow.Width = Window.Width;
                SerializedWindow.Height = Window.Height;
                SerializedWindow.PositionX = Window.PositionX;
                SerializedWindow.PositionY = Window.PositionY;

                Container.Windows.Add(SerializedWindow);
            }

            var SaveResult = m_SerializationBll.Save(m_CurrentFilename, Container);

            if (SaveResult.Succeeded)
                StatusBarText = "Aktueller Zustand gespeichert";
            else
                ShowMessage(SaveResult);
        }
        #endregion

        #region [__SaveAs]
        private void __SaveAs()
        {
            var Dialog = new SaveFileDialog();
            Dialog.Filter = "LeiBIT-Dateien|*.leibit";

            if (Dialog.ShowDialog() == true)
            {
                var Filename = Dialog.FileName;

                if (Filename.IsNullOrWhiteSpace())
                {
                    MessageBox.Show("Bitte geben Sie eine Datei an.", "Hinweis", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                m_CurrentFilename = Filename;
                __Save();
            }
        }
        #endregion

        #region [__Settings]
        private void __Settings()
        {
            var VM = new SettingsViewModel(Areas);
            var Window = new SettingsView();

            __OpenChildWindow(Window, VM);
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
                    MessageBox.Show(StatusBarText = "ESTWonline Pfad nicht gesetzt", "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
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
            var VM = new ESTWSelectionViewModel(Window.Dispatcher);
            __OpenChildWindow(Window, VM);
        }
        #endregion

        #region [__ShowTrainProgressInformationWindow]
        private void __ShowTrainProgressInformationWindow()
        {
            var Window = new TrainProgressInformationView();
            var VM = new TrainProgressInformationViewModel(Window.Dispatcher);
            __OpenChildWindow(Window, VM);
        }
        #endregion

        #region [__ShowTimeTableWindow]
        private void __ShowTimeTableWindow(Station Station)
        {
            IsTimeTableDropDownOpen = false;

            var Window = new TimeTableView(Station.ShortSymbol);
            var VM = new TimeTableViewModel(Window.Dispatcher, Station);
            __OpenChildWindow(Window, VM);
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
                var VM = new TrainScheduleViewModel(Window.Dispatcher, Train, m_CurrentArea);
                __OpenChildWindow(Window, VM);
            }
            else if (m_CurrentArea.LiveTrains.ContainsKey(TrainScheduleNumber.Value))
            {
                var Train = m_CurrentArea.LiveTrains[TrainScheduleNumber.Value];
                var Window = new TrainScheduleView(TrainScheduleNumber.Value);
                var VM = new TrainScheduleViewModel(Window.Dispatcher, Train.Train, m_CurrentArea);
                __OpenChildWindow(Window, VM);
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
            var VM = new SystemStateViewModel(Window.Dispatcher);
            __OpenChildWindow(Window, VM);
        }
        #endregion

        #region [__SaveLayout]
        private void __SaveLayout()
        {
            m_ChildViewModels.Where(vm => vm is ILayoutSavable).Cast<ILayoutSavable>().ForEach(vm => vm.SaveLayout());
            StatusBarText = "Layout wurde gespeichert";
        }
        #endregion

        #region [__ClearChildWindows]
        private void __ClearChildWindows()
        {
            ChildWindows.Clear();
        }
        #endregion

        #endregion

        #region [__OpenChildWindow]
        private bool __OpenChildWindow(ChildWindow Window, ChildWindowViewModelBase ViewModel)
        {
            var Temp = ChildWindows.FirstOrDefault(c => c.Identifier == Window.Identifier);

            if (Temp != null)
            {
                Temp.Focus();
                return false;
            }

            Window.DataContext = ViewModel;
            m_ChildViewModels.Add(ViewModel);
            ChildWindows.Add(Window);

            Window.Closed += (sender, e) =>
            {
                ChildWindows.Remove(Window);
                m_ChildViewModels.Remove(ViewModel);
            };

            ViewModel.OpenWindow += (sender, e) =>
            {
                __OpenChildWindow(e.Window, e.ViewModel);
            };

            ViewModel.StatusBarTextChanged += (sender, text) => StatusBarText = text;

            return true;
        }
        #endregion

        #region [__Initialize]
        private void __Initialize(Area Area)
        {
            m_CurrentArea = Area;

            if (m_CurrentArea == null)
                return;

            m_SaveCommand.SetCanExecute(true);
            m_SaveAsCommand.SetCanExecute(true);
            m_EstwSelectionCommand.SetCanExecute(true);
            m_TrainProgressInformationCommand.SetCanExecute(true);
            m_SystemStateCommand.SetCanExecute(true);
            IsTrainScheduleEnabled = true;

            StatusBarText = String.Format("Bereich {0} geladen", m_CurrentArea.Name);

            m_CancellationTokenSource = new CancellationTokenSource();
            m_RefreshingThread = new Thread(() => __Refresh(m_CurrentArea, m_CancellationTokenSource.Token));
            m_RefreshingThread.Start();
        }
        #endregion

        #region [__CleanUp]
        private void __CleanUp()
        {
            if (m_RefreshingThread != null)
            {
                m_CancellationTokenSource.Cancel();
                m_CancellationTokenSource = null;
            }

            if (m_CurrentArea != null)
                m_CurrentArea.LiveTrains.Clear();

            ChildWindows.Clear();
            Runtime.VisibleStations.Clear();
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
                    m_ChildViewModels.Where(vm => vm is IRefreshable).Cast<IRefreshable>().ToList().ForEach(vm => vm.Refresh(Area));
                else
                    Application.Current.Dispatcher.Invoke(() => MessageBox.Show(Result.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error));

                Thread.Sleep(100);
            }
        }
        #endregion

        #region [__CheckForSaving]
        private bool __CheckForSaving()
        {
            if (m_CurrentArea != null)
            {
                var MBResult = MessageBox.Show("Soll der aktuelle Zustand zunächst gespeichert werden?", "Frage", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

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

        #endregion

    }
}
