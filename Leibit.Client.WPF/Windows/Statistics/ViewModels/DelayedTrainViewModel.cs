using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Common;
using Leibit.Entities.LiveData;
using System.Linq;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class DelayedTrainViewModel : ViewModelBase
    {
        #region - Ctor -
        public DelayedTrainViewModel(TrainInformation train)
        {
            CurrentTrain = train;
            TrainType = train.Train.Type;
            TrainNumber = train.Train.Number;
            DelayString = $"+{train.Delay}";
            Reasons = string.Join(",", train.Schedules.SelectMany(s => s.Delays).Where(d => d.Reason.IsNotNullOrEmpty()).Select(d => $"{d.Reason} (+{d.Minutes} von {d.Schedule.Schedule.Station.ShortSymbol})"));
        }
        #endregion

        #region - Properties -

        #region [CurrentTrain]
        public TrainInformation CurrentTrain
        {
            get => Get<TrainInformation>();
            private set => Set(value);
        }
        #endregion

        #region [TrainType]
        public string TrainType
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [TrainNumber]
        public int TrainNumber
        {
            get => Get<int>();
            private set => Set(value);
        }
        #endregion

        #region [DelayString]
        public string DelayString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [Reasons]
        public string Reasons
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #endregion
    }
}
