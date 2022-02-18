using Leibit.Controls;
using System;

namespace Leibit.Client.WPF.Windows.LocalOrders.Views
{
    /// <summary>
    /// Interaction logic for LocalOrdersView.xaml
    /// </summary>
    public partial class LocalOrdersView : LeibitWindow
    {
        public LocalOrdersView()
            : base("LocalOrders")
        {
            InitializeComponent();
        }

        public LocalOrdersView(int trainNumber, string stationShortSymbol)
            : base(String.Format("LocalOrders_{0}_{1}", trainNumber, stationShortSymbol))
        {
            InitializeComponent();
        }
    }
}
