using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using Xceed.Wpf.Toolkit.Primitives;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaktionslogik für DynamicWindowContainer.xaml
    /// </summary>
    public partial class DynamicWindowContainer : WindowContainer
    {

        #region - Ctor -
        public DynamicWindowContainer()
            : base()
        {

        }
        #endregion

        #region - Dependency Properties -
        public static readonly DependencyProperty WindowsProperty = DependencyProperty.Register("Windows", typeof(ObservableCollection<ChildWindow>), typeof(DynamicWindowContainer), new PropertyMetadata(null, OnWindowsPropertyChanged));
        #endregion

        #region - Properties -

        #region [Windows]
        public ObservableCollection<ChildWindow> Windows
        {
            get
            {
                return (ObservableCollection<ChildWindow>)GetValue(WindowsProperty);
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
            ((DynamicWindowContainer)sender).OnWindowsChanged(e.OldValue as ObservableCollection<ChildWindow>, e.NewValue as ObservableCollection<ChildWindow>);
        }

        private void OnWindowsChanged(ObservableCollection<ChildWindow> OldValue, ObservableCollection<ChildWindow> NewValue)
        {
            if (OldValue != null)
            {
                OldValue.CollectionChanged -= Windows_CollectionChanged;
                Children.Clear();
            }

            if (NewValue != null)
            {
                NewValue.CollectionChanged += Windows_CollectionChanged;

                foreach (var Child in NewValue)
                    Children.Add(Child);
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
            }

            if (e.OldItems != null)
            {
                foreach (var Old in e.OldItems)
                {
                    var OldWindow = Old as ChildWindow;

                    //if (OldWindow.IsVisible)
                    //{
                    //    OldWindow.IsVisibleChanged += ChildWindow_VisibleChanged;
                    //    OldWindow.Close();
                    //}
                    //else
                    //    Children.Remove(OldWindow);

                    if (OldWindow.IsVisible)
                        OldWindow.Visibility = Visibility.Collapsed;

                    Children.Remove(OldWindow);
                }
            }

            if (e.NewItems != null)
            {
                foreach (var New in e.NewItems)
                {
                    var Window = New as ChildWindow;

                    if (double.IsInfinity(Window.MaxWidth))
                        Window.MaxWidth = ActualWidth;
                    if (double.IsInfinity(Window.MaxHeight))
                        Window.MaxHeight = ActualHeight;

                    if (Window.PositionX > ActualWidth)
                        Window.PositionX = 0;
                    if (Window.PositionY > ActualHeight)
                        Window.PositionY = 0;

                    Children.Add(Window);
                }
            }
        }

        //private void ChildWindow_VisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        //{
        //    if (!(bool)e.NewValue)
        //    {
        //        var Window = sender as ChildWindow;

        //        if (Window != null)
        //        {
        //            Children.Remove(Window);
        //            Window.IsVisibleChanged -= ChildWindow_VisibleChanged;
        //        }
        //    }
        //}

        #endregion

    }
}
