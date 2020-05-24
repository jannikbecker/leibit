using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.ViewModels;
using Leibit.Client.WPF.Windows.TrainSchedule.Views;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Scheduling;
using System;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.LocalOrders.ViewModels
{
    public class LocalOrdersViewModel : ChildWindowViewModelBase
    {

        #region - Ctor -
        public LocalOrdersViewModel(Schedule Schedule)
        {
            CurrentSchedule = Schedule;
            OpenTrainScheduleCommand = new CommandHandler<int>(__OpenTrainSchedule, true);
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
                return CurrentSchedule.LocalOrders;
            }
        }
        #endregion

        #region [OpenTrainScheduleCommand]
        public ICommand OpenTrainScheduleCommand { get; }
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
            var VM = new TrainScheduleViewModel(Window.Dispatcher, area.Trains[trainNumber], area);
            OnOpenWindow(VM, Window);
        }
        #endregion

        #endregion

    }
}
