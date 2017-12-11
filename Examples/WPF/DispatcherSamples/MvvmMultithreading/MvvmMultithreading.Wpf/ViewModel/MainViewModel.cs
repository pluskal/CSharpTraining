using System.Threading;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace MvvmMultithreading.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public const string StatusPropertyName = "Status";

        private bool _condition = true;
        private RelayCommand _startCrashCommand;
        private RelayCommand _startSuccessCommand;
        private string _status;

        public RelayCommand StartCrashCommand
        {
            get
            {
                return _startCrashCommand
                        ?? (_startCrashCommand = new RelayCommand(
                            () =>
                            {
                                var loopIndex = 0;

                                ThreadPool.QueueUserWorkItem(
                                    o =>
                                    {
                                        // This is a background operation!

                                        while (_condition)
                                        {
                                            // Do something

                                            // Notify user. If the Status property is databound to
                                            // a control in the XAML, this call will crash the app
                                            // except in WPF, where the call is automatically dispatched.
                                            Status = string.Format("Loop # {0}", loopIndex++);

                                            // Sleep for a while
                                            Thread.Sleep(500);
                                        }
                                    });
                            }));
            }
        }

        public RelayCommand StartSuccessCommand
        {
            get
            {
                return _startSuccessCommand
                        ?? (_startSuccessCommand = new RelayCommand(
                            () =>
                            {
                                var loopIndex = 0;

                                ThreadPool.QueueUserWorkItem(
                                    o =>
                                    {
                                        // This is a background operation!

                                        while (_condition)
                                        {
                                            // Do something

                                            DispatcherHelper.CheckBeginInvokeOnUI(
                                                () =>
                                                {
                                                    // Dispatch back to the main thread
                                                    Status = string.Format("Loop # {0}", loopIndex++);
                                                });

                                            // Sleep for a while
                                            Thread.Sleep(500);
                                        }
                                    });
                            }));
            }
        }

        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                Set(StatusPropertyName, ref _status, value);
            }
        }
    }
}