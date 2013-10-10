using System.Collections.ObjectModel;
using System.Linq;
using Windows.UI.Xaml.Controls;

using GUI.Common;
using GUI.Controls;
using GUI.MVVM.Model;
using GUI.MVVM.View;
using System.Collections.Generic;
using GUI.Comparer;
using GUI.Enums;

namespace GUI.MVVM.ViewModel
{
    public class SubscriptionViewModel : ViewModelBase
    {
        public ListCollectionView Subscription { get; private set; }

        #region DataProperties

        public ObservableCollection<SeriesModel> SelectedItems { get; private set; }
        public bool IsEmpty
        {
            get { return this.Subscription != null ? !this.Subscription.Any() : true; }
        }

        #endregion

        #region CommandProperties

        public RelayCommand SelectionAllCommand { get; private set; }
        public RelayCommand SelectionClearCommand { get; private set; }
        public RelayCommand UnSubscribeCommand { get; private set; }
        public RelayCommand ItemClickedCommand { get; private set; }
        public RelayCommand WatchStateCommand { get; private set; }
        public RelayCommand UnwatchStateCommand { get; private set; }

        #endregion

        public SubscriptionViewModel()
        {
            this.SelectedItems = new ObservableCollection<SeriesModel>();
            this.Subscription = new ListCollectionView(AppX.Instance.Series);
            this.Subscription.Filter = new System.Predicate<object>((S) => (S as SeriesModel).Subscription);
            this.Subscription.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
            this.Subscription.VectorChanged += Vector_Changed;

            this.UnSubscribeCommand = new RelayCommand(OnUnsubscribeExecuted, OnUnsubscribeCanExecute);
            this.ItemClickedCommand = new RelayCommand(OnItemClickExecuted);
            this.SelectionClearCommand = new RelayCommand(OnSelectionClearExecuted, OnSelectionClearCanExecute);
            this.SelectionAllCommand = new RelayCommand(OnSelectionAllExecuted, OnSelectionAllCanExecute);
            this.WatchStateCommand = new RelayCommand(OnWatchStateExecuted, OnWatchStateCanExecute);
            this.UnwatchStateCommand = new RelayCommand(OnUnwatchStateExecuted, OnUnwatchStateCanExecute);
        }

        #region CommandDelegates

        private void OnWatchStateExecuted(object parameter)
        {
            for (int i = 0; i < this.SelectedItems.Count; i++)
            {
                if (this.SelectedItems[i].State == WatchState.Unwatched)
                {
                    this.SelectedItems[i].State = WatchState.Watched;
                    TileManager.UpdateTile(this.SelectedItems[i].ID, this.SelectedItems[i].Unwatched.Count);
                    this.SelectedItems.RemoveAt(i);
                    i--;
                }
            }

            this.WatchStateCommand.Invalidate();
        }
        private bool OnWatchStateCanExecute(object parameter)
        {
            return this.SelectedItems != null && this.SelectedItems.Any(E => E.State == WatchState.Unwatched);
        }
        private void OnUnwatchStateExecuted(object parameter)
        {
            for (int i = 0; i < this.SelectedItems.Count; i++)
            {
                if (this.SelectedItems[i].State == WatchState.Watched || (this.SelectedItems[i].State == WatchState.UnAired && this.SelectedItems[i].Watched.Any()))
                {
                    this.SelectedItems[i].State = WatchState.Unwatched;
                    TileManager.UpdateTile(this.SelectedItems[i].ID, this.SelectedItems[i].Unwatched.Count);
                    this.SelectedItems.RemoveAt(i);
                    i--;
                }
            }

            this.UnwatchStateCommand.Invalidate();
        }
        private bool OnUnwatchStateCanExecute(object parameter)
        {
            return this.SelectedItems != null && this.SelectedItems.Any(E => E.State == WatchState.Watched || (E.Watched.Any() && E.State == WatchState.UnAired));
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
            foreach (SeriesModel series in this.Subscription)
            {
                if (!this.SelectedItems.Any(S => S.ID == series.ID))
                {
                    this.SelectedItems.Add(series);
                }
            }
        }
        private bool OnSelectionAllCanExecute(object parameter)
        {
            return this.SelectedItems != null && this.Subscription.Any();
        }
        private void OnUnsubscribeExecuted(object parameter)
        {
            var subscriptions = this.SelectedItems.ToList();

            foreach (var series in subscriptions)
            {
                series.Subscription = false;
            }

            this.Subscription.Refresh();
        }
        private bool OnUnsubscribeCanExecute(object parameter)
        {
            return this.SelectedItems != null && this.SelectedItems.Count > 0;
        }
        private void OnItemClickExecuted(object parameter)
        {
            var clickedItem = (parameter as ItemClickEventArgs).ClickedItem as SeriesModel;
            base.Navigation.Navigate(typeof(SeriesFrameView), clickedItem.ID);
        }

        #endregion

        private void SelectedItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.UnSubscribeCommand.Invalidate();
            this.SelectionClearCommand.Invalidate();
            this.WatchStateCommand.Invalidate();
            this.UnwatchStateCommand.Invalidate();
            AppX.Instance.NavigationBarIsOpen = this.SelectedItems.Any();
        }
        private void Vector_Changed(Windows.Foundation.Collections.IObservableVector<object> sender, Windows.Foundation.Collections.IVectorChangedEventArgs @event)
        {
            OnPropertyChanged("IsEmpty");
            this.SelectionAllCommand.Invalidate();
        }
        public override void Activate(object parameter, Dictionary<string, object> state)
        {
            this.SelectedItems.CollectionChanged += SelectedItems_CollectionChanged;
            this.SelectedItems.Clear();
            this.Subscription.Refresh();
            this.SelectionAllCommand.Invalidate();
        }
        public override void Deactivate(Dictionary<string, object> state)
        {
            this.SelectedItems.CollectionChanged -= SelectedItems_CollectionChanged;
        }
    }
}
