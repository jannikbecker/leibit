using Leibit.Core.Client.BaseClasses;
using LiveChartsCore.Painting;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp.Views.WPF;
using System.Windows.Media;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class PieDataViewModel<T> : ViewModelBase
    {
        #region - Ctor -
        public PieDataViewModel(string caption, Color fill)
        {
            Caption = caption;
            SetFillColor(fill);
        }

        public PieDataViewModel(string caption, Color fill, T value)
            : this(caption, fill)
        {
            Value = value;
        }
        #endregion

        #region - Properties -

        #region [Caption]
        public string Caption
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

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
