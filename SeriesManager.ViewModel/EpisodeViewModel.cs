using System;
using System.Linq;
using System.Windows.Input;

using GUI.Common;
using GUI.Controls;
using GUI.Extensions;
using GUI.MVVM.Model;
using GUI.Enums;
using System.Collections;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using System.Globalization;

namespace GUI.MVVM.ViewModel
{
    public sealed class EpisodeViewModel : ViewModelBase
    {
        private EpisodeModel selectedItem = null;
        private bool subscription;

        #region DataProperties

        public ObservableSortedList<EpisodeModel> Episodes { get; private set; }
        public EpisodeModel SelectedItem
        {
            get { return this.selectedItem; }
            set 
            {
                if (value != null)
                {
                    SetProperty(ref this.selectedItem, value);
                    OnPropertyChanged("Title");
                    OnPropertyChanged("MetaData");
                    OnPropertyChanged("OverviewVisibility");
                }
                StateCommand.Invalidate();
            }
        }
        public string Title
        {
            get
            {
                return this.SelectedItem != null ? this.SelectedItem.Number + " - " + this.SelectedItem.Name : string.Empty;
            }
        }
        public IEnumerable<IEnumerable<string>> MetaData
        {
            get
            {
                if (this.SelectedItem == null) return null;

                var res = new Windows.ApplicationModel.Resources.ResourceLoader();                

                var shortMetaData = new List<string>();
                if (this.SelectedItem.FirstAired.Date.CompareTo(new DateTime(1, 1, 1)) != 0)
                {
                    shortMetaData.Add(this.SelectedItem.FirstAired.ToString(this.SelectedItem.FirstAired.ToString(CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)));
                }

                if (this.SelectedItem.Rating != 0.1) 
                {
                    shortMetaData.Add(res.GetString("Rating") + ": " + this.SelectedItem.Rating.ToString("0.0"));
                }

                shortMetaData.Add(res.GetString("Season") + " " + this.SelectedItem.Season.Number);

                if (this.SelectedItem.Director.Any()) 
                {
                    shortMetaData.Add(res.GetString("Director") + ": " + string.Join(", ", this.SelectedItem.Director));
                }

                if (this.SelectedItem.Writer.Any()) 
                {
                    shortMetaData.Add(res.GetString("Writer") + ": " + string.Join(", ", this.SelectedItem.Writer));
                }

                return new List<IEnumerable<string>> 
                {
                    shortMetaData,
                    this.SelectedItem.GuestStars
                };
            }
        }
        public bool OverviewVisibility
        {
            get
            {
                return this.SelectedItem != null && this.SelectedItem.Overview.Any();
            }
        }

        #endregion

        #region CommandProperties

        public RelayCommand StateCommand { get; private set; }

        #endregion

        public EpisodeViewModel()
        {
            this.StateCommand = new RelayCommand(OnStateExecuted, OnStateCanExecute);
        }

        #region CommandDelegates

        private void OnStateExecuted(object parameter)
        {
            this.SelectedItem.State = this.SelectedItem.State.Invert();
            TileManager.UpdateTile(this.SelectedItem.Season.Series.ID, this.SelectedItem.Season.Series.Unwatched.Count);
        }
        private bool OnStateCanExecute(object parameter)
        {
            return this.subscription && this.selectedItem != null && this.selectedItem.State != WatchState.UnAired;
        }

        #endregion

        public override void Activate(object parameter, System.Collections.Generic.Dictionary<string, object> state)
        {
            int episodeID = (state != null && state.ContainsKey("SelectedItem")) ? (int)state["SelectedItem"] : (int)parameter;
            var episode = AppX.Instance.Current.GetEpisode(episodeID);

            this.Episodes = episode.Season.Episodes;            
            OnPropertyChanged("Episodes");
            this.SelectedItem = episode;
            this.subscription = episode.Season.Series.Subscription;
        }
        public override void Deactivate(System.Collections.Generic.Dictionary<string, object> state)
        {
            if (this.SelectedItem != null)
            {
                state["SelectedItem"] = this.SelectedItem.ID;
            }
        }
    }
}
