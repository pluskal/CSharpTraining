using System;
using System.Windows.Controls;
using System.Windows.Input;
using SimpleReporting.Interfaces;
using SimpleWPFReporting;

namespace SimpleReporting.Commands
{
    public class PrintPdfCommand : ICommand
    {
        private readonly IPrintableDataContext _printableDataContext;

        public PrintPdfCommand(IPrintableDataContext printableDataContext)
        {
            this._printableDataContext = printableDataContext;
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        private void Execute(StackPanel parameter)
        {
            if(parameter ==null) return;

            this._printableDataContext.IsPrinting = true;

            SimpleWPFReporting.Report.ExportReportAsPdf(
                parameter,
                this._printableDataContext,
                ReportOrientation.Portrait);

            this._printableDataContext.IsPrinting = false;
        }

        public void Execute(object parameter)
        {
            var stackPanel = parameter as StackPanel;
            this.Execute(stackPanel);
        }

        public event EventHandler CanExecuteChanged;
    }
}