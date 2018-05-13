using System.Windows.Input;
using SimpleWPFReporting.Example.Commands;

namespace SimpleWPFReporting.Example.ViewModels
{
    public class MainViewModel
    {
        public MainViewModel()
        {
            PrintReportCommand = new PrintReportCommand(this);
            ExportPdfReportCommand = new ExportPdfReportCommand(this);
        }

        public ExportPdfReportCommand ExportPdfReportCommand { get; }
        public ICommand PrintReportCommand { get; }

        public string Email => "Email";
        public string Name => "Name";
        public string Phone => "Phone";
        public string Surname => "Surname";
    }
}