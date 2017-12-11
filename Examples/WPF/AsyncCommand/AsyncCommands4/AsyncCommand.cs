using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

public class AsyncCommand<TResult> : AsyncCommandBase, INotifyPropertyChanged
{
    private readonly CancelAsyncCommand _cancelCommand;
    private readonly Func<CancellationToken, Task<TResult>> _command;
    private NotifyTaskCompletion<TResult> _execution;

    public AsyncCommand(Func<CancellationToken, Task<TResult>> command)
    {
        this._command = command;
        this._cancelCommand = new CancelAsyncCommand();
    }

    public ICommand CancelCommand => this._cancelCommand;

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
        return this.Execution == null || this.Execution.IsCompleted;
    }

    public override async Task ExecuteAsync(object parameter)
    {
        this._cancelCommand.NotifyCommandStarting();
        this.Execution = new NotifyTaskCompletion<TResult>(this._command(this._cancelCommand.Token));
        this.RaiseCanExecuteChanged();
        await this.Execution.TaskCompletion;
        this._cancelCommand.NotifyCommandFinished();
        this.RaiseCanExecuteChanged();
    }

    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        var handler = this.PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private sealed class CancelAsyncCommand : ICommand
    {
        private bool _commandExecuting;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public CancellationToken Token => this._cts.Token;

        bool ICommand.CanExecute(object parameter)
        {
            return this._commandExecuting && !this._cts.IsCancellationRequested;
        }

        void ICommand.Execute(object parameter)
        {
            this._cts.Cancel();
            this.RaiseCanExecuteChanged();
        }

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public void NotifyCommandStarting()
        {
            this._commandExecuting = true;
            if (!this._cts.IsCancellationRequested)
                return;
            this._cts = new CancellationTokenSource();
            this.RaiseCanExecuteChanged();
        }

        public void NotifyCommandFinished()
        {
            this._commandExecuting = false;
            this.RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}

public static class AsyncCommand
{
    public static AsyncCommand<object> Create(Func<Task> command)
    {
        return new AsyncCommand<object>(async _ =>
        {
            await command();
            return null;
        });
    }

    public static AsyncCommand<TResult> Create<TResult>(Func<Task<TResult>> command)
    {
        return new AsyncCommand<TResult>(_ => command());
    }

    public static AsyncCommand<object> Create(Func<CancellationToken, Task> command)
    {
        return new AsyncCommand<object>(async token =>
        {
            await command(token);
            return null;
        });
    }

    public static AsyncCommand<TResult> Create<TResult>(Func<CancellationToken, Task<TResult>> command)
    {
        return new AsyncCommand<TResult>(command);
    }
}