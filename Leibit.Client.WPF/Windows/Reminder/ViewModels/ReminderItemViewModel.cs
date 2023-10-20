using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Core.Scheduling;
using System;

namespace Leibit.Client.WPF.Windows.Reminder.ViewModels
{
    public class ReminderItemViewModel : ViewModelBase
    {

        #region - Ctor -
        public ReminderItemViewModel(Entities.LiveData.Reminder reminder)
        {
            CurrentReminder = reminder;
            DeleteCommand = new CommandHandler(__Delete, true);
        }
        #endregion

        #region - Properties -

        public Entities.LiveData.Reminder CurrentReminder { get; }

        #region [DueTime]
        public DateTime DueTime
        {
            get
            {
                return DateTime.Today.AddHours(CurrentReminder.DueTime.Hour).AddMinutes(CurrentReminder.DueTime.Minute);
            }
            set
            {
                CurrentReminder.DueTime = new LeibitTime(value.Hour, value.Minute);
                OnPropertyChanged();
            }
        }
        #endregion

        #region [Text]
        public string Text
        {
            get
            {
                return CurrentReminder.Text;
            }
            set
            {
                CurrentReminder.Text = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region [DeleteCommand]
        public CommandHandler DeleteCommand { get; }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Delete]
        private void __Delete()
        {
            OnPropertyChanged("DeleteReminder");
        }
        #endregion

        #endregion

    }
}
