using Leibit.Core.Common;
using Leibit.Entities.Common;
using Leibit.Entities.LiveData;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using System.Windows;

namespace Leibit.Client.WPF.Dialogs.OpenReminders
{
    public static class OpenRemindersDialog
    {
        #region - Needs -
        private static bool m_IsOpen;
        private static bool m_IsInitialized;
        private static Window m_Window;
        private static OpenRemindersDialogView m_View;
        private static OpenRemindersDialogViewModel m_ViewModel;
        #endregion

        #region - Public methods -

        #region [ShowReminders]
        public static void ShowReminders(ESTW estw, List<Reminder> reminders)
        {
            if (!reminders.Any())
                return;

            __Initialize();
            m_ViewModel.AddReminders(estw, reminders);
            Open();
            __PlayNotificationSound();
        }
        #endregion

        #region [Open]
        public static void Open()
        {
            __Initialize();

            if (!m_IsOpen)
            {
                m_Window = new Window();
                m_Window.Owner = Application.Current.MainWindow;
                m_Window.Content = m_View;
                m_Window.SizeToContent = SizeToContent.WidthAndHeight;
                m_Window.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                m_Window.Topmost = true;
                m_Window.Closed += __Window_Closed;
                m_Window.Show();
                m_IsOpen = true;
            }

            if (m_ViewModel.Reminders.Count == 0)
                m_Window.Title = "Keine Erinnerungen";
            else if (m_ViewModel.Reminders.Count == 1)
                m_Window.Title = "1 Erinnerung";
            else
                m_Window.Title = $"{m_ViewModel.Reminders.Count} Erinnerungen";
        }
        #endregion

        #region [CloseAndReset]
        public static void CloseAndReset()
        {
            if (m_IsOpen)
                m_Window.Close();

            if (m_IsInitialized)
            {
                m_ViewModel.DialogClosing -= __ViewModel_DialogClosing;
                m_ViewModel = null;
                m_View = null;
                m_IsInitialized = false;
            }
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Initialize]
        private static void __Initialize()
        {
            if (!m_IsInitialized)
            {
                m_ViewModel = new OpenRemindersDialogViewModel();
                m_ViewModel.DialogClosing += __ViewModel_DialogClosing;

                m_View = new OpenRemindersDialogView();
                m_View.DataContext = m_ViewModel;

                m_IsInitialized = true;
            }
        }
        #endregion

        #region [__PlayNotificationSound]
        private static void __PlayNotificationSound()
        {
            try
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(@"AppEvents\Schemes\Apps\.Default\Notification.Reminder\.Current"))
                {
                    if (key != null)
                    {
                        var file = key.GetValue(null) as string;

                        if (file.IsNotNullOrEmpty())
                            new SoundPlayer(file).Play();
                    }
                }
            }
            catch
            {
                // Ignore any exceptions here
            }
        }
        #endregion

        #region [__ViewModel_DialogClosing]
        private static void __ViewModel_DialogClosing(object sender, EventArgs e)
        {
            m_Window.Close();
        }
        #endregion

        #region [__Window_Closed]
        private static void __Window_Closed(object sender, EventArgs e)
        {
            m_Window.Closed -= __Window_Closed;
            m_Window = null;
            m_IsOpen = false;
        }
        #endregion

        #endregion

    }
}
