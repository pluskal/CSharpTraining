using System;
using System.Windows.Input;

public sealed class DelegateCommand : ICommand
{
    private readonly Action _command;

    public DelegateCommand(Action command)
    {
        this._command = command;
    }

    public void Execute(object parameter)
    {
        this._command();
    }

    bool ICommand.CanExecute(object parameter)
    {
        return true;
    }

    event EventHandler ICommand.CanExecuteChanged
    {
        add { }
        remove { }
    }
}