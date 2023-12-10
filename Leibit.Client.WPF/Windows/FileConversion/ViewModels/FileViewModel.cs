using Leibit.Core.Client.BaseClasses;
using Leibit.Core.Client.Commands;
using Leibit.Core.Common;
using System.IO;
using System.Windows.Input;

namespace Leibit.Client.WPF.Windows.FileConversion.ViewModels
{
    public class FileViewModel : ViewModelBase
    {
        #region - Ctor -
        public FileViewModel(string fileName)
        {
            OldName = fileName;

            var directoryName = Path.GetDirectoryName(fileName);
            var fileNameWithoutExtention = Path.GetFileNameWithoutExtension(fileName);
            NewName = Path.Combine(directoryName, fileNameWithoutExtention);

            RemoveCommand = new CommandHandler(__Remove, true);
            CanEdit = true;
        }
        #endregion

        #region - Properties -

        public ICommand RemoveCommand { get; }

        #region [OldName]
        public string OldName { get; }
        #endregion

        #region [NewName]
        public string NewName
        {
            get => Get<string>();
            set
            {
                Set(value);
                __Validate();
            }
        }
        #endregion

        #region [CanEdit]
        public bool CanEdit
        {
            get => Get<bool>();
            internal set => Set(value);
        }
        #endregion

        #region [IsValid]
        internal bool IsValid
        {
            get => Get<bool>();
            private set => Set(value);
        }
        #endregion

        #region [IsSuccessful]
        public bool IsSuccessful
        {
            get => Get<bool>();
            internal set => Set(value);
        }
        #endregion

        #region [HasWarning]
        public bool HasWarning
        {
            get => Get<bool>();
            internal set => Set(value);
        }
        #endregion

        #region [WarningText]
        public string WarningText
        {
            get => Get<string>();
            internal set => Set(value);
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__Validate]
        private void __Validate()
        {
            IsSuccessful = false;

            if (NewName.IsNullOrWhiteSpace())
            {
                IsValid = false;
                HasWarning = true;
                WarningText = "Bitte einen Dateinamen eingeben.";
            }
            else if (!Path.IsPathFullyQualified(NewName))
            {
                IsValid = false;
                HasWarning = true;
                WarningText = "Ungültiger Pfad.";
            }
            else if (File.Exists(NewName + ".leibit2"))
            {
                IsValid = true;
                HasWarning = true;
                WarningText = "Die Datei existiert bereits und wird überschrieben.";
            }
            else
            {
                IsValid = true;
                HasWarning = false;
                WarningText = string.Empty;
            }
        }
        #endregion

        #region [__Remove]
        private void __Remove()
        {
            OnPropertyChanged("Remove");
        }
        #endregion

        #endregion
    }
}
