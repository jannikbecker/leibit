using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;

namespace Leibit.Controls
{
    public class MoveThumb : Thumb
    {
        public MoveThumb()
        {
            DragDelta += new DragDeltaEventHandler(this.MoveThumb_DragDelta);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var Rect = GetTemplateChild("MoveRectangle") as Rectangle;

            if (Rect != null && !Double.IsNaN(Rect.Height))
                Rect.VerticalAlignment = VerticalAlignment.Top;
        }

        private void MoveThumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control item = this.DataContext as Control;

            if (item != null)
            {
                double left = Canvas.GetLeft(item) + e.HorizontalChange;
                double top = Canvas.GetTop(item) + e.VerticalChange;

                double? MaxWidth = Double.IsInfinity(item.MaxWidth) ? null : (double?)item.MaxWidth;
                double? MaxHeight = Double.IsInfinity(item.MaxHeight) ? null : (double?)item.MaxHeight;

                if (left < 0)
                    Canvas.SetLeft(item, 0);
                else if (MaxWidth.HasValue && item.ActualWidth + left > MaxWidth)
                    Canvas.SetLeft(item, MaxWidth.Value - item.ActualWidth);
                else
                    Canvas.SetLeft(item, left);

                if (top < 0)
                    Canvas.SetTop(item, 0);
                else if (MaxHeight.HasValue && item.ActualHeight + top > MaxHeight)
                    Canvas.SetTop(item, MaxHeight.Value - item.ActualHeight);
                else
                    Canvas.SetTop(item, top);
            }
        }
    }

}
