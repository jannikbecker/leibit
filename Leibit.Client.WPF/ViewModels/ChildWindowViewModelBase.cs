using Leibit.Controls;
using Leibit.Core.Client.BaseClasses;
using System;

namespace Leibit.Client.WPF.ViewModels
{

    #region ChildWindowViewModelBase
    public abstract class ChildWindowViewModelBase : WindowViewModelBase
    {

        #region - Ctor -
        public ChildWindowViewModelBase()
            : base()
        {

        }
        #endregion

        #region - Events -
        public event EventHandler<LeibitWindow> OpenWindow;
        public event EventHandler<string> StatusBarTextChanged;
        public event EventHandler<ReportProgressEventArgs> ReportProgress;
        public event EventHandler<bool> ShutdownRequested;
        #endregion

        #region - Protected methods -

        #region [OnOpenWindow]
        protected void OnOpenWindow(LeibitWindow window)
        {
            if (OpenWindow != null)
                OpenWindow(this, window);
        }
        #endregion

        #region [OnStatusBarTextChanged]
        protected void OnStatusBarTextChanged(string Text)
        {
            if (StatusBarTextChanged != null)
                StatusBarTextChanged(this, Text);
        }
        #endregion

        #region [OnReportProgress]
        protected void OnReportProgress(string progressText, double progressValue)
        {
            ReportProgress?.Invoke(this, new ReportProgressEventArgs(progressText, progressValue));
        }

        protected void OnReportProgress(bool completed)
        {
            ReportProgress?.Invoke(this, new ReportProgressEventArgs(completed));
        }
        #endregion

        #region [OnShutdownRequested]
        protected void OnShutdownRequested(bool force)
        {
            ShutdownRequested?.Invoke(this, force);
        }
        #endregion

        #endregion

    }
    #endregion

    #region ReportProgressEventArgs
    public class ReportProgressEventArgs : EventArgs
    {
        public ReportProgressEventArgs(bool completed)
        {
            Completed = completed;
        }

        public ReportProgressEventArgs(string progressText, double progressValue)
        {
            ProgressText = progressText;
            ProgressValue = progressValue;
            Completed = false;
        }

        public string ProgressText { get; set; }
        public double ProgressValue { get; set; }
        public bool Completed { get; set; }
    }
    #endregion

}
