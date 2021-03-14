using System.Windows;

namespace Leibit.Client.WPF.Dialogs.Error
{
    public class ErrorDialog
    {

        #region - Needs -
        private static ErrorDialogViewModel m_ViewModel;
        private static ErrorDialogView m_View;
        private static Window m_Window;
        #endregion

        public static bool ShowModal(string errorMessage)
        {
            m_ViewModel = new ErrorDialogViewModel(errorMessage);
            m_ViewModel.DialogClosing += ViewModel_DialogClosing;

            m_View = new ErrorDialogView();
            m_View.DataContext = m_ViewModel;

            m_Window = new Window();
            m_Window.Owner = Application.Current.MainWindow;
            m_Window.Content = m_View;
            m_Window.Title = "Fehler";
            m_Window.SizeToContent = SizeToContent.WidthAndHeight;
            m_Window.MinWidth = 400;
            m_Window.MaxWidth = 800;
            m_Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            m_Window.Topmost = true;

            var dialogResult = m_Window.ShowDialog();
            var result = dialogResult.HasValue && dialogResult.Value;

            m_Window = null;
            m_View = null;
            m_ViewModel.DialogClosing -= ViewModel_DialogClosing;
            m_ViewModel = null;

            return result;
        }

        private static void ViewModel_DialogClosing(object sender, bool e)
        {
            m_Window.DialogResult = e;
            m_Window.Close();
        }
    }
}
