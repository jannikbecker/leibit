using Leibit.Controls;

namespace Leibit.Client.WPF.Windows.TrackChange.Views
{
    /// <summary>
    /// Interaction logic for TrackChangeView.xaml
    /// </summary>
    public partial class TrackChangeView : LeibitWindow
    {
        public TrackChangeView()
            : base("TrackChange")
        {
            InitializeComponent();
        }

        public TrackChangeView(int trainNumber)
            : base($"TrackChange_{trainNumber}")
        {
            InitializeComponent();
        }
    }
}
