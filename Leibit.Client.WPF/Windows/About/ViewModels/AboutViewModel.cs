using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Leibit.Client.WPF.Windows.About.ViewModels
{
    public class AboutViewModel : ChildWindowViewModelBase
    {

        #region - Ctor -
        public AboutViewModel()
        {
            Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            OpenGithubCommand = new CommandHandler(__OpenGithub, true);

            var decoder = BitmapDecoder.Create(new Uri("pack://application:,,,/Leibit;component/icon.ico"), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);
            var maxWidth = decoder.Frames.Max(f => f.Width);
            Icon = decoder.Frames.FirstOrDefault(f => f.Width == maxWidth);
        }
        #endregion

        #region - Properties -

        public ImageSource Icon { get; }

        public string Version { get; }

        public ICommand OpenGithubCommand { get; }

        #endregion

        #region - Private methods -

        #region [__OpenGithub]
        private void __OpenGithub()
        {
            var startInfo = new ProcessStartInfo("https://github.com/jannikbecker/leibit");
            startInfo.UseShellExecute = true;
            Process.Start(startInfo);
        }
        #endregion

        #endregion

    }
}
