using Leibit.Client.WPF.Interfaces;
using Leibit.Client.WPF.ViewModels;
using Leibit.Entities.Common;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.ESTWSelection.ViewModels
{
    public class ESTWSelectionViewModel : ChildWindowViewModelBase, IRefreshable
    {

        #region - Ctor -
        public ESTWSelectionViewModel(Dispatcher Dispatcher)
            : base()
        {
            this.Dispatcher = Dispatcher;
        }
        #endregion

        #region - Properties -

        #region [Name]
        public string Name
        {
            get
            {
                return Get<string>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [Estws]
        public ObservableCollection<ESTWSelectionESTWViewModel> Estws
        {
            get
            {
                return Get<ObservableCollection<ESTWSelectionESTWViewModel>>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #region [Dispatcher]
        public Dispatcher Dispatcher
        {
            get;
            private set;
        }
        #endregion

        #region [NeedsRefresh]
        public bool NeedsRefresh
        {
            get;
            set;
        }
        #endregion

        #region [HasEstws]
        public bool HasEstws
        {
            get
            {
                return Get<bool>();
            }
            private set
            {
                Set(value);
            }
        }
        #endregion

        #endregion

        #region - Public methods -

        public void Refresh(Area Area)
        {
            Name = Area.Name;

            if (Estws == null)
                Estws = new ObservableCollection<ESTWSelectionESTWViewModel>();

            foreach (var Estw in Area.ESTWs)
            {
                if (!Estw.IsLoaded || Estws.Any(e => e.Id == Estw.Id))
                    continue;

                var VM = new ESTWSelectionESTWViewModel(Estw);
                Dispatcher.Invoke(() => Estws.Add(VM));
            }

            HasEstws = Estws.Count > 0;
        }

        #endregion

    }
}
