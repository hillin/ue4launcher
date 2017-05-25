using System;
using System.Collections.Generic;

namespace UE4Launcher.Places
{
	internal abstract class SearchResultViewModelBase : LocationViewModelBase
    {
        private class ComparerImpl : IComparer<SearchResultViewModelBase>
        {
            public int Compare(SearchResultViewModelBase x, SearchResultViewModelBase y)
            {
                var diff = (int)((x.RelevancyRating - y.RelevancyRating) * -int.MaxValue);
                return diff == 0
                    ? string.Compare(x.StringComparablePart, y.StringComparablePart, StringComparison.Ordinal)
                    : diff;
            }
        }

        public static readonly IComparer<SearchResultViewModelBase> Comparer = new ComparerImpl();

        public double RelevancyRating { get; protected set; }
        protected abstract string StringComparablePart { get; }
    }
}
