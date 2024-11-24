using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Xceed.Wpf.Toolkit.Primitives;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaktionslogik für DynamicWindowContainer.xaml
    /// </summary>
    public partial class DynamicWindowContainer : WindowContainer
    {

        private List<Window> m_Windows;

        #region - Ctor -
        public DynamicWindowContainer()
            : base()
        {
            m_Windows = new List<Window>();
            SizeChanged += __SizeChanged;
        }
        #endregion

        #region - Dependency Properties -
        public static readonly DependencyProperty WindowsProperty = DependencyProperty.Register("Windows", typeof(ObservableCollection<LeibitWindow>), typeof(DynamicWindowContainer), new PropertyMetadata(null, OnWindowsPropertyChanged));
        #endregion

        #region - Properties -

        #region [Windows]
        public ObservableCollection<LeibitWindow> Windows
        {
            get
            {
                return (ObservableCollection<LeibitWindow>)GetValue(WindowsProperty);
            }
            set
            {
                SetValue(WindowsProperty, value);
            }
        }
        #endregion

        #endregion

        #region - DP callbacks -

        #region [WindowsChanged]
        private static void OnWindowsPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((DynamicWindowContainer)sender).OnWindowsChanged(e.OldValue as ObservableCollection<LeibitWindow>, e.NewValue as ObservableCollection<LeibitWindow>);
        }

        private void OnWindowsChanged(ObservableCollection<LeibitWindow> OldValue, ObservableCollection<LeibitWindow> NewValue)
        {
            if (OldValue != null)
            {
                OldValue.CollectionChanged -= Windows_CollectionChanged;
                //Children.Clear();
            }

            if (NewValue != null)
            {
                NewValue.CollectionChanged += Windows_CollectionChanged;

                //foreach (var Child in NewValue)
                //    Children.Add(Child);
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        private void Windows_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                var Windows = new List<ChildWindow>();

                foreach (var Child in Children)
                {
                    var Window = Child as ChildWindow;

                    if (Window == null)
                        continue;

                    Window.Close();
                    Window.Visibility = Visibility.Collapsed;
                    Windows.Add(Window);
                }

                Windows.ForEach(w => Children.Remove(w));

                foreach (var window in m_Windows)
                    window.Close();

                m_Windows.Clear();
            }

            if (e.OldItems != null)
            {
                foreach (var Old in e.OldItems)
                {
                    var OldWindow = Old as LeibitWindow;

                    if (OldWindow.ChildWindow != null)
                    {
                        if (OldWindow.ChildWindow.IsVisible)
                            OldWindow.ChildWindow.Visibility = Visibility.Collapsed;

                        Children.Remove(OldWindow.ChildWindow);
                    }

                    if (OldWindow.Window != null)
                    {
                        OldWindow.Window.Close();
                        m_Windows.Remove(OldWindow.Window);
                    }
                }
            }

            if (e.NewItems != null)
            {
                foreach (var New in e.NewItems)
                {
                    var leibitWindow = New as LeibitWindow;

                    if (leibitWindow.IsDockedOut)
                    {
                        var window = leibitWindow.CreateWindow();
                        leibitWindow.Window = window;
                        window.WindowStartupLocation = WindowStartupLocation.Manual;
                        window.Closed += __Window_Closed;
                        window.Show();
                        m_Windows.Add(window);
                    }
                    else
                    {
                        var childWindow = leibitWindow.CreateChildWindow();
                        leibitWindow.ChildWindow = childWindow;

                        if (double.IsInfinity(childWindow.MaxWidth))
                            childWindow.MaxWidth = ActualWidth;
                        if (double.IsInfinity(childWindow.MaxHeight))
                            childWindow.MaxHeight = ActualHeight;

                        if (leibitWindow.PositionX > ActualWidth)
                            leibitWindow.PositionX = 0;
                        if (leibitWindow.PositionY > ActualHeight)
                            leibitWindow.PositionY = 0;

                        childWindow.Visibility = Visibility.Hidden;
                        childWindow.Loaded += __ChildWindow_Loaded;
                        childWindow.Closed += __ChildWindow_Closed;
                        childWindow.DockOutRequested += __ChildWindow_DockOutRequested;
                        Children.Add(childWindow);
                    }
                }
            }
        }

        private void __ChildWindow_DockOutRequested(object sender, EventArgs e)
        {
            var childWindow = sender as ChildWindow;

            var leibitWindow = Windows.FirstOrDefault(w => w.ChildWindow == childWindow);

            if (leibitWindow == null)
                return;

            var coord = PointToScreen(new Point(0, 0));
            childWindow.DockOutRequested -= __ChildWindow_DockOutRequested;
            childWindow.Closed -= __ChildWindow_Closed;
            childWindow.Close();

            leibitWindow.PositionX += coord.X;
            leibitWindow.PositionY += coord.Y;

            var window = leibitWindow.CreateWindow();
            leibitWindow.Window = window;
            leibitWindow.IsDockedOut = true;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
            window.Closed += __Window_Closed;
            window.Show();
            m_Windows.Add(window);
        }

        private void __ChildWindow_Loaded(object sender, RoutedEventArgs e)
        {
            var window = sender as ChildWindow;
            window.Loaded -= __ChildWindow_Loaded;
            __CalculateWindowPosition(window);
            window.Visibility = Visibility.Visible;
        }

        private void __ChildWindow_Closed(object sender, EventArgs e)
        {
            var childWindow = sender as ChildWindow;
            childWindow.Closed -= __ChildWindow_Closed;

            var leibitWindow = Windows.FirstOrDefault(w => w.ChildWindow == childWindow);

            if (leibitWindow != null)
                Windows.Remove(leibitWindow);
        }

        private void __Window_Closed(object sender, EventArgs e)
        {
            var window = sender as Window;
            window.Closed -= __Window_Closed;

            var leibitWindow = Windows.FirstOrDefault(w => w.Window == window);

            if (leibitWindow != null)
                Windows.Remove(leibitWindow);
        }

        private void __CalculateWindowPosition(ChildWindow window)
        {
            var contentControl = __GetTemplateChild(window, "DesignerContainerContent");

            if (contentControl == null || !__CheckOverlap(window, contentControl, window.PositionX, window.PositionY, out _, out _))
                return;

            double x, y;
            double nextY = 0;

            while (nextY + contentControl.ActualHeight < ActualHeight)
            {
                double nextX = 0;
                y = nextY;
                nextY = double.MaxValue;

                while (nextX + contentControl.ActualWidth < ActualWidth)
                {
                    x = nextX;
                    nextX = double.MaxValue;

                    if (__CheckOverlap(window, contentControl, x, y, out double rightBound, out double bottomBound))
                    {
                        nextX = Math.Min(nextX, rightBound + 1);
                        nextY = Math.Min(nextY, bottomBound + 1);
                    }
                    else
                    {
                        window.MaxWidth += window.PositionX - x;
                        window.MaxHeight += window.PositionY - y;
                        window.PositionX = x;
                        window.PositionY = y;
                        return;
                    }
                }
            }
        }

        private bool __CheckOverlap(ChildWindow window, FrameworkElement contentControl, double x, double y, out double rightBound, out double bottomBound)
        {
            foreach (ChildWindow refWindow in Children)
            {
                if (window == refWindow || !refWindow.IsVisible)
                    continue;

                var refContentControl = __GetTemplateChild(refWindow, "DesignerContainerContent");

                if (refContentControl == null)
                    continue;

                var xLeft = refWindow.PositionX;
                var xRight = xLeft + refContentControl.ActualWidth;
                var yTop = refWindow.PositionY;
                var yBottom = yTop + refContentControl.ActualHeight;

                if (xRight >= x && xLeft <= x + contentControl.ActualWidth && yBottom >= y && yTop <= y + contentControl.ActualHeight)
                {
                    rightBound = xRight;
                    bottomBound = yBottom;
                    return true;
                }
            }

            rightBound = 0;
            bottomBound = 0;
            return false;
        }

        #region [__GetTemplateChild]
        private FrameworkElement __GetTemplateChild(DependencyObject parent, string name)
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is FrameworkElement fe && fe.Name == name)
                    return fe;

                var result = __GetTemplateChild(child, name);

                if (result != null)
                    return result;
            }

            return null;
        }
        #endregion

        #region [__SizeChanged]
        private void __SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Children == null)
                return;

            foreach (ChildWindow Window in Children)
            {
                if (Window.PositionX > ActualWidth)
                    Window.PositionX = 0;
                if (Window.PositionY > ActualHeight)
                    Window.PositionY = 0;

                Window.MaxWidth = ActualWidth - Window.PositionX;
                Window.MaxHeight = ActualHeight - Window.PositionY;
            }
        }
        #endregion

        #endregion

    }
}
