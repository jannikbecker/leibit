using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Common;
using Leibit.Entities.LiveData;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Leibit.Client.WPF.Windows.DelayJustification.ViewModels
{
    public class DelayJustificationViewModel : ChildWindowViewModelBase
    {

        #region - Ctor -
        public DelayJustificationViewModel(TrainInformation LiveTrain)
        {
            CurrentTrain = LiveTrain;

            Delays = LiveTrain.Schedules.SelectMany(s => s.Delays.Where(d => d.Reason.IsNullOrWhiteSpace() && d.Schedule.Schedule.Station.ESTW.Stations.Any(st => Runtime.VisibleStations.Contains(st))))
                                        .Select(delay => new DelayInfoViewModel(delay)).ToObservableCollection();
            Delays.ForEach(d => d.DelaySaved += __DelaySaved);

            if (Delays.Count > 0)
                Delays.Last().IsLast = true;
        }
        #endregion

        #region - Properties -

        #region [Caption]
        public string Caption
        {
            get
            {
                return String.Format("Verspätungsbegründung {0}", CurrentTrain.Train.Number);
            }
        }
        #endregion

        #region [CurrentTrain]
        public TrainInformation CurrentTrain
        {
            get;
            private set;
        }
        #endregion

        #region [Delays]
        public ObservableCollection<DelayInfoViewModel> Delays
        {
            get;
            private set;
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__DelaySaved]
        private void __DelaySaved(object sender, EventArgs e)
        {
            var delay = sender as DelayInfoViewModel;
            Delays.Remove(delay);
            OnStatusBarTextChanged($"Verspätung für Zug {CurrentTrain.Train.Number} begründet");

            if (Delays.Count == 0)
                OnCloseWindow();
            else
                Delays.Last().IsLast = true;
        }
        #endregion

        #endregion

    }
}
