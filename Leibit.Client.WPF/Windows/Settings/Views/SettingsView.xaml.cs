using Leibit.Client.WPF.Windows.Settings.ViewModels;
using Leibit.Controls;
using System.Windows.Controls.Primitives;

namespace Leibit.Client.WPF.Windows.Settings.Views
{
    /// <summary>
    /// Interaktionslogik für SettingsView.xaml
    /// </summary>
    public partial class SettingsView : LeibitWindow
    {
        public SettingsView()
            : base("Settings")
        {
            InitializeComponent();
        }

        private void __ScaleFactor_DragStarted(object sender, DragStartedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm)
                vm.IsScaleFactorDragging = true;
        }

        private void __ScaleFactor_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (DataContext is SettingsViewModel vm)
                vm.IsScaleFactorDragging = false;
        }
    }
}
