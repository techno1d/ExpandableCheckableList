using Android.Support.V7.Widget;
using Android.Views;

using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpandableCheckableList
{
    public class ExpandableItemsAdapter : ExpandableRVAdapter<DataModel>
    {
        public event EventHandler ItemChecked;
        public event EventHandler ItemUnchecked;

        public ExpandableItemsAdapter(List<DataModel> terms) : base(terms) { }
        public ExpandableItemsAdapter(List<DataModel> terms, IEnumerable<int> selected) : base(terms, selected) { }

        protected override RecyclerView.ViewHolder OnCreateChildViewHolder(ViewGroup parent)
        {
            var v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_holder_checkable, parent, false);

            var vh = new ChildItemViewHolder(v);

            vh.CheckedTextView.Click += (o, a) =>
            {
                ToggleChecked(vh.AdapterPosition);
            };
            vh.Mark(false);
            return vh;
        }

        protected override RecyclerView.ViewHolder OnCreateParentViewHolder(ViewGroup parent)
        {
            var v = LayoutInflater.From(parent.Context).Inflate(Resource.Layout.view_holder_expandable, parent, false);

            var vh = new ParentItemViewHolder(v);

            vh.CheckedTextView.Touch += (o, a) =>
            {
                var x = a.Event.GetX();
                if (o is AppCompatCheckedTextView tv)
                {
                    if (x < tv.MeasuredWidth - tv.GetCompoundDrawablesRelative()[2].Bounds.Width())
                        vh.ActionOnClick = ParentItemViewHolder.ToggleAction.Check;
                    else
                        vh.ActionOnClick = ParentItemViewHolder.ToggleAction.Expand;
                }
                a.Handled = false;
            };

            vh.CheckedTextView.Click += (o, a) =>
            {
                if (vh.ActionOnClick == ParentItemViewHolder.ToggleAction.Expand)
                    ToggleExpanded(vh.AdapterPosition);
                else
                    ToggleChecked(vh.AdapterPosition);
            };

            vh.Mark(false);

            return vh;
        }

        protected override IEnumerable<ExpandableRVItem<DataModel>> ReOrderBeforeExpand(IEnumerable<ExpandableRVItem<DataModel>> items)
        {
            return items.OrderBy(t => t.Object, new DataModel.IsParentComparer(_parentsDict.Keys));
        }

        protected override void OnItemChecked(int position)
        {
            base.OnItemChecked(position);

            ItemChecked?.Invoke(this, new EventArgs());
        }

        protected override void OnItemUnchecked(int position)
        {
            base.OnItemUnchecked(position);

            ItemUnchecked?.Invoke(this, new EventArgs());
        }
    }
}