namespace ExpandableCheckableList
{
    public class ExpandableRVItem<T> : IExpandableRVItem where T : INLevelItem
    {
        public T Object { get; }

        public bool HasChildren { get; set; }

        public bool IsChecked { get; set; }

        public bool IsExpanded { get; set; }

        public int Level { get; set; }

        public int Id => Object.Id;

        public int Parent => Object.Parent;

        public ExpandableRVItem(T obj)
        {
            Object = obj;
        }

        public void SetChecked(bool value)
        {
            IsChecked = value;
        }
    }
}