using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private int _byteCount;

    private string _url;

    public MainWindowViewModel()
    {
        this.Url = "http://www.example.com/";
        this.CountUrlBytesCommand = new AsyncCommand(async () =>
        {
            this.ByteCount = await MyService.DownloadAndCountBytesAsync(this.Url);
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

    public IAsyncCommand CountUrlBytesCommand { get; }

    public int ByteCount
    {
        get => this._byteCount;
        private set
        {
            this._byteCount = value;
            this.OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        var handler = this.PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}