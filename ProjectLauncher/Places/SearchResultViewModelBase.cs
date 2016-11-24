using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UE4Launcher.Places
{
    class SearchResultViewModelBase : LocationViewModelBase
    {
        private class ComparerImpl : IComparer<SearchResultViewModelBase>
        {
            public int Compare(SearchResultViewModelBase x, SearchResultViewModelBase y)
            {
                return (int)((x.RelevancyRating - y.RelevancyRating) * -int.MaxValue);
            }
        }

        public static readonly IComparer<SearchResultViewModelBase> Comparer = new ComparerImpl();

        public double RelevancyRating { get; protected set; }

    }
}
