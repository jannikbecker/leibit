using Leibit.Client.WPF.ViewModels;
using Leibit.Entities.Scheduling;
using System;

namespace Leibit.Client.WPF.Windows.LocalOrders.ViewModels
{
    public class LocalOrdersViewModel : ChildWindowViewModelBase
    {

        #region - Ctor -
        public LocalOrdersViewModel(Schedule Schedule)
        {
            CurrentSchedule = Schedule;
        }
        #endregion

        #region - Properties -

        #region [CurrentSchedule]
        public Schedule CurrentSchedule
        {
            get;
            private set;
        }
        #endregion

        #region [Caption]
        public string Caption
        {
            get
            {
                return String.Format("Örtliche Anordnungen für Zug {0} ({1})", CurrentSchedule.Train.Number, CurrentSchedule.Station.Name);
            }
        }
        #endregion

        #region [LocalOrders]
        public string LocalOrders
        {
            get
            {
                return CurrentSchedule.LocalOrders;
            }
        }
        #endregion

        #endregion

    }
}
