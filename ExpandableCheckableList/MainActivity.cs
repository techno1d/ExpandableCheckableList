using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;

namespace ExpandableCheckableList
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, View.IOnClickListener
    {
        public RecyclerView ExpandableRecyclerView { get; private set; }
        public Button OkButton { get; private set; }
        public Button ResetButton { get; private set; }

        HashSet<int> _selectedTerms = new HashSet<int>();
        List<DataModel> terms = new List<DataModel>()
        {
            new DataModel
            {
                Id = 1,
                Name = "TopParent 1",
                Parent = 0
            },
            new DataModel
            {
                Id = 2,
                Name = "TopParent 2",
                Parent = 0
            },
            new DataModel
            {
                Id = 3,
                Name = "TopParent 3",
                Parent = 0
            },
            new DataModel
            {
                Id = 4,
                Name = "TopParent 4",
                Parent = 0
            },
            new DataModel
            {
                Id = 5,
                Name = "TopParent 5",
                Parent = 0
            },
            new DataModel
            {
                Id = 6,
                Name = "TopParent 6",
                Parent = 0
            }
        };

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.activity_main);

            var rnd = new Random();
            int parentCount = terms.Count;
            for (int i = parentCount + 1; i < rnd.Next(50, 150); i++)
            {
                terms.Add(new DataModel { Id = i, Name = $"Generated #{i}", Parent = rnd.Next(1, terms.Count - 1) });
            }

            ExpandableRecyclerView = FindViewById<RecyclerView>(Resource.Id.expandable_recycler_view);
            OkButton = FindViewById<Button>(Resource.Id.ok_button);
            ResetButton = FindViewById<Button>(Resource.Id.reset_button);

            InitRecyclerView();
        }

        protected override void OnResume()
        {
            base.OnResume();

            OkButton?.SetOnClickListener(this);
            ResetButton?.SetOnClickListener(this);
        }

        protected override void OnPause()
        {
            base.OnPause();

            OkButton?.SetOnClickListener(null);
            ResetButton?.SetOnClickListener(null);
        }

        private void InitRecyclerView()
        {
            ExpandableRecyclerView.SetLayoutManager(new LinearLayoutManager(this, (int)Orientation.Vertical, false));

            ExpandableItemsAdapter adapter = null;
            if ((_selectedTerms?.Count ?? 0) == 0)
                ExpandableRecyclerView.SetAdapter(adapter = new ExpandableItemsAdapter(terms));
            else
                ExpandableRecyclerView.SetAdapter(adapter = new ExpandableItemsAdapter(terms, _selectedTerms));

            adapter.ItemChecked += Adapter_ItemChecked;
            adapter.ItemUnchecked += Adapter_ItemUnchecked;
        }

        private void Adapter_ItemUnchecked(object sender, EventArgs e)
        {
            if (sender is ExpandableRVAdapter<DataModel> adapter && ResetButton != null && !adapter.HasChosen)
                ResetButton.Visibility = ViewStates.Invisible;
        }

        private void Adapter_ItemChecked(object sender, EventArgs e)
        {
            if (sender is ExpandableRVAdapter<DataModel> adapter && ResetButton != null && adapter.HasChosen)
                ResetButton.Visibility = ViewStates.Visible;
        }

        public void OnClick(View v)
        {
            switch (v.Id)
            {
                case Resource.Id.ok_button:
                    {
                        if (ExpandableRecyclerView?.GetAdapter() is ExpandableItemsAdapter adapter)
                            Toast.MakeText(this, adapter.HasChosen ? "Chosen IDs: " + string.Join(", ", adapter.ChosenIds) : "No IDs chosen!", ToastLength.Long).Show();
                    }
                    break;
                case Resource.Id.reset_button:
                    {
                        if (ExpandableRecyclerView?.GetAdapter() is ExpandableItemsAdapter adapter)
                            adapter.ResetChosen();
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
