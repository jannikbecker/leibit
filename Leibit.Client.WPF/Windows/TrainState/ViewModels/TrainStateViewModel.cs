using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using System.Linq;

namespace Leibit.Client.WPF.Windows.TrainState.ViewModels
{
    public class TrainStateViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private Area m_Area;
        #endregion

        #region - Ctor -
        public TrainStateViewModel(Area area, int? trainNumber)
        {
            SaveCommand = new CommandHandler(__Save, false);
            m_Area = area;
            TrainNumber = trainNumber;
        }
        #endregion

        #region - Properties -

        #region [Caption]
        public string Caption => "Zugstatus eingeben";
        #endregion

        #region [TrainNumber]
        public int? TrainNumber
        {
            get => Get<int?>();
            set
            {
                Set(value);
                __Initialize();
            }
        }
        #endregion

        #region [CurrentSchedule]
        private LiveSchedule CurrentSchedule
        {
            get => Get<LiveSchedule>();
            set
            {
                Set(value);
                OnPropertyChanged(nameof(StationName));
                OnPropertyChanged(nameof(TrackName));
                OnPropertyChanged(nameof(CanCompose));
                OnPropertyChanged(nameof(CanPrepare));
                OnPropertyChanged(nameof(CanRevoke));
            }
        }
        #endregion

        #region [StationName]
        public string StationName
        {
            get
            {
                if (CurrentSchedule == null)
                    return string.Empty;

                return $"{CurrentSchedule.Schedule.Station.Name} ({CurrentSchedule.Schedule.Station.ShortSymbol})";
            }
        }
        #endregion

        #region [TrackName]
        public string TrackName => CurrentSchedule?.LiveTrack?.Name;
        #endregion

        #region [CanCompose]
        public bool CanCompose
        {
            get => !CurrentSchedule?.IsComposed ?? false;
        }
        #endregion

        #region [CanPrepare]
        public bool CanPrepare
        {
            get => !CurrentSchedule?.IsPrepared ?? false;
        }
        #endregion

        #region [CanRevoke]
        public bool CanRevoke
        {
            get => CurrentSchedule != null && (CurrentSchedule.IsComposed || CurrentSchedule.IsPrepared);
        }
        #endregion

        #region [TypeIsComposed]
        public bool TypeIsComposed
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [TypeIsPrepared]
        public bool TypeIsPrepared
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [TypeRevocation]
        public bool TypeRevocation
        {
            get => Get<bool>();
            set => Set(value);
        }
        #endregion

        #region [SaveCommand]
        public CommandHandler SaveCommand { get; }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Initialize]
        private void __Initialize()
        {
            if (TrainNumber.HasValue && m_Area.LiveTrains.ContainsKey(TrainNumber.Value))
                CurrentSchedule = m_Area.LiveTrains[TrainNumber.Value].Schedules.FirstOrDefault(s => s.Schedule.Handling == eHandling.Start
                                                                                                  && s.IsArrived && !s.IsDeparted);
            else
                CurrentSchedule = null;

            if (CurrentSchedule != null)
            {
                if (CurrentSchedule.IsPrepared)
                    TypeRevocation = true;
                else if (CurrentSchedule.IsComposed)
                    TypeIsPrepared = true;
                else
                    TypeIsComposed = true;
            }
            else
            {
                TypeIsComposed = false;
                TypeIsPrepared = false;
                TypeRevocation = false;
            }

            SaveCommand.SetCanExecute(CurrentSchedule != null);
        }
        #endregion

        #region [__Save]
        private void __Save()
        {
            // TODO: Implement me!
            OnCloseWindow();
        }
        #endregion

        #endregion

    }
}
