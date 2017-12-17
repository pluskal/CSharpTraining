using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using SortingSample.dtos;

namespace SortingSample
{
    public class MainViewModel
    {
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

            this.SortCommand = new RelayCommand<MemberPathSortingDirection>(this.Sort);
        }

        public RelayCommand<MemberPathSortingDirection> SortCommand { get; }

        public ICollectionView CollectionViewSource { get; set; }

        private void Sort(MemberPathSortingDirection pathSortingDirection)
        {
            var propertyName = pathSortingDirection.MemberPath;
            var sortingDirection = pathSortingDirection.SortDirection;
            var currentDescription =
                this.CollectionViewSource.SortDescriptions.FirstOrDefault(sd => sd.PropertyName.Equals(propertyName));
            if (currentDescription.PropertyName != null && currentDescription.PropertyName.Equals(propertyName))
                this.CollectionViewSource.SortDescriptions.Remove(currentDescription);

            if (!sortingDirection.HasValue) return;

            this.CollectionViewSource.SortDescriptions.Add(new SortDescription(propertyName,
                sortingDirection.Value));
        }
    }
}