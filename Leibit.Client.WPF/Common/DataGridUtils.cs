using Leibit.BLL;
using Leibit.Entities.Settings;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Xceed.Wpf.DataGrid;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Common
{
    internal static class DataGridUtils
    {

        private static SettingsBLL m_SettingsBll;

        static DataGridUtils()
        {
            m_SettingsBll = new SettingsBLL();
        }

        internal static void LoadLayout(DataGridControl DataGrid, DataGridCollectionView Collection, string GridName)
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
                var columnSetting = gridSetting.ColumnSettings.FirstOrDefault(s => s.ColumnName == Column.FieldName);

                if (columnSetting == null)
                    continue;

                Column.VisiblePosition = columnSetting.Position;
                Column.Width = new ColumnWidth(columnSetting.Width);
            }

            Collection.GroupDescriptions.Clear();
            Collection.SortDescriptions.Clear();

            foreach (var Column in gridSetting.GroupedColumns)
                Collection.GroupDescriptions.Add(new DataGridGroupDescription(Column.ColumnName));

            foreach (var Column in gridSetting.SortingColumns)
                Collection.SortDescriptions.Add(new SortDescription(Column.ColumnName, Column.SortDirection));
        }

        internal static void SaveLayout(DataGridControl DataGrid, DataGridCollectionView Collection, string GridName)
        {
            if (DataGrid == null)
                return;

            var gridSetting = new GridSetting();
            gridSetting.GridName = GridName;

            foreach (var Column in DataGrid.Columns)
            {
                var columnSetting = new GridColumnSetting();
                columnSetting.ColumnName = Column.FieldName;
                columnSetting.Position = Column.VisiblePosition;
                columnSetting.Width = Column.ActualWidth;
                gridSetting.ColumnSettings.Add(columnSetting);
            }

            foreach (DataGridGroupDescription GroupDescription in Collection.GroupDescriptions)
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
