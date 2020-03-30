using Leibit.Client.WPF.Windows.TimeTable.ViewModels;
using Leibit.Controls;
using System.Windows;
using Xceed.Wpf.DataGrid;

namespace Leibit.Client.WPF.Windows.TimeTable.Views
{
    /// <summary>
    /// Interaction logic for TimeTableView.xaml
    /// </summary>
    public partial class TimeTableView : ChildWindow
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

        private void DataGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TimeTableViewModel)
                (DataContext as TimeTableViewModel).DataGrid = sender as DataGridControl;
        }

        private void DataRow_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is TimeTableViewModel && (DataContext as TimeTableViewModel).DoubleClickCommand != null)
                (DataContext as TimeTableViewModel).DoubleClickCommand.Execute(null);
        }
    }
}
