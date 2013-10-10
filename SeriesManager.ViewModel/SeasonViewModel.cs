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
using GUI.Enums;
using Windows.ApplicationModel.Resources;

namespace GUI.MVVM.ViewModel
{
    public class SeasonViewModel : ViewModelBase
    {
        private SeasonModel seasonModel;

        #region DataProperties

        public SeasonModel SeasonModel
        {
            get { return this.seasonModel; }
            private set 
            { 
                SetProperty(ref this.seasonModel, value);
                OnPropertyChanged("CommandBarIsOpen");
                OnPropertyChanged("SelectionMode");
                this.SeasonStateCommand.Invalidate();
                this.SelectionAllCommand.Invalidate();
            }
        }
        public ListViewSelectionMode SelectionMode
        {
            get
            {
                return this.SeasonModel != null && this.SeasonModel.Series.Subscription && this.SeasonModel.Episodes.Any(E => E.State != WatchState.UnAired) ? ListViewSelectionMode.Extended : ListViewSelectionMode.None;
            }
        }
        public ObservableCollection<EpisodeModel> SelectedItems 
        { 
            get; 
            private set; 
        }

        #endregion

        #region CommandProperties

        public RelayCommand SelectionAllCommand { get; private set; }
        public RelayCommand SelectionClearCommand { get; private set; }
        public RelayCommand SeasonStateCommand { get; private set; }
        public RelayCommand WatchStateCommand { get; private set; }
        public RelayCommand UnwatchStateCommand { get; private set; }
        public RelayCommand ItemClickedCommand { get; private set; }

        #endregion

        public SeasonViewModel()
        {
            this.SelectedItems = new ObservableCollection<EpisodeModel>();

            this.SeasonStateCommand = new RelayCommand(OnSeasonStateExecuted, OnSeasonStateCanExecute);
            this.ItemClickedCommand = new RelayCommand(OnItemClickExecuted);
            this.WatchStateCommand = new RelayCommand(OnWatchStateExecuted, OnWatchStateCanExecute);
            this.UnwatchStateCommand = new RelayCommand(OnUnwatchStateExecuted, OnUnwatchStateCanExecute);
            this.SelectionClearCommand = new RelayCommand(OnSelectionClearExecuted, OnSelectionClearCanExecute);
            this.SelectionAllCommand = new RelayCommand(OnSelectionAllExecuted, OnSelectionAllCanExecute);
        }

        #region CommandDelegates

        private void OnSelectionAllExecuted(object parameter)
        {
            foreach (var episode in this.SeasonModel.Episodes)
            {
                if (!this.SelectedItems.Any(E => E.ID == episode.ID))
                {
                    this.SelectedItems.Add(episode);
                }
            }
        }
        private bool OnSelectionAllCanExecute(object parameter)
        {
            return this.SelectedItems != null && this.SeasonModel != null && this.SeasonModel.Episodes.Any();
        }
        private void OnSelectionClearExecuted(object parameter)
        {
            this.SelectedItems.Clear();
        }
        private bool OnSelectionClearCanExecute(object parameter)
        {
            return this.SelectedItems.Any();
        }
        private void OnSeasonStateExecuted(object parameter)
        {
            this.SeasonModel.State = this.SeasonModel.State.Invert();
            InvalidateCommands();
            AppX.Instance.NavigationBarIsOpen = this.SelectedItems.Any();
            TileManager.UpdateTile(this.SeasonModel.Series.ID, this.SeasonModel.Series.Unwatched.Count);
        }
        private bool OnSeasonStateCanExecute(object parameter)
        {
            return this.SeasonModel != null && this.SeasonModel.Series.Subscription && this.SeasonModel.Episodes.Any(E => E.State != WatchState.UnAired);
        }
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
            TileManager.UpdateTile(this.SeasonModel.Series.ID, this.SeasonModel.Series.Unwatched.Count);
        }
        private bool OnWatchStateCanExecute(object parameter)
        {
            return this.SeasonModel != null && this.SeasonModel.Series.Subscription && this.SelectedItems.Any(E => E.State == WatchState.Unwatched);
        }
        private void OnUnwatchStateExecuted(object parameter)
        {
            for (int i = 0; i < this.SelectedItems.Count; i++)
            {
                if (this.SelectedItems[i].State == WatchState.Watched)
                {
                    this.SelectedItems[i].State = WatchState.Unwatched;
                    this.SelectedItems.RemoveAt(i);
                    i--;
                }
            }

            InvalidateCommands();
            TileManager.UpdateTile(this.SeasonModel.Series.ID, this.SeasonModel.Series.Unwatched.Count);
        }
        private bool OnUnwatchStateCanExecute(object parameter)
        {
            return this.SeasonModel != null && this.seasonModel.Series.Subscription && this.SelectedItems.Any(E => E.State == WatchState.Watched);
        }
        private void OnItemClickExecuted(object parameter)
        {
            var clickedItem = (parameter as ItemClickEventArgs).ClickedItem as EpisodeModel;
            base.Navigation.Navigate(typeof(EpisodeView), clickedItem.ID);
        }

        #endregion

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
        public override void Activate(object parameter, Dictionary<string, object> state)
        {
            this.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;            

            int seasonNumber = (state != null && state.ContainsKey("SeasonNumber")) ? (int)state["SeasonNumber"] : (int)parameter;

            var season = AppX.Instance.Current.Seasons.FirstOrDefault(S => S.Number == seasonNumber);
            if (season != null)
            {
                this.SeasonModel = season;
            }

            if (state != null && state.ContainsKey("SelectedItems"))
            {
                this.SelectedItems.Clear();
                int[] idCol = state["SelectedItems"] as int[];
                foreach (int id in idCol)
                {
                    var episode = this.SeasonModel.Episodes.FirstOrDefault(E => E.ID == id);
                    if (episode != null)
                    {
                        this.SelectedItems.Add(episode);
                    }
                }
            }
        }
        public override void Deactivate(Dictionary<string, object> state)
        {
            this.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;

            if (this.SeasonModel != null)
            {
                state["SeasonNumber"] = this.SeasonModel.Number;
            }

            if (this.SelectedItems.Count > 0)
            {
                int[] idCol = new int[this.SelectedItems.Count];
                for (int i = 0; i < this.SelectedItems.Count; i++)
                {
                    idCol[i] = this.SelectedItems[i].ID;
                }
                state["SelectedItems"] = idCol;
            }
        }
    }
}
