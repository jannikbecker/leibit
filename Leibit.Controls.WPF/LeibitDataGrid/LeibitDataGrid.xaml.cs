using Leibit.Core.Client.Common;
using Leibit.Core.Client.WPF;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaction logic for LeibitDataGrid.xaml
    /// </summary>
    public partial class LeibitDataGrid : UserControl, INotifyPropertyChanged
    {

        #region - Needs -
        private ICollectionView m_Collection;
        private ObservableCollection<LeibitDataGridColumn> m_GroupedColumns;
        private string m_GroupingToolTipText;
        private string m_UngroupingToolTipText;

        private Point m_DragStartPoint;
        private bool m_IsDragging;
        private LeibitDataGridColumn m_DraggingGroupedColumn;
        private bool m_CanGroupColumn;
        private bool m_CanReorderColumn;
        private bool m_CanDropGroupedColumn;
        private bool m_IsDoubleClick;
        #endregion

        #region - Ctor -
        public LeibitDataGrid()
        {
            SetValue(ColumnsPropertyKey, new ObservableCollection<LeibitDataGridColumn>());
            Columns.CollectionChanged += __Columns_CollectionChanged;
            InitializeComponent();
        }
        #endregion

        #region - Events -
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region - Properties -

        #region [LayoutKey]
        public string LayoutKey
        {
            get { return (string)GetValue(LayoutKeyProperty); }
            set { SetValue(LayoutKeyProperty, value); }
        }

        public static readonly DependencyProperty LayoutKeyProperty = DependencyProperty.Register("LayoutKey", typeof(string), typeof(LeibitDataGrid), new PropertyMetadata(null));
        #endregion

        #region [Columns]
        public ObservableCollection<LeibitDataGridColumn> Columns
        {
            get { return (ObservableCollection<LeibitDataGridColumn>)GetValue(ColumnsProperty); }
        }

        public static readonly DependencyPropertyKey ColumnsPropertyKey = DependencyProperty.RegisterReadOnly("Columns", typeof(ObservableCollection<LeibitDataGridColumn>), typeof(LeibitDataGridColumn), new PropertyMetadata(null));
        public static readonly DependencyProperty ColumnsProperty = ColumnsPropertyKey.DependencyProperty;
        #endregion

        #region [ItemsSource]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(LeibitDataGrid), new PropertyMetadata(null, __OnItemsSourceChanged));

        private static void __OnItemsSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as LeibitDataGrid;

            if (e.NewValue == null)
                sender.Collection = null;
            else
                sender.Collection = CollectionViewSource.GetDefaultView(e.NewValue);
        }
        #endregion

        #region [CanGroup]
        public bool CanGroup
        {
            get { return (bool)GetValue(CanGroupProperty); }
            set { SetValue(CanGroupProperty, value); }
        }

        public static readonly DependencyProperty CanGroupProperty = DependencyProperty.Register("CanGroup", typeof(bool), typeof(LeibitDataGrid), new PropertyMetadata(true));
        #endregion

        #region [SelectedItem]
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register("SelectedItem", typeof(object), typeof(LeibitDataGrid), new PropertyMetadata(null));
        #endregion

        #region [SelectedColumn]
        public string SelectedColumn
        {
            get { return (string)GetValue(SelectedColumnProperty); }
            set { SetValue(SelectedColumnProperty, value); }
        }

        public static readonly DependencyProperty SelectedColumnProperty = DependencyProperty.Register("SelectedColumn", typeof(string), typeof(LeibitDataGrid), new PropertyMetadata(null));
        #endregion

        #region [Collection]
        public ICollectionView Collection
        {
            get
            {
                return m_Collection;
            }
            private set
            {
                m_Collection = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #region [SaveLayout]
        public bool SaveLayout
        {
            get { return (bool)GetValue(SaveLayoutProperty); }
            set { SetValue(SaveLayoutProperty, value); }
        }

        public static readonly DependencyProperty SaveLayoutProperty = DependencyProperty.Register("SaveLayout", typeof(bool), typeof(LeibitDataGrid), new PropertyMetadata(false, __OnSaveLayoutChanged));

        private static void __OnSaveLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as LeibitDataGrid;

            if ((bool)e.NewValue)
            {
                if (sender.LayoutKey != null)
                    DataGridUtils.SaveLayout(sender.dataGrid, sender.Collection, sender.LayoutKey);

                sender.SaveLayout = false;
            }
        }
        #endregion

        #region [RowDoubleClickCommand]
        public ICommand RowDoubleClickCommand
        {
            get { return (ICommand)GetValue(RowDoubleClickCommandProperty); }
            set { SetValue(RowDoubleClickCommandProperty, value); }
        }

        public static readonly DependencyProperty RowDoubleClickCommandProperty = DependencyProperty.Register("RowDoubleClickCommand", typeof(ICommand), typeof(LeibitDataGrid), new PropertyMetadata(null));
        #endregion

        #region [CurrentColumn]
        public DataGridColumn CurrentColumn
        {
            set
            {
                SelectedColumn = value.GetFieldName();
            }
        }
        #endregion

        #region [GroupedColumns]
        public ObservableCollection<LeibitDataGridColumn> GroupedColumns
        {
            get
            {
                return m_GroupedColumns;
            }
            set
            {
                m_GroupedColumns = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #region [CanGroupColumn]
        public bool CanGroupColumn
        {
            get
            {
                return m_CanGroupColumn;
            }
            private set
            {
                m_CanGroupColumn = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #region [CanReorderColumn]
        public bool CanReorderColumn
        {
            get
            {
                return m_CanReorderColumn;
            }
            private set
            {
                m_CanReorderColumn = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #region [GroupingToolTipText]
        public string GroupingToolTipText
        {
            get
            {
                return m_GroupingToolTipText;
            }
            private set
            {
                m_GroupingToolTipText = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #region [UngroupingToolTipText]
        public string UngroupingToolTipText
        {
            get
            {
                return m_UngroupingToolTipText;
            }
            private set
            {
                m_UngroupingToolTipText = value;
                __OnPropertyChanged();
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__OnPropertyChanged]
        private void __OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region [__RefreshGroupArea]
        private void __RefreshGroupArea()
        {
            GroupedColumns = new ObservableCollection<LeibitDataGridColumn>(Collection.GroupDescriptions.Select(g => Columns.FirstOrDefault(c => (g as PropertyGroupDescription)?.PropertyName == c.FieldName)).Where(c => c != null));
        }
        #endregion

        #region [__GetReorderTargetIndicatorPosition]
        private Point __GetReorderTargetIndicatorPosition(double mouseX)
        {
            var point = new Point(0, 0);
            var headers = __GetTemplateChilds<DataGridColumnHeader>(dataGrid).Where(h => h.Column != null).OrderBy(h => h.Column.DisplayIndex);

            if (!headers.Any())
                return point;

            var offset = headers.First().TransformToAncestor(this).Transform(new Point(0, 0));
            point.Y = offset.Y + headers.First().ActualHeight;

            var currentX = offset.X;

            foreach (var header in headers)
            {
                var threshold = currentX + header.ActualWidth / 2;

                if (mouseX < threshold)
                    break;

                currentX += header.ActualWidth;
            }

            currentX -= 12;

            if (currentX < 0)
                currentX = 0;
            if (currentX + 24 >= dataGrid.ActualWidth)
                currentX = dataGrid.ActualWidth - 24;

            point.X = currentX;
            return point;
        }
        #endregion

        #region [__GetDisplayIndexForReorder]
        private int __GetDisplayIndexForReorder(double mouseX)
        {
            var displayIndex = -1;
            var headers = __GetTemplateChilds<DataGridColumnHeader>(dataGrid).Where(h => h.Column != null).OrderBy(h => h.Column.DisplayIndex);

            if (!headers.Any())
                return displayIndex;

            var offset = headers.First().TransformToAncestor(this).Transform(new Point(0, 0));
            var currentX = offset.X;

            foreach (var header in headers)
            {
                var threshold = currentX + header.ActualWidth / 2;

                if (mouseX < threshold)
                {
                    displayIndex = header.Column.DisplayIndex;
                    break;
                }

                currentX += header.ActualWidth;
            }

            if (displayIndex == -1)
                displayIndex = headers.Max(h => h.Column.DisplayIndex) + 1;

            return displayIndex;
        }
        #endregion

        #region [__GetTemplateChilds]
        private List<T> __GetTemplateChilds<T>(DependencyObject parent)
        {
            var result = new List<T>();
            var childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T header)
                    result.Add(header);
                else
                    result.AddRange(__GetTemplateChilds<T>(child));
            }

            return result;
        }
        #endregion

        #region [__IsColumnResizing]
        private bool __IsColumnResizing()
        {
            var thumbs = __GetTemplateChilds<Thumb>(dataGrid);
            return thumbs.Where(thumb => thumb.Name == "PART_LeftHeaderGripper" || thumb.Name == "PART_RightHeaderGripper").Any(thumb => thumb.IsDragging);
        }
        #endregion

        #region [__SortColumn]
        private void __SortColumn(DataGridColumnHeader header)
        {
            var columnName = header.Column.GetFieldName();
            var oldSortDirection = header.Column.SortDirection;
            ListSortDirection? newSortDirection;

            if (oldSortDirection == null)
                newSortDirection = ListSortDirection.Ascending;
            else if (oldSortDirection == ListSortDirection.Ascending)
                newSortDirection = ListSortDirection.Descending;
            else
                newSortDirection = null;

            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                var sortDescription = Collection.SortDescriptions.FirstOrDefault(s => s.PropertyName == columnName);

                if (sortDescription.PropertyName != columnName && newSortDirection.HasValue)
                    Collection.SortDescriptions.Add(new SortDescription(columnName, newSortDirection.Value));
                else if (sortDescription.PropertyName == columnName && !newSortDirection.HasValue)
                    Collection.SortDescriptions.Remove(sortDescription);
                else if (sortDescription.PropertyName == columnName && newSortDirection.HasValue)
                {
                    var sortDescriptions = Collection.SortDescriptions.ToList();
                    Collection.SortDescriptions.Clear();

                    foreach (var description in sortDescriptions)
                    {
                        if (description.PropertyName == columnName)
                            Collection.SortDescriptions.Add(new SortDescription(columnName, newSortDirection.Value));
                        else
                            Collection.SortDescriptions.Add(new SortDescription(description.PropertyName, description.Direction));
                    }
                }
            }
            else
            {
                foreach (var column in dataGrid.Columns)
                    column.SortDirection = null;

                Collection.SortDescriptions.Clear();

                if (newSortDirection.HasValue)
                    Collection.SortDescriptions.Add(new SortDescription(columnName, newSortDirection.Value));
            }

            header.Column.SortDirection = newSortDirection;
            Collection.Refresh();
        }
        #endregion

        #region [__ReorderColumn]
        private void __ReorderColumn(DataGridColumnHeader header)
        {
            var mousePosition = Mouse.GetPosition(dataGrid);
            var displayIndex = __GetDisplayIndexForReorder(mousePosition.X);

            if (header.Column.DisplayIndex < displayIndex)
                displayIndex--;

            header.Column.DisplayIndex = displayIndex;
        }
        #endregion

        #region [__GroupByColumn]
        private void __GroupByColumn(DataGridColumnHeader header)
        {
            var columnName = header.Column.GetFieldName();

            if (!Collection.GroupDescriptions.Any(g => (g as PropertyGroupDescription)?.PropertyName == columnName))
            {
                Collection.GroupDescriptions.Add(new PropertyGroupDescription(columnName));
                Collection.Refresh();
                __RefreshGroupArea();
            }
        }
        #endregion

        #region - Event handler -

        #region [__DataGrid_Loaded]
        private void __DataGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (LayoutKey == null)
                return;

            DataGridUtils.LoadLayout(dataGrid, Collection, LayoutKey);

            if (CanGroup)
                __RefreshGroupArea();
            else
            {
                Collection.GroupDescriptions.Clear();
                Collection.Refresh();
            }
        }
        #endregion

        #region [__DataRow_DoubleClick]
        private void __DataRow_DoubleClick(object sender, RoutedEventArgs e)
        {
            if (RowDoubleClickCommand != null && RowDoubleClickCommand.CanExecute(e))
                RowDoubleClickCommand.Execute(e);
        }
        #endregion

        #region [__DataGrid_ColumnReordering]
        private void __DataGrid_ColumnReordering(object sender, DataGridColumnReorderingEventArgs e)
        {
            e.Cancel = true; // We have implemented column reordering on our own, so always cancel default behaviour.
        }
        #endregion

        #region [__Columns_CollectionChanged]
        private void __Columns_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (LeibitDataGridColumn column in e.OldItems)
                {
                    var gridColumn = dataGrid.Columns.FirstOrDefault(c => c.GetFieldName() == column.FieldName);

                    if (gridColumn != null)
                        dataGrid.Columns.Remove(gridColumn);
                }
            }

            if (e.NewItems != null)
            {
                foreach (LeibitDataGridColumn column in e.NewItems)
                {
                    var gridColumn = new DataGridTextColumn();
                    gridColumn.Header = column.Header;
                    gridColumn.Binding = new Binding(column.FieldName);
                    gridColumn.ElementStyle = new Style(typeof(TextBlock));
                    gridColumn.ElementStyle.Setters.Add(new Setter(MarginProperty, new Thickness(0, 5, 0, 5)));
                    dataGrid.Columns.Add(gridColumn);
                }
            }
        }
        #endregion

        #region - Drag & Drop -

        #region [__DataGridColumnHeader_MouseLeftButtonDown]
        private void __DataGridColumnHeader_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_DragStartPoint = e.GetPosition(this);
            e.Handled = false; // Do not handle this event. Column resizing should be possible.
        }
        #endregion

        #region [__DataGridColumnHeader_MouseMove]
        private void __DataGridColumnHeader_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed || !(e.Source is DataGridColumnHeader header) || __IsColumnResizing())
                return;

            e.Handled = true;

            if (!m_IsDragging)
            {
                var position = e.GetPosition(this);

                if (Math.Abs(m_DragStartPoint.X - position.X) > SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(m_DragStartPoint.Y - position.Y) > SystemParameters.MinimumVerticalDragDistance)
                {
                    m_IsDragging = true;
                    header.CaptureMouse();
                }
            }

            if (!m_IsDragging)
                return;

            var columnName = header.Column.GetFieldName();
            var mousePosition = Mouse.GetPosition(groupedColumnsArea);

            CanGroupColumn = mousePosition.X >= 0 && mousePosition.X < groupedColumnsArea.ActualWidth && mousePosition.Y >= 0 && mousePosition.Y < groupedColumnsArea.ActualHeight
                && !Collection.GroupDescriptions.Any(g => (g as PropertyGroupDescription)?.PropertyName == columnName);

            if (CanGroupColumn)
            {
                GroupingToolTipText = $"Nach '{header.Column.Header}' gruppieren";
                groupingToolTip.HorizontalOffset = mousePosition.X;
                groupingToolTip.VerticalOffset = mousePosition.Y;
                groupingToolTip.IsOpen = true;
            }
            else
                groupingToolTip.IsOpen = false;

            mousePosition = Mouse.GetPosition(dataGrid);
            CanReorderColumn = mousePosition.X >= 0 && mousePosition.X < dataGrid.ActualWidth && mousePosition.Y >= 0 && mousePosition.Y < dataGrid.ActualHeight;

            if (CanReorderColumn)
            {
                var point = __GetReorderTargetIndicatorPosition(mousePosition.X);
                reorderTargetIndicator.Arrange(new Rect(point, new Size(24, 24)));
                reorderTargetIndicator.Visibility = Visibility.Visible;
            }
            else
                reorderTargetIndicator.Visibility = Visibility.Hidden;

            if (CanGroupColumn)
                Mouse.OverrideCursor = Cursors.Hand;
            else if (CanReorderColumn)
                Mouse.OverrideCursor = Cursors.ScrollWE;
            else
                Mouse.OverrideCursor = Cursors.No;
        }
        #endregion

        #region [__DataGridColumnHeader_MouseLeftButtonUp]
        private void __DataGridColumnHeader_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (m_IsDoubleClick)
            {
                m_IsDoubleClick = false;
                return;
            }

            if (!(e.Source is DataGridColumnHeader header) || __IsColumnResizing())
                return;

            if (!m_IsDragging)
                __SortColumn(header);
            else if (CanReorderColumn)
                __ReorderColumn(header);
            else if (CanGroupColumn)
                __GroupByColumn(header);

            m_IsDragging = false;
            CanGroupColumn = false;
            CanReorderColumn = false;
            Mouse.OverrideCursor = null;
            reorderTargetIndicator.Visibility = Visibility.Hidden;
            groupingToolTip.IsOpen = false;
            header.ReleaseMouseCapture();
        }
        #endregion

        #region [__DataGridColumnHeader_MouseDoubleClick]
        private void __DataGridColumnHeader_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            m_IsDoubleClick = true;
        }
        #endregion

        #region [__GroupedColumn_MouseLeftButtonDown]
        private void __GroupedColumn_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            m_DragStartPoint = e.GetPosition(this);
        }
        #endregion

        #region [__GroupedColumn_MouseMove]
        private void __GroupedColumn_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            var element = e.Source as FrameworkElement;

            if (element == null)
                return;

            if (m_DraggingGroupedColumn == null)
            {
                var position = e.GetPosition(this);

                if (position != m_DragStartPoint)
                {
                    m_DraggingGroupedColumn = element.DataContext as LeibitDataGridColumn;
                    element.CaptureMouse();
                }
            }

            if (m_DraggingGroupedColumn == null)
                return;

            var mousePosition = Mouse.GetPosition(dataGrid);
            m_CanDropGroupedColumn = mousePosition.X >= 0 && mousePosition.X < dataGrid.ActualWidth && mousePosition.Y >= 0 && mousePosition.Y < dataGrid.ActualHeight;

            if (m_CanDropGroupedColumn)
            {
                Mouse.OverrideCursor = Cursors.Hand;
                UngroupingToolTipText = $"'{m_DraggingGroupedColumn.Header}' aus der Gruppierung entfernen";
                ungroupingToolTip.HorizontalOffset = mousePosition.X;
                ungroupingToolTip.VerticalOffset = mousePosition.Y;
                ungroupingToolTip.IsOpen = true;
            }
            else
            {
                Mouse.OverrideCursor = Cursors.No;
                ungroupingToolTip.IsOpen = false;
            }
        }
        #endregion

        #region [__GroupedColumn_MouseLeftButtonUp]
        private void __GroupedColumn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            try
            {
                if (!m_CanDropGroupedColumn || m_DraggingGroupedColumn == null)
                    return;

                var columnName = m_DraggingGroupedColumn.FieldName;
                var groupedColumn = Collection.GroupDescriptions.FirstOrDefault(g => (g as PropertyGroupDescription)?.PropertyName == columnName);

                if (groupedColumn != null)
                {
                    Collection.GroupDescriptions.Remove(groupedColumn);
                    Collection.Refresh();
                    __RefreshGroupArea();
                }
            }
            finally
            {
                m_DraggingGroupedColumn = null;
                m_CanDropGroupedColumn = false;
                Mouse.OverrideCursor = null;
                ungroupingToolTip.IsOpen = false;

                if (e.Source is FrameworkElement element)
                    element.ReleaseMouseCapture();
            }
        }
        #endregion

        #endregion

        #endregion

        #endregion

    }
}
