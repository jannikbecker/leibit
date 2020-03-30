using Leibit.Core.Client.BaseClasses;
using Leibit.Entities.LiveData;
using System;

namespace Leibit.Client.WPF.Windows.DelayJustification.ViewModels
{
    public class DelayInfoViewModel : ViewModelBase
    {

        #region - Ctor -
        public DelayInfoViewModel(DelayInfo Delay)
            : base()
        {
            CurrentDelay = Delay;
        }
        #endregion

        #region - Properties -

        #region [CurrentDelay]
        public DelayInfo CurrentDelay
        {
            get;
            private set;
        }
        #endregion

        #region [StationName]
        public string StationName
        {
            get
            {
                if (CurrentDelay.Schedule == null || CurrentDelay.Schedule.Schedule == null || CurrentDelay.Schedule.Schedule.Station == null)
                    return null;

                return CurrentDelay.Schedule.Schedule.Station.Name;
            }
        }
        #endregion

        #region [MinutesString]
        public string MinutesString
        {
            get
            {
                return CurrentDelay.Minutes > 0 ? String.Format("+{0}", CurrentDelay.Minutes) : CurrentDelay.Minutes.ToString();
            }
        }
        #endregion

        #region [Reason]
        public string Reason
        {
            get
            {
                return Get<string>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [CausedBy]
        public int? CausedBy
        {
            get
            {
                return Get<int?>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsLast]
        public bool IsLast
        {
            get
            {
                return Get<bool>();
            }
            set
            {
                Set(value);
            }
        }
        #endregion

        #endregion

    }
}
