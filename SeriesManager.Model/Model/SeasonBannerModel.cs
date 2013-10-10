using System;
using System.Xml.Linq;

using SeriesManager.Enum;

namespace SeriesManager.Model.Banner
{
    public sealed class SeasonBannerModel : BannerBaseModel
    {
        public enum SeasonSize
        {
            season,
            seasonwide
        }

        public int Number
        {
            get;
            private set;
        }

        public SeasonSize Size
        {
            get;
            private set;
        }

        public SeasonBannerModel(int seriesID, int id, string path, Languages language, double rating, int ratingCount, int number, SeasonSize size) : base(seriesID, id, path, language, rating, ratingCount)
        {
            this.Number = number;
            this.Size = size;
        }

        public SeasonBannerModel(int seriesID, XElement data) : base(seriesID, data)
        {
            foreach (XElement elem in data.Elements())
            {
                switch (elem.Name.ToString())
                {
                    case "Season":
                        int season;
                        if (Int32.TryParse(elem.Value, out season))
                            this.Number = season;
                        break;
                    case "BannerType2":
                        SeasonSize size;
                        if (System.Enum.TryParse(elem.Value, out size))
                            this.Size = size;
                        break;
                }
            }
        }
    }
}
