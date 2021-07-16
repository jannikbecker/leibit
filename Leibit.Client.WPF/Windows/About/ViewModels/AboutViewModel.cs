using Leibit.BLL;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Leibit.Client.WPF.Windows.About.ViewModels
{
    public class AboutViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private UpdateBLL m_UpdateBll;
        #endregion

        #region - Ctor -
        public AboutViewModel()
        {
            m_UpdateBll = new UpdateBLL(@"D:\Dev\LeibitSquirrel");

            Version = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
            OpenGithubCommand = new CommandHandler(__OpenGithub, true);
            CheckForUpdatesCommand = new CommandHandler(__CheckForUpdates, true);

            var decoder = BitmapDecoder.Create(new Uri("pack://application:,,,/Leibit;component/icon.ico"), BitmapCreateOptions.DelayCreation, BitmapCacheOption.OnDemand);
            var maxWidth = decoder.Frames.Max(f => f.Width);
            Icon = decoder.Frames.FirstOrDefault(f => f.Width == maxWidth);
        }
        #endregion

        #region - Properties -

        public ImageSource Icon { get; }

        public string Version { get; }

        public ICommand OpenGithubCommand { get; }

        public CommandHandler CheckForUpdatesCommand { get; }

        #region [VersionCommand]
        public CommandHandler VersionCommand
        {
            get => Get<CommandHandler>();
            set => Set(value);
        }
        #endregion

        #region [VersionCommandText]
        public string VersionCommandText
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [IsSpinnerVisible]
        public bool IsSpinnerVisible
        {
            get => Get<bool>();
            set
            {
                Set(value);

                if (value)
                {
                    IsOkIconVisible = false;
                    IsWarningIconVisible = false;
                    IsInfoIconVisible = false;
                }
            }
        }
        #endregion

        #region [IsOkIconVisible]
        public bool IsOkIconVisible
        {
            get => Get<bool>();
            set
            {
                Set(value);

                if (value)
                {
                    IsSpinnerVisible = false;
                    IsWarningIconVisible = false;
                    IsInfoIconVisible = false;
                }
            }
        }
        #endregion

        #region [IsWarningIconVisible]
        public bool IsWarningIconVisible
        {
            get => Get<bool>();
            set
            {
                Set(value);

                if (value)
                {
                    IsOkIconVisible = false;
                    IsSpinnerVisible = false;
                    IsInfoIconVisible = false;
                }
            }
        }
        #endregion

        #region [IsInfoIconVisible]
        public bool IsInfoIconVisible
        {
            get => Get<bool>();
            set
            {
                Set(value);

                if (value)
                {
                    IsOkIconVisible = false;
                    IsWarningIconVisible = false;
                    IsSpinnerVisible = false;
                }
            }
        }
        #endregion

        #region [VersionStatusText]
        public string VersionStatusText
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

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

        #region [__CheckForUpdates]
        private void __CheckForUpdates()
        {
            VersionCommandText = string.Empty;
            VersionCommand = null;
            CheckForUpdatesCommand.SetCanExecute(false);
            IsSpinnerVisible = true;
            VersionStatusText = "Suche nach Updates";

            Task.Run(async () =>
            {
                var checkForUpdateResult = await m_UpdateBll.CheckForUpdates();

                Application.Current?.Dispatcher?.Invoke(() =>
                {
                    CheckForUpdatesCommand.SetCanExecute(true);

                    if (!checkForUpdateResult.Succeeded)
                    {
                        IsWarningIconVisible = true;
                        VersionStatusText = "Fehlgeschlagen";
                        return;
                    }

                    if (checkForUpdateResult.Result.ReleasesToApply.Any())
                    {
                        IsInfoIconVisible = true;
                        VersionStatusText = $"Update verfügbar: {checkForUpdateResult.Result.FutureVersion}";
                        VersionCommand = new CommandHandler(__InstallUpdate, true);
                        VersionCommandText = "Installieren";
                    }
                    else
                    {
                        IsOkIconVisible = true;
                        VersionStatusText = "LeiBIT ist aktuell.";
                    }
                });
            });
        }
        #endregion

        #region [__InstallUpdate]
        private void __InstallUpdate()
        {
            VersionCommandText = string.Empty;
            VersionCommand = null;
            CheckForUpdatesCommand.SetCanExecute(false);
            IsSpinnerVisible = true;
            VersionStatusText = "Update wird installiert";

            Task.Run(async () =>
            {
                m_UpdateBll.UpdateProgress += __UpdateProgress;
                var updateResult = await m_UpdateBll.Update();
                m_UpdateBll.UpdateProgress -= __UpdateProgress;

                OnReportProgress(true);

                Application.Current?.Dispatcher?.Invoke(() => CheckForUpdatesCommand.SetCanExecute(true));

                if (!updateResult.Succeeded || !updateResult.Result)
                {
                    IsWarningIconVisible = true;
                    VersionStatusText = "Installation fehlgeschlagen";
                    return;
                }

                IsOkIconVisible = true;
                VersionStatusText = "Update erfolgreich installiert";
                VersionCommand = new CommandHandler(__Restart, true);
                VersionCommandText = "Neustart";
            });
        }
        #endregion

        #region [__UpdateProgress]
        private void __UpdateProgress(object sender, int e)
        {
            OnReportProgress("Update wird installiert", e);
        }
        #endregion

        #region [__Restart]
        private void __Restart()
        {
            var restartResult = m_UpdateBll.RestartApp();

            if (restartResult.Succeeded && restartResult.Result)
                OnShutdownRequested(true);
        }
        #endregion

        #endregion

    }
}
