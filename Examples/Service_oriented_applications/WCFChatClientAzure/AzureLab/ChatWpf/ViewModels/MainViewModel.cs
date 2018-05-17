using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using ChatWpf.ChatService;

namespace ChatWpf.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private IEnumerable<ChatMessage> _chatMessages = new List<ChatMessage>();
        private ChatMessage _newMessage;

        public MainViewModel()
        {
            this.SendNewMessageCommand = new RelayCommand(async () => await this.SendNewMessage());
            this.ClearMessagesCommand = new RelayCommand(async () => await this.ClearMessages());
            this._newMessage = new ChatMessage {Message = "New message...", Sender = this.Hostname};

            Task.Run(() => this.PeriodicRefreshAsync(new TimeSpan(0, 0, 10), CancellationToken.None)).ConfigureAwait(false);
        }

        public IEnumerable<ChatMessage> ChatMessages
        {
            get => this._chatMessages;
            private set
            {
                if (Equals(value, this._chatMessages)) return;
                this._chatMessages = value;
                this.RaisePropertyChanged();
            }
        }

        private string Hostname => Environment.MachineName;

        public ICommand RefreshCommand => new RelayCommand(async () => await this.RefreshChatMessages());

        public ChatMessage NewMessage
        {
            get => this._newMessage;
            set
            {
                this._newMessage = value;
                this.RaisePropertyChanged();
            }
        }

        public ICommand SendNewMessageCommand { get; }
        public ICommand ClearMessagesCommand { get; }


        public async Task RefreshChatMessages()
        {
            using (var chatApi = new ChatServiceClient())
            {
                //TODO
                // Replace Task.FromResult(new List<ChatMessage>() with valid call
                var chatMessages = new ObservableCollection<ChatMessage>(await Task.FromResult(new List<ChatMessage>()));
                var orderedChatMessages = chatMessages.OrderByDescending(m => m.TimeStamp);
                this.ChatMessages = new List<ChatMessage>(orderedChatMessages);
            }
        }

        public async Task SendNewMessage()
        {
            using (var chatApi = new ChatServiceClient())
            {
                // TODO
                // Replace Task.CompletedTask with valid call
                await Task.CompletedTask;
            }
            this.NewMessage = new ChatMessage { Sender = this.Hostname };
            await this.RefreshChatMessages();
        }

        private async Task ClearMessages()
        {
            using (var chatApi = new ChatServiceClient())
            {
                // TODO
                // Replace Task.CompletedTask with valid call
                await Task.CompletedTask;
            }
            await this.RefreshChatMessages();
        }

        private async Task PeriodicRefreshAsync(TimeSpan interval, CancellationToken cancellationToken)
        {
            while (true)
            {
                await this.RefreshChatMessages();
                await Task.Delay(interval, cancellationToken);
            }
        }
    }
}