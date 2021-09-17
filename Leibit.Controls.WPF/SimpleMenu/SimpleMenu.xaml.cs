using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaction logic for SimpleMenu.xaml
    /// </summary>
    public partial class SimpleMenu : UserControl
    {

        #region - Ctor -
        public SimpleMenu()
        {
            InitializeComponent();
        }
        #endregion

        #region - Dependency properties -
        public static readonly DependencyProperty DisplayMemberPathProperty = DependencyProperty.Register("DisplayMemberPath", typeof(string), typeof(SimpleMenu), new PropertyMetadata(null));
        public static readonly DependencyProperty ValueMemberPathProperty = DependencyProperty.Register("ValueMemberPath", typeof(string), typeof(SimpleMenu), new PropertyMetadata(null));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(SimpleMenu), new PropertyMetadata(null));
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(IEnumerable), typeof(SimpleMenu), new PropertyMetadata(null));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(SimpleMenu), new PropertyMetadata(null));
        #endregion

        #region - Properties -

        #region [DisplayMemberPath]
        public string DisplayMemberPath
        {
            get { return (string)GetValue(DisplayMemberPathProperty); }
            set { SetValue(DisplayMemberPathProperty, value); }
        }
        #endregion

        #region [ValueMemberPath]
        public string ValueMemberPath
        {
            get { return (string)GetValue(ValueMemberPathProperty); }
            set { SetValue(ValueMemberPathProperty, value); }
        }
        #endregion

        #region [Command]
        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        #endregion

        #region [ItemsSource]
        public IEnumerable ItemsSource
        {
            get { return (IEnumerable)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        #endregion

        #region [ButtonStyle]
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        #endregion

        #endregion

        #region - Private helpers -

        #region [__MenuButton_Loaded]
        private void __MenuButton_Loaded(object sender, RoutedEventArgs e)
        {
            var Button = sender as Button;

            if (Button == null)
                return;

            if (DisplayMemberPath == null)
            {
                Button.Content = Button.DataContext;
            }
            else
            {
                var DisplayMemberPathBinding = new Binding(DisplayMemberPath);
                Button.SetBinding(Button.ContentProperty, DisplayMemberPathBinding);
            }

            if (ValueMemberPath == null)
            {
                Button.CommandParameter = Button.DataContext;
            }
            else
            {
                var CommandParameterBinding = new Binding(ValueMemberPath);
                Button.SetBinding(Button.CommandParameterProperty, CommandParameterBinding);
            }
        }
        #endregion

        #endregion

    }
}
