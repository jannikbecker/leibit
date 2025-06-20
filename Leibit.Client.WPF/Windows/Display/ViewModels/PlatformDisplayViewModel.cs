﻿using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class PlatformDisplayViewModel : DisplayViewModelBase
    {

        #region - Ctor -
        public PlatformDisplayViewModel(DisplayViewModel parent)
           : base(parent)
        {

        }
        #endregion

        #region - Properties -

        #region [IsInTwinTrainMode]
        public bool IsInTwinTrainMode
        {
            get => Get<bool>();
            private set => Set(value);
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

        #endregion

        #region - Overrides -

        #region [Refresh]
        internal override void Refresh(Area area)
        {
            if (SelectedStation == null || SelectedTrack == null)
            {
                __ClearCurrentTrain();
                __ClearFollowingTrain1();
                __ClearFollowingTrain2();
                return;
            }

            var candidates = GetScheduleCandidates(area, 120, true);
            var orderedSchedules = candidates.OrderBy(x => x.ReferenceTime);
            var specialTextDisplayed = false;
            ScheduleItem currentItem, followingTrain1, followingTrain2;

            if (area.LiveTrains.Values.Any(__IsPassingTrain))
            {
                __ClearCurrentTrain();
                CurrentTrainDestinationSize = 24;
                CurrentTrainDestination = "Zugdurchfahrt";
                specialTextDisplayed = true;
                currentItem = null;
                followingTrain1 = orderedSchedules.FirstOrDefault();
                followingTrain2 = orderedSchedules.ElementAtOrDefault(1);
            }
            else
            {
                currentItem = orderedSchedules.FirstOrDefault();
                followingTrain1 = orderedSchedules.ElementAtOrDefault(1);
                followingTrain2 = orderedSchedules.ElementAtOrDefault(2);
            }

            if (currentItem != null)
            {
                var infoTexts = new List<string>();
                var isDestination = IsDestination(currentItem);
                var isCancelled = currentItem.LiveSchedule?.IsCancelled == true;
                Schedule twinSchedule = null;
                ScheduleItem twinScheduleItem = null;
                CurrentTrainTime = __GetTime(currentItem);
                CurrentTrainNumber = GetTrainNumber(currentItem);

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
                    TwinTrainTime = __GetTime(twinScheduleItem);

                if (isDestination && !isCancelled && currentItem.Schedule.Handling != eHandling.Start)
                {
                    var followUpTrain = GetFollowUpTrain(currentItem, area);

                    if (followUpTrain != null && !IsInTwinTrainMode)
                    {
                        infoTexts.Add($"Dieser Zug endet hier und fährt weiter als {GetTrainNumber(followUpTrain)} nach {followUpTrain.Destination}");
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

                if (!isCancelled)
                {
                    var delay = GetDelayMinutes(currentItem);

                    if (delay == 1)
                        infoTexts.Add("Wenige Minuten später");
                    else if (delay >= 5)
                        infoTexts.Add($"ca. {delay} Minuten später");

                    if (IsTrackChanged(currentItem) && (currentItem.Schedule.Track == SelectedTrack || currentItem.Schedule.Track == SelectedTrack.Parent))
                        infoTexts.Add($"Heute von Gleis {GetTrackName(currentItem.LiveSchedule.LiveTrack)}");
                }

                if (currentItem.LiveSchedule != null)
                {
                    if (isCancelled || (isDestination && currentItem.Schedule.Handling == eHandling.Start))
                        infoTexts.Add("Zug fällt heute aus");
                    else
                    {
                        var differingDestination = GetDifferingDestinationSchedule(currentItem);

                        if (differingDestination != null)
                        {
                            if (isDestination)
                                infoTexts.Add($"Keine Weiterfahrt bis {currentItem.Schedule.Destination}");
                            else
                                infoTexts.Add($"Fährt heute nur bis {GetDisplayName(differingDestination.Station)}");
                        }

                        if (!isDestination)
                        {
                            var skippedStations = GetSkippedSchedules(currentItem);

                            if (skippedStations.Any())
                                infoTexts.Add($"Hält nicht in {GetStationList(skippedStations)}");
                        }
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
            }
            else if (!specialTextDisplayed)
                __ClearCurrentTrain();

            if (SelectedDisplayType?.Type == eDisplayType.PlatformDisplay_Small)
                return;

            if (followingTrain1 != null)
            {
                FollowingTrain1Time = __GetTime(followingTrain1);
                FollowingTrain1Number = GetTrainNumber(followingTrain1);
                FollowingTrain1Destination = GetDestination(followingTrain1);

                if (followingTrain1.LiveSchedule?.IsCancelled == true)
                    FollowingTrain1Info = "fällt aus";
                else if (IsTrackChanged(followingTrain1) && (followingTrain1.Schedule.Track == SelectedTrack || followingTrain1.Schedule.Track == SelectedTrack.Parent))
                    FollowingTrain1Info = $"Gleis {GetTrackName(followingTrain1.LiveSchedule.LiveTrack)}";
                else
                    FollowingTrain1Info = string.Empty;

                var delay = GetDelayMinutes(followingTrain1);

                if (delay >= 5)
                    FollowingTrain1Delay = $"+{delay}";
                else
                    FollowingTrain1Delay = string.Empty;
            }
            else
                __ClearFollowingTrain1();

            if (followingTrain2 != null)
            {
                FollowingTrain2Time = __GetTime(followingTrain2);
                FollowingTrain2Number = GetTrainNumber(followingTrain2);
                FollowingTrain2Destination = GetDestination(followingTrain2);

                if (followingTrain2.LiveSchedule?.IsCancelled == true)
                    FollowingTrain2Info = "fällt aus";
                else if (IsTrackChanged(followingTrain2) && (followingTrain2.Schedule.Track == SelectedTrack || followingTrain2.Schedule.Track == SelectedTrack.Parent))
                    FollowingTrain2Info = $"Gleis {GetTrackName(followingTrain2.LiveSchedule.LiveTrack)}";
                else
                    FollowingTrain2Info = string.Empty;

                var delay = GetDelayMinutes(followingTrain2);

                if (delay >= 5)
                    FollowingTrain2Delay = $"+{delay}";
                else
                    FollowingTrain2Delay = string.Empty;
            }
            else
                __ClearFollowingTrain2();
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
            if (IsDestination(scheduleItem) && scheduleItem.Schedule.Arrival != null)
                return scheduleItem.Schedule.Arrival.ToString();
            else
                return scheduleItem.Schedule.Departure.ToString();
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
            TwinTrainNumber = string.Empty;
            IsInTwinTrainMode = false;
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

        #region [__IsPassingTrain]
        private bool __IsPassingTrain(TrainInformation train)
        {
            if (!train.IsActive)
                return false;
            if (train.RealBlock == null)
                return false;
            if (!SelectedTrack.Blocks.Contains(train.RealBlock))
                return false;

            var scheduleForStation = train.Schedules.FirstOrDefault(s => s.Schedule.Station == SelectedStation && !s.IsDeparted);

            if (scheduleForStation == null)
                return true;

            return scheduleForStation.Schedule.Handling == eHandling.Transit;
        }
        #endregion

        #endregion

    }
}
