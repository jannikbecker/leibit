using Leibit.Core.Common;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using MessageBox = Xceed.Wpf.Toolkit.MessageBox;

namespace Leibit.Core.Client.BaseClasses
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        #region - Needs -
        private Dictionary<string, object> m_Properties;
        #endregion

        #region - Ctor -
        public ViewModelBase()
        {
            m_Properties = new Dictionary<string, object>();
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        #region - Protected methods -

        protected virtual void OnPropertyChanged([CallerMemberName] string PropertyName = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
        }

        protected T Get<T>([CallerMemberName] string PropertyName = "")
        {
            if (!m_Properties.ContainsKey(PropertyName))
                return default(T);

            return (T)m_Properties[PropertyName];
        }

        protected void Set<T>(T value, [CallerMemberName] string PropertyName = "")
        {
            m_Properties[PropertyName] = value;
            OnPropertyChanged(PropertyName);
        }

        protected void ShowMessage<T>(OperationResult<T> Result)
        {
            string Caption;
            MessageBoxImage Icon;

            if (Result.Succeeded)
            {
                Caption = "Erfolgreich";
                Icon = MessageBoxImage.Information;
            }
            else
            {
                Caption = "Fehlgeschlagen";
                Icon = MessageBoxImage.Error;
            }

            MessageBox.Show(Result.Message, Caption, MessageBoxButton.OK, Icon);
        }

        #endregion

    }
}
