using System.Windows;
using System.Windows.Data;

namespace Leibit.Core.Client.Common
{
    public static class BindingExtensions
    {

        public static readonly DependencyProperty DummyProperty = DependencyProperty.RegisterAttached("Dummy", typeof(object), typeof(BindingExtensions), new PropertyMetadata(null));

        #region [Eval]
        public static object Eval(this Binding binding, object source)
        {
            var clone = new Binding();
            clone.Path = binding.Path;
            clone.Converter = binding.Converter;
            clone.Source = source;

            var dependencyObject = new DependencyObject();
            BindingOperations.SetBinding(dependencyObject, DummyProperty, clone);
            return dependencyObject.GetValue(DummyProperty);
        }
        #endregion

    }
}
