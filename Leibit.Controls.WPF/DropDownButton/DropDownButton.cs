using Leibit.Core.Client.Commands;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaction logic for DropDownButton.xaml
    /// </summary>
    public class DropDownButton : UserControl
    {

        #region - Needs -
        private CommandHandler m_ToggleMenuCommand;
        #endregion

        #region - Ctor -
        public DropDownButton()
        {
            m_ToggleMenuCommand = new CommandHandler(__ToggleMenu, IsEnabled);
            IsEnabledChanged += (s, e) => m_ToggleMenuCommand.SetCanExecute(IsEnabled);
        }
        #endregion

        #region - Dependency properties -
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(DropDownButton));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(DropDownButton));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(DropDownButton));
        public static readonly DependencyProperty DropDownContentProperty = DependencyProperty.Register("DropDownContent", typeof(object), typeof(DropDownButton), new PropertyMetadata(null));
        public static readonly DependencyProperty IsDropDownOpenProperty = DependencyProperty.Register("IsDropDownOpen", typeof(bool), typeof(DropDownButton), new PropertyMetadata(false));
        public static readonly DependencyProperty ButtonStyleProperty = DependencyProperty.Register("ButtonStyle", typeof(Style), typeof(DropDownButton), new PropertyMetadata(null));
        public static readonly DependencyProperty ArrowStyleProperty = DependencyProperty.Register("ArrowStyle", typeof(Style), typeof(DropDownButton), new PropertyMetadata(null));
        public static readonly DependencyProperty ArrowFillProperty = DependencyProperty.Register("ArrowFill", typeof(Brush), typeof(DropDownButton), new PropertyMetadata(Brushes.Transparent));
        #endregion

        #region - Properties -

        #region [Image]
        public ImageSource Image
        {
            get
            {
                return (ImageSource)GetValue(ImageProperty);
            }
            set
            {
                SetValue(ImageProperty, value);
            }
        }
        #endregion

        #region [Command]
        public ICommand Command
        {
            get
            {
                return (ICommand)GetValue(CommandProperty);
            }
            set
            {
                SetValue(CommandProperty, value);
            }
        }
        #endregion

        #region [CommandParameter]
        public object CommandParameter
        {
            get
            {
                return (ICommand)GetValue(CommandParameterProperty);
            }
            set
            {
                SetValue(CommandParameterProperty, value);
            }
        }
        #endregion

        #region [DropDownContent]
        public object DropDownContent
        {
            get { return (object)GetValue(DropDownContentProperty); }
            set { SetValue(DropDownContentProperty, value); }
        }
        #endregion

        #region [ToggleMenuCommand]
        public ICommand ToggleMenuCommand
        {
            get
            {
                return m_ToggleMenuCommand;
            }
        }
        #endregion

        #region [IsDropDownOpen]
        public bool IsDropDownOpen
        {
            get { return (bool)GetValue(IsDropDownOpenProperty); }
            set { SetValue(IsDropDownOpenProperty, value); }
        }
        #endregion

        #region [ButtonStyle]
        public Style ButtonStyle
        {
            get { return (Style)GetValue(ButtonStyleProperty); }
            set { SetValue(ButtonStyleProperty, value); }
        }
        #endregion

        #region [ArrowStyle]
        public Style ArrowStyle
        {
            get { return (Style)GetValue(ArrowStyleProperty); }
            set { SetValue(ArrowStyleProperty, value); }
        }
        #endregion

        #region [ArrowFill]
        public Brush ArrowFill
        {
            get { return (Brush)GetValue(ArrowFillProperty); }
            set { SetValue(ArrowFillProperty, value); }
        }
        #endregion

        #endregion

        #region - Private helpers -

        #region [__ToggleMenu]
        private void __ToggleMenu()
        {
            IsDropDownOpen = !IsDropDownOpen;
        }
        #endregion

        #endregion

    }
}
