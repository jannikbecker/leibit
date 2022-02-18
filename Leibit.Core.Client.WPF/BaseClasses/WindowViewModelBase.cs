using System;
using System.ComponentModel;

namespace Leibit.Core.Client.BaseClasses
{
    public abstract class WindowViewModelBase : ViewModelBase
    {

        #region - Events -
        public event EventHandler CloseWindow;
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
        public virtual void OnWindowClosing(object sender, CancelEventArgs e)
        {

        }
        #endregion

        #region [OnWindowClosed]
        public virtual void OnWindowClosed()
        {

        }
        #endregion

        #endregion

    }
}
