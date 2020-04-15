using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Leibit.Controls
{
    internal class ResizeThumb : Thumb
    {

        #region - Ctor -
        public ResizeThumb()
        {
            DragDelta += __DragDelta;
        }

        static ResizeThumb()
        {
            OpacityProperty.OverrideMetadata(typeof(ResizeThumb), new FrameworkPropertyMetadata(0.0));
        }
        #endregion

        #region - Private methods -

        private void __DragDelta(object sender, DragDeltaEventArgs e)
        {
            Control item = this.DataContext as Control;

            if (item != null)
            {
                double deltaVertical, deltaHorizontal;

                double ActualWidth = Double.IsNaN(item.Width) ? item.ActualWidth : item.Width;
                double ActualHeight = Double.IsNaN(item.Height) ? item.ActualHeight : item.Height;

                double? MinWidth = Double.IsInfinity(item.MinWidth) ? null : (double?)item.MinWidth;
                double? MinHeight = Double.IsInfinity(item.MinHeight) ? null : (double?)item.MinHeight;
                double? MaxWidth = Double.IsInfinity(item.MaxWidth) ? null : (double?)item.MaxWidth;
                double? MaxHeight = Double.IsInfinity(item.MaxHeight) ? null : (double?)item.MaxHeight;

                var Top = Canvas.GetTop(item);
                var Left = Canvas.GetLeft(item);

                double NewWidth, NewHeight;

                switch (VerticalAlignment)
                {
                    case VerticalAlignment.Bottom:
                        deltaVertical = Math.Min(-e.VerticalChange, item.ActualHeight - item.MinHeight);
                        NewHeight = ActualHeight - deltaVertical;

                        if (MaxHeight.HasValue && NewHeight > MaxHeight)
                            NewHeight = MaxHeight.Value;

                        if (MinHeight.HasValue && NewHeight < MinHeight)
                            NewHeight = MinHeight.Value;

                        item.Height = NewHeight;
                        break;

                    case VerticalAlignment.Top:
                        deltaVertical = Math.Min(e.VerticalChange, item.ActualHeight - item.MinHeight);
                        var NewTop = Top + deltaVertical;
                        NewHeight = ActualHeight - deltaVertical;

                        if (NewTop < 0)
                        {
                            NewHeight += NewTop;
                            NewTop = 0;
                        }

                        if (MinHeight.HasValue && NewHeight < MinHeight)
                            NewHeight = MinHeight.Value;

                        Canvas.SetTop(item, NewTop);
                        item.Height = NewHeight;
                        item.MaxHeight += Top - NewTop;
                        break;

                    default:
                        break;
                }

                switch (HorizontalAlignment)
                {
                    case HorizontalAlignment.Left:
                        deltaHorizontal = Math.Min(e.HorizontalChange, item.ActualWidth - item.MinWidth);
                        var NewLeft = Left + deltaHorizontal;
                        NewWidth = ActualWidth - deltaHorizontal;

                        if (NewLeft < 0)
                        {
                            NewWidth += NewLeft;
                            NewLeft = 0;
                        }

                        if (MinWidth.HasValue && NewWidth < MinWidth)
                            NewWidth = MinWidth.Value;

                        Canvas.SetLeft(item, NewLeft);
                        item.Width = NewWidth;
                        item.MaxWidth += Left - NewLeft;
                        break;

                    case HorizontalAlignment.Right:
                        deltaHorizontal = Math.Min(-e.HorizontalChange, item.ActualWidth - item.MinWidth);
                        NewWidth = ActualWidth - deltaHorizontal;

                        if (MaxWidth.HasValue && NewWidth > MaxWidth)
                            NewWidth = MaxWidth.Value;

                        if (MinWidth.HasValue && NewWidth < MinWidth)
                            NewWidth = MinWidth.Value;

                        item.Width = NewWidth;
                        break;

                    default:
                        break;
                }
            }

            e.Handled = true;
        }

        #endregion

    }

}
