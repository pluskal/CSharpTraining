using System.Windows.Input;

public sealed class CountUrlBytesViewModel
{
    public CountUrlBytesViewModel(MainWindowViewModel parent, string url, IAsyncCommand command)
    {
        this.LoadingMessage = "Loading (" + url + ")...";
        this.Command = command;
        this.RemoveCommand = new DelegateCommand(() => parent.Operations.Remove(this));
    }

    public string LoadingMessage { get; }

    public IAsyncCommand Command { get; }

    public ICommand RemoveCommand { get; }
}