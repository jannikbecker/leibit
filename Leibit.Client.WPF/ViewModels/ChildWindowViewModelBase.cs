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
        public event EventHandler<OpenWindowEventArgs> OpenWindow;
        public event EventHandler<string> StatusBarTextChanged;
        #endregion

        #region - Protected methods -

        #region [OnOpenWindow]
        protected void OnOpenWindow(ChildWindowViewModelBase ViewModel, ChildWindow Window)
        {
            if (OpenWindow != null)
                OpenWindow(this, new OpenWindowEventArgs(ViewModel, Window));
        }
        #endregion

        #region [OnStatusBarTextChanged]
        protected void OnStatusBarTextChanged(string Text)
        {
            if (StatusBarTextChanged != null)
                StatusBarTextChanged(this, Text);
        }
        #endregion

        #endregion

    }
    #endregion

    #region OpenWindowEventArgs
    public class OpenWindowEventArgs : EventArgs
    {
        public OpenWindowEventArgs(ChildWindowViewModelBase ViewModel, ChildWindow Window)
        {
            this.ViewModel = ViewModel;
            this.Window = Window;
        }

        public ChildWindowViewModelBase ViewModel { get; set; }
        public ChildWindow Window { get; set; }
    }
    #endregion

}
