using Leibit.Entities.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Leibit.Client.WPF.Common
{
    internal static class Runtime
    {

        #region - Needs -
        private static ObservableCollection<Station> m_VisibleStations;
        #endregion

        #region - Ctor -
        static Runtime()
        {
            VisibleStations = new ObservableCollection<Station>();
            VisibleTrains = new List<VisibleTrainInfo>();
            HiddenSchedules = new List<HiddenScheduleInfo>();
        }
        #endregion

        #region - Events -
        internal static event Action VisibleStationsChanged;
        #endregion

        #region - Properties -

        #region [VisibleStations]
        internal static ObservableCollection<Station> VisibleStations
        {
            get
            {
                return m_VisibleStations;
            }
            private set
            {
                if (m_VisibleStations != null)
                    m_VisibleStations.CollectionChanged -= OnVisibleStationsChanged;

                m_VisibleStations = value;
                OnVisibleStationsChanged();

                if (m_VisibleStations != null)
                    m_VisibleStations.CollectionChanged += OnVisibleStationsChanged;
            }
        }
        #endregion

        #region [VisibleTrains]
        internal static List<VisibleTrainInfo> VisibleTrains { get; }
        #endregion

        #region [HiddenSchedules]
        internal static List<HiddenScheduleInfo> HiddenSchedules { get; }
        #endregion

        #endregion

        #region - Private helpers -

        #region [OnVisibleStationsChanged]
        private static void OnVisibleStationsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnVisibleStationsChanged();
        }

        private static void OnVisibleStationsChanged()
        {
            if (VisibleStationsChanged != null)
                VisibleStationsChanged();
        }
        #endregion

        #endregion

    }
}
