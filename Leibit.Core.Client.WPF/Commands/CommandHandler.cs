using System;
using System.Windows.Input;

namespace Leibit.Core.Client.Commands
{
    public class CommandHandler : ICommand
    {

        #region - Needs -
        private Action m_Action;
        private bool m_CanExecute;
        #endregion

        #region - Ctor -
        public CommandHandler(Action action, bool canExecute)
            : this(canExecute)
        {
            if (action == null)
                throw new ArgumentNullException("action must not be null");

            m_Action = action;
        }

        protected CommandHandler(bool canExecute)
        {
            m_CanExecute = canExecute;
        }
        #endregion

        public event EventHandler CanExecuteChanged;

        #region [CanExecute]
        public bool CanExecute(object parameter)
        {
            return m_CanExecute;
        }

        public void SetCanExecute(bool value)
        {
            m_CanExecute = value;

            if (CanExecuteChanged != null)
                CanExecuteChanged(this, EventArgs.Empty);
        }
        #endregion

        #region [Execute]
        public virtual void Execute(object parameter)
        {
            m_Action();
        }
        #endregion

    }

    public class CommandHandler<T> : CommandHandler
    {

        #region - Needs -
        private Action<T> m_Action;
        #endregion

        #region - Ctor -
        public CommandHandler(Action<T> action, bool canExecute)
            : base(canExecute)
        {
            if (action == null)
                throw new ArgumentNullException("action must not be null");

            m_Action = action;
        }
        #endregion

        #region [Execute]
        public override void Execute(object parameter)
        {
            if (parameter is T || parameter == null)
                m_Action((T)parameter);
            else
                throw new ArgumentException($"parameter needs to be of type '{typeof(T).Name}', but is '{parameter.GetType().Name}'");
        }
        #endregion

    }
}
