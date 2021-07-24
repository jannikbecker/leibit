using Leibit.Client.WPF.Common;
using Leibit.Core.Client.BaseClasses;
using Leibit.Entities.Common;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Leibit.Client.WPF.Windows.ESTWSelection.ViewModels
{
    public class ESTWSelectionESTWViewModel : ViewModelBase
    {

        #region - Needs -
        private ESTW m_Estw;
        #endregion

        #region - Ctor -
        public ESTWSelectionESTWViewModel(ESTW Estw)
            : base()
        {
            if (Estw == null)
                throw new ArgumentNullException("Estw must not be null");

            m_Estw = Estw;
            __InitializeStations();
        }
        #endregion

        #region - Properties -

        #region [Id]
        public string Id
        {
            get
            {
                return m_Estw.Id;
            }
        }
        #endregion

        #region [Name]
        public string Name
        {
            get
            {
                return m_Estw.Name;
            }
        }
        #endregion

        #region [Stations]
        public ObservableCollection<ESTWSelectionStationViewModel> Stations
        {
            get
            {
                return Get<ObservableCollection<ESTWSelectionStationViewModel>>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [IsSelected]
        public bool? IsSelected
        {
            get
            {
                return Get<bool?>();
            }
            set
            {
                var OldValue = IsSelected;

                Set(value);

                if (value.HasValue && value.Value != OldValue)
                {
                    if (value.Value)
                        foreach (var Station in Stations)
                            Station.IsSelected = true;
                    else
                        foreach (var Station in Stations)
                            Station.IsSelected = false;
                }
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        private void __InitializeStations()
        {
            if (Stations == null)
                Stations = new ObservableCollection<ESTWSelectionStationViewModel>();
            else
                Stations.Clear();

            foreach (var Station in m_Estw.Stations)
            {
                if (!Station.HasScheduleFile)
                    continue;

                var VM = new ESTWSelectionStationViewModel(Station);
                VM.IsSelected = Runtime.VisibleStations.Contains(Station);
                VM.PropertyChanged += __Station_PropertyChanged;
                Stations.Add(VM);
            }

            __RefreshSelection();
        }

        private void __Station_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var SenderVM = sender as ESTWSelectionStationViewModel;

            if (SenderVM == null)
                return;

            if (e.PropertyName == "IsSelected")
            {
                if (SenderVM.IsSelected && !Runtime.VisibleStations.Contains(SenderVM.CurrentStation))
                    Runtime.VisibleStations.Add(SenderVM.CurrentStation);
                else if (!SenderVM.IsSelected)
                    Runtime.VisibleStations.Remove(SenderVM.CurrentStation);

                __RefreshSelection();
            }
        }

        private void __RefreshSelection()
        {
            bool AnySelected = Stations.Any(station => station.IsSelected);
            bool AnyNotSelected = Stations.Any(station => !station.IsSelected);

            if (AnySelected && AnyNotSelected)
                IsSelected = null;
            else if (AnySelected)
                IsSelected = true;
            else
                IsSelected = false;
        }

        #endregion

    }
}
