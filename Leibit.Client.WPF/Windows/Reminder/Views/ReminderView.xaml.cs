using Leibit.Controls;

namespace Leibit.Client.WPF.Windows.Reminder.Views
{
    /// <summary>
    /// Interaction logic for ReminderView.xaml
    /// </summary>
    public partial class ReminderView : LeibitWindow
    {
        public ReminderView()
            : base("Reminder")
        {
            InitializeComponent();
        }

        public ReminderView(int trainNumber, string stationShort)
            : base($"Reminder_{trainNumber}_{stationShort}")
        {
            InitializeComponent();
        }
    }
}
