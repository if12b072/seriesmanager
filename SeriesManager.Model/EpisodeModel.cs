using System;
using System.Globalization;
using System.Xml.Linq;

using Windows.UI.Xaml.Media.Imaging;
using Windows.Storage;
using Windows.Storage.Streams;

using SeriesManager.Common;
using SeriesManager.Enum;
using SeriesManager.Model.Banner;

namespace SeriesManager.Model
{
    public sealed class EpisodeModel : BindableBase
    {
        #region Events

        public delegate void StateChangedEventHandler(EpisodeModel sender, WatchState oldState);
        public event StateChangedEventHandler StateChanged;

        #endregion

        #region Properties

        private readonly int id;
        private int number;
        private string name;
        private DateTime firstAired;
        private string[] director;
        private string[] guestStars;
        private string overview;
        private double rating = 0.1;
        private string[] writer;
        private BannerBaseModel image;

        public int ID
        {
            get
            {
                return this.id;
            }
        }

        public SeasonModel Season
        { 
            get; 
            private set; 
        }
      
        public int Number
        {
            get 
            { 
                return this.number; 
            }
            private set 
            { 
                SetProperty(ref this.number, value); 
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

        public string[] Director
        {
            get 
            { 
                return director; 
            }
            private set 
            { 
                base.SetProperty(ref this.director, value); 
            }
        }

        public string[] GuestStars
        {
            get 
            { 
                return guestStars; 
            }
            private set 
            { 
                base.SetProperty(ref this.guestStars, value); 
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

        public string[] Writer
        {
            get 
            { 
                return writer; 
            }
            private set 
            { 
                base.SetProperty(ref this.writer, value); 
            }
        }

        public BannerBaseModel Image
        {
            get 
            { 
                return this.image; 
            }
            private set 
            {
                if (value == null || (this.image != null && this.image.ImageSource == value.ImageSource))
                    return;

                SetProperty(ref this.image, value); 
            }
        }

        public WatchState State
        {
            get
            {
                if (FirstAired.Date.CompareTo(DateTime.Now.Date) >= 0 || FirstAired.CompareTo(new DateTime(1,1,0001)) == 0)
                    return WatchState.UnAired;

                if (ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.Season.Series.ID.ToString()))
                {
                    var container = ApplicationData.Current.RoamingSettings.Containers[this.Season.Series.ID.ToString()];
                    if (container.Values.ContainsKey(this.ID.ToString()))
                    {
                        return (bool)container.Values[this.ID.ToString()] ? WatchState.Watched : WatchState.Unwatched;
                    }
                }

                return WatchState.Unwatched;
            }
            set
            {
                var oldState = this.State;

                if (value != WatchState.UnAired && oldState != WatchState.UnAired && oldState != value)
                {
                    if (ApplicationData.Current.RoamingSettings.Containers.ContainsKey(this.Season.Series.ID.ToString()))
                    {
                        ApplicationData.Current.RoamingSettings.Containers[this.Season.Series.ID.ToString()].Values[this.ID.ToString()] = value == WatchState.Watched;
                    }

                    if (StateChanged != null)
                    {
                        StateChanged(this, oldState);
                    }
                }
            }
        }

        #endregion
        
        #region Constructor

        public EpisodeModel(SeasonModel season, int episodeID, int number, XElement episode) 
        {
            this.Season = season;
            this.id = episodeID;
            this.Number = number;
            Update(episode);
        }

        #endregion

        #region Public

        public void Update(XElement episode)
        {
            foreach (XElement elem in episode.Elements())
            {
                if (elem.Name == "EpisodeName")
                {
                    this.Name = elem.Value;
                }
                else if (elem.Name == "FirstAired")
                {
                    DateTime firstAired;
                    if (DateTime.TryParse(elem.Value, out firstAired))
                        this.FirstAired = firstAired;
                }
                else if (elem.Name == "Director")
                {
                    this.Director = elem.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (elem.Name == "GuestStars")
                {
                    this.GuestStars = elem.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (elem.Name == "Overview")
                {
                    this.Overview = elem.Value.Trim();
                }
                else if (elem.Name == "Rating")
                {
                    double rating;
                    if (double.TryParse(elem.Value, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out rating))
                        this.Rating = rating;
                }
                else if (elem.Name == "Writer")
                {
                    this.Writer = elem.Value.Split(new char[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else if (elem.Name == "filename")
                {
                    this.Image = new BlankBannerModel(this.Season.Series.ID, elem.Value);
                }
            }
        }

        #endregion
    }
}
