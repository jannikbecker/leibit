using System.Collections.Generic;

namespace Leibit.Entities.Settings
{
    public class GridSetting
    {

        public GridSetting()
        {
            ColumnSettings = new List<GridColumnSetting>();
            GroupedColumns = new List<GridGroupingColumn>();
            SortingColumns = new List<GridSortingColumn>();
        }

        public string GridName { get; set; }

        public List<GridColumnSetting> ColumnSettings { get; set; }

        public List<GridGroupingColumn> GroupedColumns { get; set; }

        public List<GridSortingColumn> SortingColumns { get; set; }

    }
}
