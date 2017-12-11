using System.Windows;

namespace AsyncCommands
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = new MainWindowViewModel();
            this.InitializeComponent();
        }
    }
}