using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Core.Scheduling;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Scheduling;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.ExpectedDelay.ViewModels
{
    public class ExpectedDelayViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private Area m_Area;
        private TrainInformation m_Train;
        private LiveDataBLL m_LiveDataBll;
        private bool m_ArrivalChanged;
        private bool m_DepartureChanged;
        #endregion

        #region - Ctor -
        public ExpectedDelayViewModel(Area area, TrainInformation train, Schedule schedule)
        {
            m_Area = area;
            m_Train = train;
            SaveCommand = new CommandHandler(__Save, false);
            m_LiveDataBll = new LiveDataBLL();

            if (m_Train == null)
            {
                var createResult = m_LiveDataBll.CreateLiveTrainInformation(schedule.Train.Number, schedule.Station.ESTW);

                if (createResult.Succeeded)
                {
                    m_Train = createResult.Result;
                }
                else
                {
                    MessageBox.Show(createResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            var candidates = m_Train.Schedules.Where((s1, i1) => !m_Train.Schedules.Skip(i1 + 1).Any(s2 => s2.IsArrived)) // Must be first criterion due to indizes
                                              .Where(s => !s.IsDeparted)
                                              .Where(s => s.Schedule.Station.ESTW.Stations.Any(s2 => Runtime.VisibleStations.Contains(s2)))
                                              .GroupBy(s => new { s.Schedule.Station.ShortSymbol, s.Schedule.Time })
                                              .Select(g => g.FirstOrDefault());

            Schedules = new ObservableCollection<LiveSchedule>(candidates);
            SelectedSchedule = Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == schedule.Station.ShortSymbol && s.Schedule.Time == schedule.Time);

            m_ArrivalChanged = false;
            m_DepartureChanged = false;
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
                SaveCommand.SetCanExecute(value != null);

                if (SelectedSchedule == null)
                    return;

                if (SelectedSchedule.Schedule.Arrival != null)
                {
                    if (SelectedSchedule.ExpectedArrival == null)
                        ExpectedDelayArrival = 0;
                    else
                        ExpectedDelayArrival = (SelectedSchedule.ExpectedArrival - SelectedSchedule.Schedule.Arrival).TotalMinutes;

                    ArrivalVisibility = Visibility.Visible;
                }
                else
                    ArrivalVisibility = Visibility.Collapsed;

                if (SelectedSchedule.Schedule.Departure != null)
                {
                    if (SelectedSchedule.ExpectedDeparture == null)
                        ExpectedDelayDeparture = 0;
                    else
                        ExpectedDelayDeparture = (SelectedSchedule.ExpectedDeparture - SelectedSchedule.Schedule.Departure).TotalMinutes;

                    DepartureVisibility = Visibility.Visible;
                }
                else
                    DepartureVisibility = Visibility.Collapsed;

            }
        }
        #endregion

        #region [ExpectedDelayArrival]
        public int ExpectedDelayArrival
        {
            get => Get<int>();
            set
            {
                Set(value);
                ExpectedArrival = SelectedSchedule.Schedule.Arrival.AddMinutes(value);
                m_ArrivalChanged = true;
                OnPropertyChanged(nameof(DelayArrivalString));
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
                m_DepartureChanged = true;
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
                var expectedArrival = SelectedSchedule.ExpectedArrival ?? arrival;
                var delay = (expectedArrival - arrival).TotalMinutes;
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

        #region [ExpectedArrival]
        public LeibitTime ExpectedArrival
        {
            get => Get<LeibitTime>();
            set => Set(value);
        }
        #endregion

        #region [ExpectedDeparture]
        public LeibitTime ExpectedDeparture
        {
            get => Get<LeibitTime>();
            set => Set(value);
        }
        #endregion

        #region [ArrivalVisibility]
        public Visibility ArrivalVisibility
        {
            get => Get<Visibility>();
            set => Set(value);
        }
        #endregion

        #region [DepartureVisibility]
        public Visibility DepartureVisibility
        {
            get => Get<Visibility>();
            set => Set(value);
        }
        #endregion

        #region [SaveCommand]
        public CommandHandler SaveCommand { get; }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Save]
        private void __Save()
        {
            if (SelectedSchedule == null)
                return;

            var schedule = SelectedSchedule;

            // Set LastModified before adding train to LiveTrain list.
            m_Train.LastModified = schedule.Schedule.Station.ESTW.Time;

            // It could happen that live data has been generated while the window was open.
            var liveTrain = m_Area.LiveTrains.GetOrAdd(m_Train.Train.Number, m_Train);
            schedule = liveTrain.Schedules.FirstOrDefault(s => s.Schedule.Station.ShortSymbol == SelectedSchedule.Schedule.Station.ShortSymbol && s.Schedule.Time == SelectedSchedule.Schedule.Time);

            var result = m_LiveDataBll.SetExpectedDelay(schedule, m_ArrivalChanged ? (int?)ExpectedDelayArrival : null, m_DepartureChanged ? (int?)ExpectedDelayDeparture : null);

            if (result.Succeeded)
            {
                OnStatusBarTextChanged($"Voraussichtliche Verspätung für Zug {m_Train.Train.Number} in {schedule.Schedule.Station.ShortSymbol} eingetragen");
                OnCloseWindow();
            }
            else
                MessageBox.Show(result.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        #endregion

        #endregion

    }
}
