namespace NotifyTaskCompletionSample
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            this.UrlByteCount = new NotifyTaskCompletion<int>(
                MyStaticService.CountBytesInUrlAsync("http://www.example.com"));
        }
        public NotifyTaskCompletion<int> UrlByteCount { get; private set; }
    }
}