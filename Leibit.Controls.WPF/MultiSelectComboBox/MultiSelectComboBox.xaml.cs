using System.Collections;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaction logic for MultiSelectComboBox.xaml
    /// </summary>
    public partial class MultiSelectComboBox : UserControl, INotifyPropertyChanged
    {
        #region - Ctor -
        public MultiSelectComboBox()
        {
            InitializeComponent();
        }

        static MultiSelectComboBox()
        {
            var separatorTemplate = new DataTemplate();
            var textBlock = new FrameworkElementFactory(typeof(TextBlock));
            textBlock.SetValue(TextBlock.TextProperty, ", ");
            separatorTemplate.VisualTree = textBlock;

            ResultItemSeparatorTemplateProperty.OverrideMetadata(typeof(MultiSelectComboBox), new PropertyMetadata(separatorTemplate));
        }
        #endregion

        #region - Events -
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region - Dependency properties -
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IList), typeof(MultiSelectComboBox), new PropertyMetadata(null));
        public static readonly DependencyProperty SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(IList), typeof(MultiSelectComboBox), new PropertyMetadata(null));
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(MultiSelectComboBox), new PropertyMetadata(false));
        public static readonly DependencyProperty DropDownItemTemplateProperty = DependencyProperty.Register("DropDownItemTemplate", typeof(DataTemplate), typeof(MultiSelectComboBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ResultItemTemplateProperty = DependencyProperty.Register("ResultItemTemplate", typeof(DataTemplate), typeof(MultiSelectComboBox), new PropertyMetadata(null));
        public static readonly DependencyProperty ResultItemSeparatorTemplateProperty = DependencyProperty.Register("ResultItemSeparatorTemplate", typeof(DataTemplate), typeof(MultiSelectComboBox));
        public static readonly DependencyProperty EmptyTemplateProperty = DependencyProperty.Register("EmptyTemplate", typeof(DataTemplate), typeof(MultiSelectComboBox));
        public static readonly DependencyProperty ArrowFillProperty = DependencyProperty.Register("ArrowFill", typeof(Brush), typeof(MultiSelectComboBox), new PropertyMetadata(Brushes.Transparent));
        public static readonly DependencyProperty PopupStyleProperty = DependencyProperty.Register("PopupStyle", typeof(Style), typeof(MultiSelectComboBox), new PropertyMetadata(null));
        #endregion

        #region - Properties -

        #region [ItemsSource]
        public IList ItemsSource
        {
            get { return (IList)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

        #region [SelectedItems]
        public IList SelectedItems
        {
            get { return (IList)GetValue(SelectedItemsProperty); }
            set { SetValue(SelectedItemsProperty, value); }
        }
        #endregion

        #region [IsDropDownOpen]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
        #endregion

        #region [DropDownItemTemplate]
        public DataTemplate DropDownItemTemplate
        {
            get { return (DataTemplate)GetValue(DropDownItemTemplateProperty); }
            set { SetValue(DropDownItemTemplateProperty, value); }
        }
        #endregion

        #region [ResultItemTemplate]
        public DataTemplate ResultItemTemplate
        {
            get { return (DataTemplate)GetValue(ResultItemTemplateProperty); }
            set { SetValue(ResultItemTemplateProperty, value); }
        }
        #endregion

        #region [ResultItemTemplate]
        public DataTemplate ResultItemSeparatorTemplate
        {
            get { return (DataTemplate)GetValue(ResultItemSeparatorTemplateProperty); }
            set { SetValue(ResultItemSeparatorTemplateProperty, value); }
        }
        #endregion

        #region [EmptyTemplate]
        public DataTemplate EmptyTemplate
        {
            get { return (DataTemplate)GetValue(EmptyTemplateProperty); }
            set { SetValue(EmptyTemplateProperty, value); }
        }
        #endregion

        #region [ArrowFill]
        public Brush ArrowFill
        {
            get { return (Brush)GetValue(ArrowFillProperty); }
            set { SetValue(ArrowFillProperty, value); }
        }
        #endregion

        #region [PopupStyle]
        public Style PopupStyle
        {
            get { return (Style)GetValue(PopupStyleProperty); }
            set { SetValue(PopupStyleProperty, value); }
        }
        #endregion

        #region [IsEmptyTemplateVisible]
        public bool IsEmptyTemplateVisible
        {
            get => (SelectedItems?.Count ?? 0) == 0;
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

        #region [__SelectionChanged]
        private void __SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SelectedItems == null)
                return;

            if (e.RemovedItems != null)
                foreach (var item in e.RemovedItems)
                    SelectedItems.Remove(item);

            if (e.AddedItems != null)
            {
                foreach (var item in e.AddedItems)
                {
                    int indexToInsert;
                    for (indexToInsert = 0; indexToInsert < SelectedItems.Count && ItemsSource.IndexOf(item) > ItemsSource.IndexOf(SelectedItems[indexToInsert]); indexToInsert++) ;
                    SelectedItems.Insert(indexToInsert, item);
                }
            }

            __OnPropertyChanged(nameof(IsEmptyTemplateVisible));
        }
        #endregion

        #endregion
    }
}
