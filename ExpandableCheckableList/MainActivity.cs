using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Support.V7.Widget;
using System;
using System.Collections;
using System.Collections.Generic;

namespace ExpandableCheckableList
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public RecyclerView ExpandableRecyclerView { get; private set; }

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
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            var rnd = new Random();            
            for (int i = 0; i < rnd.Next(50,150); i++)
            {
                terms.Add(new DataModel { Id = terms.Count, Name = $"Generated #{i}", Parent = rnd.Next(terms.Count - 1) });
            }

            InitRecyclerView();
        }

        private void InitRecyclerView()
        {
            ExpandableRecyclerView = FindViewById<RecyclerView>(Resource.Id.expandable_recycler_view);

            ExpandableRecyclerView.SetLayoutManager(new LinearLayoutManager(this, (int)Orientation.Vertical, false));

            ExpandableItemsAdapter adapter = null;
            if ((_selectedTerms?.Count ?? 0) == 0)
                ExpandableRecyclerView.SetAdapter(adapter = new ExpandableItemsAdapter(terms));
            else
                ExpandableRecyclerView.SetAdapter(adapter = new ExpandableItemsAdapter(terms, _selectedTerms));
        }
    }
}
