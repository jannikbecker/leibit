using Leibit.Client.WPF.ViewModels;
using Leibit.Client.WPF.Views;
using Leibit.Entities;
using System;
using System.Windows;
using System.Windows.Media;

namespace Leibit.Client.WPF
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        internal void ChangeSkin(eSkin newSkin)
        {
            Resources.Clear();
            Resources.MergedDictionaries.Clear();

            if (newSkin == eSkin.Light)
                ApplyResources("Resources/Skins/LightSkin.xaml");
            else if (newSkin == eSkin.Dark)
                ApplyResources("Resources/Skins/DarkSkin.xaml");

            ApplyResources("Resources/Skins/Shared.xaml");
        }

        internal void ChangeScaleFactor(int newScaleFactor)
        {
            var factor = newScaleFactor / 100.0;

            if (MainWindow.Content is FrameworkElement frameworkElement)
                frameworkElement.LayoutTransform = new ScaleTransform(factor, factor);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            ChangeSkin(eSkin.Light);
            base.OnStartup(e);
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var View = new MainView();
            var VM = new MainViewModel();
            View.DataContext = VM;
            View.Show();
        }

        private void ApplyResources(string src)
        {
            var dict = new ResourceDictionary() { Source = new Uri(src, UriKind.Relative) };
            foreach (var mergeDict in dict.MergedDictionaries)
            {
                Resources.MergedDictionaries.Add(mergeDict);
            }

            foreach (var key in dict.Keys)
            {
                Resources[key] = dict[key];
            }
        }

    }
}
