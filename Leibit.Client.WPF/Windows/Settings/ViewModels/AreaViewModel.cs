using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Common;
using Leibit.Entities.Common;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Leibit.Client.WPF.Windows.Settings.ViewModels
{
    public class AreaViewModel : ViewModelBase
    {

        #region - Needs -
        private Entities.Settings.Settings m_Settings;
        #endregion

        #region - Ctor -
        public AreaViewModel(Area area, Entities.Settings.Settings settings)
        {
            m_Settings = settings;
            AreaName = area.Name;
            Paths = new ObservableCollection<PathViewModel>();

            foreach (var estw in area.ESTWs)
            {
                string Path = m_Settings.Paths.ContainsKey(estw.Id) ? m_Settings.Paths[estw.Id] : null;

                var VM = new PathViewModel(estw, Path);
                VM.PropertyChanged += PathVM_PropertyChanged;
                Paths.Add(VM);
            }
        }
        #endregion

        #region - Properties -

        public string AreaName
        {
            get => Get<string>();
            private set => Set(value);
        }

        #region [Paths]
        public ObservableCollection<PathViewModel> Paths
        {
            get => Get<ObservableCollection<PathViewModel>>();
            set => Set(value);
        }
        #endregion

        #endregion

        #region [PathVM_PropertyChanged]
        private void PathVM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var VM = sender as PathViewModel;

            if (VM == null)
                return;

            if (e.PropertyName == "Path")
            {
                if (VM.Path.IsNullOrWhiteSpace())
                    m_Settings.Paths.Remove(VM.EstwId);
                else
                    m_Settings.Paths[VM.EstwId] = VM.Path;

                OnPropertyChanged("Path");
            }
        }
        #endregion

    }
}
