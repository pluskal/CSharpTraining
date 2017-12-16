using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
{
    private readonly Func<Task<TResult>> _command;
    private NotifyTaskCompletion<TResult> _execution;

    public AsyncCommand(Func<Task<TResult>> command)
    {
        this._command = command;
    }

    public NotifyTaskCompletion<TResult> Execution
    {
        get => this._execution;
        private set
        {
            this._execution = value;
            this.OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    public override bool CanExecute(object parameter)
    {
        return true;
    }

    public override Task ExecuteAsync(object parameter)
    {
        this.Execution = new NotifyTaskCompletion<TResult>(this._command());
        return this.Execution.TaskCompletion;
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        var handler = this.PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public static class AsyncCommand
{
    public static AsyncCommand<object> Create(Func<Task> command)
    {
        return new AsyncCommand<object>(async () =>
        {
            await command();
            return null;
        });
    }

    public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command)
    {
        return new AsyncCommand<TResult>(command);
    }
}