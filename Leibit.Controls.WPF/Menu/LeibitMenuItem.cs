using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Leibit.Controls
{
    public class LeibitMenuItem : MenuItem
    {

        #region [SubMenuBackground]
        public Brush SubMenuBackground
        {
            get { return (Brush)GetValue(SubMenuBackgroundProperty); }
            set { SetValue(SubMenuBackgroundProperty, value); }
        }

        public static readonly DependencyProperty SubMenuBackgroundProperty = DependencyProperty.Register("SubMenuBackground", typeof(Brush), typeof(LeibitMenuItem), new PropertyMetadata(Brushes.Gray));
        #endregion

        #region [OnApplyTemplate]
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            var subMenuBorder = GetTemplateChild("SubMenuBorder") as Border;

            if (subMenuBorder != null)
            {
                var binding = new Binding(nameof(SubMenuBackground));
                binding.Source = this;
                subMenuBorder.SetBinding(Border.BackgroundProperty, binding);
            }
        }
        #endregion

    }
}
