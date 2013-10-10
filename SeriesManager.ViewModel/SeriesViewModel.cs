using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

using GUI.Common;
using GUI.Extensions;
using GUI.MVVM;
using GUI.MVVM.Model;
using GUI.MVVM.View;
using Windows.UI.StartScreen;
using Windows.Foundation;
using Windows.ApplicationModel.Resources;
using GUI.Controls;
using System.Globalization;
using Windows.UI.Xaml.Media;
using Windows.UI.Notifications;
using GUI.Enums;

namespace GUI.MVVM.ViewModel
{
    public class SeriesViewModel : ViewModelBase
    {
        private bool tileExists;
        private SeriesModel viewSource;

        #region DataProperties

        public IEnumerable<IEnumerable<string>> MetaData
        {
            get
            {
                if (this.ViewSource == null) return null;
                var res = new ResourceLoader();
                int seasons = this.ViewSource.Seasons.Count;
                int episodes = this.ViewSource.Count;

                var shortMetaData = new List<string>();
                if (this.ViewSource.FirstAired.Date.CompareTo(new DateTime(1, 1, 1)) != 0)
                {
                    shortMetaData.Add(this.ViewSource.FirstAired.Year.ToString());
                }

                if (this.ViewSource.Rating != 0.1)
                {
                    shortMetaData.Add(res.GetString("Rating") + ": " + this.ViewSource.Rating.ToString("0.0"));
                }

                if (this.ViewSource.Runtime > 0)
                {
                    shortMetaData.Add(res.GetString("Duration") + ": " + this.ViewSource.Runtime.ToString() + " " + res.GetString("MinutesAbbreviation"));
                }

                if (this.ViewSource.AirTime.Ticks > 0 && this.ViewSource.Running)
                {
                    shortMetaData.Add(CultureInfo.CurrentCulture.DateTimeFormat.GetDayName((System.DayOfWeek)this.ViewSource.AirDay) + " " + new DateTime(this.ViewSource.AirTime.Ticks).ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortTimePattern));
                }

                if (!this.ViewSource.Running)
                {
                    shortMetaData.Add(new ResourceLoader().GetString("NotRunning"));
                }

                shortMetaData.Add(seasons.ToString() + " " + (seasons == 1 ? res.GetString("Season") : res.GetString("Seasons")));
                shortMetaData.Add(episodes.ToString() + " " + (episodes == 1 ? res.GetString("Episode") : res.GetString("Episodes")));

                return new List<IEnumerable<string>>
                {
                    shortMetaData,
                    this.ViewSource.Genre
                };
            }
        }
        public SeriesModel ViewSource
        {
            get { return this.viewSource; }
            private set 
            {
                if (this.viewSource != null)
                {
                    this.viewSource.PropertyChanged -= viewSource_PropertyChanged;
                }

                SetProperty(ref this.viewSource, value); 
                OnPropertyChanged("LatestUnwatched");
                OnPropertyChanged("LatestUnwatchedVisibility");
                OnPropertyChanged("NextUnAired");
                OnPropertyChanged("NextUnAiredVisibility");
                OnPropertyChanged("MetaData");
                OnPropertyChanged("RandomFanArt");
                OnPropertyChanged("UnAiredVisibility");
                OnPropertyChanged("SelectionMode");
                this.StateCommand.Invalidate();
                this.MediaCommand.Invalidate();
                this.TileCommand.Invalidate();
                this.UnAiredCommand.Invalidate();
                this.ActorCommand.Invalidate();
                this.LatestUnwatchedCommand.Invalidate();
                this.NextUnAiredCommand.Invalidate();
                this.WatchStateCommand.Invalidate();
                this.UnwatchStateCommand.Invalidate();
                this.SelectionAllCommand.Invalidate();

                if (this.viewSource != null)
                {
                    this.viewSource.PropertyChanged += viewSource_PropertyChanged;
                }
            }
        }
        public EpisodeModel LatestUnwatched
        {
            get
            {
                return this.ViewSource != null ? this.ViewSource.Unwatched.FirstOrDefault() : null;
            }
        }
        public EpisodeModel NextUnAired
        {
            get
            {
                return this.ViewSource != null ? this.ViewSource.UnAired.FirstOrDefault() : null;
            }
        }
        public Visibility LatestUnwatchedVisibility
        {
            get { return this.ViewSource != null && this.ViewSource.Subscription && this.ViewSource.Unwatched.Any() ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility NextUnAiredVisibility
        {
            get { return this.ViewSource != null && this.ViewSource.State == WatchState.UnAired ? Visibility.Visible : Visibility.Collapsed; }
        }
        public Visibility UnAiredVisibility
        {
            get { return this.ViewSource != null && this.ViewSource.UnAired.Any() ? Visibility.Visible : Visibility.Collapsed; }
        }
        public bool TileExists
        {
            get { return this.tileExists; }
            private set { this.tileExists = value; OnPropertyChanged("TileExists"); }
        }
        public FanArtBannerModel RandomFanArt
        {
            get
            {
                if (this.ViewSource == null) return null;

                int max = this.ViewSource.FanArtBanners.Count-1;
                if (max > 10)
                {
                    max = 10;
                }

                int rnd = new Random().Next(0, max);
                return this.ViewSource.FanArtBanners[rnd];
            }
        }
        public ListViewSelectionMode SelectionMode
        {
            get
            {
                return this.ViewSource != null && this.ViewSource.Subscription && (this.ViewSource.Unwatched.Any() || this.ViewSource.Watched.Any()) ? ListViewSelectionMode.Extended : ListViewSelectionMode.None;
            }
        }
        public ObservableCollection<SeasonModel> SelectedItems
        { 
            get; 
            private set; 
        }

        #endregion

        #region CommandProperties

        public RelayCommand WatchStateCommand { get; private set; }
        public RelayCommand UnwatchStateCommand { get; private set; }
        public RelayCommand SelectionAllCommand { get; private set; }
        public RelayCommand SelectionClearCommand { get; private set; }
        public RelayCommand SeasonCommand { get; private set; }
        public RelayCommand StateCommand { get; private set; }
        public RelayCommand MediaCommand { get; private set; }
        public RelayCommand TileCommand { get; private set; }
        public RelayCommand UnAiredCommand { get; private set; }
        public RelayCommand ActorCommand { get; private set; }
        public RelayCommand LatestUnwatchedCommand { get; private set; }
        public RelayCommand NextUnAiredCommand { get; private set; }

        #endregion

        public SeriesViewModel()
        {
            this.SelectedItems = new ObservableCollection<SeasonModel>();

            this.SeasonCommand = new RelayCommand(OnSeasonExecuted);
            this.StateCommand = new RelayCommand(OnStateExecuted, OnStateCanExecute);
            this.MediaCommand = new RelayCommand(OnMediaExecuted, OnMediaCanExecute);
            this.TileCommand = new RelayCommand(OnTileExecuted, OnTileCanExecute);
            this.UnAiredCommand = new RelayCommand(OnUnAiredExecuted, OnUnAiredCanExecute);
            this.ActorCommand = new RelayCommand(OnActorExecuted, OnActorCanExecute);
            this.LatestUnwatchedCommand = new RelayCommand(OnLatestUnwatchedExecuted, OnLatestUnwatchedCanExecute);
            this.WatchStateCommand = new RelayCommand(OnWatchStateExecuted, OnWatchStateCanExecute);
            this.UnwatchStateCommand = new RelayCommand(OnUnwatchStateExecuted, OnUnwatchStateCanExecute);
            this.SelectionClearCommand = new RelayCommand(OnSelectionClearExecuted, OnSelectionClearCanExecute);
            this.SelectionAllCommand = new RelayCommand(OnSelectionAllExecuted, OnSelectionAllCanExecute);
            this.NextUnAiredCommand = new RelayCommand(OnNextUnAiredExecuted, OnNextUnAiredCanExecute);
        }

        #region DelegateCommands

        private void OnWatchStateExecuted(object parameter)
        {
            for (int i = 0; i < this.SelectedItems.Count; i++)
            {
                if (this.SelectedItems[i].State == WatchState.Unwatched)
                {
                    this.SelectedItems[i].State = WatchState.Watched;
                    this.SelectedItems.RemoveAt(i);
                    i--;
                }
            }

            InvalidateCommands();
            TileManager.UpdateTile(this.ViewSource.ID, this.ViewSource.Unwatched.Count);
        }
        private bool OnWatchStateCanExecute(object parameter)
        {
            return this.ViewSource != null && this.ViewSource.Subscription && this.SelectedItems.Any(E => E.State == WatchState.Unwatched);
        }
        private void OnUnwatchStateExecuted(object parameter)
        {
            for (int i = 0; i < this.SelectedItems.Count; i++)
            {
                if (this.SelectedItems[i].State == WatchState.Watched || (this.SelectedItems[i].State == WatchState.UnAired && this.SelectedItems[i].Watched.Any()))
                {
                    this.SelectedItems[i].State = WatchState.Unwatched;
                    this.SelectedItems.RemoveAt(i);
                    i--;
                }
            }

            InvalidateCommands();
            TileManager.UpdateTile(this.ViewSource.ID, this.ViewSource.Unwatched.Count);
        }
        private bool OnUnwatchStateCanExecute(object parameter)
        {
            return this.ViewSource != null && this.ViewSource.Subscription && this.SelectedItems.Any(E => E.State == WatchState.Watched || (E.Watched.Any() && E.State == WatchState.UnAired));
        }
        private void OnSelectionClearExecuted(object parameter)
        {
            this.SelectedItems.Clear();
        }
        private bool OnSelectionClearCanExecute(object parameter)
        {
            return this.SelectedItems.Any();
        }
        private void OnSelectionAllExecuted(object parameter)
        {
            foreach (var season in this.ViewSource.Seasons)
            {
                if (!this.SelectedItems.Any(S => S.Number == season.Number))
                {
                    this.SelectedItems.Add(season);
                }
            }
        }
        private bool OnSelectionAllCanExecute(object parameter)
        {
            return this.SelectedItems != null && this.ViewSource != null && this.ViewSource.Seasons.Any();
        }
        private void OnSeasonExecuted(object parameter)
        {
            var clickedItem = (parameter as ItemClickEventArgs).ClickedItem as SeasonModel;
            base.Navigation.Navigate(typeof(SeasonView), clickedItem.Number);
        }
        private void OnStateExecuted(object parameter)
        {
            TileManager.UpdateTile(this.ViewSource.ID, this.ViewSource.Unwatched.Count);
        }
        private bool OnStateCanExecute(object parameter)
        {
            return this.ViewSource != null && this.ViewSource.Subscription && (this.ViewSource.Watched.Count > 0 || this.ViewSource.Unwatched.Count > 0);
        }
        private void OnMediaExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(MediaView));
        }
        private bool OnMediaCanExecute(object parameter)
        {
            return this.ViewSource != null && this.ViewSource.FanArtBanners.Count > 1;
        }
        private async void OnTileExecuted(object parameter)
        {
            var button = parameter as FrameworkElement;
            GeneralTransform buttonTransform = button.TransformToVisual(null);
            Point point = buttonTransform.TransformPoint(new Point());
            var rect = new Rect(point, new Size(button.ActualWidth, button.ActualHeight));

            if (!SecondaryTile.Exists(this.ViewSource.ID.ToString()))
            {
                await this.ViewSource.FanArt.DownloadThumbnail();
                Uri thumb = new Uri(this.ViewSource.FanArt.Thumbnail);
                SecondaryTile secondaryTile = new SecondaryTile(this.ViewSource.ID.ToString(), this.ViewSource.Name, this.ViewSource.ID.ToString(), thumb, TileSize.Wide310x150);
                secondaryTile.VisualElements.Wide310x150Logo = thumb;
                secondaryTile.VisualElements.ShowNameOnSquare150x150Logo = true;
                secondaryTile.VisualElements.ShowNameOnWide310x150Logo = true;
                secondaryTile.RoamingEnabled = true;
                this.TileExists = await secondaryTile.RequestCreateForSelectionAsync(rect, Windows.UI.Popups.Placement.Above);
                if (this.TileExists)
                {
                    TileManager.UpdateTile(this.ViewSource.ID, this.ViewSource.Unwatched.Count);
                }
                return;
            }
            else
            {
                SecondaryTile secondaryTile = new SecondaryTile(this.ViewSource.ID.ToString());
                this.TileExists = !(await secondaryTile.RequestDeleteForSelectionAsync(rect, Windows.UI.Popups.Placement.Above));
            }
        }
        private bool OnTileCanExecute(object parameter)
        {
            return this.ViewSource != null && this.ViewSource.Subscription;
        }
        private void OnUnAiredExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(AirView), 0);
        }
        private bool OnUnAiredCanExecute(object parameter)
        {
            return this.ViewSource != null && this.ViewSource.UnAired.Any();
        }
        private void OnActorExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(ActorView));
        }
        private bool OnActorCanExecute(object parameter)
        {
            return this.ViewSource != null && this.ViewSource.Actors.Any();
        }
        private void OnLatestUnwatchedExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(EpisodeView), this.LatestUnwatched.ID);
        }
        private bool OnLatestUnwatchedCanExecute(object parameter)
        {
            return this.LatestUnwatched != null;
        }
        private void OnNextUnAiredExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(EpisodeView), this.NextUnAired.ID);
        }
        private bool OnNextUnAiredCanExecute(object parameter)
        {
            return this.NextUnAired != null;
        }

        #endregion

        public override void Activate(object parameter, Dictionary<string, object> state)
        {
            this.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;            

            this.ViewSource = AppX.Instance.Current;
            this.TileExists = SecondaryTile.Exists(this.ViewSource.ID.ToString());

            if (state != null && state.ContainsKey("SelectedItems"))
            {
                this.SelectedItems.Clear();
                int[] idCol = state["SelectedItems"] as int[];
                foreach (int id in idCol)
                {
                    var season = this.ViewSource.Seasons.FirstOrDefault(S => S.Number == id);
                    if (season != null)
                    {
                        this.SelectedItems.Add(season);
                    }
                }
            }
        }
        public override void Deactivate(Dictionary<string, object> state)
        {
            this.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
            
            if (this.SelectedItems.Count > 0)
            {
                int[] idCol = new int[this.SelectedItems.Count];
                for (int i = 0; i < this.SelectedItems.Count; i++)
                {
                    idCol[i] = this.SelectedItems[i].Number;
                }
                state["SelectedItems"] = idCol;
            }
        }
        private void viewSource_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "State" || e.PropertyName == "Subscription")
            {               
                OnPropertyChanged("LatestUnwatched");
                OnPropertyChanged("LatestUnwatchedVisibility");
                OnPropertyChanged("NextUnAired");
                OnPropertyChanged("NextUnAiredVisibility");
                LatestUnwatchedCommand.Invalidate();
                NextUnAiredCommand.Invalidate();
            }

            if (e.PropertyName == "Subscription")
            {
                this.TileCommand.Invalidate();
                this.StateCommand.Invalidate();
                OnPropertyChanged("SelectionMode");
            }
        }
        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.SelectionClearCommand.Invalidate();
            this.WatchStateCommand.Invalidate();
            this.UnwatchStateCommand.Invalidate();
            AppX.Instance.NavigationBarIsOpen = this.SelectedItems.Any();
        }
        private void InvalidateCommands()
        {
            this.WatchStateCommand.Invalidate();
            this.UnwatchStateCommand.Invalidate();
        }
    }
}
