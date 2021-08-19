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
            if (SelectedStation == null || SelectedTrack == null)
            {
                LEDText = "...";
                return;
            }


            var candidates = GetScheduleCandidates(area, 30);
            var orderedSchedules = candidates.OrderBy(x => x.ReferenceTime);
            var textsToDisplay = new List<string>();
            textsToDisplay.Add($"Zeit: {SelectedStation.ESTW.Time} Uhr");

            var nextTrain = candidates.FirstOrDefault();

            if (nextTrain != null && nextTrain.Schedule.Time < SelectedStation.ESTW.Time.AddMinutes(10))
            {
                var track = nextTrain.LiveSchedule?.LiveTrack ?? nextTrain.Schedule.Track;
                textsToDisplay.Add($"{__GetLEDBaseText(nextTrain)} auf Gleis {GetTrackName(track)}");
            }

            foreach (var currentItem in orderedSchedules)
            {
                string infoText = string.Empty;
                var delay = GetDelayMinutes(currentItem);

                if (delay > 0)
                    infoText = delay == 1 ? "wenige Minuten später" : $"circa {delay} Minuten später";

                if (currentItem.LiveSchedule != null && (currentItem.Schedule.Track == SelectedTrack || currentItem.Schedule.Track == SelectedTrack.Parent) && currentItem.LiveSchedule.LiveTrack != null && currentItem.Schedule.Track != currentItem.LiveSchedule.LiveTrack)
                {
                    if (infoText.IsNotNullOrEmpty())
                        infoText += " und ";

                    infoText += $"von Gleis {GetTrackName(currentItem.LiveSchedule.LiveTrack)}";
                }

                if (infoText.IsNotNullOrEmpty())
                    textsToDisplay.Add($"Information zu {__GetLEDBaseText(currentItem)}, heute {infoText}.");
            }

            LEDText = LED_SEPARATOR + string.Join(LED_SEPARATOR, textsToDisplay);
            IsLEDSliding = true;
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
            if (scheduleItem.Schedule.Handling == eHandling.Destination)
                return $"{scheduleItem.Schedule.Train.Type} {scheduleItem.Schedule.Train.Number} von {scheduleItem.Schedule.Train.Start}, Ankunft {scheduleItem.Schedule.Arrival} Uhr";
            else
                return $"{scheduleItem.Schedule.Train.Type} {scheduleItem.Schedule.Train.Number} nach {scheduleItem.Schedule.Train.Destination}, Abfahrt {scheduleItem.Schedule.Departure} Uhr";
        }
        #endregion

        #endregion

    }
}
