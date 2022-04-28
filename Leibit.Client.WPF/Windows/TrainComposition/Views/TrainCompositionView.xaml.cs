using Leibit.Controls;

namespace Leibit.Client.WPF.Windows.TrainComposition.Views
{
    /// <summary>
    /// Interaction logic for TrainCompositionView.xaml
    /// </summary>
    public partial class TrainCompositionView : LeibitWindow
    {
        public TrainCompositionView(int trainNumber)
            : base($"TrainComposition_{trainNumber}")
        {
            InitializeComponent();
        }
    }
}
