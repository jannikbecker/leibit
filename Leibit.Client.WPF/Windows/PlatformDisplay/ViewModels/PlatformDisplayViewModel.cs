using Leibit.BLL;
using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Common;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.PlatformDisplay.ViewModels
{
    public class PlatformDisplayViewModel : ChildWindowViewModelBase, IRefreshable
    {

        #region - Needs -
        private CalculationBLL m_CalculationBll;
        #endregion

        #region - Const -
        private const string LED_SEPARATOR = "          ###          ";
        #endregion

        #region - Ctor -
        public PlatformDisplayViewModel(Dispatcher dispatcher, Area area)
        {
            Dispatcher = dispatcher;
            m_CalculationBll = new CalculationBLL();
            StationList = area.ESTWs.Where(e => e.IsLoaded && e.SchedulesLoaded).SelectMany(e => e.Stations.Where(s => s.Tracks.Any(t => t.IsPlatform))).ToObservableCollection();
            SelectedType = 0;
        }
        #endregion

        #region - Properties -

        #region [Dispatcher]
        public Dispatcher Dispatcher { get; }
        #endregion

        #region [SuppressSlide]
        private bool SuppressSlide
        {
            get => Get<bool>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(IsCurrentTrainInfoMarquee));
                OnPropertyChanged(nameof(IsLEDSliding));
            }
        }
        #endregion

        #region [Caption]
        public string Caption
        {
            get
            {
                var caption = "Zugzielanzeiger";

                if (SelectedStation != null && SelectedTrack != null)
                    caption += $" {SelectedStation.Name}, Gleis {SelectedTrack.Name}";

                return caption;
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

        #region [SelectedStation]
        public Station SelectedStation
        {
            get => Get<Station>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(IsTrackListEnabled));
                OnPropertyChanged(nameof(Caption));

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
                OnPropertyChanged(nameof(TrackName));
                OnPropertyChanged(nameof(SubTrackName));
                OnPropertyChanged(nameof(Caption));
                SuppressSlide = true;
            }
        }
        #endregion

        #region [SelectedType]
        public int SelectedType
        {
            get => Get<int>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(LCDVisibility));
                OnPropertyChanged(nameof(LEDVisibility));
                OnPropertyChanged(nameof(IsLEDSliding));
            }
        }
        #endregion

        #region [LCDVisibility]
        public Visibility LCDVisibility => SelectedType == 0 ? Visibility.Visible : Visibility.Collapsed;
        #endregion

        #region [LEDVisibility]
        public Visibility LEDVisibility => SelectedType == 1 ? Visibility.Visible : Visibility.Collapsed;
        #endregion

        #region [TrackName]
        public string TrackName
        {
            get
            {
                if (SelectedTrack == null)
                    return string.Empty;

                if (SelectedTrack.DisplayName.IsNullOrWhiteSpace())
                    return SelectedTrack.Name;

                return SelectedTrack.DisplayName;
            }
        }
        #endregion

        #region [SubTrackName]
        public string SubTrackName => SelectedTrack?.DisplaySubName;
        #endregion

        #region [CurrentTrainInfo]
        public string CurrentTrainInfo
        {
            get => Get<string>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(CurrentTrainInfoVisibility));
            }
        }
        #endregion

        #region [CurrentTrainInfoVisibility]
        public Visibility CurrentTrainInfoVisibility => CurrentTrainInfo.IsNullOrWhiteSpace() ? Visibility.Collapsed : Visibility.Visible;
        #endregion

        #region [IsCurrentTrainInfoMarquee]
        public bool IsCurrentTrainInfoMarquee
        {
            get => Get<bool>() && !SuppressSlide;
            private set => Set(value);
        }
        #endregion

        #region [Via]
        public string Via
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [CurrentTrainDestination]
        public string CurrentTrainDestination
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [CurrentTrainTime]
        public string CurrentTrainTime
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [CurrentTrainNumber]
        public string CurrentTrainNumber
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [FollowingTrain1Time]
        public string FollowingTrain1Time
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [FollowingTrain2Time]
        public string FollowingTrain2Time
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [FollowingTrain1Delay]
        public string FollowingTrain1Delay
        {
            get => Get<string>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(FollowingTrain1DelayVisibility));
            }
        }
        #endregion

        #region [FollowingTrain2Delay]
        public string FollowingTrain2Delay
        {
            get => Get<string>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(FollowingTrain2DelayVisibility));
            }
        }
        #endregion

        #region [FollowingTrain1DelayVisibility]
        public Visibility FollowingTrain1DelayVisibility => FollowingTrain1Delay.IsNullOrWhiteSpace() ? Visibility.Collapsed : Visibility.Visible;
        #endregion

        #region [FollowingTrain2DelayVisibility]
        public Visibility FollowingTrain2DelayVisibility => FollowingTrain2Delay.IsNullOrWhiteSpace() ? Visibility.Collapsed : Visibility.Visible;
        #endregion

        #region [FollowingTrain1Number]
        public string FollowingTrain1Number
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [FollowingTrain2Number]
        public string FollowingTrain2Number
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [FollowingTrain1Destination]
        public string FollowingTrain1Destination
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [FollowingTrain2Destination]
        public string FollowingTrain2Destination
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [FollowingTrain1Info]
        public string FollowingTrain1Info
        {
            get => Get<string>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(FollowingTrain1InfoVisibility));
            }
        }
        #endregion

        #region [FollowingTrain2Info]
        public string FollowingTrain2Info
        {
            get => Get<string>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(FollowingTrain2InfoVisibility));
            }
        }
        #endregion

        #region [FollowingTrain1InfoVisibility]
        public Visibility FollowingTrain1InfoVisibility => FollowingTrain1Info.IsNullOrWhiteSpace() ? Visibility.Collapsed : Visibility.Visible;
        #endregion

        #region [FollowingTrain2InfoVisibility]
        public Visibility FollowingTrain2InfoVisibility => FollowingTrain2Info.IsNullOrWhiteSpace() ? Visibility.Collapsed : Visibility.Visible;
        #endregion

        #region [LEDText]
        public string LEDText
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [IsLEDSliding]
        public bool IsLEDSliding
        {
            get => SelectedType == 1 && !SuppressSlide;
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(Area area)
        {
            var wasSlideSuppressed = SuppressSlide;

            if (SelectedStation == null || SelectedTrack == null)
                __ClearAll();
            else if (SelectedType == 0)
                __GenerateLCD(area);
            else if (SelectedType == 1)
                __GenerateLED(area);

            if (wasSlideSuppressed)
                SuppressSlide = false;
        }
        #endregion

        #region [__GenerateLCD]
        private void __GenerateLCD(Area area)
        {
            var candidates = __GetScheduleCandidates(area, 120);
            var orderedSchedules = candidates.OrderBy(x => x.ReferenceTime);
            var currentItem = orderedSchedules.FirstOrDefault();

            if (currentItem != null)
            {
                CurrentTrainTime = __GetTime(currentItem);
                CurrentTrainNumber = __GetTrainNumber(currentItem);
                Via = __GetViaString(currentItem);
                CurrentTrainDestination = __GetDestination(currentItem, true);

                var infoTexts = new List<string>();

                if (currentItem.Schedule.Handling == eHandling.Destination)
                    infoTexts.Add("Zug endet hier");

                var delay = __GetDelayMinutes(currentItem);

                if (delay == 1)
                    infoTexts.Add("Wenige Minuten später");
                else if (delay >= 5)
                    infoTexts.Add($"ca. {delay} Minuten später");

                if (currentItem.LiveSchedule != null && (currentItem.Schedule.Track == SelectedTrack || currentItem.Schedule.Track == SelectedTrack.Parent) && currentItem.LiveSchedule.LiveTrack != null && currentItem.Schedule.Track != currentItem.LiveSchedule.LiveTrack)
                    infoTexts.Add($"Heute von Gleis {__GetTrackName(currentItem.LiveSchedule.LiveTrack)}");

                var info = string.Join(" - ", infoTexts);

                if (info.IsNotNullOrWhiteSpace())
                {
                    CurrentTrainInfo = $" - {info} - ";
                    IsCurrentTrainInfoMarquee = true;
                }
                else
                {
                    CurrentTrainInfo = info;
                    IsCurrentTrainInfoMarquee = false;
                }
            }
            else
                __ClearCurrentTrain();

            var followingTrain1 = orderedSchedules.ElementAtOrDefault(1);

            if (followingTrain1 != null)
            {
                FollowingTrain1Time = __GetTime(followingTrain1);
                FollowingTrain1Number = __GetTrainNumber(followingTrain1);
                FollowingTrain1Destination = __GetDestination(followingTrain1, false);

                if (followingTrain1.LiveSchedule != null && (followingTrain1.Schedule.Track == SelectedTrack || followingTrain1.Schedule.Track == SelectedTrack.Parent) && followingTrain1.LiveSchedule.LiveTrack != null && followingTrain1.Schedule.Track != followingTrain1.LiveSchedule.LiveTrack)
                    FollowingTrain1Info = $"Gleis {__GetTrackName(followingTrain1.LiveSchedule.LiveTrack)}";
                else
                    FollowingTrain1Info = string.Empty;

                var delay = __GetDelayMinutes(followingTrain1);

                if (delay >= 5)
                    FollowingTrain1Delay = $"+{delay}";
                else
                    FollowingTrain1Delay = string.Empty;
            }
            else
                __ClearFollowingTrain1();

            var followingTrain2 = orderedSchedules.ElementAtOrDefault(2);

            if (followingTrain2 != null)
            {
                FollowingTrain2Time = __GetTime(followingTrain2);
                FollowingTrain2Number = __GetTrainNumber(followingTrain2);
                FollowingTrain2Destination = __GetDestination(followingTrain2, false);

                if (followingTrain2.LiveSchedule != null && (followingTrain2.Schedule.Track == SelectedTrack || followingTrain2.Schedule.Track == SelectedTrack.Parent) && followingTrain2.LiveSchedule.LiveTrack != null && followingTrain2.Schedule.Track != followingTrain2.LiveSchedule.LiveTrack)
                    FollowingTrain2Info = $"Gleis {__GetTrackName(followingTrain2.LiveSchedule.LiveTrack)}";
                else
                    FollowingTrain2Info = string.Empty;

                var delay = __GetDelayMinutes(followingTrain2);

                if (delay >= 5)
                    FollowingTrain2Delay = $"+{delay}";
                else
                    FollowingTrain2Delay = string.Empty;
            }
            else
                __ClearFollowingTrain2();
        }
        #endregion

        #region [__GenerateLED]
        private void __GenerateLED(Area area)
        {
            var candidates = __GetScheduleCandidates(area, 30);
            var orderedSchedules = candidates.OrderBy(x => x.ReferenceTime);
            var textsToDisplay = new List<string>();
            textsToDisplay.Add($"Zeit: {SelectedStation.ESTW.Time} Uhr");

            var nextTrain = candidates.FirstOrDefault();

            if (nextTrain != null && nextTrain.Schedule.Time < SelectedStation.ESTW.Time.AddMinutes(10))
            {
                var track = nextTrain.LiveSchedule?.LiveTrack ?? nextTrain.Schedule.Track;
                textsToDisplay.Add($"{__GetLEDBaseText(nextTrain)} auf Gleis {__GetTrackName(track)}");
            }

            foreach (var currentItem in orderedSchedules)
            {
                string infoText = string.Empty;
                var delay = __GetDelayMinutes(currentItem);

                if (delay > 0)
                    infoText = delay == 1 ? "wenige Minuten später" : $"circa {delay} Minuten später";

                if (currentItem.LiveSchedule != null && (currentItem.Schedule.Track == SelectedTrack || currentItem.Schedule.Track == SelectedTrack.Parent) && currentItem.LiveSchedule.LiveTrack != null && currentItem.Schedule.Track != currentItem.LiveSchedule.LiveTrack)
                {
                    if (infoText.IsNotNullOrEmpty())
                        infoText += " und ";

                    infoText += $"von Gleis {__GetTrackName(currentItem.LiveSchedule.LiveTrack)}";
                }

                if (infoText.IsNotNullOrEmpty())
                    textsToDisplay.Add($"Information zu {__GetLEDBaseText(currentItem)}, heute {infoText}.");
            }

            LEDText = LED_SEPARATOR + string.Join(LED_SEPARATOR, textsToDisplay);
        }
        #endregion

        #region [__GetScheduleCandidates]
        private List<ScheduleItem> __GetScheduleCandidates(Area area, int leadMinutes)
        {
            var currentTime = SelectedStation.ESTW.Time;
            var schedulesResult = m_CalculationBll.GetSchedulesByTime(SelectedStation.Schedules, currentTime);

            if (!schedulesResult.Succeeded)
            {
                Dispatcher.Invoke(() => MessageBox.Show(schedulesResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error));
                return new List<ScheduleItem>();
            }

            var schedules = schedulesResult.Result.Where(s => s.Handling == eHandling.Start || s.Handling == eHandling.StopPassengerTrain || s.Handling == eHandling.Destination);
            var candidates = new List<ScheduleItem>();

            foreach (var schedule in schedules)
            {
                var train = schedule.Train;
                LiveSchedule liveSchedule = null;

                if (train == null)
                    continue;

                if (area.LiveTrains.ContainsKey(train.Number))
                {
                    var liveTrain = area.LiveTrains[train.Number];
                    liveSchedule = liveTrain.Schedules.FirstOrDefault(s => s.Schedule == schedule);
                }

                if (liveSchedule == null)
                {
                    if (schedule.Track != SelectedTrack && schedule.Track != SelectedTrack.Parent)
                        continue;
                    if (schedule.Time < currentTime)
                        continue;
                    if (schedule.Time > currentTime.AddMinutes(leadMinutes))
                        continue;

                    candidates.Add(new ScheduleItem(schedule.Time, schedule));
                }
                else
                {
                    if (liveSchedule.IsDeparted)
                        continue;

                    if (schedule.Handling == eHandling.Destination && schedule.Station.ESTW.Time > liveSchedule.Train.LastModified)
                        continue;

                    if (schedule.Track != SelectedTrack && schedule.Track != SelectedTrack.Parent && liveSchedule.LiveTrack != SelectedTrack && liveSchedule.LiveTrack != SelectedTrack.Parent)
                        continue;

                    var liveScheduleIndex = liveSchedule.Train.Schedules.IndexOf(liveSchedule);
                    var nextSchedules = liveSchedule.Train.Schedules.Skip(liveScheduleIndex + 1);

                    // No live data is available for the current station, but the train has already arrived at one of the next stations.
                    // This is the case at the beginning of the simulation or for diverted trains.
                    if (nextSchedules.Any(s => s.IsArrived))
                        continue;

                    var referenceTime = schedule.Arrival ?? schedule.Departure;

                    if (liveSchedule.Schedule.Handling == eHandling.Start && liveSchedule.ExpectedDeparture != null)
                        referenceTime = liveSchedule.ExpectedDeparture;
                    else if (liveSchedule.Schedule.Handling != eHandling.Start && liveSchedule.ExpectedArrival != null)
                        referenceTime = liveSchedule.ExpectedArrival;

                    if (referenceTime > currentTime.AddMinutes(leadMinutes))
                        continue;

                    candidates.Add(new ScheduleItem(referenceTime, schedule, liveSchedule));
                }
            }

            return candidates;
        }
        #endregion

        #region [__GetTime]
        private string __GetTime(ScheduleItem scheduleItem)
        {
            if (scheduleItem.Schedule.Handling == eHandling.Destination)
                return scheduleItem.Schedule.Arrival.ToString();
            else
                return scheduleItem.Schedule.Departure.ToString();
        }
        #endregion

        #region [__GetTrackName]
        private string __GetTrackName(Track track)
        {
            if (track.DisplayName.IsNullOrWhiteSpace())
                return track.Name;

            if (track.DisplaySubName.IsNullOrWhiteSpace())
                return track.DisplayName;

            return $"{track.DisplayName} {track.DisplaySubName}";
        }
        #endregion

        #region [__GetTrainNumber]
        private string __GetTrainNumber(ScheduleItem scheduleItem)
        {
            return $"{scheduleItem.Schedule.Train.Type} {scheduleItem.Schedule.Train.Number}";
        }
        #endregion

        #region [__GetDestination]
        private string __GetDestination(ScheduleItem scheduleItem, bool isCurrent)
        {
            if (scheduleItem.Schedule.Handling == eHandling.Destination)
            {
                if (isCurrent)
                    return "Bitte nicht einsteigen";
                else
                    return $"von {scheduleItem.Schedule.Train.Start}";
            }
            else
                return scheduleItem.Schedule.Train.Destination;
        }
        #endregion

        #region [__GetViaString]
        private string __GetViaString(ScheduleItem scheduleItem)
        {
            if (scheduleItem.Schedule.Handling == eHandling.Destination)
                return $"von {scheduleItem.Schedule.Train.Start}";

            var schedulesResult = m_CalculationBll.GetSchedulesByTime(scheduleItem.Schedule.Train.Schedules, scheduleItem.Schedule.Station.ESTW.Time);

            if (!schedulesResult.Succeeded)
                return string.Empty;

            var schedules = schedulesResult.Result;
            var scheduleIndex = schedules.IndexOf(scheduleItem.Schedule);
            var nextSchedules = schedules.Skip(scheduleIndex + 1);
            var candidateSchedules = nextSchedules.Where(x => x.Handling == eHandling.StopPassengerTrain).OrderByDescending(s => s.Station.Tracks.Count(t => t.IsPlatform)).ThenBy(x => x.Time);
            var viaSchedules = new List<Schedule>();
            var currentViaString = string.Empty;
            var result = string.Empty;

            foreach (var schedule in candidateSchedules)
            {
                string candidate;

                if (currentViaString.IsNullOrEmpty())
                    candidate = schedule.Station.Name;
                else
                    candidate = $"{currentViaString} - {schedule.Station.Name}";

                if (__MeasureString(candidate, 12) > 240)
                    continue;

                currentViaString = candidate;
                viaSchedules.Add(schedule);
            }

            foreach (var schedule in viaSchedules.OrderBy(s => s.Time))
            {
                if (result.IsNullOrEmpty())
                    result += schedule.Station.Name;
                else
                    result += $" - {schedule.Station.Name}";
            }

            return result;
        }
        #endregion

        #region [__GetDelayMinutes]
        private int __GetDelayMinutes(ScheduleItem scheduleItem)
        {
            if (scheduleItem.LiveSchedule == null)
                return 0;

            LeibitTime scheduledTime, expectedTime;

            if (scheduleItem.Schedule.Handling == eHandling.Destination)
            {
                scheduledTime = scheduleItem.Schedule.Arrival;
                expectedTime = scheduleItem.LiveSchedule.ExpectedArrival;
            }
            else
            {
                scheduledTime = scheduleItem.Schedule.Departure;
                expectedTime = scheduleItem.LiveSchedule.ExpectedDeparture;
            }

            if (scheduledTime == null || expectedTime == null)
                return 0;

            var delay = (expectedTime - scheduledTime).TotalMinutes;

            if (delay < 2)
                return 0;

            if (delay < 4)
                return 1;

            return (delay + 1) / 5 * 5;
        }
        #endregion

        #region [__GetLEDBaseText]
        private string __GetLEDBaseText(ScheduleItem scheduleItem)
        {
            if (scheduleItem.Schedule.Handling == eHandling.Destination)
                return $"{scheduleItem.Schedule.Train.Type} {scheduleItem.Schedule.Train.Number} von {scheduleItem.Schedule.Train.Start}, Ankunft {scheduleItem.Schedule.Arrival} Uhr";
            else
                return $"{scheduleItem.Schedule.Train.Type} {scheduleItem.Schedule.Train.Number} nach {scheduleItem.Schedule.Train.Destination}, Abfahrt {scheduleItem.Schedule.Departure} Uhr";
        }
        #endregion

        #region [__ClearAll]
        private void __ClearAll()
        {
            __ClearCurrentTrain();
            __ClearFollowingTrain1();
            __ClearFollowingTrain2();

            LEDText = "...";
        }
        #endregion

        #region [__ClearCurrentTrain]
        private void __ClearCurrentTrain()
        {
            CurrentTrainTime = string.Empty;
            CurrentTrainNumber = string.Empty;
            CurrentTrainInfo = string.Empty;
            Via = string.Empty;
            CurrentTrainDestination = string.Empty;
        }
        #endregion

        #region [__ClearFollowingTrain1]
        private void __ClearFollowingTrain1()
        {
            FollowingTrain1Time = string.Empty;
            FollowingTrain1Delay = string.Empty;
            FollowingTrain1Number = string.Empty;
            FollowingTrain1Destination = string.Empty;
            FollowingTrain1Info = string.Empty;
        }
        #endregion

        #region [__ClearFollowingTrain2]
        private void __ClearFollowingTrain2()
        {
            FollowingTrain2Time = string.Empty;
            FollowingTrain2Delay = string.Empty;
            FollowingTrain2Number = string.Empty;
            FollowingTrain2Destination = string.Empty;
            FollowingTrain2Info = string.Empty;
        }
        #endregion

        #region [__MeasureString]
        private double __MeasureString(string candidate, double size)
        {
            var formattedText = new FormattedText(candidate, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe"), size, Brushes.Black, 1);
            return formattedText.Width;
        }
        #endregion

        #endregion

    }
}
