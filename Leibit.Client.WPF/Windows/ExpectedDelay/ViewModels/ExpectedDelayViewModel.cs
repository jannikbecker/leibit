using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.ExpectedDelay.ViewModels
{
    public class ExpectedDelayViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private TrainInformation m_Train;
        #endregion

        #region - Ctor -
        public ExpectedDelayViewModel(TrainInformation train, Schedule schedule)
        {
            m_Train = train;

            var candidates = train.Schedules.Where(s => s.Schedule.Handling != eHandling.Destination && !s.IsDeparted)
                                            .Where(s => s.Schedule.Station.ESTW.Stations.Any(s2 => Runtime.VisibleStations.Contains(s2)))
                                            .GroupBy(s => new { s.Schedule.Station.ShortSymbol, s.Schedule.Time })
                                            .Select(g => g.FirstOrDefault());

            Schedules = new ObservableCollection<LiveSchedule>(candidates);
            SelectedSchedule = Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == schedule.Station.ShortSymbol && s.Schedule.Time == schedule.Time);

            SaveCommand = new CommandHandler(__Save, true);
        }
        #endregion

        #region - Properties -

        #region [Caption]
        public string Caption => $"Vsl. Verspätung {m_Train.Train.Number}";
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
                OnPropertyChanged(nameof(DelayArrivalString));

                if (SelectedSchedule != null)
                    ExpectedDelayDeparture = (SelectedSchedule.ExpectedDeparture - SelectedSchedule.Schedule.Departure).TotalMinutes;
            }
        }
        #endregion

        #region [ExpectedDelayDeparture]
        public int ExpectedDelayDeparture
        {
            get => Get<int>();
            set
            {
                Set(value);
                ExpectedDeparture = SelectedSchedule.Schedule.Departure.AddMinutes(value);
                OnPropertyChanged(nameof(DelayDepartureString));
            }
        }
        #endregion

        #region [DelayArrivalString]
        public string DelayArrivalString
        {
            get
            {
                if (SelectedSchedule == null)
                    return string.Empty;

                var arrival = SelectedSchedule.Schedule.Arrival ?? SelectedSchedule.Schedule.Departure;
                var delay = (SelectedSchedule.ExpectedArrival - arrival).TotalMinutes;
                return delay >= 0 ? $"+{delay}" : delay.ToString();
            }
        }
        #endregion

        #region [DelayDepartureString]
        public string DelayDepartureString
        {
            get
            {
                if (SelectedSchedule == null)
                    return string.Empty;

                var delay = (ExpectedDeparture - SelectedSchedule.Schedule.Departure).TotalMinutes;
                return delay >= 0 ? $"+{delay}" : delay.ToString();
            }
        }
        #endregion

        #region [ExpectedDeparture]
        public LeibitTime ExpectedDeparture
        {
            get => Get<LeibitTime>();
            set => Set(value);
        }
        #endregion

        #region [SaveCommand]
        public ICommand SaveCommand { get; }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Save]
        private void __Save()
        {
            OnCloseWindow();
        }
        #endregion

        #endregion

    }
}
