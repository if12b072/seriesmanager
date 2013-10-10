using System.Collections.Generic;

using SeriesManager.Model;

namespace SeriesManager.Model.Comparer
{
    public sealed class EpisodeComparer : IComparer<EpisodeModel>
    {
        public int Compare(EpisodeModel x, EpisodeModel y)
        {
            var season = x.Season.Number.CompareTo(y.Season.Number);

            if (season < 0)
            {
                return -1;
            }
            else if (season == 0)
            {
                return x.Number.CompareTo(y.Number);
            }
            else
            {
                return +1;
            }
        }
    }
}
