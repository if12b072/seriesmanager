using System.Collections.ObjectModel;
using System;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Net.Http;
using System.IO.Compression;
using Windows.Storage;
using System.Globalization;

using SeriesManager.Enum;
using SeriesManager.Common;
using SeriesManager.Control;
using SeriesManager.Model.Comparer;
using SeriesManager.Extension;
using SeriesManager.Model.Banner;

namespace SeriesManager.Model
{
    public sealed class SeriesModel : BindableBase
    {
        #region Properties

        private WatchState state = WatchState.Unwatched;
        private string imdb;
        private Languages language;
        private string name;
        private double rating = 0.1;
        private TimeSpan airTime;
        private DayOfWeek airDay;
        private DateTime firstAired;
        private int runtime;
        private string[] genre;
        private string network;
        private string overview;
        private bool running;
        private FanArtBannerModel fanArt;
        private WideBannerModel banner;
        private PosterBannerModel poster;

        public SeasonModel this[int seasonNumber]
        {
            get
            {
                return this.Seasons.FirstOrDefault(S => S.Number == seasonNumber);
            }
        }

        public ObservableSortedList<SeasonBannerModel> SeasonBanners
        {
            get;
            private set;
        }

        public ObservableSortedList<WideBannerModel> WideBanners
        {
            get;
            private set;
        }

        public ObservableSortedList<FanArtBannerModel> FanArtBanners
        {
            get;
            private set;
        }

        public ObservableSortedList<PosterBannerModel> PosterBanners
        {
            get;
            private set;
        }

        public ObservableCollection<ActorModel> Actors
        {
            get;
            private set;
        }

        public ObservableSortedList<SeasonModel> Seasons
        {
            get;
            private set;
        }

        public ObservableSortedList<EpisodeModel> Watched
        {
            get;
            private set;
        }

        public ObservableSortedList<EpisodeModel> Unwatched
        {
            get;
            private set;
        }

        public ObservableSortedList<EpisodeModel> UnAired
        {
            get;
            private set;
        }

        public int ID
        {
            get;
            private set;
        }

        public string IMDB
        {
            get 
            { 
                return imdb; 
            }
            private set 
            {
                base.SetProperty(ref this.imdb, "http://www.imdb.com/title/" + value); 
            }
        }

        public Languages Language
        {
            get 
            { 
                return language; 
            }
            set 
            {
                base.SetProperty(ref this.language, value);
            }
        }

        public string Name
        {
            get 
            { 
                return name; 
            }
            private set 
            { 
                base.SetProperty(ref this.name, value); 
            }
        }

        public double Rating
        {
            get 
            { 
                return rating; 
            }
            private set 
            { 
                base.SetProperty(ref this.rating, value); 
            }
        }

        public TimeSpan AirTime
        {
            get 
            { 
                return airTime; 
            }
            private set 
            { 
                base.SetProperty(ref this.airTime, value); 
            }
        }

        public DayOfWeek AirDay
        {
            get 
            { 
                return airDay; 
            }
            private set 
            { 
                base.SetProperty(ref this.airDay, value); 
            }
        }

        public DateTime FirstAired
        {
            get 
            { 
                return firstAired; 
            }
            private set 
            { 
                base.SetProperty(ref this.firstAired, value); 
            }
        }

        public int Runtime
        {
            get 
            { 
                return runtime; 
            }
            private set 
            { 
                base.SetProperty(ref this.runtime, value); 
            }
        }

        public string[] Genre
        {
            get 
            { 
                return genre; 
            }
            private set 
            { 
                base.SetProperty(ref this.genre, value); 
            }
        }

        public string Network
        {
            get 
            {
                return network; 
            }
            private set 
            { 
                base.SetProperty(ref this.network, value); 
            }
        }

        public string Overview
        {
            get 
            { 
                return overview; 
            }
            private set 
            { 
                base.SetProperty(ref this.overview, value); 
            }
        }

        public bool Running
        {
            get 
            { 
                return running; 
            }
            private set 
            { 
                base.SetProperty(ref this.running, value); 
            }
        }

        public FanArtBannerModel FanArt
        {
            get 
            { 
                return this.fanArt; 
            }
            set 
            {
                if (value != null && ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
                {
                    ApplicationData.Current.RoamingSettings.Containers[this.ID.ToString()].Values["fanart"] = value.ID.ToString();
                }
                                
                SetProperty(ref this.fanArt, value);
            }
        }

        public WideBannerModel Banner
        {
            get 
            { 
                return this.banner; 
            }
            set 
            {
                if (value != null && ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
                {
                    ApplicationData.Current.RoamingSettings.Containers[this.ID.ToString()].Values["banner"] = value.ID.ToString();
                }
                
                SetProperty(ref this.banner, value);
            }
        }

        public PosterBannerModel Poster
        {
            get 
            { 
                return this.poster;
            }
            set
            {
                if (value != null && ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
                {
                    ApplicationData.Current.RoamingSettings.Containers[this.ID.ToString()].Values["poster"] = value.ID.ToString();
                }
                
                SetProperty(ref this.poster, value);
            }
        }

        public bool Subscription
        {
            get
            {
                return ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString());
            }
            set
            {
                if (value)
                {
                    if (!ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
                    {
                        ApplicationData.Current.RoamingSettings.CreateContainer(this.ID.ToString(), ApplicationDataCreateDisposition.Always);
                    }

                    foreach (var episode in this.Watched)
                    {
                        if (ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
                        {
                            ApplicationData.Current.RoamingSettings.Containers[this.ID.ToString()].Values[episode.ID.ToString()] = episode.State == WatchState.Watched;
                        }
                    }
                }
                else
                {                    
                    this.State = WatchState.Unwatched;
                    if (ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
                    {
                        ApplicationData.Current.RoamingSettings.DeleteContainer(this.ID.ToString());
                    }
                }

                OnPropertyChanged("Subscription");
            }
        }

        public WatchState State
        {
            get
            {
                return this.state;
            }
            set
            {
                if (value != WatchState.UnAired)
                {
                    foreach (var season in this.Seasons)
                    {
                        season.State = value;
                    }
                }
            }
        }

        public int Count
        {
            get
            {
                int i = 0;
                foreach (SeasonModel s in this.Seasons)
                {
                    i += s.Episodes.Count;
                }
                return i;
            }
        }

        public IEnumerable<EpisodeModel> AirToday
        {
            get
            {
                return this.UnAired.Where(E => E.FirstAired.Date.CompareTo(DateTime.Now.Date) == 0);
            }
        }

        public IEnumerable<EpisodeModel> AirWeek
        {
            get
            {
                var today = DateTime.Today;
                var startOfNextWeek = today.StartOfWeek(DayOfWeek.Monday).AddDays(7);

                return this.UnAired.Where(E => E.FirstAired.Date.CompareTo(today.Date) > 0 && E.FirstAired.Date.CompareTo(startOfNextWeek.Date) < 0);
            }
        }

        public IEnumerable<EpisodeModel> AirMonth
        {
            get
            {
                DateTime today = DateTime.Today;
                DateTime nextMonth = today.AddMonths(1);
                var startOfNextWeek = today.StartOfWeek(DayOfWeek.Monday).AddDays(7);
                nextMonth = new DateTime(nextMonth.Year, nextMonth.Month, 1);

                return this.UnAired.Where(E => E.FirstAired.Date.CompareTo(startOfNextWeek.Date) >= 0 && E.FirstAired.Date.CompareTo(nextMonth.Date) < 0);
            }
        }

        public IEnumerable<EpisodeModel> AirYear
        {
            get
            {
                var today = DateTime.Today;
                return this.UnAired.Where(E => E.FirstAired.Month > today.Month && E.FirstAired.Year == today.Year);
            }
        }

        public IEnumerable<EpisodeModel> AirOther
        {
            get
            {
                var nextYear = new DateTime(DateTime.Today.Year + 1, 1, 1);
                return this.UnAired.Where(E => E.FirstAired.Date.CompareTo(nextYear.Date) >= 0 || E.FirstAired.Date.CompareTo(new DateTime(1, 1, 1)) == 0);
            }
        }

        #endregion

        #region Constructor

        private SeriesModel()
        {
            this.SeasonBanners = new ObservableSortedList<SeasonBannerModel>(1000, new BannerBaseComparer());
            this.WideBanners = new ObservableSortedList<WideBannerModel>(1000, new BannerBaseComparer());
            this.FanArtBanners = new ObservableSortedList<FanArtBannerModel>(1000, new BannerBaseComparer());
            this.PosterBanners = new ObservableSortedList<PosterBannerModel>(1000, new BannerBaseComparer());
            this.Seasons = new ObservableSortedList<SeasonModel>(1000, new SeasonComparer());
            this.Actors = new ObservableCollection<ActorModel>();
            this.Watched = new ObservableSortedList<EpisodeModel>(1000, new EpisodeComparer());
            this.Unwatched = new ObservableSortedList<EpisodeModel>(1000, new EpisodeComparer());
            this.UnAired = new ObservableSortedList<EpisodeModel>(1000, new EpisodeComparer());
        }

        public SeriesModel(int seriesID, Languages language) : this()
        {
            this.ID = seriesID;
            this.language = language;
        }

        public SeriesModel(XDocument doc) : this()
        {
            Update(doc);
        }

        #endregion

        #region Public

        public void Update(XDocument doc)
        {
            if (doc == null || doc.Element("Data") == null)
                return;

            var series = doc.Element("Data").Element("Series");

            if (series == null || series.Element("id") == null)
                return;
            
            int id;
            if (!int.TryParse(series.Element("id").Value, out id))
                return;

            this.ID = id;

            // Update Actors
            var actorCol = doc.Element("Data").Element("Actors");
            if (actorCol != null)
                UpdateActors(actorCol);

            // Update Banners
            var bannerCol = doc.Element("Data").Element("Banners");
            if (bannerCol != null)
                UpdateBanners(bannerCol);

            // Update Series Info
            UpdateSeriesInfo(series);

            // Update FanArt
            UpdateFanArt(series.Element("fanart"));

            // Update Banner
            UpdateBanner(series.Element("banner"));

            // Update Poster
            UpdatePoster();

            // Update Episodes
            var epiCol = doc.Element("Data").Elements("Episode");
            if (epiCol != null)
                UpdateEpisodeInfo(epiCol);

            InvokeState();

            OnPropertyChanged("Count");
        }

        #endregion

        #region Private

        private void UpdateFanArt(XElement elem)
        {
            object fanArtSave = null;

            if (ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
            {
                var container = ApplicationData.Current.RoamingSettings.Containers[this.ID.ToString()];
                if (container.Values.ContainsKey("fanart"))
                {
                    fanArtSave = container.Values["fanart"];
                }
            }

            if (fanArtSave != null)
            {
                int fanArtID = Int32.Parse((string)fanArtSave);
                var fanArt = this.FanArtBanners.FirstOrDefault(F => F.ID == fanArtID);
                if (fanArt != null)
                {
                    this.FanArt = fanArt;
                    return;
                }
            }

            var first = this.FanArtBanners.FirstOrDefault();
            if (first != null)
            {
                this.FanArt = first;
                return;
            }

            if (elem != null)
            {
                this.FanArt = new FanArtBannerModel(this.ID, elem.Value);
            }
        }

        private void UpdateBanner(XElement elem)
        {
            object bannerSave = null;

            if (ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
            {
                var container = ApplicationData.Current.RoamingSettings.Containers[this.ID.ToString()];
                if (container.Values.ContainsKey("banner"))
                {
                    bannerSave = container.Values["banner"];
                }
            }

            if (bannerSave != null)
            {
                int bannerID = Int32.Parse((string)bannerSave);
                var banner = this.WideBanners.FirstOrDefault(B => B.ID == bannerID);
                if (banner != null)
                {
                    this.Banner = banner;
                    return;
                }
            }

            var first = this.WideBanners.FirstOrDefault();
            if (first != null)
            {
                this.Banner = first;
                return;
            }

            if (elem != null)
                this.Banner = new WideBannerModel(this.ID, elem.Value);
        }

        private void UpdatePoster()
        {
            object posterSave = null;

            if (ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.ID.ToString()))
            {
                var container = ApplicationData.Current.RoamingSettings.Containers[this.ID.ToString()];
                if (container.Values.ContainsKey("poster"))
                {
                    posterSave = container.Values["poster"];
                }
            }

            if (posterSave != null)
            {
                int bannerID = Int32.Parse((string)posterSave);
                var poster = this.PosterBanners.FirstOrDefault(B => B.ID == bannerID);
                if (poster != null)
                {
                    this.Poster = poster;
                    return;
                }
            }

            var first = this.PosterBanners.FirstOrDefault();
            if (first != null)
            {
                this.Poster = first;
                return;
            }
        }

        private void UpdateActors(XElement actors)
        {
            if (actors != null)
            {
                foreach (XElement elem in actors.Elements("Actor"))
                {
                    ActorModel actor = new ActorModel(elem, this.ID);
                    if (!this.Actors.Any(A => A.Name == actor.Name))
                    {
                        this.Actors.Add(actor);
                    }
                }
            }
        }

        private void UpdateBanners(XElement banners)
        {
            if (banners == null) return;

            foreach (XElement elem in banners.Elements("Banner"))
            {
                var banner = BannerBaseModel.Create(this.ID, elem);
                if (banner == null) continue;

                if (banner is SeasonBannerModel && !this.SeasonBanners.Any(B => B.ID == banner.ID))
                {
                    this.SeasonBanners.Add((SeasonBannerModel)banner);
                }
                else if (banner is FanArtBannerModel && !this.FanArtBanners.Any(B => B.ID == banner.ID))
                {
                    this.FanArtBanners.Add((FanArtBannerModel)banner);
                }
                else if (banner is WideBannerModel && !this.WideBanners.Any(B => B.ID == banner.ID))
                {
                    this.WideBanners.Add((WideBannerModel)banner);
                }
                else if (banner is PosterBannerModel && !this.PosterBanners.Any(B => B.ID == banner.ID))
                {
                    this.PosterBanners.Add((PosterBannerModel)banner);
                }
            }
        }

        private void UpdateSeriesInfo(XElement series)
        {
            if (series == null)
                return;

            foreach (XElement elem in series.Elements())
            {
                if (elem.Name == "SeriesName")
                {
                    this.Name = elem.Value;
                }
                else if (elem.Name == "Rating")
                {
                    double rating;
                    if (double.TryParse(elem.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out rating))
                        this.Rating = rating;
                }
                else if (elem.Name == "Airs_Time")
                {
                    string airTime = elem.Value.Replace(" ", "");
                    DateTime tmp;
                    if (DateTime.TryParse(airTime, out tmp))
                    {
                        this.AirTime = tmp.TimeOfDay;
                    }
                }
                else if (elem.Name == "Airs_DayOfWeek")
                {
                    DayOfWeek airDay;
                    if (System.Enum.TryParse<DayOfWeek>(elem.Value, out airDay))
                        this.AirDay = airDay;
                }
                else if (elem.Name == "FirstAired")
                {
                    DateTime firstAired;
                    if (DateTime.TryParse(elem.Value, out firstAired))
                        this.FirstAired = firstAired;
                }
                else if (elem.Name == "Runtime")
                {
                    byte runtime;
                    if (byte.TryParse(elem.Value, out runtime))
                        this.Runtime = runtime;
                }
                else if (elem.Name == "Genre")
                {
                    this.Genre = elem.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (elem.Name == "Network")
                {
                    this.Network = elem.Value;
                }
                else if (elem.Name == "Overview")
                {
                    this.Overview = elem.Value.Trim();
                }
                else if (elem.Name == "Status")
                {
                    this.Running = elem.Value == "Continuing";
                }
                else if (elem.Name.ToString().ToLower() == "language")
                {
                    this.language = elem.Value.ToLanguage();
                }
                else if (elem.Name == "IMDB_ID")
                {
                    this.IMDB = elem.Value;
                }
            }
        }

        private void UpdateEpisodeInfo(IEnumerable<XElement> episodesXML)
        {
            if (episodesXML == null)
                return;

            var newSeasons = new List<SeasonModel>();

            foreach (XElement elem in episodesXML)
            {
                var epiSeason = elem.Element("SeasonNumber");
                int seasonNumber;
                if (epiSeason == null || epiSeason.Value == null || !int.TryParse(epiSeason.Value, out seasonNumber) || seasonNumber < 1)
                    continue;

                var idElem = elem.Element("id");
                int id;
                if (idElem == null || idElem.Value == null || string.IsNullOrWhiteSpace(idElem.Value) || !Int32.TryParse(idElem.Value, out id)) 
                    continue;

                var numberElem = elem.Element("EpisodeNumber");
                int number;
                if (numberElem == null || numberElem.Value == null || string.IsNullOrWhiteSpace(numberElem.Value) || !Int32.TryParse(numberElem.Value, out number) || number <= 0) 
                    continue;
                
                var sBanner = this.SeasonBanners.FirstOrDefault(B => B.Number == seasonNumber);
                var season = this.Seasons.FirstOrDefault(S => S.Number == seasonNumber);
                if (season == null)
                {
                    season = new SeasonModel(this, seasonNumber);
                    newSeasons.Add(season);
                    this.Seasons.Add(season);
                }
                season.Banner = sBanner;

                var episode = season.Episodes.FirstOrDefault(E => E.ID == id);
                if (episode == null)
                {
                    episode = new EpisodeModel(season, id, number, elem);
                    episode.StateChanged += episode_StateChanged;
                    AddEpisodeState(episode);
                    season.Episodes.Add(episode);
                }
                else
                {
                    episode.Update(elem);
                }
            }

            foreach (var season in newSeasons)
            {
                season.PropertyChanged += season_PropertyChanged;
            }
        }

        private void episode_StateChanged(EpisodeModel episode, WatchState oldState)
        {
            switch (oldState)
            {
                case WatchState.Watched:
                    this.Watched.Remove(episode);
                    break;
                case WatchState.Unwatched:
                    this.Unwatched.Remove(episode);
                    break;
                case WatchState.UnAired:
                    this.UnAired.Remove(episode);
                    break;
            }

            AddEpisodeState(episode);
        }

        private void season_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State")
            {
                InvokeState();
            }           
        }

        private void AddEpisodeState(EpisodeModel episode)
        {
            switch (episode.State)
            {
                case WatchState.Watched:
                    this.Watched.Add(episode);
                    break;
                case WatchState.Unwatched:
                    this.Unwatched.Add(episode);
                    break;
                case WatchState.UnAired:
                    this.UnAired.Add(episode);
                    break;
            }
        }

        private void InvokeState()
        {
            if (!this.Unwatched.Any())
            {
                var state = this.UnAired.Any() ? WatchState.UnAired : WatchState.Watched;
                SetProperty(ref this.state, state, "State");
            }
            else
            {
                SetProperty(ref this.state, WatchState.Unwatched, "State");
            }
        }

        #endregion
    }
}
