using System.Windows.Controls;
using System.Windows.Data;

namespace Leibit.Core.Client.Common
{
    public static class Extender
    {

        public static string GetFieldName(this DataGridColumn column)
        {
            return ((column as DataGridBoundColumn)?.Binding as Binding)?.Path?.Path;
        }

    }
}
