using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaktionslogik für DesignerContainer.xaml
    /// </summary>
    public partial class DesignerContainer : ContentControl
    {

        #region - Ctor -
        public DesignerContainer()
        {
            InitializeComponent();
        }
        #endregion

        #region - Dependency properties -
        public static readonly DependencyProperty MoveThumbHeightProperty = DependencyProperty.Register("MoveThumbHeight", typeof(double), typeof(DesignerContainer), new PropertyMetadata(Double.NaN));
        public static readonly DependencyProperty MoveThumbMarginProperty = DependencyProperty.Register("MoveThumbMargin", typeof(Thickness), typeof(DesignerContainer), new PropertyMetadata(new Thickness(0)));
        public static readonly DependencyProperty TopProperty = DependencyProperty.Register("Top", typeof(double), typeof(DesignerContainer), new PropertyMetadata(0.0));
        public static readonly DependencyProperty LeftProperty = DependencyProperty.Register("Left", typeof(double), typeof(DesignerContainer), new PropertyMetadata(0.0));
        public static readonly DependencyProperty DoubleClickCommandProperty = DependencyProperty.Register("DoubleClickCommand", typeof(ICommand), typeof(DesignerContainer), new PropertyMetadata(null));
        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.Register("ResizeMode", typeof(eResizeMode), typeof(DesignerContainer), new PropertyMetadata(eResizeMode.ResizeAll));
        #endregion

        #region - Properties -

        #region [MoveThumbHeight]
        [TypeConverter(typeof(LengthConverter))]
        public double MoveThumbHeight
        {
            get { return (double)GetValue(MoveThumbHeightProperty); }
            set { SetValue(MoveThumbHeightProperty, value); }
        }
        #endregion

        #region [MoveThumbMargin]
        public Thickness MoveThumbMargin
        {
            get { return (Thickness)GetValue(MoveThumbMarginProperty); }
            set { SetValue(MoveThumbMarginProperty, value); }
        }
        #endregion

        #region [Top]
        public double Top
        {
            get { return (double)GetValue(TopProperty); }
            set { SetValue(TopProperty, value); }
        }
        #endregion

        #region [Left]
        public double Left
        {
            get { return (double)GetValue(LeftProperty); }
            set { SetValue(LeftProperty, value); }
        }
        #endregion

        #region [DoubleClickCommand]
        public ICommand DoubleClickCommand
        {
            get { return (ICommand)GetValue(DoubleClickCommandProperty); }
            set { SetValue(DoubleClickCommandProperty, value); }
        }
        #endregion

        #region [ResizeMode]
        public eResizeMode ResizeMode
        {
            get { return (eResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [OnApplyTemplate]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            MaxWidth -= Left;
            MaxHeight -= Top;
        }
        #endregion

        #endregion

        #region - Private methods -

        private void __MoveThumbDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DoubleClickCommand != null)
                DoubleClickCommand.Execute(e);
        }

        #endregion

    }

    public class ResizeModeConverter : IValueConverter
    {

        #region [Convert]
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is eResizeMode) || !(parameter is eResizeMode))
                return Visibility.Collapsed;

            var ResizeMode = (eResizeMode)value;
            var Parameter = (eResizeMode)parameter;

            return (ResizeMode & Parameter) == Parameter ? Visibility.Visible : Visibility.Collapsed;
        }
        #endregion

        #region [ConvertBack]
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
