using Leibit.BLL;
using Leibit.Core.Common;
using System;
using System.ComponentModel;
using System.IO;
using Xceed.Wpf.DataGrid;

namespace Leibit.Client.WPF.Common
{
    internal static class DataGridUtils
    {

        private static SettingsBLL m_SettingsBll;

        static DataGridUtils()
        {
            m_SettingsBll = new SettingsBLL();
        }

        internal static void LoadLayout(DataGridControl DataGrid, DataGridCollectionView Collection, string RuleKey)
        {
            if (DataGrid == null)
                return;

            foreach (var Column in DataGrid.Columns)
            {
                var Position = m_SettingsBll.LoadRule<int>(Path.Combine(RuleKey, "ColumnPositions"), Column.FieldName, -1);
                var sWidth = m_SettingsBll.LoadRule<string>(Path.Combine(RuleKey, "ColumnWidths"), Column.FieldName);
                double Width;

                if (Position != -1)
                    Column.VisiblePosition = Position;

                if (sWidth.IsNotNullOrEmpty() && Double.TryParse(sWidth, out Width))
                    Column.Width = new ColumnWidth(Width);
            }

            Collection.GroupDescriptions.Clear();
            Collection.SortDescriptions.Clear();

            for (int i = 0; true; i++)
            {
                var ColumnName = m_SettingsBll.LoadRule<string>(Path.Combine(RuleKey, "Grouping", i.ToString()), "ColumnName");

                if (ColumnName.IsNullOrEmpty())
                    break;

                Collection.GroupDescriptions.Add(new DataGridGroupDescription(ColumnName));
            }

            for (int i = 0; true; i++)
            {
                var ColumnName = m_SettingsBll.LoadRule<string>(Path.Combine(RuleKey, "Sorting", i.ToString()), "ColumnName");

                if (ColumnName.IsNullOrEmpty())
                    break;

                var SortDirection = m_SettingsBll.LoadRule<ListSortDirection>(Path.Combine(RuleKey, "Sorting", i.ToString()), "SortDirection");
                Collection.SortDescriptions.Add(new SortDescription(ColumnName, SortDirection));
            }
        }

        internal static void SaveLayout(DataGridControl DataGrid, DataGridCollectionView Collection, string RuleKey)
        {
            if (DataGrid == null)
                return;

            foreach (var Column in DataGrid.Columns)
            {
                m_SettingsBll.SaveRule(Path.Combine(RuleKey, "ColumnPositions"), Column.FieldName, Column.VisiblePosition);
                m_SettingsBll.SaveRule(Path.Combine(RuleKey, "ColumnWidths"), Column.FieldName, Column.ActualWidth);
            }

            m_SettingsBll.ClearRules(Path.Combine(RuleKey, "Grouping"));
            m_SettingsBll.ClearRules(Path.Combine(RuleKey, "Sorting"));

            for (int i = 0; i < Collection.GroupDescriptions.Count; i++)
            {
                var GroupDescription = Collection.GroupDescriptions[i] as DataGridGroupDescription;

                if (GroupDescription == null)
                    break;

                m_SettingsBll.SaveRule(Path.Combine(RuleKey, "Grouping", i.ToString()), "ColumnName", GroupDescription.PropertyName);
            }

            for (int i = 0; i < Collection.SortDescriptions.Count; i++)
            {
                var SortingDescription = Collection.SortDescriptions[i];

                m_SettingsBll.SaveRule(Path.Combine(RuleKey, "Sorting", i.ToString()), "ColumnName", SortingDescription.PropertyName);
                m_SettingsBll.SaveRule(Path.Combine(RuleKey, "Sorting", i.ToString()), "SortDirection", SortingDescription.Direction);
            }
        }

    }
}
