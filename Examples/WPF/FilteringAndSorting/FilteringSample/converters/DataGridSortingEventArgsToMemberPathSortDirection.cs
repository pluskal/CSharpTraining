using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using FilteringSample.dtos;
using FilteringSample.extensions;
using GalaSoft.MvvmLight.Command;

namespace FilteringSample.converters
{
    public class TextChangedEventArgsToMemberPathAndText : IEventArgsConverter
    {
        public object Convert(object value, object parameter)
        {
            if (value is TextChangedEventArgs eventArgs)
                if (eventArgs.Source is TextBox textBox)
                {
                    var dataGridColumnHeader = textBox.ParentOfType<DataGridColumnHeader>();
                    if (dataGridColumnHeader != null)
                    {
                        var columnSortMemberPath = dataGridColumnHeader.Column.SortMemberPath;
                        var filterText = textBox.Text;
                        return new MemberPathFilterText { MemberPath = columnSortMemberPath, FilterText = filterText };
                    }
                }
            return null;
        }
    }
}