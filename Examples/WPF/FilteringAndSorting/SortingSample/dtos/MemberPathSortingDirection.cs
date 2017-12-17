using System.ComponentModel;

namespace SortingSample.dtos
{
    public class MemberPathSortingDirection
    {
        public string MemberPath { get; set; }
        public ListSortDirection? SortDirection { get; set; }
    }
}