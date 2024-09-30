﻿using Leibit.BLL;
using Leibit.Core.Client.BaseClasses;
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

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public abstract class DisplayViewModelBase : ViewModelBase
    {

        #region - Needs -
        private readonly CalculationBLL m_CalculationBll;
        private readonly DisplayViewModel m_Parent;
        #endregion

        #region - Ctor -
        protected DisplayViewModelBase(DisplayViewModel parent)
        {
            m_Parent = parent;
            m_CalculationBll = new CalculationBLL();
        }
        #endregion

        #region - Properties -
        protected Dispatcher Dispatcher => m_Parent.Dispatcher;
        protected DisplayType SelectedDisplayType => m_Parent.SelectedDisplayType;
        protected Station SelectedStation => m_Parent.SelectedStation;
        protected Track SelectedTrack => m_Parent.SelectedTrack;
        protected ObservableCollection<Track> SelectedTracks => m_Parent.SelectedTracks;
        #endregion

        #region - Internal methods -

        internal abstract void Refresh(Area area);

        internal virtual void SelectionChanged() { }

        #endregion

        #region - Protected methods -

        #region [GetScheduleCandidates]
        protected List<ScheduleItem> GetScheduleCandidates(Area area, int leadMinutes, bool trackRequired)
        {
            var matchTrack = trackRequired || SelectedTracks.Any();
            var currentTime = SelectedStation.ESTW.Time;
            var schedulesResult = m_CalculationBll.GetSchedulesByTime(SelectedStation.Schedules, currentTime);

            if (!schedulesResult.Succeeded)
            {
                Dispatcher.Invoke(() => MessageBox.Show(schedulesResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error));
                return new List<ScheduleItem>();
            }

            var schedules = schedulesResult.Result.Where(s => s.Handling == eHandling.Start || s.Handling == eHandling.StopPassengerTrain || s.Handling == eHandling.StopFreightTrain || s.Handling == eHandling.Destination);
            var candidates = new List<ScheduleItem>();

            foreach (var schedule in schedules)
            {
                var train = schedule.Train;
                LiveSchedule liveSchedule = null;

                if (train == null || !schedule.TrainType.IsPassengerTrain())
                    continue;

                if (area.LiveTrains.ContainsKey(train.Number))
                {
                    var liveTrain = area.LiveTrains[train.Number];
                    liveSchedule = liveTrain.Schedules.FirstOrDefault(s => s.Schedule == schedule);
                }

                if (liveSchedule == null)
                {
                    if (matchTrack && !SelectedDisplayType.MultiTrack && schedule.Track != SelectedTrack && schedule.Track != SelectedTrack.Parent)
                        continue;
                    if (matchTrack && SelectedDisplayType.MultiTrack && !SelectedTracks.Contains(schedule.Track) && !SelectedTracks.Any(t => t.Parent == schedule.Track))
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

                    if (schedule.Handling == eHandling.Destination && !liveSchedule.Train.IsActive)
                        continue;

                    if (matchTrack && !SelectedDisplayType.MultiTrack
                        && schedule.Track != SelectedTrack && schedule.Track != SelectedTrack.Parent && liveSchedule.LiveTrack != SelectedTrack && liveSchedule.LiveTrack != SelectedTrack.Parent)
                        continue;

                    if (matchTrack && SelectedDisplayType.MultiTrack
                        && !SelectedTracks.Contains(schedule.Track) && !SelectedTracks.Any(t => t.Parent == schedule.Track)
                        && !SelectedTracks.Contains(liveSchedule.LiveTrack) && !SelectedTracks.Any(t => t.Parent == liveSchedule.LiveTrack))
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

                    if (referenceTime < currentTime && !liveSchedule.Train.IsActive)
                        continue;

                    candidates.Add(new ScheduleItem(referenceTime, schedule, liveSchedule));
                }
            }

            return candidates;
        }
        #endregion

        #region [GetTrackName]
        protected string GetTrackName(Track track)
        {
            if (track.DisplayName.IsNullOrWhiteSpace())
                return track.Name;

            if (track.DisplaySubName.IsNullOrWhiteSpace())
                return track.DisplayName;

            return $"{track.DisplayName} {track.DisplaySubName}";
        }
        #endregion

        #region [GetTrainNumber]
        protected string GetTrainNumber(ScheduleItem scheduleItem)
        {
            return GetTrainNumber(scheduleItem.Schedule);
        }

        protected string GetTrainNumber(Schedule schedule)
        {
            if (schedule.Train.Line.IsNullOrWhiteSpace())
                return $"{schedule.TrainType} {schedule.Train.Number}";
            else
                return schedule.Train.Line;
        }

        protected string GetTrainNumber(Train train)
        {
            if (train.Line.IsNullOrWhiteSpace())
                return $"{train.Type} {train.Number}";
            else
                return train.Line;
        }
        #endregion

        #region [GetViaString]
        protected string GetViaString(ScheduleItem scheduleItem, double fontSize, double maxSpace)
        {
            var schedulesResult = m_CalculationBll.GetSchedulesByTime(scheduleItem.Schedule.Train.Schedules, scheduleItem.Schedule.Station.ESTW.Time);

            if (!schedulesResult.Succeeded)
                return string.Empty;

            var schedules = schedulesResult.Result;
            var scheduleIndex = schedules.IndexOf(scheduleItem.Schedule);
            var nextSchedules = schedules.Skip(scheduleIndex + 1);
            var candidateSchedules = nextSchedules.Where(x => x.Handling == eHandling.StopPassengerTrain);
            var viaSchedules = new List<Schedule>();
            var currentViaString = string.Empty;
            var result = string.Empty;

            if (scheduleItem.LiveSchedule != null && !scheduleItem.LiveSchedule.IsCancelled)
            {
                var destinationSchedule = GetDifferingDestinationSchedule(scheduleItem);

                if (destinationSchedule == null || destinationSchedule.Station.ShortSymbol != scheduleItem.Schedule.Station.ShortSymbol || destinationSchedule.Time != scheduleItem.Schedule.Time)
                {
                    // Remove all cancelled stations
                    candidateSchedules = candidateSchedules.Where(s =>
                    {
                        var liveSchedule = scheduleItem.LiveSchedule.Train.Schedules.FirstOrDefault(s2 => s.Station.ShortSymbol == s2.Schedule.Station.ShortSymbol && s.Time == s2.Schedule.Time);

                        if (liveSchedule == null)
                            return true;

                        return !liveSchedule.IsCancelled;
                    });

                    // Remove destination station
                    if (scheduleItem.LiveSchedule.Train.IsDestinationStationCancelled)
                        candidateSchedules = candidateSchedules.Where(s => s.Station.ShortSymbol != destinationSchedule.Station.ShortSymbol && s.Time != destinationSchedule.Time);
                }
            }

            var orderedSchedules = candidateSchedules.OrderByDescending(s => s.Station.Tracks.Count(t => t.IsPlatform)).ThenBy(x => x.Time);

            foreach (var schedule in orderedSchedules)
            {
                string candidate;

                if (currentViaString.IsNullOrEmpty())
                    candidate = GetDisplayName(schedule.Station);
                else
                    candidate = $"{currentViaString} - {GetDisplayName(schedule.Station)}";

                if (MeasureString(candidate, fontSize) > maxSpace)
                    continue;

                currentViaString = candidate;
                viaSchedules.Add(schedule);
            }

            foreach (var schedule in viaSchedules.OrderBy(s => s.Time))
            {
                if (result.IsNullOrEmpty())
                    result += GetDisplayName(schedule.Station);
                else
                    result += $" - {GetDisplayName(schedule.Station)}";
            }

            return result;
        }
        #endregion

        #region [GetDelayMinutes]
        protected int GetDelayMinutes(ScheduleItem scheduleItem)
        {
            if (scheduleItem.LiveSchedule == null || scheduleItem.LiveSchedule.IsCancelled)
                return 0;

            LeibitTime scheduledTime, expectedTime;

            if (IsDestination(scheduleItem))
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

        #region [IsTrackChanged]
        protected bool IsTrackChanged(ScheduleItem scheduleItem)
        {
            return scheduleItem.LiveSchedule != null && scheduleItem.LiveSchedule.LiveTrack != null && scheduleItem.Schedule.Track != scheduleItem.LiveSchedule.LiveTrack;
        }
        #endregion

        #region [IsDestination]
        protected bool IsDestination(ScheduleItem scheduleItem, bool considerCancellations = true)
        {
            if (scheduleItem.Schedule.Handling == eHandling.Destination || scheduleItem.Schedule.IsPassengerDestination)
                return true;

            if (scheduleItem.Schedule.Handling == eHandling.StopPassengerTrain || scheduleItem.Schedule.Handling == eHandling.StopFreightTrain || scheduleItem.Schedule.Handling == eHandling.Start)
            {
                var schedulesResult = m_CalculationBll.GetSchedulesByTime(scheduleItem.Schedule.Train.Schedules, scheduleItem.Schedule.Station.ESTW.Time);

                if (schedulesResult.Succeeded)
                {
                    var schedules = schedulesResult.Result;
                    var scheduleIndex = schedules.IndexOf(scheduleItem.Schedule);
                    var nextSchedule = schedules.Skip(scheduleIndex + 1).FirstOrDefault();

                    if (nextSchedule != null && !nextSchedule.TrainType.IsPassengerTrain())
                        return true;
                }

                if (considerCancellations && scheduleItem.LiveSchedule != null && !scheduleItem.LiveSchedule.IsCancelled && scheduleItem.LiveSchedule.Train.IsDestinationStationCancelled)
                {
                    var liveTrain = scheduleItem.LiveSchedule.Train;
                    var scheduleIndex = liveTrain.Schedules.IndexOf(scheduleItem.LiveSchedule);

                    if (liveTrain.Schedules.Skip(scheduleIndex + 1).Where(s => s.Schedule.Handling == eHandling.StopPassengerTrain).All(s => s.IsCancelled))
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region [GetDestination]
        protected string GetDestination(ScheduleItem scheduleItem)
        {
            if (IsDestination(scheduleItem) && scheduleItem.Schedule.Handling != eHandling.Start)
                return $"von {scheduleItem.Schedule.Start}";

            var differingDestination = GetDifferingDestinationSchedule(scheduleItem);

            if (differingDestination != null)
                return GetDisplayName(differingDestination.Station);

            return scheduleItem.Schedule.Destination;
        }
        #endregion

        #region [GetDifferingDestinationSchedule]
        protected Schedule GetDifferingDestinationSchedule(ScheduleItem scheduleItem)
        {
            if (scheduleItem.LiveSchedule != null && !scheduleItem.LiveSchedule.IsCancelled && scheduleItem.LiveSchedule.Train.IsDestinationStationCancelled)
                return scheduleItem.LiveSchedule.Train.Schedules.LastOrDefault(s => s.Schedule.Handling == eHandling.StopPassengerTrain && !s.IsCancelled)?.Schedule;

            return null;
        }
        #endregion

        #region [GetSkippedSchedules]
        protected List<Schedule> GetSkippedSchedules(ScheduleItem scheduleItem)
        {
            if (scheduleItem.LiveSchedule == null)
                return new List<Schedule>();

            var liveTrain = scheduleItem.LiveSchedule.Train;
            var scheduleIndex = liveTrain.Schedules.IndexOf(scheduleItem.LiveSchedule);

            var result = liveTrain.Schedules.Skip(scheduleIndex + 1).Where(s => s.Schedule.Handling == eHandling.StopPassengerTrain && s.IsCancelled).Select(s => s.Schedule);

            if (liveTrain.IsDestinationStationCancelled)
            {
                var destinationSchedule = GetDifferingDestinationSchedule(scheduleItem);
                result = result.Where(s => s.Station.ShortSymbol != destinationSchedule.Station.ShortSymbol && s.Time < destinationSchedule.Time);
            }

            return result.ToList();
        }
        #endregion

        #region [GetDisplayName]
        protected string GetDisplayName(Station station)
        {
            if (station.DisplayName.IsNullOrWhiteSpace())
                return station.Name;
            else
                return station.DisplayName;
        }
        #endregion

        #region [GetStationList]
        protected string GetStationList(List<Schedule> schedules)
        {
            if (!schedules.Any())
                return string.Empty;

            var stationNames = schedules.Select(s => GetDisplayName(s.Station)).ToList();

            if (stationNames.Count == 1)
                return stationNames.Single();

            return $"{string.Join(", ", stationNames.Take(stationNames.Count - 1))} und {stationNames.Last()}";
        }
        #endregion

        #region [MeasureString]
        protected double MeasureString(string candidate, double size)
        {
            var formattedText = new FormattedText(candidate, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, new Typeface("Segoe"), size, Brushes.Black, 1);
            return formattedText.Width;
        }
        #endregion

        #endregion

    }
}
