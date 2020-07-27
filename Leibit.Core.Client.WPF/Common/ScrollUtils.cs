using System.Windows;
using System.Windows.Input;

namespace Leibit.Core.Client.Common
{
    public class ScrollUtils
    {

        public static bool GetSuppressScroll(DependencyObject obj)
        {
            return (bool)obj.GetValue(SuppressScrollProperty);
        }

        public static void SetSuppressScroll(DependencyObject obj, bool value)
        {
            obj.SetValue(SuppressScrollProperty, value);
        }

        public static readonly DependencyProperty SuppressScrollProperty = DependencyProperty.RegisterAttached("SuppressScroll", typeof(bool), typeof(ScrollUtils), new PropertyMetadata(false, __OnSuppressScrollChanged));

        private static void __OnSuppressScrollChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue && d is FrameworkElement frameworkElement)
                frameworkElement.RequestBringIntoView += __StackPanel_RequestBringIntoView;
        }

        private static void __StackPanel_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.Down) || Keyboard.IsKeyDown(Key.Up))
                return;

            e.Handled = true;
        }
    }
}
