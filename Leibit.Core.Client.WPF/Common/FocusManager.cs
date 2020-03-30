using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Leibit.Core.Client.Common
{
    public static class FocusManager
    {

        public static bool GetIsFocused(DependencyObject obj)
        {
            return (bool)obj.GetValue(IsFocusedProperty);
        }

        public static void SetIsFocused(DependencyObject obj, bool value)
        {
            obj.SetValue(IsFocusedProperty, value);
        }

        public static readonly DependencyProperty IsFocusedProperty = DependencyProperty.RegisterAttached("IsFocused", typeof(bool), typeof(FocusManager), new PropertyMetadata(false, OnIsFocusedPropertyChanged));

        public static void OnIsFocusedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var Target = sender as UIElement;

            if (Target == null)
                return;

            if (e.NewValue is bool && (bool)e.NewValue)
            {
                Target.Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(delegate ()
                    {
                        Target.Focus();
                        Keyboard.Focus(Target);
                    }));
            }
        }

    }
}
