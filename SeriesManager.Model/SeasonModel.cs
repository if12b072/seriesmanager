using System.Linq;

using SeriesManager.Common;
using SeriesManager.Control;
using SeriesManager.Enum;
using SeriesManager.Model.Banner;
using SeriesManager.Model.Comparer;

namespace SeriesManager.Model
{
    public sealed class SeasonModel : BindableBase
    {
        #region Properties

        private BannerBaseModel banner;
        private int number;
        private WatchState state = WatchState.Unwatched;

        public EpisodeModel this[int episodeNumber]
        {
            get
            {
                return this.Episodes.FirstOrDefault(E => E.Number == episodeNumber);
            }
        }

        public SeriesModel Series
        { 
            get; 
            private set; 
        }

        public BannerBaseModel Banner
        {
            get { return this.banner; }
            set 
            {
                SetProperty(ref this.banner, value);
            }
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

        public ObservableSortedList<EpisodeModel> Episodes 
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
                    foreach (var episode in this.Episodes)
                    {
                        episode.State = value;
                    }
                }
            }
        }

        #endregion

        #region Contructor

        public SeasonModel(SeriesModel series, int number)
        {
            this.Watched = new ObservableSortedList<EpisodeModel>(1000, new EpisodeComparer());
            this.Unwatched = new ObservableSortedList<EpisodeModel>(1000, new EpisodeComparer());
            this.UnAired = new ObservableSortedList<EpisodeModel>(1000, new EpisodeComparer());
            this.Episodes = new ObservableSortedList<EpisodeModel>(100, new EpisodeComparer());
            this.Episodes.CollectionChanged += Episodes_CollectionChanged;

            this.Series = series;
            this.Number = number;
        }

        #endregion

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

            CheckIfStateChanged();
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

        private void Episodes_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (EpisodeModel episode in e.NewItems)
                {
                    episode.StateChanged += episode_StateChanged;
                    AddEpisodeState(episode);
                }
            }
        }

        private void CheckIfStateChanged()
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
    }
}
