using System;
using System.Xml.Linq;

using SeriesManager.Enum;

namespace SeriesManager.Model.Banner
{
    public sealed class WideBannerModel : BannerBaseModel
    {
        public enum SeriesType
        {
            graphical,
            text,
            blank
        }

        public SeriesType Type
        {
            get;
            private set;
        }

        public WideBannerModel(int seriesID, string path) : base(seriesID, path) 
        { 
        }

        public WideBannerModel(int seriesID, int id, string path, Languages language, double rating, int ratingCount, SeriesType type) : base(seriesID, id, path, language, rating, ratingCount)
        {
            this.Type = type;
        }

        public WideBannerModel(int seriesID, XElement data) : base(seriesID, data)
        {
            foreach (XElement elem in data.Elements())
            {
                switch (elem.Name.ToString())
                {
                    case "BannerType2":
                        SeriesType type;
                        if (System.Enum.TryParse(elem.Value, out type))
                            this.Type = type;
                        break;
                }
            }
        }
    }
}
