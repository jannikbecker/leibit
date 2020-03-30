using Leibit.BLL;
using Leibit.Client.WPF.Common;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using Leibit.Entities.LiveData;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Client.WPF.Windows.DelayJustification.ViewModels
{
    public class DelayJustificationViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private LiveDataBLL m_LiveDataBll;

        private CommandHandler m_SaveCommand;
        private CommandHandler m_CancelCommand;
        #endregion

        #region - Ctor -
        public DelayJustificationViewModel(TrainInformation LiveTrain)
        {
            CurrentTrain = LiveTrain;

            Delays = LiveTrain.Schedules.SelectMany(s => s.Delays.Where(d => d.Reason.IsNullOrWhiteSpace() && d.Schedule.Schedule.Station.ESTW.Stations.Any(st => Runtime.VisibleStations.Contains(st))))
                                        .Select(delay => new DelayInfoViewModel(delay)).ToObservableCollection();
            Delays.ForEach(d => d.PropertyChanged += __Delay_PropertyChanged);

            if (Delays.Count > 0)
                Delays.Last().IsLast = true;

            m_LiveDataBll = new LiveDataBLL();
            m_SaveCommand = new CommandHandler(__Save, false);
            m_CancelCommand = new CommandHandler(__Cancel, true);
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

        #region [SaveCommand]
        public ICommand SaveCommand
        {
            get
            {
                return m_SaveCommand;
            }
        }
        #endregion

        #region [CancelCommand]
        public ICommand CancelCommand
        {
            get
            {
                return m_CancelCommand;
            }
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

        #region [__Save]
        private void __Save()
        {
            foreach (var Delay in Delays.ToList())
            {
                var DelayInfo = Delay.CurrentDelay;
                DelayInfo.Reason = Delay.Reason;
                DelayInfo.CausedBy = Delay.CausedBy;

                var SaveResult = m_LiveDataBll.JustifyDelay(DelayInfo);

                if (SaveResult.Succeeded)
                    Delays.Remove(Delay);
                else
                    MessageBox.Show(SaveResult.Message, "Fehler", MessageBoxButton.OK, MessageBoxImage.Error);
            }

            if (Delays.Count == 0)
            {
                OnStatusBarTextChanged(String.Format("Verspätung für Zug {0} begründet", CurrentTrain.Train.Number));
                OnCloseWindow();
            }
        }
        #endregion

        #region [__Cancel]
        private void __Cancel()
        {
            OnCloseWindow();
        }
        #endregion

        #region [__Delay_PropertyChanged]
        private void __Delay_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Reason")
                m_SaveCommand.SetCanExecute(Delays.All(d => d.Reason.IsNotNullOrWhiteSpace()));
        }
        #endregion

        #endregion

    }
}
