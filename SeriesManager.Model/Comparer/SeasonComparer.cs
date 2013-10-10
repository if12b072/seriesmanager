using System.Collections.Generic;

using SeriesManager.Model;

namespace SeriesManager.Model.Comparer
{
    public sealed class SeasonComparer : IComparer<SeasonModel>
    {
        public int Compare(SeasonModel x, SeasonModel y)
        {
            return x.Number.CompareTo(y.Number);
        }
    }
}
