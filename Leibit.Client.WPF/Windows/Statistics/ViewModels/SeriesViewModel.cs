using Leibit.Core.Client.BaseClasses;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp.Views.WPF;
using System.Windows.Media;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class SeriesViewModel<T> : ViewModelBase
    {
        #region - Ctor -
        public SeriesViewModel(Color fill)
        {
            SetFillColor(fill);
        }

        public SeriesViewModel(Color fill, T value)
            : this(fill)
        {
            Value = value;
        }
        #endregion

        #region - Properties -

        #region [Value]
        public T Value
        {
            get => Get<T>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(Values));
            }
        }
        #endregion

        #region [Values]
        public T[] Values
        {
            get => [Value];
        }
        #endregion

        #region [Fill]
        public Paint Fill
        {
            get => Get<Paint>();
            set => Set(value);
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [SetFillColor]
        public void SetFillColor(Color fillColor)
        {
            Fill = new SolidColorPaint(fillColor.ToSKColor());
        }
        #endregion

        #endregion
    }
}
