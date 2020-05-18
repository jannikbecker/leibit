using Leibit.BLL;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.SystemState.ViewModels
{
    public class SystemStateViewModel : ChildWindowViewModelBase, IRefreshable, ILayoutSavable
    {

        #region - Needs -
        private CommandHandler m_DoubleClickCommand;
        private SettingsBLL m_SettingsBll;
        #endregion

        #region - Ctor -
        public SystemStateViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
            LiveTrains = new ObservableCollection<LiveTrainViewModel>();
            m_DoubleClickCommand = new CommandHandler(__RowDoubleClick, true);
            m_SettingsBll = new SettingsBLL();
        }
        #endregion

        #region - Properties -

        #region [Dispatcher]
        public Dispatcher Dispatcher
        {
            get;
            private set;
        }
        #endregion

        #region [Estws]
        public ObservableCollection<ESTWViewModel> Estws
        {
            get => Get<ObservableCollection<ESTWViewModel>>();
            private set => Set(value);
        }
        #endregion

        #region [LiveTrains]
        public ObservableCollection<LiveTrainViewModel> LiveTrains
        {
            get => Get<ObservableCollection<LiveTrainViewModel>>();
            private set => Set(value);
        }
        #endregion

        #region [SaveGridLayout]
        public bool SaveGridLayout
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

        #region [SelectedItem]
        public LiveTrainViewModel SelectedItem
        {
            get
            {
                return Get<LiveTrainViewModel>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [DoubleClickCommand]
        public ICommand DoubleClickCommand
        {
            get
            {
                return m_DoubleClickCommand;
            }
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(Area Area)
        {
            __RefreshLoadedEstws(Area);
            __RefreshLiveTrains(Area);
        }
        #endregion

        #region [SaveLayout]
        public void SaveLayout()
        {
            SaveGridLayout = true;
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__RefreshLoadedEstws]
        private void __RefreshLoadedEstws(Area Area)
        {
            var settingsResult = m_SettingsBll.GetSettings();

            if (!settingsResult.Succeeded)
            {
                MessageBox.Show(settingsResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var settings = settingsResult.Result;

            if (Estws == null)
                Estws = new ObservableCollection<ESTWViewModel>();

            foreach (var estw in Area.ESTWs)
            {
                if (!estw.IsLoaded)
                    continue;

                var isNew = false;
                var current = Estws.FirstOrDefault(e => e.CurrentEstw.Id == estw.Id);

                if (current == null)
                {
                    current = new ESTWViewModel(estw);
                    isNew = true;
                }

                current.Time = estw.Time;
                current.IsActive = (DateTime.Now - current.CurrentEstw.LastUpdatedOn).TotalSeconds < settings.EstwTimeout;

                if (isNew)
                    Dispatcher.Invoke(() => Estws.Add(current));
            }
        }
        #endregion

        #region [__RefreshLiveTrains]
        private void __RefreshLiveTrains(Area Area)
        {
            var currentTrains = new List<LiveTrainViewModel>();

            foreach (var train in Area.LiveTrains)
            {
                var current = LiveTrains.FirstOrDefault(t => t.CurrentTrain == train.Value);

                if (current == null)
                {
                    current = new LiveTrainViewModel(Area, train.Value);
                    Dispatcher.Invoke(() => LiveTrains.Add(current));
                }
                else
                    current.Refresh();

                currentTrains.Add(current);
            }

            var removedTrains = LiveTrains.Except(currentTrains).ToList();
            Dispatcher.Invoke(() => removedTrains.ForEach(t => LiveTrains.Remove(t)));
        }
        #endregion

        #region [__RowDoubleClick]
        private void __RowDoubleClick()
        {
            if (SelectedItem == null)
                return;

            var Window = new TrainScheduleView(SelectedItem.CurrentTrain.Train.Number);
            var VM = new TrainScheduleViewModel(Window.Dispatcher, SelectedItem.CurrentTrain.Train, SelectedItem.Area);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #endregion

    }
}
