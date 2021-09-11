using System.Windows;
using System.Windows.Media;

namespace Leibit.Controls
{
    public class ColumnBackgroundCondition : DependencyObject
    {

        public ColumnBackgroundCondition()
        {
            Conditions = new ConditionCollection();
        }

        public ConditionCollection Conditions { get; set; }

        //public Brush BackgroundColor { get; set; }

        public Brush BackgroundColor
        {
            get { return (Brush)GetValue(BackgroundColorProperty); }
            set { SetValue(BackgroundColorProperty, value); }
        }

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Brush), typeof(ColumnBackgroundCondition), new PropertyMetadata(Brushes.Transparent));



    }
}
