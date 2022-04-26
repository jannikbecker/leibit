using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Data;

namespace Leibit.Controls
{
    public class LeibitDataGridColumn : DependencyObject
    {

        public LeibitDataGridColumn()
        {
            BackgroundConditions = new ObservableCollection<ColumnBackgroundCondition>();
        }

        public string Header { get; set; }

        public string FieldName { get; set; }

        public string ToolTipFieldName { get; set; }

        public Binding VisibilityBinding { get; set; }

        public TextAlignment TextAlignment { get; set; }

        public ObservableCollection<ColumnBackgroundCondition> BackgroundConditions { get; }

    }
}
