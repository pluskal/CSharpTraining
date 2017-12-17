using System.ComponentModel;

namespace FilteringSample.dtos
{
    public class MemberPathSortingDirection
    {
        public string MemberPath { get; set; }
        public ListSortDirection? SortDirection { get; set; }
    }
}
