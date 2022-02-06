using Leibit.BLL;
using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Leibit.Controls
{
    public class ChildWindow : Xceed.Wpf.Toolkit.ChildWindow, INotifyPropertyChanged
    {

        #region - Needs -
        private SettingsBLL m_SettingsBll;
        private int? m_WindowColor;
        private Brush m_CaptionForegroundColor;
        #endregion

        #region - Ctor -
        public ChildWindow(string Identifier)
            : base()
        {
            this.Identifier = Identifier;

            CloseCommand = new CommandHandler(Close, true);
            SizeToContentCommand = new CommandHandler(__SizeToContent, true);

            DataContextChanged += __DataContextChanged;
            Closed += __Closed;
            WindowState = Xceed.Wpf.Toolkit.WindowState.Open;

            m_SettingsBll = new SettingsBLL();
            SetWindowColor();
        }
        #endregion

        #region - Events -
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region - Properties -

        #region [Identifier]
        public string Identifier
        {
            get;
            private set;
        }
        #endregion

        #region [CloseCommand]
        public ICommand CloseCommand
        {
            get;
            private set;
        }
        #endregion

        #region [SizeToContentCommand]
        public ICommand SizeToContentCommand
        {
            get;
            private set;
        }
        #endregion

        #region [WindowColor]
        public int? WindowColor
        {
            get => m_WindowColor;
            private set
            {
                m_WindowColor = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #region [CaptionForegroundColor]
        public Brush CaptionForegroundColor
        {
            get => m_CaptionForegroundColor;
            private set
            {
                m_CaptionForegroundColor = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #region [ResizeMode]
        public eResizeMode ResizeMode
        {
            get { return (eResizeMode)GetValue(ResizeModeProperty); }
            set { SetValue(ResizeModeProperty, value); }
        }
        #endregion

        #region [PositionX]
        public double PositionX
        {
            get { return (double)GetValue(PositionXProperty); }
            set { SetValue(PositionXProperty, value); }
        }
        #endregion

        #region [PositionY]
        public double PositionY
        {
            get { return (double)GetValue(PositionYProperty); }
            set { SetValue(PositionYProperty, value); }
        }
        #endregion

        #endregion

        #region - Dependency properties -
        public static readonly DependencyProperty ResizeModeProperty = DependencyProperty.Register("ResizeMode", typeof(eResizeMode), typeof(ChildWindow), new PropertyMetadata(eResizeMode.ResizeAll));
        public static readonly DependencyProperty PositionXProperty = DependencyProperty.Register("PositionX", typeof(double), typeof(ChildWindow), new PropertyMetadata(0.0));
        public static readonly DependencyProperty PositionYProperty = DependencyProperty.Register("PositionY", typeof(double), typeof(ChildWindow), new PropertyMetadata(0.0));
        #endregion

        #region [SetWindowColor]
        public void SetWindowColor()
        {
            var settingsResult = m_SettingsBll.GetSettings();

            if (settingsResult.Succeeded)
            {
                WindowColor = settingsResult.Result.WindowColor;
                CaptionForeground = Brushes.Black;

                if (WindowColor.HasValue)
                {
                    var Bytes = BitConverter.GetBytes(WindowColor.Value);

                    if (Bytes.Length == 4)
                        CaptionForeground = (Bytes[2] + Bytes[1] + Bytes[0]) / 3 > 128 ? Brushes.Black : new SolidColorBrush(Color.FromRgb(225, 225, 225));
                }
            }
        }
        #endregion

        #region [OnClosing]
        protected override void OnClosing(CancelEventArgs e)
        {
            if (DataContext is WindowViewModelBase vm)
                vm.OnWindowClosing(this, e);

            base.OnClosing(e);
        }
        #endregion

        #region [OnClosed]
        protected override void OnClosed(EventArgs e)
        {
            if (DataContext is WindowViewModelBase vm)
                vm.OnWindowClosed();

            base.OnClosed(e);
        }
        #endregion

        #region - Private methods -

        #region [__OnPropertyChanged]
        private void __OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region [__CloseWindow]
        private void __CloseWindow(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region [__Closed]
        private void __Closed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
        #endregion

        #region [__DataContextChanged]
        private void __DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null && e.OldValue is WindowViewModelBase)
                (e.OldValue as WindowViewModelBase).CloseWindow -= __CloseWindow;

            if (e.NewValue != null && e.NewValue is WindowViewModelBase)
                (e.NewValue as WindowViewModelBase).CloseWindow += __CloseWindow;
        }
        #endregion

        #region [__SizeToContent]
        private void __SizeToContent()
        {
            Width = Double.NaN;
            Height = Double.NaN;
        }
        #endregion

        #endregion

    }
}
