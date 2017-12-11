using System;
using System.Threading.Tasks;

public class AsyncCommand : AsyncCommandBase
{
    private readonly Func<Task> _command;

    public AsyncCommand(Func<Task> command)
    {
        this._command = command;
    }

    public override bool CanExecute(object parameter)
    {
        return true;
    }

    public override Task ExecuteAsync(object parameter)
    {
        return this._command();
    }
}