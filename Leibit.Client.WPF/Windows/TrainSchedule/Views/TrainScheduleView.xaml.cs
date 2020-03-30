using Leibit.Controls;
using System;

namespace Leibit.Client.WPF.Windows.TrainSchedule.Views
{
    /// <summary>
    /// Interaction logic for TrainScheduleView.xaml
    /// </summary>
    public partial class TrainScheduleView : ChildWindow
    {
        public TrainScheduleView()
            : base("TrainSchedule")
        {
            InitializeComponent();
        }

        public TrainScheduleView(int TrainNumber)
            : base(String.Format("TrainSchedule_{0}", TrainNumber))
        {
            InitializeComponent();
        }
    }
}
