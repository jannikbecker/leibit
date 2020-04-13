using Leibit.Core.Client.BaseClasses;
using Leibit.Entities;
using Leibit.Entities.LiveData;

namespace Leibit.Client.WPF.Windows.SystemState.ViewModels
{
    public class LiveTrainViewModel : ViewModelBase
    {

        #region - Ctor -
        public LiveTrainViewModel(TrainInformation train)
        {
            CurrentTrain = train;
        }
        #endregion

        #region - Properties -

        #region [CurrentTrain]
        public TrainInformation CurrentTrain { get; private set; }
        #endregion

        //#region [TrainNumber]
        //public int TrainNumber => CurrentTrain.Train.Number;
        //#endregion

        //#region [Block]
        //public string Block => CurrentTrain.Block?.Name ?? string.Empty;
        //#endregion

        //#region [Station]
        //public string Station => CurrentTrain.Block?.Track?.Station?.ShortSymbol ?? string.Empty;
        //#endregion

        //#region [Track]
        //public string Track => CurrentTrain.Block?.Track?.Name ?? string.Empty;
        //#endregion

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

        //#region [LastModified]
        //public LeibitTime LastModified => CurrentTrain.LastModified;
        //#endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh()
        {
            OnPropertyChanged(null);
        }
        #endregion

        #endregion

    }
}
