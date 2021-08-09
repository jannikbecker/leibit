using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaction logic for SlidingTextBox.xaml
    /// </summary>
    public partial class SlidingTextBox : UserControl
    {

        #region - Needs -
        private Storyboard m_Storyboard;
        private DoubleAnimation m_DoubleAnimation;
        #endregion

        #region - Ctor -
        public SlidingTextBox()
        {
            InitializeComponent();
        }
        #endregion

        #region - Dependency properties -

        #region [Text]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SlidingTextBox), new PropertyMetadata(null));
        #endregion

        #region [IsSliding]
        public bool IsSliding
        {
            get { return (bool)GetValue(IsSlidingProperty); }
            set { SetValue(IsSlidingProperty, value); }
        }

        public static readonly DependencyProperty IsSlidingProperty = DependencyProperty.Register("IsSliding", typeof(bool), typeof(SlidingTextBox), new PropertyMetadata(false, __IsSlidingChanged));
        #endregion

        #region [Speed]
        public int Speed
        {
            get { return (int)GetValue(SpeedProperty); }
            set { SetValue(SpeedProperty, value); }
        }

        public static readonly DependencyProperty SpeedProperty = DependencyProperty.Register("Speed", typeof(int), typeof(SlidingTextBox), new PropertyMetadata(100));
        #endregion

        #endregion

        #region - Private methods -

        #region [__IsSlidingChanged]
        private static void __IsSlidingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SlidingTextBox sender)
                sender.__IsSlidingChanged();
        }

        private void __IsSlidingChanged()
        {
            if (IsSliding)
                __Start();
            else
                __Stop();
        }
        #endregion

        #region [__SizeChanged]
        private void __SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(txtCopy, Math.Max(ActualWidth, txtOriginal.ActualWidth));

            if (IsSliding)
            {
                __Stop();
                __Start();
            }
        }
        #endregion

        #region [__Start]
        private void __Start()
        {
            if (m_Storyboard != null)
                return;

            m_DoubleAnimation = new DoubleAnimation();
            m_DoubleAnimation.From = 0;
            m_DoubleAnimation.To = -Math.Max(ActualWidth, txtOriginal.ActualWidth);
            m_DoubleAnimation.Duration = TimeSpan.FromMilliseconds(txtOriginal.Text.Length * Speed);
            m_DoubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            Storyboard.SetTargetName(m_DoubleAnimation, "translate");
            Storyboard.SetTargetProperty(m_DoubleAnimation, new PropertyPath(TranslateTransform.XProperty));

            m_Storyboard = new Storyboard();
            m_Storyboard.Children.Add(m_DoubleAnimation);
            m_Storyboard.Begin(slideGrid);
        }
        #endregion

        #region [__Stop]
        private void __Stop()
        {
            if (m_Storyboard == null)
                return;

            m_Storyboard.Stop(slideGrid);
            m_Storyboard = null;
            m_DoubleAnimation = null;
        }
        #endregion

        #endregion

    }
}
