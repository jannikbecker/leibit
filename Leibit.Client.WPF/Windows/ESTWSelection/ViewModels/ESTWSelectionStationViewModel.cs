using Leibit.Core.Client.BaseClasses;
using Leibit.Entities.Common;
using System;

namespace Leibit.Client.WPF.Windows.ESTWSelection.ViewModels
{
    public class ESTWSelectionStationViewModel : ViewModelBase
    {

        #region - Needs -
        private Station m_Station;
        #endregion

        #region - Ctor -
        public ESTWSelectionStationViewModel(Station Station)
            : base()
        {
            if (Station == null)
                throw new ArgumentNullException("Station must not be null");

            m_Station = Station;
        }
        #endregion

        #region - Properties -

        #region [CurrentStation]
        public Station CurrentStation
        {
            get
            {
                return m_Station;
            }
        }
        #endregion

        #region [Name]
        public string Name
        {
            get
            {
                return m_Station.Name;
            }
        }
        #endregion

        #region [ShortSymbol]
        internal string ShortSymbol
        {
            get
            {
                return m_Station.ShortSymbol;
            }
        }
        #endregion

        #region [IsSelected]
        public bool IsSelected
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
