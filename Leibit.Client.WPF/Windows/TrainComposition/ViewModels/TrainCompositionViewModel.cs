using Leibit.Client.WPF.ViewModels;
using Leibit.Entities.Scheduling;

namespace Leibit.Client.WPF.Windows.TrainComposition.ViewModels
{
    public class TrainCompositionViewModel : ChildWindowViewModelBase
    {

        #region - Needs -
        private readonly Train m_Train;
        #endregion

        #region - Ctor -
        public TrainCompositionViewModel(Train train)
        {
            m_Train = train;
        }
        #endregion

        #region - Properties -

        public string Caption => $"Zugbildung {m_Train.Type} {TrainNumber}";

        public int TrainNumber => m_Train.Number;

        public string Composition => m_Train.Composition;

        #endregion

    }
}
