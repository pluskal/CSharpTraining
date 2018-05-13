using System.Windows.Controls;
using SimpleWPFReporting.Example.ViewModels;

namespace SimpleWPFReporting.Example.Commands
{
    public class ExportPdfReportCommand : ReportCommandBase
    {
        public ExportPdfReportCommand(MainViewModel mainViewModel) : base(mainViewModel)
        {
        }

        protected override void Execute(StackPanel reportStackPanel)
        {
            Report.ExportReportAsPdf(reportStackPanel, MainViewModel, ReportOrientation.Portrait);
        }
    }
}