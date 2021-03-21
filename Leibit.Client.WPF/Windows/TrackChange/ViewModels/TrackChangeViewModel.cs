using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.TrackChange.ViewModels
{
    public class TrackChangeViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private TrainInformation m_Train;
        private LiveDataBLL m_LiveDataBll;
        #endregion

        #region - Ctor -
        public TrackChangeViewModel(TrainInformation train, Schedule schedule)
        {
            m_Train = train;
            SaveCommand = new CommandHandler(__Save, false);
            m_LiveDataBll = new LiveDataBLL();

            var candidates = train.Schedules.Where((s1, i1) => !train.Schedules.Skip(i1 + 1).Any(s2 => s2.IsArrived)) // Must be first criterion due to indizes
                                            .Where(s => !s.IsArrived && (s.Schedule.Track == null || s.Schedule.Track.IsPlatform))
                                            .Where(s => s.Schedule.Station.ESTW.Stations.Any(s2 => Runtime.VisibleStations.Contains(s2)))
                                            .GroupBy(s => new { s.Schedule.Station.ShortSymbol, s.Schedule.Time })
                                            .Select(g => g.FirstOrDefault());

            Schedules = new ObservableCollection<LiveSchedule>(candidates);
            SelectedSchedule = Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == schedule.Station.ShortSymbol && s.Schedule.Time == schedule.Time);
            SelectedTrack = SelectedSchedule?.LiveTrack;
        }
        #endregion

        #region - Properties -

        #region [Caption]
        public string Caption => $"Gleiswechsel {m_Train.Train.Number}";
        #endregion

        #region [Schedules]
        public ObservableCollection<LiveSchedule> Schedules
        {
            get => Get<ObservableCollection<LiveSchedule>>();
            private set => Set(value);
        }
        #endregion

        #region [SelectedSchedule]
        public LiveSchedule SelectedSchedule
        {
            get => Get<LiveSchedule>();
            set
            {
                Set(value);
                __RefreshTracks();
            }
        }
        #endregion

        #region [Tracks]
        public ObservableCollection<Track> Tracks
        {
            get => Get<ObservableCollection<Track>>();
            private set => Set(value);
        }
        #endregion

        #region [SelectedTrack]
        public Track SelectedTrack
        {
            get => Get<Track>();
            set
            {
                Set(value);
                SaveCommand.SetCanExecute(value != null);
            }
        }
        #endregion

        #region [SaveCommand]
        public CommandHandler SaveCommand { get; }
        #endregion

        #endregion

        #region - Private methods -

        private void __RefreshTracks()
        {
            if (SelectedSchedule == null)
                Tracks = new ObservableCollection<Track>();
            else if (SelectedSchedule.Schedule.Track == null || (!SelectedSchedule.Schedule.Track.Alternatives.Any() && !SelectedSchedule.Schedule.Track.Parent.Alternatives.Any()))
                // No alternative tracks configured -> Show all tracks
                Tracks = new ObservableCollection<Track>(SelectedSchedule.Schedule.Station.Tracks);
            else
            {
                // We need to evaluate the alternatives...
                Tracks = new ObservableCollection<Track>();

                foreach (var track in SelectedSchedule.Schedule.Station.Tracks)
                {
                    // Scheduled track is 1. Add track 1 to list
                    if (track.Name.Equals(SelectedSchedule.Schedule.Track.Name, StringComparison.InvariantCultureIgnoreCase))
                        Tracks.Add(track);

                    // Scheduled track is 1. Add tracks 1A and 1B to list
                    else if (SelectedSchedule.Schedule.Track.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase))
                        Tracks.Add(track);

                    // Scheduled track is 1. Add tracks 2, 3, ... to list
                    else if (SelectedSchedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase)))
                        Tracks.Add(track);

                    // Scheduled track is 1. Add tracks 2A, 2B, 3A, 3B, ... to list
                    else if (SelectedSchedule.Schedule.Track.Alternatives.Any(a => a.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase)))
                        Tracks.Add(track);

                    // Scheduled track is 1A. Add track 1 to list
                    else if (SelectedSchedule.Schedule.Track.Parent.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase))
                        Tracks.Add(track);

                    // Scheduled track is 1A. Add track 1B to list
                    else if (SelectedSchedule.Schedule.Track.Parent.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase))
                        Tracks.Add(track);

                    // Scheduled track is 1A. Add tracks 2, 3, ... to list
                    else if (SelectedSchedule.Schedule.Track.Parent.Alternatives.Any(a => a.Name.Equals(track.Name, StringComparison.InvariantCultureIgnoreCase)))
                        Tracks.Add(track);

                    // Scheduled track is 1A. Add tracks 2A, 2B, 3A, 3B, ... to list
                    else if (SelectedSchedule.Schedule.Track.Parent.Alternatives.Any(a => a.Name.Equals(track.Parent.Name, StringComparison.InvariantCultureIgnoreCase)))
                        Tracks.Add(track);
                }
            }
        }

        #region [__Save]
        private void __Save()
        {
            if (SelectedSchedule == null || SelectedTrack == null)
                return;

            var result = m_LiveDataBll.ChangeTrack(SelectedSchedule, SelectedTrack);

            if (result.Succeeded)
            {
                OnStatusBarTextChanged($"Gleiswechsel für Zug {m_Train.Train.Number} in {SelectedSchedule.Schedule.Station.ShortSymbol} eingetragen");
                OnRefresh();
                OnCloseWindow();
            }
            else
                MessageBox.Show(result.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #endregion

    }
}
