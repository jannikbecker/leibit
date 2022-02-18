using Leibit.Controls;

namespace Leibit.Client.WPF.Windows.ESTWSelection.Views
{
    /// <summary>
    /// Interaktionslogik für ESTWSelectionView.xaml
    /// </summary>
    public partial class ESTWSelectionView : LeibitWindow
    {
        public ESTWSelectionView()
            : base("ESTWSelection")
        {
            InitializeComponent();
        }

        public bool SelectionDummy
        {
            get
            {
                return false;
            }
            set { }
        }
    }
}
