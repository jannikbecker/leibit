using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using System;

namespace Leibit.Client.WPF.Dialogs.TrainNumber
{
    public class TrainNumberDialogViewModel : ViewModelBase
    {

        #region - Ctor -
        public TrainNumberDialogViewModel()
        {
            OkCommand = new CommandHandler(() =>
            {
                TrainNumberEntered?.Invoke(this, EventArgs.Empty);
            }, false);
        }
        #endregion

        #region - Events -
        public event EventHandler TrainNumberEntered;
        #endregion

        #region - Properties -

        public int? TrainNumber
        {
            get => Get<int?>();
            set
            {
                Set(value);
                OkCommand.SetCanExecute(value.HasValue);
            }
        }

        public CommandHandler OkCommand { get; }

        #endregion

    }
}
