using Leibit.Controls;

namespace Leibit.Client.WPF.Windows.TimeTable.Views
{
    /// <summary>
    /// Interaction logic for TimeTableView.xaml
    /// </summary>
    public partial class TimeTableView : LeibitWindow
    {
        public TimeTableView()
            : base("TimeTable")
        {
            InitializeComponent();
        }

        public TimeTableView(string ShortSymbol)
            : base(string.Format("TimeTable_{0}", ShortSymbol))
        {
            InitializeComponent();
        }
    }
}
