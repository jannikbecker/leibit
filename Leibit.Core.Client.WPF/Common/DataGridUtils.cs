using Leibit.BLL;
using Leibit.Core.Client.Common;
using Leibit.Entities.Settings;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Core.Client.WPF
{
    public static class DataGridUtils
    {

        private static SettingsBLL m_SettingsBll;

        static DataGridUtils()
        {
            m_SettingsBll = new SettingsBLL();
        }

        public static void LoadLayout(DataGrid DataGrid, ICollectionView Collection, string GridName)
        {
            if (DataGrid == null)
                return;

            var settingsResult = m_SettingsBll.GetSettings();

            if (!settingsResult.Succeeded)
                return;

            var gridSetting = settingsResult.Result.GridSettings.FirstOrDefault(s => s.GridName == GridName);

            if (gridSetting == null)
                return;

            foreach (var Column in DataGrid.Columns)
            {
                var columnSetting = gridSetting.ColumnSettings.FirstOrDefault(s => s.ColumnName == Column.GetFieldName());

                if (columnSetting == null)
                    continue;

                Column.DisplayIndex = columnSetting.Position;
                Column.Width = new DataGridLength(columnSetting.Width);
            }

            Collection.GroupDescriptions.Clear();
            Collection.SortDescriptions.Clear();

            foreach (var Column in gridSetting.GroupedColumns)
                Collection.GroupDescriptions.Add(new PropertyGroupDescription(Column.ColumnName));

            foreach (var Column in gridSetting.SortingColumns)
            {
                Collection.SortDescriptions.Add(new SortDescription(Column.ColumnName, Column.SortDirection));

                var GridColumn = DataGrid.Columns.FirstOrDefault(c => c.GetFieldName() == Column.ColumnName);

                if (GridColumn != null)
                    GridColumn.SortDirection = Column.SortDirection;
            }
        }

        public static void SaveLayout(DataGrid DataGrid, ICollectionView Collection, string GridName)
        {
            if (DataGrid == null)
                return;

            var gridSetting = new GridSetting();
            gridSetting.GridName = GridName;

            foreach (var Column in DataGrid.Columns)
            {
                var columnSetting = new GridColumnSetting();
                columnSetting.ColumnName = Column.GetFieldName();
                columnSetting.Position = Column.DisplayIndex;
                columnSetting.Width = Column.ActualWidth;
                gridSetting.ColumnSettings.Add(columnSetting);
            }

            foreach (PropertyGroupDescription GroupDescription in Collection.GroupDescriptions)
                gridSetting.GroupedColumns.Add(new GridGroupingColumn { ColumnName = GroupDescription.PropertyName });

            foreach (var SortDescription in Collection.SortDescriptions)
                gridSetting.SortingColumns.Add(new GridSortingColumn { ColumnName = SortDescription.PropertyName, SortDirection = SortDescription.Direction });

            var settingsResult = m_SettingsBll.GetSettings();

            if (!settingsResult.Succeeded)
                return;

            var settings = settingsResult.Result;
            settings.GridSettings.RemoveAll(s => s.GridName == GridName);
            settings.GridSettings.Add(gridSetting);

            var saveResult = m_SettingsBll.SaveSettings(settings);

            if (!saveResult.Succeeded)
                MessageBox.Show(saveResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
