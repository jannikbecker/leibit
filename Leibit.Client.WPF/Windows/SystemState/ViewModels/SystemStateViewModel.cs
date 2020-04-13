using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Entities.Common;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.SystemState.ViewModels
{
    public class SystemStateViewModel : ChildWindowViewModelBase, IRefreshable
    {

        #region - Ctor -
        public SystemStateViewModel(Dispatcher dispatcher)
        {
            Dispatcher = dispatcher;
        }
        #endregion

        #region - Properties -

        #region [Dispatcher]
        public Dispatcher Dispatcher
        {
            get;
            private set;
        }
        #endregion

        #region [Estws]
        public ObservableCollection<ESTWViewModel> Estws
        {
            get => Get<ObservableCollection<ESTWViewModel>>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [Refresh]
        public void Refresh(Area Area)
        {
            if (Estws == null)
                Estws = new ObservableCollection<ESTWViewModel>();

            foreach (var estw in Area.ESTWs)
            {
                if (!estw.IsLoaded)
                    continue;

                var isNew = false;
                var current = Estws.FirstOrDefault(e => e.CurrentEstw.Id == estw.Id);

                if (current == null)
                {
                    current = new ESTWViewModel(estw);
                    isNew = true;
                }

                current.Time = estw.Time;
                current.IsActive = (DateTime.Now - current.CurrentEstw.LastUpdatedOn).TotalSeconds < 30;

                if (isNew)
                    Dispatcher.Invoke(() => Estws.Add(current));
            }
        }
        #endregion

        #endregion

    }
}
