using Leibit.Controls;
using System;

namespace Leibit.Client.WPF.Windows.DelayJustification.Views
{
    /// <summary>
    /// Interaction logic for DelayJustificationView.xaml
    /// </summary>
    public partial class DelayJustificationView : ChildWindow
    {
        public DelayJustificationView()
            : base("DelayJustification")
        {
            InitializeComponent();
        }

        public DelayJustificationView(int TrainNumber)
            : base(String.Format("DelayJustification_{0}", TrainNumber))
        {
            InitializeComponent();
        }
    }
}
