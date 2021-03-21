using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using System;
using System.Windows;
using System.Windows.Input;

namespace Leibit.Client.WPF.Dialogs.Error
{
    public class ErrorDialogViewModel : ViewModelBase
    {

        public event EventHandler<bool> DialogClosing;

        public ErrorDialogViewModel(string errorMessage)
        {
            ErrorMessage = errorMessage;
            IgnoreCommand = new CommandHandler(() => DialogClosing?.Invoke(this, false), true);
            ExitCommand = new CommandHandler(() => DialogClosing?.Invoke(this, true), true);
            CopyToClipboardCommand = new CommandHandler(() => Clipboard.SetText(ErrorMessage), true);
        }

        public string ErrorMessage { get; }

        public ICommand IgnoreCommand { get; }

        public ICommand ExitCommand { get; }

        public ICommand CopyToClipboardCommand { get; }
    }
}
