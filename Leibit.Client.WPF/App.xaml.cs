using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Views;
using System.Windows;

namespace Leibit.Client.WPF
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var View = new MainView();
            var VM = new MainViewModel();
            View.DataContext = VM;
            View.Show();
        }
    }
}
