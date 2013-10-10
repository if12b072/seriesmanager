using System;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Runtime.CompilerServices;

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Networking.BackgroundTransfer;

using SeriesManager.Common;
using SeriesManager.Enum;
using SeriesManager.Extension;

namespace SeriesManager.Model.Banner
{
    public abstract class BannerBaseModel : BindableBase
    {
        private const string bannerPath = "http://thetvdb.com/banners/";
        private string image;
        protected readonly int seriesID;

        public int ID 
        { 
            get; 
            private set; 
        }

        public string ImageSource 
        { 
            get; 
            private set; 
        }

        public string Image
        {
            get 
            {
                string path = "ms-appdata:///local/" + this.seriesID.ToString() + "/" + this.image;
                if (!this.Loading && !this.Loaded)
                {
                    this.Loading = true;
                    Download(this.ImageSource, path).ContinueWith(T =>
                    {
                        try
                        {
                            this.Loaded = T.Result;
                        }
                        catch { this.Loaded = false; }
                        this.Loading = false;
                        OnPropertyChanged();
                    }, TaskContinuationOptions.ExecuteSynchronously);
                }

                return (!this.Loaded) ? null : path;
            }
            private set { this.ImageSource = value; base.SetProperty(ref this.image, System.IO.Path.GetFileName(value)); }
        }
        
        public Languages Language 
        {
            get; 
            private set;
        }

        public double Rating
        {
            get;
            private set;
        }

        public int RatingCount
        {
            get;
            private set;
        }

        public bool Loaded
        {
            get;
            private set;
        }

        public bool Loading
        {
            get;
            private set;
        }

        public BannerBaseModel(int seriesID, string source)
        {
            this.seriesID = seriesID;
            this.Image = source;
        }

        public BannerBaseModel(int seriesID, int id, string source, Languages language, double rating, int ratingCount)
        {
            this.seriesID = seriesID;
            this.ID = id;
            this.Image = source;
            this.Language = language;
            this.Rating = rating;
            this.RatingCount = ratingCount;
        }

        public BannerBaseModel(int seriesID, XElement data)
        {
            this.seriesID = seriesID;

            foreach (XElement elem in data.Elements())
            {
                switch (elem.Name.ToString())
                {
                    case "id":
                        int id;
                        if (Int32.TryParse(elem.Value, out id))
                            this.ID = id;
                        break;
                    case "BannerPath":
                        this.Image = elem.Value;
                        break;
                    case "Language":
                        this.Language = elem.Value.ToLanguage();
                        break;
                    case "Rating":
                        int rating;
                        if (Int32.TryParse(elem.Value, out rating))
                            this.Rating = rating;
                        break;
                    case "RatingCount":
                        int ratingCount;
                        if (Int32.TryParse(elem.Value, out ratingCount))
                            this.RatingCount = ratingCount;
                        break;
                }
            }
        }

        public static BannerBaseModel Create(int seriesID, XElement data)
        {
            BannerBaseModel banner = null;

            string bannerType = data.Element("BannerType").Value;
            switch (bannerType) 
            {
                case "fanart":
                    banner = new FanArtBannerModel(seriesID, data);
                    break;
                case "poster":
                    banner = new PosterBannerModel(seriesID, data);
                    break;
                case "season":
                    banner = new SeasonBannerModel(seriesID, data);
                    break;
                case "series":
                    banner = new WideBannerModel(seriesID, data);
                    break;
            }

            return banner;
        }

        protected async Task<bool> Download(string source, string target)
        {
            if (string.IsNullOrWhiteSpace(source) || string.IsNullOrWhiteSpace(target))
                return false;

            Uri dwlPath = new Uri(bannerPath + source);
            string fileName = Path.GetFileName(target);
            string dirName = Path.GetFileName(Path.GetDirectoryName(target));
            var folder = await ApplicationData.Current.LocalFolder.CreateFolderAsync(dirName, CreationCollisionOption.OpenIfExists);

            var file = await folder.TryGetItemAsync(fileName);
            if (file != null)
            {
                var prop = await file.GetBasicPropertiesAsync();
                if (prop.Size > 0) return true;
            }

            file = await folder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
            var downloader = new BackgroundDownloader();
            var op = downloader.CreateDownload(dwlPath, (StorageFile)file);
            await op.StartAsync();

            return true;
        }

        public BannerBaseModel Clone()
        {
            return (BannerBaseModel)this.MemberwiseClone();
        }
    }
}
