using Android.Support.V7.Widget;
using Android.Views;

namespace ExpandableCheckableList
{
    public class ChildItemViewHolder : RecyclerView.ViewHolder, IBindableViewHolder<ExpandableRVItem<DataModel>>
    {
        public AppCompatCheckedTextView CheckedTextView { get; }

        int _defaultPadding = -1;
        int _expandedPadding = -1;

        public ChildItemViewHolder(View itemView) : base(itemView)
        {
            _expandedPadding = UIHelper.ResolveDimensionAttr(itemView.Context, Android.Resource.Attribute.ListPreferredItemPaddingStart);
            _defaultPadding = (int)itemView.Context.Resources.GetDimension(Resource.Dimension.expandable_item_padding);

            CheckedTextView = itemView.FindViewById<AppCompatCheckedTextView>(Resource.Id.checkbox);
        }

        public void Bind(ExpandableRVItem<DataModel> data)
        {
            if (_expandedPadding >= 0 && _defaultPadding >= 0)
                CheckedTextView.SetPadding(data.Level * _expandedPadding + _defaultPadding, _defaultPadding, _defaultPadding, _defaultPadding);

            CheckedTextView.Text = data.Object.Name;
            CheckedTextView.Checked = data.IsChecked;
        }

        public void Mark(bool mark)
        {
            if (mark)
            {
                CheckedTextView.SetBackgroundColor(new Android.Graphics.Color(UIHelper.ResolveColorAttr(ItemView.Context, Android.Resource.Attribute.ColorControlHighlight)));
            }
            else
            {
                CheckedTextView.SetBackgroundColor(Android.Graphics.Color.Transparent);
            }
        }
    }
}