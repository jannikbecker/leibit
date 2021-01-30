using Leibit.BLL;
using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Entities.LiveData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.DelayJustification.ViewModels
{
    public class DelayInfoViewModel : ViewModelBase
    {

        #region - Needs -
        private LiveDataBLL m_LiveDataBll;
        #endregion

        #region - Ctor -
        public DelayInfoViewModel(DelayInfo Delay)
            : base()
        {
            CurrentDelay = Delay;
            SaveCommand = new CommandHandler(__Save, false);
            m_LiveDataBll = new LiveDataBLL();
            AllDelayReasons = DelayReason.GetAllDelayReasons();
        }
        #endregion

        #region - Events -
        public event EventHandler DelaySaved;
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

        #region [ReasonNo]
        public int? ReasonNo
        {
            get => Get<int?>();
            set
            {
                var oldValue = ReasonNo;
                Set(value);

                if (value != null && value != oldValue)
                {
                    var reason = AllDelayReasons.FirstOrDefault(r => r.No == value);

                    if (reason != null)
                        SelectedReason = reason;
                    else
                        ReasonNo = oldValue;
                }
            }
        }
        #endregion

        #region [SelectedReason]
        public DelayReason SelectedReason
        {
            get => Get<DelayReason>();
            set
            {
                Set(value);

                if (value != null)
                    ReasonNo = value.No;

                (SaveCommand as CommandHandler).SetCanExecute(value != null);
            }
        }
        #endregion

        #region [AllDelayReasons]
        public List<DelayReason> AllDelayReasons { get; }
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

        #region [IsFirst]
        public bool IsFirst
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

        #region [SaveCommand]
        public ICommand SaveCommand { get; }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Save]
        private void __Save()
        {
            var SaveResult = m_LiveDataBll.JustifyDelay(CurrentDelay, SelectedReason.Text, CausedBy);

            if (SaveResult.Succeeded)
                DelaySaved?.Invoke(this, EventArgs.Empty);
            else
                MessageBox.Show(SaveResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #endregion

    }
}
