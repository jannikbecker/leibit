using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Leibit.Core.Client.Common
{
    public class ComboBoxTemplateSelector : DataTemplateSelector
    {

        public DataTemplate SelectedItemTemplate { get; set; }
        public DataTemplate DropDownTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var itemToCheck = container;

            while (itemToCheck != null && !(itemToCheck is ComboBoxItem) && !(itemToCheck is ComboBox))
                itemToCheck = VisualTreeHelper.GetParent(itemToCheck);

            return itemToCheck is ComboBoxItem ? DropDownTemplate : SelectedItemTemplate;
        }

    }
}
