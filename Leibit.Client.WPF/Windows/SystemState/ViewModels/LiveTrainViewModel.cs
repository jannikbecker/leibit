using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Core.Scheduling;
using Leibit.Entities;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using System.Linq;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.SystemState.ViewModels
{
    public class LiveTrainViewModel : ViewModelBase
    {

        #region - Ctor -
        public LiveTrainViewModel(Area area, TrainInformation train)
        {
            Area = area;
            CurrentTrain = train;
            DeleteLiveTrainCommand = new CommandHandler(__Delete, true);
        }
        #endregion

        #region - Properties -

        public Area Area { get; private set; }

        #region [CurrentTrain]
        public TrainInformation CurrentTrain { get; private set; }
        #endregion

        #region [DeleteLiveTrainCommand]
        public ICommand DeleteLiveTrainCommand { get; private set; }
        #endregion

        #region [Actions]
        // Dummy, don't delete
        public string Actions { get; set; }
        #endregion

        #region [Direction]
        public string Direction
        {
            get
            {
                if (CurrentTrain.Block == null)
                    return string.Empty;

                switch (CurrentTrain.Direction)
                {
                    case eBlockDirection.Left:
                        return "links";
                    case eBlockDirection.Right:
                        return "rechts";
                    default:
                        return string.Empty;
                }
            }
        }
        #endregion

        #region [LastModified]
        public LeibitTime LastModified
        {
            get => CurrentTrain.LastModification.Values.OrderByDescending(x => x).FirstOrDefault();
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh()
        {
            OnPropertyChanged(null);
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Delete]
        private void __Delete()
        {
            Area.LiveTrains.TryRemove(CurrentTrain.Train.Number, out _);
        }
        #endregion

        #endregion

    }
}
