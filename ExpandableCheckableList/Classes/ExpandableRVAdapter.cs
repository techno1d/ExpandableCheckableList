using Android.Support.V7.Widget;
using Android.Views;
using System.Collections.Generic;
using System.Linq;

namespace ExpandableCheckableList
{
    public abstract class ExpandableRVAdapter<T> : RecyclerView.Adapter where T : INLevelItem
    {
        const int CHILD_VH_TYPE = 10;
        const int PARENT_VH_TYPE = 20;

        public override int ItemCount => _items?.Count ?? 0;

        public bool HasChosen => (_chosenIds?.Count ?? 0) > 0;

        public HashSet<int> ChosenIds
        {
            get
            {
                HashSet<int> result = new HashSet<int>(_chosenIds);

                if (result.Count > 0)
                {
                    HashSet<int> toRemove = new HashSet<int>();
                    foreach (var id in result)
                    {
                        if (_parentsDict.ContainsKey(id))
                            toRemove.UnionWith(_parentsDict[id].Select(i => i.Id));
                    }

                    if (toRemove.Count > 0)
                        result.RemoveWhere(i => toRemove.Contains(i));
                }

                return result;
            }
        }

        protected int Padding { get; } = -1;

        List<ExpandableRVItem<T>> _items = new List<ExpandableRVItem<T>>();

        protected Dictionary<int, List<T>> _parentsDict = new Dictionary<int, List<T>>();
        private HashSet<int> _chosenIds = new HashSet<int>();

        #region ctors
        public ExpandableRVAdapter(List<T> items)
        {
            foreach (var t in items)
            {
                if (t.Parent != 0)
                {
                    if (!_parentsDict.ContainsKey(t.Parent) || _parentsDict[t.Parent] is null)
                        _parentsDict[t.Parent] = new List<T>();

                    _parentsDict[t.Parent].Add(t);
                }
                else
                {
                    _items.Add(new ExpandableRVItem<T>(t) { IsChecked = false, IsExpanded = false, HasChildren = true, Level = 0 });
                }
            }
        }

        public ExpandableRVAdapter(List<T> items, IEnumerable<int> selected) : this(items)
        {
            if ((selected?.Count() ?? 0) > 0)
            {
                _chosenIds.UnionWith(selected);

                foreach (var item in selected)
                {
                    _chosenIds.UnionWith(GetAllChildren(item));

                    InitParentsChecked(items, item);
                }
            }

            if (_chosenIds.Count > 0)
            {
                foreach (var id in _chosenIds)
                {
                    _items.FirstOrDefault(i => i.Id == id)?.SetChecked(true);
                }
            }
        }

        protected void InitParentsChecked(List<T> items, int id)
        {
            var term = items.FirstOrDefault(i => i.Id == id);
            if ((term?.Parent ?? 0) != 0)
            {
                //если все дочерние эл-ты оказываются отмеченными, то необходимо отметить и ролителя
                if (_parentsDict[term.Parent].All(i => _chosenIds.Contains(i.Id)))
                {
                    _chosenIds.Add(term.Parent);
                    if (_items.Any(i => i.Object.Id == term.Parent))
                        _items.FirstOrDefault(i => i.Object.Id == term.Parent).IsChecked = true;

                    InitParentsChecked(items, term.Parent);
                }
            }
        }
        #endregion

        #region overrides
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            if (holder is IBindableViewHolder<ExpandableRVItem<T>> vh)
            {
                vh.Bind(_items[position]);

                var children = GetAllChildren(_items[position].Object.Id);
                if (holder is IMarkableViewHolder markVH && (children?.Count() ?? 0) > 0 && !_items[position].IsExpanded && !_items[position].IsChecked && children.Any(i => _chosenIds.Contains(i)))
                {
                    markVH.Mark(true);
                }
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            switch (viewType)
            {
                case CHILD_VH_TYPE:
                    return OnCreateChildViewHolder(parent);
                case PARENT_VH_TYPE:
                    return OnCreateParentViewHolder(parent);
                default:
                    return null;
            }
        }

        public override long GetItemId(int position)
        {
            return _items[position].Object.Id;
        }

        public override int GetItemViewType(int position)
        {
            if (_parentsDict.ContainsKey(_items[position].Object.Id))
                return PARENT_VH_TYPE;
            else
                return CHILD_VH_TYPE;
        }

        public override void OnViewRecycled(Java.Lang.Object holder)
        {
            if (holder is IMarkableViewHolder vh)
                vh.Mark(false);

            base.OnViewRecycled(holder);
        }
        #endregion

        public void ToggleExpanded(int position)
        {
            var item = _items[position];

            if (item.IsExpanded)
            {
                int i = position + 1;

                while (i < _items.Count && _items[i].Level > item.Level)
                    i++;

                if (i == position + 1)
                    return;

                _items.RemoveRange(position + 1, i - position - 1);

                item.IsExpanded = false;

                NotifyItemRangeRemoved(position + 1, i - position - 1);
                NotifyItemChanged(position);
            }
            else
            {
                var insert = _parentsDict[item.Object.Id].Select(o => new ExpandableRVItem<T>(o) { Level = item.Level + 1, IsChecked = _chosenIds.Contains(o.Id), IsExpanded = false, HasChildren = _parentsDict.ContainsKey(o.Id) });
                if ((insert?.Count() ?? 0) == 0)
                    return;

                _items.InsertRange(position + 1, ReOrderBeforeExpand(insert));

                item.IsExpanded = true;

                NotifyItemRangeInserted(position + 1, insert.Count());
                NotifyItemChanged(position);

                //MayBeExpandChosen(insert);
            }
        }

        public void ToggleChecked(int position)
        {
            var item = _items[position];
            item.IsChecked = !item.IsChecked;

            NotifyItemChanged(position);

            if (item.IsChecked)
            {
                _chosenIds.Add(item.Object.Id);
                OnItemChecked(position);
            }
            else
            {
                _chosenIds.Remove(item.Object.Id);
                OnItemUnchecked(position);
            }
        }

        public void ResetChosen()
        {
            _chosenIds.Clear();
            _items.ForEach(i => i.IsChecked = false);

            NotifyDataSetChanged();
        }

        protected abstract RecyclerView.ViewHolder OnCreateChildViewHolder(ViewGroup parent);

        protected abstract RecyclerView.ViewHolder OnCreateParentViewHolder(ViewGroup parent);

        protected void MayBeExpandChosen(IEnumerable<ExpandableRVItem<T>> inserted)
        {
            foreach (var item in inserted)
            {
                if (_parentsDict.ContainsKey(item.Object.Id) && _parentsDict[item.Object.Id].Any(i => _chosenIds.Contains(i.Id)))
                {
                    int index = _items.IndexOf(item);
                    if (index >= 0)
                        ToggleExpanded(index);
                }
            }
        }

        protected virtual IEnumerable<ExpandableRVItem<T>> ReOrderBeforeExpand(IEnumerable<ExpandableRVItem<T>> items)
        {
            return items;
        }

        protected virtual void OnItemChecked(int position)
        {
            var item = _items[position];

            if (item.HasChildren)
            {
                AddChildrenToChecked(item.Object.Id);

                if (item.IsExpanded)
                {
                    int i = position + 1;

                    while (i < _items.Count && _items[i].Level > item.Level)
                    {
                        _items[i].IsChecked = true;
                        i++;
                    }

                    if (i > position + 1)
                        NotifyItemRangeChanged(position + 1, i - position - 1);
                }
            }

            AddParentsToChecked(position);
        }

        protected virtual void OnItemUnchecked(int position)
        {
            ExpandableRVItem<T> item = _items[position];

            if (item.HasChildren)
            {
                RemoveChildrenFromChecked(item.Object.Id);

                int i = position + 1;

                while (i < _items.Count && _items[i].Level > item.Level)
                {
                    _items[i].IsChecked = false;
                    i++;
                }

                if (i > position + 1)
                    NotifyItemRangeChanged(position + 1, i - position - 1);
            }

            RemoveParentsFromChecked(position);
        }

        protected void AddChildrenToChecked(int id)
        {
            if (_parentsDict.ContainsKey(id))
                foreach (var child in _parentsDict[id])
                {
                    _chosenIds.Add(child.Id);

                    if (_parentsDict.ContainsKey(child.Id))
                        AddChildrenToChecked(child.Id);
                }
        }

        protected void AddParentsToChecked(int position)
        {
            if (_items[position].Object.Parent != 0)
            {
                //если все дочерние эл-ты оказываются отмеченными, то необходимо отметить и ролителя
                if (_parentsDict[_items[position].Object.Parent].All(i => _chosenIds.Contains(i.Id)))
                {
                    var parentInd = _items.FindIndex(i => i.Object.Id == _items[position].Object.Parent);

                    _items[parentInd].IsChecked = true;
                    _chosenIds.Add(_items[parentInd].Object.Id);

                    NotifyItemChanged(parentInd);

                    AddParentsToChecked(parentInd);
                }
            }
        }

        protected void RemoveChildrenFromChecked(int id)
        {
            foreach (var child in _parentsDict[id])
            {
                _chosenIds.Remove(child.Id);
                if (_parentsDict.ContainsKey(child.Id))
                    RemoveChildrenFromChecked(child.Id);
            }
        }

        protected void RemoveParentsFromChecked(int position)
        {
            if (_items[position].Object.Parent != 0)
            {
                var parentInd = _items.FindIndex(i => i.Object.Id == _items[position].Object.Parent);
                _items[parentInd].IsChecked = false;
                _chosenIds.Remove(_items[parentInd].Object.Id);

                NotifyItemChanged(parentInd);

                RemoveParentsFromChecked(parentInd);
            }
        }

        protected IEnumerable<int> GetAllChildren(int id)
        {
            List<int> result = new List<int>();

            if (_parentsDict.ContainsKey(id))
                foreach (var child in _parentsDict[id])
                {
                    result.Add(child.Id);
                    result.AddRange(GetAllChildren(child.Id));
                }

            return result;
        }
    }
}