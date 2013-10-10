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
using Windows.Storage;
using Windows.System.UserProfile;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;

using GUI.Common;
using GUI.MVVM;
using GUI.MVVM.Model;
using GUI.Enums;
using GUI.MVVM.View;

namespace GUI.MVVM.ViewModel
{
    public class MediaViewModel : ViewModelBase
    {
        private FanArtBannerModel selectedItem;
        private WatchState state;
        private IEnumerable<FanArtBannerModel> banners;

        #region DataProperties

        public WatchState State
        {
            get { return this.state; }
            private set { SetProperty(ref this.state, value); }
        }
        public IEnumerable<FanArtBannerModel> Banners
        {
            get { return this.banners; }
            private set { SetProperty(ref this.banners, value); }
        }
        public FanArtBannerModel SelectedItem
        {
            get { return this.selectedItem; }
            set 
            {
                SetProperty(ref this.selectedItem, value);
                if (value != null)
                {
                    base.Navigation.Navigate(typeof(MediaDetailView), value.ID);
                }
            }
        }

        #endregion

        #region CommandProperties

        public RelayCommand LoadedCommand { get; private set; }

        #endregion

        public MediaViewModel()
        {
            this.LoadedCommand = new RelayCommand(OnLoadedExecuted);
        }

        #region CommandDelegates

        private void OnLoadedExecuted(object parameter)
        {
            //this.BannerGrid.ScrollIntoView(this.BannerGrid.SelectedItem);
        }

        #endregion

        public override void Activate(object parameter, Dictionary<string, object> state)
        {
            var seriesModel = AppX.Instance.Current;

            this.State = seriesModel.State;
            this.Banners = seriesModel.FanArtBanners;
            this.selectedItem = this.Banners.FirstOrDefault(B => B.ID == seriesModel.FanArt.ID);
            OnPropertyChanged("SelectedItem");
        }

        public override void Deactivate(Dictionary<string, object> state)
        {
        }
    }
}
