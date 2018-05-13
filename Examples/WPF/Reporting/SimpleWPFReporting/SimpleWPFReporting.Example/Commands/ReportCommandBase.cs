using System;
using System.Windows.Controls;
using System.Windows.Input;
using SimpleWPFReporting.Example.ViewModels;

namespace SimpleWPFReporting.Example.Commands
{
    public abstract class ReportCommandBase : ICommand
    {
        protected ReportCommandBase(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;
        }

        protected MainViewModel MainViewModel { get; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Execute(parameter as StackPanel);
        }

        public event EventHandler CanExecuteChanged;

        protected abstract void Execute(StackPanel reportStackPanel);
    }
}