using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class CountdownDisplayViewModel : DisplayViewModelBase
    {

        #region - Ctor -
        public CountdownDisplayViewModel(DisplayViewModel parent)
           : base(parent)
        {
            Items = new ObservableCollection<CountdownDisplayItemViewModel>();
        }
        #endregion

        #region - Properties -

        #region [IsInTwinTrainMode]
        public bool IsInTwinTrainMode
        {
            get => Get<bool>();
            private set
            {
                Set(value);
                OnPropertyChanged(nameof(ShowSingleTrain));
            }
        }
        #endregion

        #region [ShowSingleTrain]
        public bool ShowSingleTrain => !IsInTwinTrainMode && !ShowPreview;
        #endregion

        #region [ShowPreview]
        public bool ShowPreview
        {
            get => Get<bool>();
            private set
            {
                Set(value);
                OnPropertyChanged(nameof(ShowSingleTrain));
            }
        }
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
            get => Get<bool>();
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

        #region [CurrentTrainDestinationSize]
        public double CurrentTrainDestinationSize
        {
            get => Get<double>();
            private set => Set(value);
        }
        #endregion

        #region [TimeSize]
        public double TimeSize
        {
            get => Get<double>();
            private set => Set(value);
        }
        #endregion

        #region [TrainNumberSize]
        public double TrainNumberSize
        {
            get => Get<double>();
            private set => Set(value);
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

        #region [TwinTrainVia]
        public string TwinTrainVia
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [TwinTrainDestination]
        public string TwinTrainDestination
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [TwinTrainTime]
        public string TwinTrainTime
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [TwinTrainNumber]
        public string TwinTrainNumber
        {
            get => Get<string>();
            set => Set(value);
        }
        #endregion

        #region [Items]
        public ObservableCollection<CountdownDisplayItemViewModel> Items { get; }
        #endregion

        #endregion

        #region - Overrides -

        #region [Refresh]
        internal override void Refresh(Area area)
        {
            if (SelectedStation == null || SelectedTrack == null)
            {
                __Clear();
                return;
            }

            var candidates = GetScheduleCandidates(area, 120, true);
            var orderedSchedules = candidates.Where(x => x.LiveSchedule?.LiveTrack == null || x.LiveSchedule.LiveTrack == SelectedTrack).OrderBy(x => x.ReferenceTime);

            if (!orderedSchedules.Any())
            {
                __Clear();
                return;
            }

            var currentItem = orderedSchedules.FirstOrDefault();

            if (currentItem != null && currentItem.LiveSchedule != null && currentItem.LiveSchedule.IsArrived)
            {
                var infoTexts = new List<string>();
                var isDestination = IsDestination(currentItem);
                var isCancelled = currentItem.LiveSchedule.IsCancelled;
                Schedule twinSchedule = null;
                ScheduleItem twinScheduleItem = null;
                CurrentTrainNumber = GetTrainNumber(currentItem);

                int minutes;

                if (isDestination)
                    minutes = (currentItem.LiveSchedule.ExpectedArrival - SelectedStation.ESTW.Time).TotalMinutes;
                else
                    minutes = (currentItem.LiveSchedule.ExpectedDeparture - SelectedStation.ESTW.Time).TotalMinutes;

                if (minutes <= 0)
                    CurrentTrainTime = "sofort";
                else
                    CurrentTrainTime = $"in {minutes} min";

                if (currentItem.Schedule.TwinScheduleArrival != null)
                {
                    twinSchedule = currentItem.Schedule.TwinScheduleArrival;

                    if (isDestination)
                        IsInTwinTrainMode = true;
                    else if (currentItem.Schedule.TwinScheduleDeparture == null)
                    {
                        IsInTwinTrainMode = true;
                        infoTexts.Add("Zug wird hier geteilt");
                    }
                    else if (currentItem.Schedule.Destination != currentItem.Schedule.TwinScheduleDeparture.Destination || currentItem.Schedule.SplitStation.IsNotNullOrWhiteSpace())
                    {
                        IsInTwinTrainMode = true;

                        if (currentItem.Schedule.SplitStation.IsNotNullOrWhiteSpace())
                            infoTexts.Add($"Zug wird in {currentItem.Schedule.SplitStation} geteilt");
                    }
                    else
                        IsInTwinTrainMode = false;
                }
                else if (currentItem.Schedule.TwinScheduleDeparture != null)
                {
                    twinSchedule = currentItem.Schedule.TwinScheduleDeparture;
                    IsInTwinTrainMode = false;
                }
                else
                    IsInTwinTrainMode = false;

                if (twinSchedule != null)
                {
                    twinScheduleItem = candidates.FirstOrDefault(x => x.Schedule == twinSchedule);

                    if (twinScheduleItem == null)
                    {
                        IsInTwinTrainMode = false;
                        TwinTrainNumber = string.Empty;
                    }
                    else
                    {
                        TwinTrainNumber = GetTrainNumber(twinSchedule);
                        orderedSchedules = candidates.Except(new List<ScheduleItem> { twinScheduleItem }).OrderBy(x => x.ReferenceTime);
                    }
                }
                else
                    TwinTrainNumber = string.Empty;

                if (IsInTwinTrainMode)
                {
                    if (isDestination)
                        minutes = (twinScheduleItem.LiveSchedule.ExpectedArrival - SelectedStation.ESTW.Time).TotalMinutes;
                    else
                        minutes = (twinScheduleItem.LiveSchedule.ExpectedDeparture - SelectedStation.ESTW.Time).TotalMinutes;

                    if (minutes <= 0)
                        TwinTrainTime = "sofort";
                    else
                        TwinTrainTime = $"in {minutes} min";
                }

                if (isDestination && !isCancelled)
                {
                    int? followUpService = __GetFollowUpService(currentItem);

                    if (followUpService.HasValue && area.Trains.TryGetValue(followUpService.Value, out var followUpTrain) && followUpTrain.Type.IsPassengerTrain() && !IsInTwinTrainMode)
                    {
                        infoTexts.Add($"Dieser Zug endet hier und fährt weiter als {followUpTrain.Type} {followUpTrain.Number} nach {followUpTrain.Destination}");
                        Via = string.Empty;
                        CurrentTrainDestination = $"von {currentItem.Schedule.Start}";
                    }
                    else
                    {
                        infoTexts.Add("Dieser Zug endet hier");
                        Via = $"von {currentItem.Schedule.Start}";
                        CurrentTrainDestination = "Bitte nicht einsteigen";

                        if (IsInTwinTrainMode)
                        {
                            TwinTrainVia = $"von {twinScheduleItem.Schedule.Start}";
                            TwinTrainDestination = "Bitte nicht einsteigen";
                        }
                    }
                }
                else
                {
                    Via = GetViaString(currentItem, 12, 240);
                    CurrentTrainDestination = GetDestination(currentItem);

                    if (IsInTwinTrainMode)
                    {
                        TwinTrainVia = GetViaString(twinScheduleItem, 12, 240);
                        TwinTrainDestination = GetDestination(twinScheduleItem);
                    }
                }

                CurrentTrainDestinationSize = IsInTwinTrainMode ? 20 : 24;
                TimeSize = IsInTwinTrainMode ? 20 : 24;
                TrainNumberSize = IsInTwinTrainMode ? 12 : 14;

                if (currentItem.LiveSchedule != null)
                {
                    if (isCancelled)
                        infoTexts.Add("Zug fällt heute aus");
                    else
                    {
                        var differingDestination = GetDifferingDestinationSchedule(currentItem);

                        if (differingDestination != null)
                            infoTexts.Add($"Fährt heute nur bis {GetDisplayName(differingDestination.Station)}");

                        var skippedStations = GetSkippedSchedules(currentItem);

                        if (skippedStations.Any())
                            infoTexts.Add($"Hält nicht in {GetStationList(skippedStations)}");
                    }
                }

                var info = string.Join(" - ", infoTexts);

                if (info.IsNotNullOrWhiteSpace())
                {
                    if (MeasureString(info, 11) > 240)
                        CurrentTrainInfo = $" - {info}";
                    else
                        CurrentTrainInfo = $" - {info} - ";

                    IsCurrentTrainInfoMarquee = true;
                }
                else
                {
                    CurrentTrainInfo = info;
                    IsCurrentTrainInfoMarquee = false;
                }

                ShowPreview = false;
            }
            else
            {
                var currentItems = new List<CountdownDisplayItemViewModel>();

                foreach (var scheduleItem in orderedSchedules.Take(3))
                {
                    var currentVM = Items.FirstOrDefault(x => x.Schedule == scheduleItem.Schedule);

                    if (currentVM == null)
                    {
                        currentVM = new CountdownDisplayItemViewModel(scheduleItem);
                        currentVM.TrainNumber = GetTrainNumber(scheduleItem);

                        Dispatcher.Invoke(() => Items.Add(currentVM));
                    }

                    currentVM.Destination = GetDestination(scheduleItem);

                    if (scheduleItem.LiveSchedule == null)
                        currentVM.ExpectedDeparture = __GetTime(scheduleItem);
                    else if (scheduleItem.LiveSchedule.IsCancelled)
                        currentVM.ExpectedDeparture = "fällt aus";
                    else
                    {
                        int minutes;

                        if (IsDestination(scheduleItem))
                            minutes = (scheduleItem.LiveSchedule.ExpectedArrival - SelectedStation.ESTW.Time).TotalMinutes;
                        else
                            minutes = (scheduleItem.LiveSchedule.ExpectedDeparture - SelectedStation.ESTW.Time).TotalMinutes;

                        if (minutes <= 0)
                            currentVM.ExpectedDeparture = "in Kürze";
                        else
                            currentVM.ExpectedDeparture = $"in {minutes} min";
                    }

                    currentItems.Add(currentVM);
                }

                var removedItems = Items.Except(currentItems).ToList();

                Dispatcher.Invoke(() =>
                {
                    removedItems.ForEach(x => Items.Remove(x));

                    for (int i = 0; i < currentItems.Count; i++)
                        Items.Move(Items.IndexOf(currentItems[i]), i);
                });

                ShowPreview = true;
            }
        }
        #endregion

        #region [SelectionChanged]
        internal override void SelectionChanged()
        {
            OnPropertyChanged(nameof(TrackName));
            OnPropertyChanged(nameof(SubTrackName));
            IsCurrentTrainInfoMarquee = false;
        }
        #endregion

        #endregion

        #region - Private helper -

        #region [__GetTime]
        private string __GetTime(ScheduleItem scheduleItem)
        {
            if (IsDestination(scheduleItem))
                return scheduleItem.Schedule.Arrival.ToString();
            else
                return scheduleItem.Schedule.Departure.ToString();
        }
        #endregion

        #region [__GetFollowUpService]
        private int? __GetFollowUpService(ScheduleItem currentItem)
        {
            if (currentItem.Schedule.Handling == eHandling.Destination)
            {
                if (currentItem.LiveSchedule != null)
                {
                    if (currentItem.LiveSchedule.Train.FollowUpService.HasValue)
                        return currentItem.LiveSchedule.Train.FollowUpService;
                }
                else
                    return currentItem.Schedule.Train.FollowUpServices.FirstOrDefault(r => r.Days.Contains(currentItem.Schedule.Station.ESTW.Time.Day))?.TrainNumber;
            }

            return null;
        }
        #endregion

        #region [__Clear]
        private void __Clear()
        {
            Dispatcher.Invoke(() => Items.Clear());
            ShowPreview = true;
            IsInTwinTrainMode = false;
        }
        #endregion

        #endregion

    }
}
