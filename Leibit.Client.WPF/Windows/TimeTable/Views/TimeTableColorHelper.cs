using System.Windows;
using System.Windows.Media;

namespace Leibit.Client.WPF.Windows.TimeTable.Views
{
    public class TimeTableColorHelper : Freezable
    {

        #region [ReadyColor]
        public Brush ReadyColor
        {
            get { return (Brush)GetValue(ReadyColorProperty); }
            set { SetValue(ReadyColorProperty, value); }
        }

        public static readonly DependencyProperty ReadyColorProperty = DependencyProperty.Register("ReadyColor", typeof(Brush), typeof(TimeTableColorHelper), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region [DelayBlue]
        public Brush DelayBlue
        {
            get { return (Brush)GetValue(DelayBlueProperty); }
            set { SetValue(DelayBlueProperty, value); }
        }

        public static readonly DependencyProperty DelayBlueProperty = DependencyProperty.Register("DelayBlue", typeof(Brush), typeof(TimeTableColorHelper), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region [DelayYellow]
        public Brush DelayYellow
        {
            get { return (Brush)GetValue(DelayYellowProperty); }
            set { SetValue(DelayYellowProperty, value); }
        }

        public static readonly DependencyProperty DelayYellowProperty = DependencyProperty.Register("DelayYellow", typeof(Brush), typeof(TimeTableColorHelper), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region [DelayRed]
        public Brush DelayRed
        {
            get { return (Brush)GetValue(DelayRedProperty); }
            set { SetValue(DelayRedProperty, value); }
        }

        public static readonly DependencyProperty DelayRedProperty = DependencyProperty.Register("DelayRed", typeof(Brush), typeof(TimeTableColorHelper), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region [CreateInstanceCore]
        protected override Freezable CreateInstanceCore()
        {
            return new TimeTableColorHelper();
        }
        #endregion

    }
}
