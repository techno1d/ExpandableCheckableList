using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;

namespace ExpandableCheckableList
{
    public class ParentItemViewHolder : RecyclerView.ViewHolder, IBindableViewHolder<ExpandableRVItem<DataModel>>, IMarkableViewHolder
    {
        public enum ToggleAction { Expand, Check }
        public AppCompatCheckedTextView CheckedTextView { get; }

        public ToggleAction? ActionOnClick { get; set; } = null;

        int _expandedPadding = -1;
        int _defaultPadding = -1;

        public ParentItemViewHolder(View itemView) : base(itemView)
        {
            _expandedPadding = UIHelper.ResolveDimensionAttr(itemView.Context, Android.Resource.Attribute.ListPreferredItemPaddingStart);
            _defaultPadding = (int)itemView.Context.Resources.GetDimension(Resource.Dimension.expandable_item_padding);

            CheckedTextView = itemView.FindViewById<AppCompatCheckedTextView>(Resource.Id.expandable_checkbox);
        }

        public void Bind(ExpandableRVItem<DataModel> data)
        {
            if (_expandedPadding >= 0 && _defaultPadding >= 0)
                CheckedTextView.SetPadding(data.Level * _expandedPadding + _defaultPadding, _defaultPadding, _defaultPadding, _defaultPadding);

            CheckedTextView.Text = data.Object.Name;
            CheckedTextView.Checked = data.IsChecked;

            CheckedTextView.SetCompoundDrawablesRelativeWithIntrinsicBounds(
                CheckedTextView.GetCompoundDrawablesRelative()[0],
                null,
                ContextCompat.GetDrawable(CheckedTextView.Context, data.IsExpanded ? Resource.Drawable.ic_keyboard_arrow_up : Resource.Drawable.ic_keyboard_arrow_down),
                null);
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