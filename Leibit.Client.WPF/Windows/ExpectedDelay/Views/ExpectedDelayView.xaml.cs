using Leibit.Controls;

namespace Leibit.Client.WPF.Windows.ExpectedDelay.Views
{
    /// <summary>
    /// Interaction logic for ExpectedDelayView.xaml
    /// </summary>
    public partial class ExpectedDelayView : ChildWindow
    {
        public ExpectedDelayView()
            : base("ExpectedDelay")
        {
            InitializeComponent();
        }

        public ExpectedDelayView(int trainNumber)
            : base($"ExpectedDelay_{trainNumber}")
        {
            InitializeComponent();
        }
    }
}
