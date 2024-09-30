using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Common;
using Leibit.Entities.Common;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class DisplayViewModel : ChildWindowViewModelBase, IRefreshable
    {

        #region - Ctor -
        public DisplayViewModel(Dispatcher dispatcher, Area area)
        {
            Dispatcher = dispatcher;
            StationList = area.ESTWs.Where(e => e.IsLoaded && e.SchedulesLoaded).SelectMany(e => e.Stations.Where(s => s.Tracks.Any(t => t.IsPlatform && t.Blocks.Any()))).ToObservableCollection();

            DisplayTypes = new ObservableCollection<DisplayType>
            {
                new DisplayType(eDisplayType.PlatformDisplay_Small, "Zugzielanzeiger klein", new PlatformDisplayViewModel(this), false),
                new DisplayType(eDisplayType.PlatformDisplay_Large, "Zugzielanzeiger groß", new PlatformDisplayViewModel(this), false),
                new DisplayType(eDisplayType.Countdown, "Zugzielanzeiger S-Bahn", new CountdownDisplayViewModel(this), false),
                new DisplayType(eDisplayType.DepartureBoard_Small, "Abfahrtstafel klein", new DepartureBoardViewModel(this), true),
                new DisplayType(eDisplayType.DepartureBoard_Large, "Abfahrtstafel groß", new DepartureBoardViewModel(this), true),
                new DisplayType(eDisplayType.PassengerInformation, "Fahrgastinformation", new PassengerInformationViewModel(this), true),
            };

            SelectedDisplayType = DisplayTypes[0];
            SelectedTracks = new ObservableCollection<Track>();
            SelectedTracks.CollectionChanged += __SelectedTracks_CollectionChanged;
        }
        #endregion

        #region - Properties -

        #region [Dispatcher]
        public Dispatcher Dispatcher { get; }
        #endregion

        #region [Caption]
        public string Caption
        {
            get
            {
                if (SelectedDisplayType == null)
                    return "Fahrgastinformation";

                var caption = SelectedDisplayType.Name;

                if (SelectedStation != null)
                    caption += $" {SelectedStation.Name}";

                if (SelectedTrack != null && !SelectedDisplayType.MultiTrack)
                    caption += $", Gleis {SelectedTrack.Name}";

                if (SelectedTracks.Any() && SelectedDisplayType.MultiTrack)
                    caption += $", Gleise {string.Join(", ", SelectedTracks.Select(t => t.Name))}";

                return caption;
            }
        }
        #endregion

        #region [DisplayTypes]
        public ObservableCollection<DisplayType> DisplayTypes
        {
            get => Get<ObservableCollection<DisplayType>>();
            private set => Set(value);
        }
        #endregion

        #region [SelectedDisplayType]
        public DisplayType SelectedDisplayType
        {
            get => Get<DisplayType>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(CanSelectMultipleTracks));
                OnPropertyChanged(nameof(Caption));
                __SelectionChanged();
            }
        }
        #endregion

        #region [StationList]
        public ObservableCollection<Station> StationList
        {
            get => Get<ObservableCollection<Station>>();
            private set => Set(value);
        }
        #endregion

        #region [TrackList]
        public ObservableCollection<Track> TrackList
        {
            get => Get<ObservableCollection<Track>>();
            private set => Set(value);
        }
        #endregion

        #region [IsTrackListEnabled]
        public bool IsTrackListEnabled => SelectedStation != null;
        #endregion

        #region [CanSelectMultipleTracks]
        public bool CanSelectMultipleTracks => SelectedDisplayType?.MultiTrack == true;
        #endregion

        #region [SelectedStation]
        public Station SelectedStation
        {
            get => Get<Station>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(IsTrackListEnabled));
                OnPropertyChanged(nameof(Caption));
                __SelectionChanged();

                if (value != null)
                    TrackList = value.Tracks.Where(x => x.IsPlatform && x.Blocks.Any()).ToObservableCollection();
            }
        }
        #endregion

        #region [SelectedTrack]
        public Track SelectedTrack
        {
            get => Get<Track>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(Caption));
                __SelectionChanged();
            }
        }
        #endregion

        #region [SelectedTracks]
        public ObservableCollection<Track> SelectedTracks
        {
            get => Get<ObservableCollection<Track>>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(Area area)
        {
            SelectedDisplayType?.ViewModel.Refresh(area);
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__SelectionChanged]
        private void __SelectionChanged()
        {
            SelectedDisplayType?.ViewModel.SelectionChanged();
        }
        #endregion

        #region [__SelectedTracks_CollectionChanged]
        private void __SelectedTracks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(Caption));
            __SelectionChanged();
        }
        #endregion

        #endregion
    }
}
