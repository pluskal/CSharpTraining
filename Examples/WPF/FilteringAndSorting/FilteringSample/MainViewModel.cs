using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using FilteringSample.dtos;
using GalaSoft.MvvmLight.Command;

namespace FilteringSample
{
    public class MainViewModel
    {
        private int _filterWaitingCount;

        public MainViewModel()
        {
            var items = new List<Item>
            {
                new Item("name0", "str0", "str_0", 0, 0.0),
                new Item("name1", "str1", "str_1", 1, 1.0),
                new Item("name2", "str2", "str_2", 2, 2.0),
                new Item("name3", "str3", "str_3", 3, 3.0),
                new Item("name4", "str4", "str_4", 4, 4.0)
            };

            this.CollectionViewSource = System.Windows.Data.CollectionViewSource.GetDefaultView(items);

            this.FilterCommand = new RelayCommand<MemberPathFilterText>(async i => await this.Filter(i));
        }

        public RelayCommand<MemberPathFilterText> FilterCommand { get; set; }

        public ICollectionView CollectionViewSource { get; set; }

        private async Task Filter(MemberPathFilterText memberPathFilterText)
        {
            var propertyInfo = typeof(Item).GetProperty(memberPathFilterText.MemberPath);
            if (propertyInfo == null)
            {
                this.CollectionViewSource.Filter = null;
                return;
            }

            Interlocked.Increment(ref this._filterWaitingCount);
            await Task.Delay(500);
            if (Interlocked.Decrement(ref this._filterWaitingCount) != 0) return;

            this.CollectionViewSource.Filter = o =>
            {
                if (o is Item item)
                    return propertyInfo.GetValue(item).ToString().Contains(memberPathFilterText.FilterText);
                return false;
            };
        }
    }
}