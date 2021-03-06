﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

public sealed class MainWindowViewModel : INotifyPropertyChanged
{
    private string _url;

    public MainWindowViewModel()
    {
        this.Url = "http://www.example.com/";
        this.CountUrlBytesCommand = AsyncCommand.Create(token => MyService.DownloadAndCountBytesAsync(this.Url, token));
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

    public event PropertyChangedEventHandler PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        var handler = this.PropertyChanged;
        handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}