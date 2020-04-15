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
                var currentLeft = Canvas.GetLeft(item);
                var currentTop = Canvas.GetTop(item);
                var horizontalDelta = e.HorizontalChange;
                var verticalDelta = e.VerticalChange;

                double? MaxWidth = Double.IsInfinity(item.MaxWidth) ? null : (double?)item.MaxWidth;
                double? MaxHeight = Double.IsInfinity(item.MaxHeight) ? null : (double?)item.MaxHeight;

                if (currentLeft + horizontalDelta < 0)
                    horizontalDelta = -currentLeft;
                else if (MaxWidth.HasValue && item.ActualWidth + horizontalDelta > MaxWidth.Value)
                    horizontalDelta = MaxWidth.Value - item.ActualWidth;

                Canvas.SetLeft(item, currentLeft + horizontalDelta);

                if (currentTop + verticalDelta < 0)
                    verticalDelta = -currentTop;
                else if (MaxHeight.HasValue && item.ActualHeight + verticalDelta > MaxHeight.Value)
                    verticalDelta = MaxHeight.Value - item.ActualHeight;

                Canvas.SetTop(item, currentTop + verticalDelta);

                if (MaxWidth.HasValue)
                    item.MaxWidth -= horizontalDelta;
                if (MaxHeight.HasValue)
                    item.MaxHeight -= verticalDelta;
            }
        }
    }

}
