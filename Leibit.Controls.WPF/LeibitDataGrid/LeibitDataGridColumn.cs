using System.Windows;
using System.Windows.Data;

namespace Leibit.Controls
{
    public class LeibitDataGridColumn : DependencyObject
    {

        public string Header { get; set; }

        public string FieldName { get; set; }

        public Binding VisibilityBinding { get; set; }

    }
}
