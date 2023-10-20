using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Leibit.Client.WPF.Dialogs.OpenReminders
{
    public class OpenRemindersDialogViewModel : ViewModelBase
    {

        #region - Events -
        public event EventHandler DialogClosing;
        #endregion

        #region - Ctor -
        public OpenRemindersDialogViewModel()
        {
            Reminders = new ObservableCollection<ReminderItemViewModel>();
            SnoozeTime = 2;
            CloseCommand = new CommandHandler(__Close, true);
            SnoozeCommand = new CommandHandler(__Snooze, true);
            CloseAllCommand = new CommandHandler(__CloseAll, true);
        }
        #endregion

        #region - Properties -

        #region [Reminders]
        public ObservableCollection<ReminderItemViewModel> Reminders { get; }
        #endregion

        #region [SelectedReminder]
        public ReminderItemViewModel SelectedReminder
        {
            get => Get<ReminderItemViewModel>();
            set => Set(value);
        }
        #endregion

        #region [CloseCommand]
        public CommandHandler CloseCommand { get; }
        #endregion

        #region [SnoozeCommand]
        public CommandHandler SnoozeCommand { get; }
        #endregion

        #region [CloseAllCommand]
        public CommandHandler CloseAllCommand { get; }
        #endregion

        #region [SnoozeTime]
        public int SnoozeTime
        {
            get => Get<int>();
            set => Set(value);
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [AddReminders]
        public void AddReminders(ESTW estw, List<Reminder> reminders)
        {
            foreach (var reminder in reminders)
                Reminders.Add(new ReminderItemViewModel(estw, reminder));

            if (SelectedReminder == null)
                SelectedReminder = Reminders[0];
        }
        #endregion

        #endregion

        #region - Private methods -


        #region [__CloseAll]
        private void __CloseAll()
        {
            Reminders.Clear();
            DialogClosing?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region [__Snooze]
        private void __Snooze()
        {
            if (SelectedReminder == null)
                return;

            var newReminder = new Reminder();
            newReminder.TrainNumber = SelectedReminder.TrainNumber;
            newReminder.StationShort = SelectedReminder.StationShort;
            newReminder.Text = SelectedReminder.Text;
            newReminder.DueTime = SelectedReminder.ESTW.Time.AddMinutes(SnoozeTime);
            SelectedReminder.ESTW.Reminders.Add(newReminder);

            __Close();
        }
        #endregion

        #region [__Close]
        private void __Close()
        {
            if (SelectedReminder == null)
                return;

            Reminders.Remove(SelectedReminder);

            if (!Reminders.Any())
                DialogClosing?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #endregion

    }
}
