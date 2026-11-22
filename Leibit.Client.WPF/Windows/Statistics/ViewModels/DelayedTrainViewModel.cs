using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Common;
using Leibit.Entities.LiveData;
using Leibit.Entities.Statistics;
using System.Linq;

namespace Leibit.Client.WPF.Windows.Statistics.ViewModels
{
    public class DelayedTrainViewModel : ViewModelBase
    {
        #region - Ctor -
        public DelayedTrainViewModel(TrainDelayStatistic statistic)
        {
            CurrentTrain = statistic.Train;
            TrainType = CurrentTrain.Train.Type;
            TrainNumber = CurrentTrain.Train.Number;
            EntryDelayString = __FormatDelay(statistic.EntryDelay);
            ExitDelayString = __FormatDelay(statistic.ExitDelay);
            NetDelayString = __FormatDelay(statistic.NetDelay);
            Reasons = string.Join(",", CurrentTrain.Schedules.SelectMany(s => s.Delays).Where(d => d.Reason.IsNotNullOrEmpty()).Select(d => $"{d.Reason} (+{d.Minutes} von {d.Schedule.Schedule.Station.ShortSymbol})"));
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

        #region [EntryDelayString]
        public string EntryDelayString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [ExitDelayString]
        public string ExitDelayString
        {
            get => Get<string>();
            private set => Set(value);
        }
        #endregion

        #region [NetDelayString]
        public string NetDelayString
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

        #region - Private methods -

        #region [__FormatDelay]
        private string __FormatDelay(int delay)
        {
            if (delay < 0)
                return delay.ToString();
            else
                return $"+{delay}";
        }
        #endregion

        #endregion
    }
}
