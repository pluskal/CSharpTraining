using System;
using System.Threading;
using System.Windows;

namespace SimpleMultithreading
{
    public partial class MainWindow
    {
        private bool _condition = true;

        public MainWindow()
        {
            InitializeComponent();

        }

        private void StartCrashClick(object sender, RoutedEventArgs e)
        {
            var loopIndex = 0;

            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    // This is a background operation!

                    while (_condition)
                    {
                        // Do something

                        // Notify user
                        StatusTextBlock.Text = string.Format("Loop # {0}", loopIndex++);

                        // Sleep for a while
                        Thread.Sleep(500);
                    }
                });
        }

        private void StartSuccessClick(object sender, RoutedEventArgs e)
        {
            var loopIndex = 0;

            ThreadPool.QueueUserWorkItem(
                o =>
                {
                    // This is a background operation!

                    while (_condition)
                    {
                        // Do something

                        Dispatcher.BeginInvoke(
                            (Action)(() =>
                            {
                                // Notify user
                                StatusTextBlock.Text = string.Format("Loop # {0}", loopIndex++);
                            }));

                        // Sleep for a while
                        Thread.Sleep(500);
                    }
                });
        }
    }
}
