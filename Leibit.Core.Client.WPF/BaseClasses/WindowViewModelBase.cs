using System;
using System.ComponentModel;

namespace Leibit.Core.Client.BaseClasses
{
    public abstract class WindowViewModelBase : ViewModelBase
    {

        #region - Events -
        public event EventHandler CloseWindow;
        #endregion

        #region - Properties -

        public object DialogResult { get; protected set; }

        #endregion

        #region - Protected methods -

        #region [OnSourceInitialized]
        protected internal virtual void OnSourceInitialized(object sender, EventArgs e)
        {

        }
        #endregion

        #region [OnCloseWindow]
        protected virtual void OnCloseWindow()
        {
            if (CloseWindow != null)
                CloseWindow(this, EventArgs.Empty);
        }
        #endregion

        #region [OnWindowClosing]
        protected internal virtual void OnWindowClosing(object sender, CancelEventArgs e)
        {

        }
        #endregion

        #region [OnWindowClosed]
        protected internal virtual void OnWindowClosed()
        {

        }
        #endregion

        #endregion

    }
}
