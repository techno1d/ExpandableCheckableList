namespace ExpandableCheckableList
{
    public interface IExpandableRVItem
    {
        int Id { get; }
        int Parent { get; }
        bool HasChildren { get; }
        bool IsChecked { get; set; }
        bool IsExpanded { get; set; }
        int Level { get; }
    }
}