using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Leibit.Controls
{
    /// <summary>
    /// Interaktionslogik für ImageButton.xaml
    /// </summary>
    public partial class ImageButton : UserControl
    {

        #region - Ctor -
        public ImageButton()
        {
            InitializeComponent();
            //Width = 20;
            //Height = 20;
        }

        static ImageButton()
        {
            WidthProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(20.0));
            HeightProperty.OverrideMetadata(typeof(ImageButton), new FrameworkPropertyMetadata(20.0));
        }
        #endregion

        #region - Dependency properties -
        public static readonly DependencyProperty ImageProperty = DependencyProperty.Register("Image", typeof(ImageSource), typeof(ImageButton));
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ImageButton));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(ImageButton));
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

        #endregion

    }
}
