using System.Windows;
using System.Windows.Media;

namespace Leibit.Controls
{
    public class ColumnBackgroundCondition
    {

        public ColumnBackgroundCondition()
        {
            Conditions = new ConditionCollection();
        }

        public ConditionCollection Conditions { get; set; }

        public Brush BackgroundColor { get; set; }

    }
}
