using Leibit.Client.WPF.Windows.TrainProgressInformation.ViewModels;
using Leibit.Controls;
using System.Windows;
using Xceed.Wpf.DataGrid;

namespace Leibit.Client.WPF.Windows.TrainProgressInformation.Views
{
    /// <summary>
    /// Interaktionslogik für TrainProgressInformationView.xaml
    /// </summary>
    public partial class TrainProgressInformationView : ChildWindow
    {
        public TrainProgressInformationView()
            : base("TrainProgressInformation")
        {
            InitializeComponent();
        }

        private void DataGridControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is TrainProgressInformationViewModel)
                (DataContext as TrainProgressInformationViewModel).DataGrid = sender as DataGridControl;
        }

        private void DataRow_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (DataContext is TrainProgressInformationViewModel && (DataContext as TrainProgressInformationViewModel).DoubleClickCommand != null)
                (DataContext as TrainProgressInformationViewModel).DoubleClickCommand.Execute(null);
        }
    }
}
