using System;
using System.ComponentModel;
using System.Windows;

namespace Leibit.Core.Client.BaseClasses
{
    public abstract class WindowBase<T> : Window
    {
        public WindowBase()
            : base()
        {
            SourceInitialized += __SourceInitialized;
            Closing += __Closing;
            Closed += __Closed;
            DataContextChanged += __DataContextChanged;
        }

        private void __CloseWindow(object sender, EventArgs e)
        {
            Close();
        }

        private void __SourceInitialized(object sender, EventArgs e)
        {
            if (DataContext is WindowViewModelBase)
                (DataContext as WindowViewModelBase).OnSourceInitialized(sender, e);
        }

        private void __Closing(object sender, CancelEventArgs e)
        {
            if (DataContext is WindowViewModelBase)
                (DataContext as WindowViewModelBase).OnWindowClosing(sender, e);
        }

        private void __Closed(object sender, EventArgs e)
        {
            if (DataContext is WindowViewModelBase)
                (DataContext as WindowViewModelBase).OnWindowClosed();
        }

        private void __DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is WindowViewModelBase)
                (e.OldValue as WindowViewModelBase).CloseWindow -= __CloseWindow;

            if (e.NewValue != null && e.NewValue is WindowViewModelBase)
                (e.NewValue as WindowViewModelBase).CloseWindow += __CloseWindow;
        }
    }
}
