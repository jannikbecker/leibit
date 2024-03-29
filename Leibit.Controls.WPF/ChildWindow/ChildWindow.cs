﻿using Leibit.BLL;
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
        private bool m_HasLightWindowColor;
        #endregion

        #region - Ctor -
        public ChildWindow()
            : base()
        {
            CloseCommand = new CommandHandler(Close, true);
            DockOutCommand = new CommandHandler(__DockOut, true);
            SizeToContentCommand = new CommandHandler(__SizeToContent, true);

            Closed += __Closed;
            WindowState = Xceed.Wpf.Toolkit.WindowState.Open;

            m_SettingsBll = new SettingsBLL();
            SetWindowColor();
        }
        #endregion

        #region - Events -
        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler DockOutRequested;
        #endregion

        #region - Properties -

        #region [CloseCommand]
        public ICommand CloseCommand
        {
            get;
            private set;
        }
        #endregion

        #region [DockOutCommand]
        public ICommand DockOutCommand
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

        #region [HasLightWindowColor]
        public bool HasLightWindowColor
        {
            get => m_HasLightWindowColor;
            private set
            {
                m_HasLightWindowColor = value;
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
                    {
                        HasLightWindowColor = (Bytes[2] + Bytes[1] + Bytes[0]) / 3 > 128;
                        CaptionForeground = HasLightWindowColor ? Brushes.Black : new SolidColorBrush(Color.FromRgb(225, 225, 225));
                    }
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

        #region [__Closed]
        private void __Closed(object sender, EventArgs e)
        {
            Visibility = Visibility.Collapsed;
        }
        #endregion

        #region [__DockOut]
        private void __DockOut()
        {
            DockOutRequested?.Invoke(this, EventArgs.Empty);
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
