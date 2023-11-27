using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System;
using System.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.LocalOrders.ViewModels
{
    public class LocalOrdersViewModel : ChildWindowViewModelBase, IRefreshable
    {

        #region - Ctor -
        public LocalOrdersViewModel(Dispatcher Dispatcher, Schedule Schedule, Area Area)
        {
            CurrentSchedule = Schedule;
            OpenTrainScheduleCommand = new CommandHandler<int>(__OpenTrainSchedule, true);
            this.Dispatcher = Dispatcher;

            EditCommand = new CommandHandler(__Edit, true);
            ResetCommand = new CommandHandler(__Reset, true);
            FinishCommand = new CommandHandler(__Finish, true);

            if (Area != null)
                Refresh(Area);
        }
        #endregion

        #region - Properties -

        #region [CurrentSchedule]
        public Schedule CurrentSchedule
        {
            get;
            private set;
        }
        #endregion

        #region [LiveSchedule]
        public LiveSchedule LiveSchedule
        {
            get
            {
                return Get<LiveSchedule>();
            }

            private set
            {
                if (value == LiveSchedule)
                    return;

                Set(value);
                OnPropertyChanged(nameof(LocalOrders));
                OnPropertyChanged(nameof(CanEdit));
            }
        }
        #endregion

        #region [Caption]
        public string Caption
        {
            get
            {
                return String.Format("Örtliche Anordnungen für Zug {0} ({1})", CurrentSchedule.Train.Number, CurrentSchedule.Station.Name);
            }
        }
        #endregion

        #region [LocalOrders]
        public string LocalOrders
        {
            get
            {
                if (LiveSchedule != null)
                    return LiveSchedule.LocalOrders;
                else
                    return CurrentSchedule.LocalOrders;
            }
            set
            {
                if (LiveSchedule != null)
                    LiveSchedule.LocalOrders = value;

                OnPropertyChanged();
            }
        }
        #endregion

        #region [OpenTrainScheduleCommand]
        public ICommand OpenTrainScheduleCommand { get; }
        #endregion

        #region [IsInEditMode]
        public bool IsInEditMode
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [CanEdit]
        public bool CanEdit => LiveSchedule != null;
        #endregion

        #region [Dispatcher]
        public Dispatcher Dispatcher { get; }
        #endregion

        #region [EditCommand]
        public CommandHandler EditCommand { get; }
        #endregion

        #region [ResetCommand]
        public CommandHandler ResetCommand { get; }
        #endregion

        #region [FinishCommand]
        public CommandHandler FinishCommand { get; }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(Area Area)
        {
            if (Area.LiveTrains.TryGetValue(CurrentSchedule.Train.Number, out var liveTrain))
                LiveSchedule = liveTrain.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == CurrentSchedule.Station.ShortSymbol && s.Schedule.Time == CurrentSchedule.Time);
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__OpenTrainSchedule]
        private void __OpenTrainSchedule(int trainNumber)
        {
            var area = CurrentSchedule.Station.ESTW.Area;

            if (!area.Trains.ContainsKey(trainNumber))
                return;

            var Window = new TrainScheduleView(trainNumber);
            Window.DataContext = new TrainScheduleViewModel(Window.Dispatcher, area.Trains[trainNumber], area);
            OnOpenWindow(Window);
        }
        #endregion

        #region [__Edit]
        private void __Edit()
        {
            if (CanEdit)
                IsInEditMode = true;
        }
        #endregion

        #region [__Reset]
        private void __Reset()
        {
            if (LiveSchedule != null)
            {
                LiveSchedule.LocalOrders = CurrentSchedule.LocalOrders;
                OnPropertyChanged(nameof(LocalOrders));
            }
        }
        #endregion

        #region [__Finish]
        private void __Finish()
        {
            IsInEditMode = false;
        }
        #endregion

        #endregion

    }
}
