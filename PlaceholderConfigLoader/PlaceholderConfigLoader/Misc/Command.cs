using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace PlaceholderConfigLoader
{
    public class Command : ICommand
    {
        private static readonly Dictionary<string, Command> StaticCommands = new Dictionary<string, Command>();

        private Action<object> _executeP;
        private Func<object, bool> _conditionP;
        private Action _execute;
        private Func<bool> _condition;

        public event EventHandler CanExecuteChanged;

        public static Command Static(string name)
        {
            if (!StaticCommands.ContainsKey(name))
                StaticCommands.Add(name, new Command());
            return StaticCommands[name];
        }

        public static void Update(string name)
        {
            if (StaticCommands.ContainsKey(name))
                StaticCommands[name].Update();
        }

        public bool CanExecute(object parameter)
        {
            return (_execute != null || _executeP != null) 
                && (_condition == null || _condition())
                && (_conditionP == null || _conditionP(parameter));
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                if (_execute != null)
                    _execute();
                else
                    _executeP(parameter);
            }
        }

        public Command When(Func<bool> condition)
        {
            _condition = condition;
            _conditionP = null;
            return this;
        }

        public Command Do(Action action)
        {
            _execute = action;
            _executeP = null;
            return this;
        }

        public Command When(Func<object, bool> condition)
        {
            _conditionP = condition;
            _condition = null;
            return this;
        }

        public Command Do(Action<object> action)
        {
            _executeP = action;
            _execute = null;
            return this;
        }

        private void Update()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }
    }
}
