using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Leibit.Controls
{
    public class LeibitWindow : ContentControl
    {

        #region - Ctor -
        public LeibitWindow(string identifier)
        {
            Identifier = identifier;
        }
        #endregion

        #region - Properties -
        public string Identifier { get; private set; }
        public bool IsDockedOut { get; set; }
        public ChildWindow ChildWindow { get; internal set; }
        public Window Window { get; internal set; }
        public Style ChildWindowStyle { get; set; }
        #endregion

        #region - Dependency properties -

        #region [PositionX]
        public double PositionX
        {
            get { return (double)GetValue(PositionXProperty); }
            set { SetValue(PositionXProperty, value); }
        }

        public static readonly DependencyProperty PositionXProperty = DependencyProperty.Register("PositionX", typeof(double), typeof(LeibitWindow), new PropertyMetadata(0.0));
        #endregion

        #region [PositionY]
        public double PositionY
        {
            get { return (double)GetValue(PositionYProperty); }
            set { SetValue(PositionYProperty, value); }
        }

        public static readonly DependencyProperty PositionYProperty = DependencyProperty.Register("PositionY", typeof(double), typeof(LeibitWindow), new PropertyMetadata(0.0));
        #endregion

        #region [Caption]
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }

        public static readonly DependencyProperty CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(LeibitWindow), new PropertyMetadata(null));
        #endregion

        #region [ResizeMode]
        public eResizeMode ResizeMode
        {
            get { return (eResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }

        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.Register("ResizeMode", typeof(eResizeMode), typeof(LeibitWindow), new PropertyMetadata(eResizeMode.ResizeAll));
        #endregion

        #endregion

        #region - Public methods -

        #region [BringToFront]
        public void BringToFront()
        {
            ChildWindow?.Focus();
            Window?.Focus();
        }
        #endregion

        #endregion

        #region - Internal methods -

        #region [CreateChildWindow]
        internal ChildWindow CreateChildWindow()
        {
            var window = new ChildWindow();
            window.DataContext = DataContext;
            window.Content = Content;
            window.Style = ChildWindowStyle;

            __SetBinding(window, nameof(Caption), ChildWindow.CaptionProperty);
            __SetBinding(window, nameof(ResizeMode), ChildWindow.ResizeModeProperty);
            __SetBinding(window, nameof(Width), ChildWindow.WidthProperty);
            __SetBinding(window, nameof(Height), ChildWindow.HeightProperty);
            __SetBinding(window, nameof(PositionX), ChildWindow.PositionXProperty);
            __SetBinding(window, nameof(PositionY), ChildWindow.PositionYProperty);

            return window;
        }
        #endregion

        #region [CreateWindow]
        internal Window CreateWindow()
        {
            var window = new Window();
            window.DataContext = DataContext;
            window.Content = Content;
            window.SizeToContent = SizeToContent.WidthAndHeight;

            if (ResizeMode == eResizeMode.NoResize)
                window.ResizeMode = System.Windows.ResizeMode.NoResize;
            else
                window.ResizeMode = System.Windows.ResizeMode.CanResize;

            __SetBinding(window, nameof(Caption), Window.TitleProperty);
            __SetBinding(window, nameof(Width), Window.WidthProperty);
            __SetBinding(window, nameof(Height), Window.HeightProperty);
            __SetBinding(window, nameof(PositionX), Window.LeftProperty);
            __SetBinding(window, nameof(PositionY), Window.TopProperty);

            return window;
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__SetBinding]
        private void __SetBinding(FrameworkElement obj, string propertyName, DependencyProperty dependencyProperty)
        {
            var binding = new Binding(propertyName);
            binding.Source = this;
            binding.Mode = BindingMode.TwoWay;
            obj.SetBinding(dependencyProperty, binding);
        }
        #endregion

        #endregion

    }
}
