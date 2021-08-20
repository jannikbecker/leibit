using Leibit.Entities;
using Leibit.Entities.Common;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Leibit.Client.WPF.Windows.Display.ViewModels
{
    public class DepartureBoardViewModel : DisplayViewModelBase
    {

        #region - Ctor -
        public DepartureBoardViewModel(DisplayViewModel parent)
           : base(parent)
        {
            Items = new ObservableCollection<DepartureBoardItemViewModel>();
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

            var count = SelectedDisplayType.Type == eDisplayType.DepartureBoard_Small ? 10 : 12;
            var candidates = GetScheduleCandidates(area, 120, false);
            var orderedSchedules = candidates.Where(x => x.Schedule.Handling == eHandling.Start || x.Schedule.Handling == eHandling.StopPassengerTrain).OrderBy(x => x.Schedule.Departure).Take(count);
            var currentItems = new List<DepartureBoardItemViewModel>();

            foreach (var scheduleItem in orderedSchedules)
            {
                var currentItem = Items.FirstOrDefault(x => x.Schedule == scheduleItem.Schedule);

                if (currentItem == null)
                {
                    currentItem = new DepartureBoardItemViewModel(scheduleItem);
                    currentItem.TrainNumber = GetTrainNumber(scheduleItem);

                    if (SelectedDisplayType.Type == eDisplayType.DepartureBoard_Small)
                        currentItem.Via = GetViaString(scheduleItem, 16, 240);
                    else
                        currentItem.Via = GetViaString(scheduleItem, 14, 230);

                    Dispatcher.Invoke(() => Items.Add(currentItem));
                }

                var track = scheduleItem.LiveSchedule?.LiveTrack ?? scheduleItem.Schedule.Track;
                currentItem.TrackName = GetTrackName(track);

                var infoTexts = new List<string>();
                var delay = GetDelayMinutes(scheduleItem);

                if (delay == 1)
                    infoTexts.Add("Wenige Minuten später");
                else if (delay >= 5)
                    infoTexts.Add($"ca. {delay} Minuten später");

                currentItem.IsTrackChanged = IsTrackChanged(scheduleItem);

                if (currentItem.IsTrackChanged && SelectedDisplayType.Type == eDisplayType.DepartureBoard_Small)
                    infoTexts.Add($"Heute von Gleis {GetTrackName(scheduleItem.LiveSchedule.LiveTrack)}");

                if (infoTexts.Any())
                {
                    if (SelectedDisplayType.Type == eDisplayType.DepartureBoard_Small)
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

    }
}
