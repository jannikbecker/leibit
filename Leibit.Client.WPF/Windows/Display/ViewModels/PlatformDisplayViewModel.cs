using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
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
            var currentItem = orderedSchedules.FirstOrDefault();

            if (currentItem != null)
            {
                CurrentTrainTime = __GetTime(currentItem);
                CurrentTrainNumber = GetTrainNumber(currentItem);
                Via = GetViaString(currentItem, 12, 240);
                CurrentTrainDestination = __GetDestination(currentItem, true);

                var infoTexts = new List<string>();

                if (currentItem.Schedule.Handling == eHandling.Destination)
                    infoTexts.Add("Zug endet hier");

                var delay = GetDelayMinutes(currentItem);

                if (delay == 1)
                    infoTexts.Add("Wenige Minuten später");
                else if (delay >= 5)
                    infoTexts.Add($"ca. {delay} Minuten später");

                if (IsTrackChanged(currentItem) && (currentItem.Schedule.Track == SelectedTrack || currentItem.Schedule.Track == SelectedTrack.Parent))
                    infoTexts.Add($"Heute von Gleis {GetTrackName(currentItem.LiveSchedule.LiveTrack)}");

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

            if (SelectedDisplayType?.Type == eDisplayType.PlatformDisplay_Small)
                return;

            var followingTrain1 = orderedSchedules.ElementAtOrDefault(1);

            if (followingTrain1 != null)
            {
                FollowingTrain1Time = __GetTime(followingTrain1);
                FollowingTrain1Number = GetTrainNumber(followingTrain1);
                FollowingTrain1Destination = __GetDestination(followingTrain1, false);

                if (IsTrackChanged(followingTrain1) && (followingTrain1.Schedule.Track == SelectedTrack || followingTrain1.Schedule.Track == SelectedTrack.Parent))
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

            var followingTrain2 = orderedSchedules.ElementAtOrDefault(2);

            if (followingTrain2 != null)
            {
                FollowingTrain2Time = __GetTime(followingTrain2);
                FollowingTrain2Number = GetTrainNumber(followingTrain2);
                FollowingTrain2Destination = __GetDestination(followingTrain2, false);

                if (IsTrackChanged(followingTrain2) && (followingTrain2.Schedule.Track == SelectedTrack || followingTrain2.Schedule.Track == SelectedTrack.Parent))
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
            if (scheduleItem.Schedule.Handling == eHandling.Destination)
                return scheduleItem.Schedule.Arrival.ToString();
            else
                return scheduleItem.Schedule.Departure.ToString();
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

        #endregion

    }
}
