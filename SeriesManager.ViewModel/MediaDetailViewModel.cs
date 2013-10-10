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
using Windows.Storage.Pickers;
using System.IO;

namespace GUI.MVVM.ViewModel
{
    public class MediaDetailViewModel : ViewModelBase
    {
        private FanArtBannerModel selectedItem;
        private SeriesModel seriesModel;
        private bool loading;

        #region DataProperties

        public IEnumerable<FanArtBannerModel> Banners
        {
            get
            {
                return this.seriesModel != null ? this.seriesModel.FanArtBanners : null;
            }
        }
        public FanArtBannerModel SelectedItem
        {
            get { return this.selectedItem; }
            set 
            {
                if (this.selectedItem != null)
                {
                    this.selectedItem.PropertyChanged -= SelectedItem_PropertyChanged;
                }

                this.Loading = (value != null) ? !value.Loaded : false;
                SetProperty(ref this.selectedItem, value);
                this.OKCommand.Invalidate();
                this.LockScreenCommand.Invalidate();
                this.SavePictureCommand.Invalidate();

                if (value != null)
                {
                    value.PropertyChanged += SelectedItem_PropertyChanged;
                }
            }
        }
        public bool Loading
        {
            get { return this.loading; }
            private set { SetProperty(ref this.loading, value); }
        }

        #endregion

        #region CommandProperties

        public RelayCommand LockScreenCommand { get; private set; }
        public RelayCommand OKCommand { get; private set; }
        public RelayCommand SavePictureCommand { get; private set; }

        #endregion

        public MediaDetailViewModel()
        {
            this.LockScreenCommand = new RelayCommand(OnLockscreenExecuted, OnLockscreenCanExecute);
            this.OKCommand = new RelayCommand(OnOKExecuted, OnOKCanExecute);
            this.SavePictureCommand = new RelayCommand(OnSavePictureExecuted, OnSavePictureCanExecute);
        }

        #region DelegateCommands

        private async void OnSavePictureExecuted(object parameter)
        {
            StorageFile source;
            try
            {
                source = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.SelectedItem.Image));
            }
            catch (FileNotFoundException) { return; }

            FileSavePicker savePicker = new FileSavePicker();
            savePicker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            savePicker.FileTypeChoices.Add("JPG/JPEG", new List<string>() { ".jpg" });
            savePicker.SuggestedFileName = this.seriesModel.Name;
            StorageFile file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                await source.CopyAndReplaceAsync(file);
            }
        }
        private bool OnSavePictureCanExecute(object parameter)
        {
            return this.SelectedItem != null && this.SelectedItem.Loaded;
        }
        private async void OnLockscreenExecuted(object parameter)
        {
            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.SelectedItem.Image));
                await LockScreen.SetImageFileAsync(file);

                var res = new ResourceLoader();
                await new MessageDialog(res.GetString("LockscreenMessage"), res.GetString("LockscreenTitle")).ShowAsync();
            }
            catch { }
        }
        private bool OnLockscreenCanExecute(object parameter)
        {
            return this.SelectedItem != null && this.SelectedItem.Loaded;
        }
        private void OnOKExecuted(object parameter)
        {
            this.seriesModel.FanArt = (FanArtBannerModel)this.SelectedItem.Clone();
            base.Navigation.GoBack();
        }
        private bool OnOKCanExecute(object parameter)
        {
            return this.SelectedItem != null && this.seriesModel.Subscription && this.SelectedItem.Loaded;
        }

        #endregion

        private void SelectedItem_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("Loaded"))
            {
                LockScreenCommand.Invalidate();
                OKCommand.Invalidate();
                SavePictureCommand.Invalidate();
                this.Loading = false;
            }
        }
        public override void Activate(object parameter, Dictionary<string, object> state)
        {
            this.seriesModel = AppX.Instance.Current;
            OnPropertyChanged("Banners");
            
            int bannerID = (state != null && state.ContainsKey("SelectedItem")) ? (int)state["SelectedItem"] : (int)parameter;
            this.SelectedItem = this.Banners.FirstOrDefault(B => B.ID == bannerID);
        }
        public override void Deactivate(Dictionary<string, object> state)
        {
            if (this.SelectedItem != null)
            {
                state["SelectedItem"] = this.SelectedItem.ID;
            }
        }
    }
}
