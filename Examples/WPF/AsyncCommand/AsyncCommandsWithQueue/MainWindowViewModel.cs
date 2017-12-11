using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private string _url;

    public MainWindowViewModel()
    {
        this.Url = "http://www.example.com/";
        this.Operations = new ObservableCollection<CountUrlBytesViewModel>();
        this.CountUrlBytesCommand = new DelegateCommand(() =>
        {
            var countBytes = AsyncCommand.Create(token => MyService.DownloadAndCountBytesAsync(this.Url, token));
            countBytes.Execute(null);
            this.Operations.Add(new CountUrlBytesViewModel(this, this.Url, countBytes));
        });
    }

    public string Url
    {
        get => this._url;
        set
        {
            this._url = value;
            this.OnPropertyChanged();
        }
    }

    public ObservableCollection<CountUrlBytesViewModel> Operations { get; }

    public ICommand CountUrlBytesCommand { get; }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        var handler = this.PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}