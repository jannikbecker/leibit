using Leibit.BLL;
using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.Scheduling;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class ArrivalBoardViewModel : DisplayViewModelBase
    {

        #region - Needs -
        private readonly CalculationBLL m_CalculationBll;
        #endregion

        #region - Ctor -
        public ArrivalBoardViewModel(DisplayViewModel parent)
           : base(parent)
        {
            Items = new ObservableCollection<DepartureBoardItemViewModel>();
            m_CalculationBll = new CalculationBLL();
        }
        #endregion

        #region - Properties -

        #region [Items]
        public ObservableCollection<DepartureBoardItemViewModel> Items
        {
            get => Get<ObservableCollection<DepartureBoardItemViewModel>>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region [Refresh]
        internal override void Refresh(Area area)
        {
            if (SelectedStation == null)
            {
                Dispatcher.Invoke(() => Items.Clear());
                return;
            }

            var count = SelectedDisplayType.Type == eDisplayType.ArrivalBoard_Small ? 10 : 12;
            var candidates = GetScheduleCandidates(area, 120, false);
            var orderedSchedules = candidates.Where(x => x.Schedule.Handling == eHandling.Destination || x.Schedule.Handling == eHandling.StopPassengerTrain)
                                             .Where(x => !__IsStart(x, false))
                                             .OrderBy(x => x.Schedule.Arrival)
                                             .Take(count);
            var currentItems = new List<DepartureBoardItemViewModel>();

            foreach (var scheduleItem in orderedSchedules)
            {
                var currentItem = Items.FirstOrDefault(x => x.Schedule == scheduleItem.Schedule);

                if (currentItem == null)
                {
                    currentItem = new DepartureBoardItemViewModel(scheduleItem, true);
                    currentItem.TrainNumber = GetTrainNumber(scheduleItem);

                    int indexToInsert;
                    for (indexToInsert = 0; indexToInsert < Items.Count && Items[indexToInsert].Time <= scheduleItem.Schedule.Arrival; indexToInsert++) ;

                    Dispatcher.Invoke(() => Items.Insert(indexToInsert, currentItem));
                }

                if (SelectedDisplayType.Type == eDisplayType.ArrivalBoard_Small)
                    currentItem.Via = __GetViaString(scheduleItem, 16, 240);
                else
                    currentItem.Via = __GetViaString(scheduleItem, 14, 230);

                var infoTexts = new List<string>();
                var isStart = __IsStart(scheduleItem);
                var isDestination = IsDestination(scheduleItem);

                if (isDestination && scheduleItem.LiveSchedule?.IsCancelled != true && currentItem.Schedule.Handling != eHandling.Start)
                {
                    var followUpTrain = GetFollowUpTrain(scheduleItem, area);

                    if (followUpTrain != null)
                        infoTexts.Add($"Dieser Zug endet hier und fährt weiter als {GetTrainNumber(followUpTrain)} nach {followUpTrain.Destination}");
                    else
                        infoTexts.Add("Dieser Zug endet hier");
                }

                if (currentItem.Schedule.TwinScheduleArrival != null && currentItem.Schedule.TwinScheduleDeparture == null && !isDestination)
                    infoTexts.Add("Zug wird hier geteilt");

                if (scheduleItem.LiveSchedule?.IsCancelled != true && !isStart)
                {
                    var track = scheduleItem.LiveSchedule?.LiveTrack ?? scheduleItem.Schedule.Track;
                    currentItem.TrackName = GetTrackName(track);

                    var delay = __GetDelayMinutes(scheduleItem);

                    if (delay == 1)
                        infoTexts.Add("Wenige Minuten später");
                    else if (delay >= 5)
                        infoTexts.Add($"ca. {delay} Minuten später");

                    currentItem.IsTrackChanged = IsTrackChanged(scheduleItem);

                    if (currentItem.IsTrackChanged && SelectedDisplayType.Type == eDisplayType.ArrivalBoard_Small)
                        infoTexts.Add($"Heute von Gleis {GetTrackName(scheduleItem.LiveSchedule.LiveTrack)}");
                }
                else
                {
                    currentItem.TrackName = GetTrackName(scheduleItem.Schedule.Track);
                    currentItem.IsTrackChanged = false;
                }

                if (isStart)
                {
                    currentItem.Destination = scheduleItem.Schedule.Start;
                    infoTexts.Add("Zug fällt heute aus");
                }
                else
                    currentItem.Destination = scheduleItem.Schedule.Start;

                if (scheduleItem.LiveSchedule != null)
                {
                    if (scheduleItem.LiveSchedule.IsCancelled)
                        infoTexts.Add("Zug fällt heute aus");
                    else
                    {
                        var differingDestination = GetDifferingDestinationSchedule(scheduleItem);

                        if (differingDestination != null)
                        {
                            if (differingDestination == scheduleItem.Schedule)
                                infoTexts.Add($"Keine Weiterfahrt bis {scheduleItem.Schedule.Destination}");
                            else
                                infoTexts.Add($"Fährt heute nur bis {GetDisplayName(differingDestination.Station)}");
                        }
                    }
                }

                if (infoTexts.Any())
                {
                    if (SelectedDisplayType.Type == eDisplayType.ArrivalBoard_Small)
                        currentItem.InfoText = " - " + string.Join(" - ", infoTexts) + " - ";
                    else
                        currentItem.InfoText = string.Join(" - ", infoTexts) + " - ";
                }
                else
                    currentItem.InfoText = string.Empty;

                currentItems.Add(currentItem);
            }

            var removedItems = Items.Except(currentItems).ToList();
            Dispatcher.Invoke(() => removedItems.ForEach(x => Items.Remove(x)));

            for (int i = 0; i < Items.Count; i++)
            {
                if (i > 0 && i % 2 == 0)
                    Items[i].Margin = new System.Windows.Thickness(0, 5, 0, 0);
                else
                    Items[i].Margin = new System.Windows.Thickness(0, 0, 0, 0);
            }
        }
        #endregion

        #region - Private methods -

        #region [IsStart]
        private bool __IsStart(ScheduleItem scheduleItem, bool considerCancellations = true)
        {
            if (scheduleItem.Schedule.Handling == eHandling.Start)
                return true;

            if (scheduleItem.Schedule.Handling == eHandling.StopPassengerTrain || scheduleItem.Schedule.Handling == eHandling.StopFreightTrain || scheduleItem.Schedule.Handling == eHandling.Destination)
            {
                var schedulesResult = m_CalculationBll.GetSchedulesByTime(scheduleItem.Schedule.Train.Schedules, scheduleItem.Schedule.Station.ESTW.Time);

                if (schedulesResult.Succeeded)
                {
                    var schedules = schedulesResult.Result;
                    var scheduleIndex = schedules.IndexOf(scheduleItem.Schedule);
                    var previousSchedule = schedules.Take(scheduleIndex).LastOrDefault();

                    if (previousSchedule != null && !previousSchedule.TrainType.IsPassengerTrain())
                        return true;
                }

                if (considerCancellations && scheduleItem.LiveSchedule != null && !scheduleItem.LiveSchedule.IsCancelled)
                {
                    var liveTrain = scheduleItem.LiveSchedule.Train;
                    var scheduleIndex = liveTrain.Schedules.IndexOf(scheduleItem.LiveSchedule);
                    var previousStops = liveTrain.Schedules.Take(scheduleIndex).Where(s => s.Schedule.Handling == eHandling.StopPassengerTrain);

                    if (previousStops.Any() && previousStops.All(s => s.IsCancelled))
                        return true;
                }
            }

            return false;
        }
        #endregion

        #region [__GetDelayMinutes]
        private int __GetDelayMinutes(ScheduleItem scheduleItem)
        {
            if (scheduleItem.LiveSchedule == null || scheduleItem.LiveSchedule.IsCancelled)
                return 0;

            var scheduledTime = scheduleItem.Schedule.Arrival;
            var expectedTime = scheduleItem.LiveSchedule.ExpectedArrival;

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

        #region [__GetViaString]
        private string __GetViaString(ScheduleItem scheduleItem, double fontSize, double maxSpace)
        {
            var schedulesResult = m_CalculationBll.GetSchedulesByTime(scheduleItem.Schedule.Train.Schedules, scheduleItem.Schedule.Station.ESTW.Time);

            if (!schedulesResult.Succeeded)
                return string.Empty;

            var schedules = schedulesResult.Result;
            var scheduleIndex = schedules.IndexOf(scheduleItem.Schedule);
            var previousSchedules = schedules.Take(scheduleIndex);
            var candidateSchedules = previousSchedules.Where(x => x.Handling == eHandling.StopPassengerTrain);
            var viaSchedules = new List<Schedule>();
            var currentViaString = string.Empty;
            var result = string.Empty;

            if (scheduleItem.LiveSchedule != null && !scheduleItem.LiveSchedule.IsCancelled)
            {
                // Remove all cancelled stations
                candidateSchedules = candidateSchedules.Where(s =>
                {
                    var liveSchedule = scheduleItem.LiveSchedule.Train.Schedules.FirstOrDefault(s2 => s.Station.ShortSymbol == s2.Schedule.Station.ShortSymbol && s.Time == s2.Schedule.Time);

                    if (liveSchedule == null)
                        return true;

                    return !liveSchedule.IsCancelled;
                });
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

        #endregion

    }
}
