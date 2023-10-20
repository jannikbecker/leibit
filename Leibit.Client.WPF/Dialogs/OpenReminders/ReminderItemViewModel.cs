using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using System.Linq;

namespace Leibit.Client.WPF.Dialogs.OpenReminders
{
    public class ReminderItemViewModel : ViewModelBase
    {

        #region - Ctor -
        public ReminderItemViewModel(ESTW estw, Reminder reminder)
        {
            ESTW = estw;
            Text = reminder.Text;
            HeaderText = Text.IsNullOrWhiteSpace() ? "Erinnerung" : Text;
            DetailText = $"Zug {reminder.TrainNumber} in {estw.Stations.FirstOrDefault(s => s.ShortSymbol == reminder.StationShort)?.Name}";
            TrainNumber = reminder.TrainNumber;
            StationShort = reminder.StationShort;
            DueTime = reminder.DueTime;
        }
        #endregion

        #region - Properties -

        public ESTW ESTW { get; }
        public string Text { get; }
        public string HeaderText { get; }
        public string DetailText { get; }
        public int TrainNumber { get; }
        public string StationShort { get; }
        public LeibitTime DueTime { get; }

        #endregion

    }
}
