using System.Threading.Tasks;
using System.Xml.Linq;

using SeriesManager.Enum;

namespace SeriesManager.Model.Banner
{
    public sealed class FanArtBannerModel : BannerBaseModel
    {
        #region Properties

        private bool thumbLoaded;
        private bool thumbLoading;
        private string thumbnailSource;
        private string thumbnail;

        public bool ThumbLoading
        {
            get { return thumbLoading; }
            private set { SetProperty(ref thumbLoading, value); }
        }

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

        public string Thumbnail
        {
            get
            {
                string path = "ms-appdata:///local/" + base.seriesID.ToString() + "/" + this.thumbnail;
                if (!ThumbLoading && !thumbLoaded)
                {
                    this.ThumbLoading = true;
                    base.Download(thumbnailSource, path).ContinueWith(W =>
                    {
                        try
                        {
                            this.thumbLoaded = W.Result;
                        }
                        catch { this.thumbLoaded = false; }
                        this.ThumbLoading = false;
                        OnPropertyChanged();
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }
                return (!this.thumbLoaded) ? null : path;
            }
            private set { this.thumbnailSource = value; base.SetProperty(ref this.thumbnail, "thumb_" + System.IO.Path.GetFileName(value)); }
        }

        public async Task DownloadThumbnail()
        {
            string path = "ms-appdata:///local/" + base.seriesID.ToString() + "/" + this.thumbnail;
            if (!ThumbLoading && !thumbLoaded)
            {
                this.ThumbLoading = true;
                this.thumbLoaded = await base.Download(thumbnailSource, path);
                this.ThumbLoading = false;
                OnPropertyChanged("Thumbnail");
            }
        }

        #endregion

        #region Contructor

        public FanArtBannerModel(int seriesID, string path) : base(seriesID, path) 
        { 
        }

        public FanArtBannerModel(int seriesID, int id, string path, Languages language, double rating, int ratingCount, string thumbnailPath, string size) : base(seriesID, id, path, language, rating, ratingCount)
        {
            this.Thumbnail = thumbnailPath;            
        }

        public FanArtBannerModel(int seriesID, XElement data) : base(seriesID, data)
        {
            foreach (XElement elem in data.Elements())
            {
                switch (elem.Name.ToString())
                {
                    case "ThumbnailPath":
                        this.Thumbnail = elem.Value;
                        break;
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

        #endregion
    }
}
