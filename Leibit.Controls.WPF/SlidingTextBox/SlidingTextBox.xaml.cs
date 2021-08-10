using System;
using System.Threading.Tasks;
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
        private bool m_TextChanged;
        #endregion

        #region - Ctor -
        public SlidingTextBox()
        {
            InitializeComponent();
            IsVisibleChanged += __IsVisibleChanged;
        }
        #endregion

        #region - Dependency properties -

        #region [Text]
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SlidingTextBox), new PropertyMetadata(null, __TextChanged));
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

        #region [__IsVisibleChanged]
        private void __IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
            {
                Canvas.SetLeft(txtCopy, Math.Max(ActualWidth, txtOriginal.ActualWidth));
                __Start(true);
            }
            else
                __Stop();
        }
        #endregion

        #region [__IsSlidingChanged]
        private static void __IsSlidingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SlidingTextBox sender)
                sender.__IsSlidingChanged();
        }

        private void __IsSlidingChanged()
        {
            if (IsSliding)
                __Start(true);
            else
                __Stop();
        }
        #endregion

        #region [__TextChanged]
        private static void __TextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is SlidingTextBox sender)
                sender.__TextChanged();
        }

        private void __TextChanged()
        {
            if (m_Storyboard == null)
                txtOriginal.Text = Text; // Animation is currently not running -> set text directly
            else if (!m_TextChanged)
            {
                // Stop the animation after the next iteration.
                var currentTime = m_Storyboard.GetCurrentTime(slideGrid);
                m_TextChanged = true;
                m_Storyboard.RepeatBehavior = new RepeatBehavior(1);
                m_Storyboard.Completed += __Storyboard_Completed;
                m_Storyboard.Begin(slideGrid, true);

                if (currentTime.HasValue)
                    m_Storyboard.Seek(slideGrid, currentTime.Value, TimeSeekOrigin.BeginTime);
            }
        }
        #endregion

        #region [__Storyboard_Completed]
        private void __Storyboard_Completed(object sender, EventArgs e)
        {
            __Stop();
            __Start(false);
        }
        #endregion

        #region [__SizeChanged]
        private void __SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(txtCopy, Math.Max(ActualWidth, txtOriginal.ActualWidth));
        }
        #endregion

        #region [__Start]
        private async void __Start(bool defer)
        {
            if (m_Storyboard != null || Visibility != Visibility.Visible || !IsSliding)
                return;

            if (defer)
                await Task.Delay(500);

            txtOriginal.UpdateLayout();

            m_DoubleAnimation = new DoubleAnimation();
            m_DoubleAnimation.From = 0;
            m_DoubleAnimation.To = -Math.Max(ActualWidth, txtOriginal.ActualWidth);
            m_DoubleAnimation.Duration = TimeSpan.FromSeconds(Math.Max(ActualWidth, txtOriginal.ActualWidth) / Speed);

            Storyboard.SetTargetName(m_DoubleAnimation, "translate");
            Storyboard.SetTargetProperty(m_DoubleAnimation, new PropertyPath(TranslateTransform.XProperty));

            m_Storyboard = new Storyboard();
            m_Storyboard.RepeatBehavior = RepeatBehavior.Forever;
            m_Storyboard.Children.Add(m_DoubleAnimation);
            m_Storyboard.Begin(slideGrid, true);
        }
        #endregion

        #region [__Stop]
        private void __Stop()
        {
            if (m_Storyboard == null)
                return;

            if (m_TextChanged)
            {
                m_Storyboard.Completed -= __Storyboard_Completed;
                txtOriginal.Text = Text;
                m_TextChanged = false;
            }

            m_Storyboard.Stop(slideGrid);
            m_Storyboard.Remove(slideGrid);
            m_Storyboard = null;
            m_DoubleAnimation = null;
        }
        #endregion

        #endregion

    }
}
