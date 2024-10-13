using Leibit.Core.Common;
using Leibit.Entities;
using Leibit.Entities.Common;
using System.Collections.Generic;
using System.Linq;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class PassengerInformationViewModel : DisplayViewModelBase
    {
        #region - Const -
        private const string LED_SEPARATOR = "          ###          ";
        #endregion

        #region - Ctor -
        public PassengerInformationViewModel(DisplayViewModel parent)
          : base(parent)
        {
        }
        #endregion

        #region - Properties -

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
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region - Overrides -

        #region [Refresh]
        internal override void Refresh(Area area)
        {
            if (SelectedStation == null || !SelectedTracks.Any())
            {
                LEDText = "...";
                IsLEDSliding = true;
                return;
            }


            var candidates = GetScheduleCandidates(area, 30, true);
            var orderedSchedules = candidates.OrderBy(x => x.ReferenceTime);
            var textsToDisplay = new List<string>();
            textsToDisplay.Add($"Zeit: {SelectedStation.ESTW.Time} Uhr");

            var nextTrains = orderedSchedules.Where(x => x.LiveSchedule?.IsCancelled != true
                                                      && !(IsDestination(x) && x.Schedule.Handling == eHandling.Start)
                                                      && x.Schedule.Time < SelectedStation.ESTW.Time.AddMinutes(10));

            foreach (var nextTrain in nextTrains)
            {
                var track = nextTrain.LiveSchedule?.LiveTrack ?? nextTrain.Schedule.Track;
                textsToDisplay.Add($"{__GetLEDBaseText(nextTrain)} auf Gleis {GetTrackName(track)}");
            }

            foreach (var currentItem in orderedSchedules)
            {
                string infoText = string.Empty;

                if (currentItem.LiveSchedule?.IsCancelled != true)
                {
                    var delay = GetDelayMinutes(currentItem);

                    if (delay > 0)
                        infoText = delay == 1 ? "wenige Minuten später" : $"circa {delay} Minuten später";

                    if (IsTrackChanged(currentItem) && (SelectedTracks.Contains(currentItem.Schedule.Track) || SelectedTracks.Any(t => t.Parent == currentItem.Schedule.Track)))
                    {
                        if (infoText.IsNotNullOrEmpty())
                            infoText += " und ";

                        infoText += $"von Gleis {GetTrackName(currentItem.LiveSchedule.LiveTrack)}";
                    }

                    if (infoText.IsNotNullOrEmpty())
                        textsToDisplay.Add($"Information zu {__GetLEDBaseText(currentItem)}, heute {infoText}.");
                }

                infoText = string.Empty;

                if (currentItem.LiveSchedule != null)
                {
                    if (currentItem.LiveSchedule.IsCancelled || (IsDestination(currentItem) && currentItem.Schedule.Handling == eHandling.Start))
                        textsToDisplay.Add($"Information zu {__GetLEDBaseText(currentItem)}, fällt heute leider aus.");
                    else
                    {
                        var differingDestination = GetDifferingDestinationSchedule(currentItem);

                        if (differingDestination != null)
                            infoText = $"fährt heute nur bis {GetDisplayName(differingDestination.Station)}";

                        var skippedStations = GetSkippedSchedules(currentItem);

                        if (skippedStations.Any())
                        {
                            if (infoText.IsNotNullOrEmpty())
                                infoText += " und ";

                            infoText += $"hält nicht in {GetStationList(skippedStations)}";
                        }
                    }

                    if (infoText.IsNotNullOrEmpty())
                        textsToDisplay.Add($"Information zu {__GetLEDBaseText(currentItem)}, {infoText}.");
                }
            }

            if (textsToDisplay.Count == 1)
            {
                LEDText = textsToDisplay[0];
                IsLEDSliding = false;
            }
            else
            {
                LEDText = LED_SEPARATOR + string.Join(LED_SEPARATOR, textsToDisplay);
                IsLEDSliding = true;
            }
        }
        #endregion

        #region [SelectionChanged]
        internal override void SelectionChanged()
        {
            IsLEDSliding = false;
        }
        #endregion

        #endregion

        #region - Private helper -

        #region [__GetLEDBaseText]
        private string __GetLEDBaseText(ScheduleItem scheduleItem)
        {
            if (IsDestination(scheduleItem) && scheduleItem.Schedule.Arrival != null)
                return $"{scheduleItem.Schedule.TrainType} {scheduleItem.Schedule.Train.Number} von {scheduleItem.Schedule.Start}, Ankunft {scheduleItem.Schedule.Arrival} Uhr";
            else
                return $"{scheduleItem.Schedule.TrainType} {scheduleItem.Schedule.Train.Number} nach {scheduleItem.Schedule.Destination}, Abfahrt {scheduleItem.Schedule.Departure} Uhr";
        }
        #endregion

        #endregion

    }
}
