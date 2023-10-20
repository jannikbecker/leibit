using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Windows.LocalOrders.ViewModels;
using Leibit.Client.WPF.Windows.LocalOrders.Views;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Leibit.Client.WPF.Windows.Reminder.ViewModels
{
    public class ReminderViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private readonly Schedule m_Schedule;
        #endregion

        #region - Ctor -
        public ReminderViewModel(TrainInformation train, Schedule schedule)
        {
            m_Schedule = schedule;
            NewCommand = new CommandHandler(__InsertNewReminder, true);
            SaveCommand = new CommandHandler(__Save, true);

            var allSchedulesForStation = schedule.Train.Schedules.Where(s => s.Station.ShortSymbol == schedule.Station.ShortSymbol && s.Station.ESTW.Id == schedule.Station.ESTW.Id);
            Schedules = new ObservableCollection<ScheduleViewModel>(allSchedulesForStation.Select(s => new ScheduleViewModel(train, s)));

            foreach (var s in Schedules)
                s.PropertyChanged += __Schedule_PropertyChanged;

            var reminders = schedule.Station.ESTW.Reminders.Where(r => r.TrainNumber == schedule.Train.Number && r.StationShort == schedule.Station.ShortSymbol);
            Reminders = new ObservableCollection<ReminderItemViewModel>(reminders.Select(r => new ReminderItemViewModel(r.CloneObject())));

            foreach (var r in Reminders)
                r.PropertyChanged += __Reminder_PropertyChanged;

            if (!reminders.Any())
                __InsertNewReminder();
        }
        #endregion

        #region - Properties -

        #region [Caption]
        public string Caption => $"Erinnerungen {m_Schedule.Train.Number} ({m_Schedule.Station.ShortSymbol})";
        #endregion

        #region [Schedules]
        public ObservableCollection<ScheduleViewModel> Schedules { get; }
        #endregion

        #region [Reminders]
        public ObservableCollection<ReminderItemViewModel> Reminders { get; }
        #endregion

        #region [NewCommand]
        public CommandHandler NewCommand { get; }
        #endregion

        #region [SaveCommand]
        public CommandHandler SaveCommand { get; }
        #endregion

        #endregion

        #region - Private methods -

        #region [__InsertNewReminder]
        private void __InsertNewReminder()
        {
            var reminder = new Entities.LiveData.Reminder();
            reminder.TrainNumber = m_Schedule.Train.Number;
            reminder.StationShort = m_Schedule.Station.ShortSymbol;
            reminder.DueTime = m_Schedule.Time;

            var vm = new ReminderItemViewModel(reminder);
            vm.PropertyChanged += __Reminder_PropertyChanged;
            Reminders.Add(vm);
        }
        #endregion

        #region [__Save]
        private void __Save()
        {
            m_Schedule.Station.ESTW.Reminders.RemoveAll(r => r.TrainNumber == m_Schedule.Train.Number && r.StationShort == m_Schedule.Station.ShortSymbol);
            m_Schedule.Station.ESTW.Reminders.AddRange(Reminders.Select(r => r.CurrentReminder));
            OnCloseWindow();
        }
        #endregion

        #region [__Schedule_PropertyChanged]
        private void __Schedule_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var senderVm = sender as ScheduleViewModel;

            if (e.PropertyName == "ShowLocalOrders")
            {
                var window = new LocalOrdersView(m_Schedule.Train.Number, m_Schedule.Station.ShortSymbol);
                window.DataContext = new LocalOrdersViewModel(senderVm.CurrentSchedule);
                OnOpenWindow(window);
            }
        }
        #endregion

        #region [__Reminder_PropertyChanged]
        private void __Reminder_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var senderVm = sender as ReminderItemViewModel;

            if (e.PropertyName == "DeleteReminder")
            {
                senderVm.PropertyChanged -= __Reminder_PropertyChanged;
                Reminders.Remove(senderVm);
            }
        }
        #endregion

        #endregion

    }
}
