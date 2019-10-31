
using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpandableCheckableList
{
    public class DataModel : INLevelItem
    {
        public class IsParentComparer : IComparer<DataModel>
        {
            IEnumerable<int> _parents = null;
            public IsParentComparer(IEnumerable<int> parents)
            {
                _parents = parents;
            }

            public int Compare(DataModel x, DataModel y)
            {
                bool isXParent = _parents?.Contains(x.Id) ?? false;
                bool isYParent = _parents?.Contains(y.Id) ?? false;

                if (isXParent && !isYParent) return -1;
                if (isYParent && !isXParent) return 1;

                return 0;
            }
        }

        public int Id { get; set; } = -1;
        public string Name { get; set; }
        public string Description { get; set; }
        public int Parent { get; set; }
    }
}