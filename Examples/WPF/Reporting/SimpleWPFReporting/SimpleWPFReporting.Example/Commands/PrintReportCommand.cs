using System.Windows.Controls;
using SimpleWPFReporting.Example.ViewModels;

namespace SimpleWPFReporting.Example.Commands
{
    public class PrintReportCommand : ReportCommandBase
    {
        public PrintReportCommand(MainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override void Execute(StackPanel reportStackPanel)
        {
            Report.PrintReport(reportStackPanel, MainViewModel, ReportOrientation.Portrait);
        }
    }
}