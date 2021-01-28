using System;
using System.Windows;

namespace Leibit.Client.WPF.Dialogs.TrainNumber
{
    public class TrainNumberDialog
    {

        #region - Needs -
        private static TrainNumberDialogViewModel m_ViewModel;
        private static TrainNumberDialogView m_View;
        private static Window m_Window;
        #endregion

        public static int? ShowModal()
        {
            m_ViewModel = new TrainNumberDialogViewModel();
            m_ViewModel.TrainNumberEntered += ViewModel_TrainNumberEntered;

            m_View = new TrainNumberDialogView();
            m_View.DataContext = m_ViewModel;

            m_Window = new Window();
            m_Window.Content = m_View;
            m_Window.Title = "ZN eingeben";
            m_Window.ResizeMode = ResizeMode.NoResize;
            m_Window.SizeToContent = SizeToContent.WidthAndHeight;
            m_Window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            var dialogResult = m_Window.ShowDialog();
            int? result = null;

            if (dialogResult == true)
                result = m_ViewModel.TrainNumber;

            m_Window = null;
            m_View = null;
            m_ViewModel.TrainNumberEntered -= ViewModel_TrainNumberEntered;
            m_ViewModel = null;

            return result;
        }

        private static void ViewModel_TrainNumberEntered(object sender, EventArgs e)
        {
            m_Window.DialogResult = true;
            m_Window.Close();
        }
    }
}
