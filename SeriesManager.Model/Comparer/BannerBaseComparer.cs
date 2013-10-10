using System.Collections.Generic;

using SeriesManager.Model;
using SeriesManager.Model.Banner;

namespace SeriesManager.Model.Comparer
{
    public sealed class BannerBaseComparer : IComparer<BannerBaseModel>
    {
        public int Compare(BannerBaseModel x, BannerBaseModel y)
        {
            return x.Rating.CompareTo(y.Rating);
        }
    }
}
