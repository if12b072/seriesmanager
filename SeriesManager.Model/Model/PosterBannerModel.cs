using System.Xml.Linq;

using SeriesManager.Enum;

namespace SeriesManager.Model.Banner
{
    public sealed class PosterBannerModel : BannerBaseModel
    {
        public int Width
        {
            get;
            private set;
        }

        public int Height
        {
            get;
            private set;
        }

        public PosterBannerModel(int seriesID, string path) : base(seriesID, path) 
        { 
        }

        public PosterBannerModel(int seriesID, int id, string path, Languages language, double rating, int ratingCount, string size) : base(seriesID, id, path, language, rating, ratingCount)
        {
            int index = size.IndexOf('x');
            if (index > 0)
            {
                this.Width = int.Parse(size.Substring(0, index));
                this.Height = int.Parse(size.Substring(index + 1, size.Length));
            }
        }

        public PosterBannerModel(int seriesID, XElement data): base(seriesID, data)
        {
            foreach (XElement elem in data.Elements())
            {
                switch (elem.Name.ToString())
                {
                    case "BannerType2":
                        string size = elem.Value;
                        int index = size.IndexOf('x');
                        if (index > 0)
                        {
                            this.Width = int.Parse(size.Substring(0, index));
                            this.Height = int.Parse(size.Substring(index + 1));
                        }
                        break;
                }
            }
        }
    }
}
