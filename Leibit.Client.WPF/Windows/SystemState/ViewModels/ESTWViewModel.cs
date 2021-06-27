using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Scheduling;
using Leibit.Entities.Common;

namespace Leibit.Client.WPF.Windows.SystemState.ViewModels
{
    public class ESTWViewModel : ViewModelBase
    {

        #region - Ctor -
        public ESTWViewModel(ESTW estw)
        {
            CurrentEstw = estw;
        }
        #endregion

        #region - Properties -

        #region [CurrentEstw]
        public ESTW CurrentEstw { get; private set; }
        #endregion

        #region [Name]
        public string Name => CurrentEstw.Name;
        #endregion

        #region [Time]
        public LeibitTime Time
        {
            get => Get<LeibitTime>();
            set => Set(value);
        }
        #endregion

        #region [IsLoadedAndHasSchedules]
        public bool IsLoadedAndHasSchedules
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [IsLoadedWithoutSchedules]
        public bool IsLoadedWithoutSchedules
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [IsInactive]
        public bool IsInactive
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #endregion

    }
}
