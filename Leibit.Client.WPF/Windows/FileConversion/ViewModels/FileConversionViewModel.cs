using Leibit.BLL;
using Leibit.Client.WPF.ViewModels;
using Leibit.Core.Client.Commands;
using Leibit.Entities;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Leibit.Client.WPF.Windows.FileConversion.ViewModels
{
    public class FileConversionViewModel : ChildWindowViewModelBase
    {
        #region - Needs -
        private bool m_IsWorking;
        private readonly Dispatcher m_Dispatcher;
        #endregion

        #region - Ctor -
        public FileConversionViewModel(Dispatcher dispatcher)
        {
            m_Dispatcher = dispatcher;
            SelectFilesCommand = new CommandHandler(__SelectFiles, true);
            ConvertCommand = new CommandHandler(__Convert, false);
            CancelCommand = new CommandHandler(__Cancel, true);
            Files = new ObservableCollection<FileViewModel>();
        }
        #endregion

        #region - Properties -

        #region [Commands]
        public CommandHandler SelectFilesCommand { get; }
        public CommandHandler ConvertCommand { get; }
        public CommandHandler CancelCommand { get; }
        #endregion

        #region [Files]
        public ObservableCollection<FileViewModel> Files
        {
            get => Get<ObservableCollection<FileViewModel>>();
            private set => Set(value);
        }
        #endregion

        #endregion

        #region - Public methods -

        #region [OnWindowClosing]
        public override void OnWindowClosing(object sender, CancelEventArgs e)
        {
            if (m_IsWorking)
                e.Cancel = true;
        }
        #endregion

        #endregion

        #region - Private methods -

        #region [__SelectFiles]
        private void __SelectFiles()
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = "LeiBIT-Dateien|*.leibit|Alle Dateien|*.*";
            dialog.Multiselect = true;

            if (dialog.ShowDialog() != true)
                return;

            foreach (var vm in Files)
                vm.PropertyChanged -= __File_PropertyChanged;

            Files.Clear();

            foreach (var file in dialog.FileNames)
            {
                var vm = new FileViewModel(file);
                vm.PropertyChanged += __File_PropertyChanged;
                Files.Add(vm);
            }

            ConvertCommand.SetCanExecute(Files.Any(x => x.IsValid));
        }
        #endregion

        #region [__Convert]
        private void __Convert()
        {
            m_IsWorking = true;
            SelectFilesCommand.SetCanExecute(false);
            ConvertCommand.SetCanExecute(false);
            CancelCommand.SetCanExecute(false);

            foreach (var vm in Files)
                vm.CanEdit = false;

            Task.Run(() =>
            {
                var serializationBLL = new SerializationBLL();
                var filesToConvert = Files.Where(x => x.IsValid && !x.IsSuccessful).ToList();

                for (int i = 0; i < filesToConvert.Count; i++)
                {
                    var vm = filesToConvert[i];
                    OnReportProgress(vm.OldName, 100.0 * i / filesToConvert.Count);

                    var openResult = serializationBLL.Open(vm.OldName);

                    if (!openResult.Succeeded)
                    {
                        vm.HasWarning = true;
                        vm.WarningText = openResult.Message;
                        continue;
                    }

                    if (openResult.Result.FileFormat != Entities.eFileFormat.Binary)
                    {
                        vm.HasWarning = true;
                        vm.WarningText = "Die Datei hat ein ungültiges Format.";
                        continue;
                    }

                    var container = openResult.Result;

                    foreach (var window in container.Windows.Where(x => x.Type == eChildWindowType.LocalOrders && x.Tag is KeyValuePair<int, string>))
                    {
                        // Special logic to convert KeyValuePair to string. KeyValuePair causes problems with JSON.
                        var kvp = (KeyValuePair<int, string>)window.Tag;
                        window.Tag = $"{kvp.Key};{kvp.Value}";
                    }

                    var saveResult = serializationBLL.Save(vm.NewName + ".leibit2", container);

                    if (saveResult.Succeeded)
                    {
                        vm.HasWarning = false;
                        vm.IsSuccessful = true;
                    }
                    else
                    {
                        vm.HasWarning = true;
                        vm.WarningText = saveResult.Message;
                    }
                }

                foreach (var vm in Files)
                    vm.CanEdit = true;

                OnReportProgress(true);
                OnStatusBarTextChanged("Konvertierung abgeschlossen");
                m_IsWorking = false;

                m_Dispatcher.BeginInvoke(new Action(() =>
                {
                    SelectFilesCommand.SetCanExecute(true);
                    ConvertCommand.SetCanExecute(Files.Any(x => x.IsValid));
                    CancelCommand.SetCanExecute(true);
                }));
            });
        }
        #endregion

        #region [__Cancel]
        private void __Cancel()
        {
            OnCloseWindow();
        }
        #endregion

        #region [__File_PropertyChanged]
        private void __File_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var senderVm = sender as FileViewModel;

            if (senderVm == null)
                return;

            if (e.PropertyName == "IsValid")
            {
                ConvertCommand.SetCanExecute(Files.Any(x => x.IsValid));
            }
            else if (e.PropertyName == "Remove")
            {
                senderVm.PropertyChanged -= __File_PropertyChanged;
                Files.Remove(senderVm);
                ConvertCommand.SetCanExecute(Files.Any(x => x.IsValid));
            }
        }
        #endregion

        #endregion
    }
}
