using GUI.Common;
using GUI.Controls;
using GUI.Enums;
using GUI.MVVM.Model;
using GUI.MVVM.View;
using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using System.Linq;
using Windows.UI.Xaml;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using System;
using Windows.Storage.Streams;
using System.IO;
using Windows.Foundation;

namespace GUI.MVVM.ViewModel
{
    public sealed class SeriesFrameViewModel : ViewModelBase
    {
        #region Properties

        public SeriesModel SeriesModel { get; private set; }

        public RelayCommand NavigatedCommand { get; private set; }

        #endregion

        #region Contructor

        public SeriesFrameViewModel(NavigationService innerFrame)
        {
            base.Navigation = innerFrame;

            this.NavigatedCommand = new RelayCommand(OnNavigatedExecuted);
        }

        #endregion

        #region CommandDelegates

        private void OnNavigatedExecuted(object parameter)
        {
            AppX.Instance.NavigationBarIsOpen = false;
        }

        #endregion

        public override void Activate(object parameter, System.Collections.Generic.Dictionary<string, object> state)
        {
            var args = parameter as int[];

            int seriesID = (state != null && state.ContainsKey("SeriesID")) ? (int)state["SeriesID"] : (args != null && args.Length == 2) ? args[0] : (int)parameter;
            int episodeID = (args != null && args.Length == 2) ? args[1] : -1;

            AppX.Instance.Current = this.SeriesModel = AppX.Instance.Series.FirstOrDefault(S => S.ID == seriesID);
            OnPropertyChanged("SeriesModel");
            if (AppX.Instance.Current != null)
            {
                if (state == null)
                {
                    state = new System.Collections.Generic.Dictionary<string, object>();
                }

                state["SeriesID"] = AppX.Instance.Current.ID;
            }

            DataTransferManager.GetForCurrentView().DataRequested += new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);

            if (!base.Navigation.Register("SeriesFrame"))
            {
                if (episodeID != -1)
                {
                    base.Navigation.Navigate(typeof(EpisodeView), episodeID);
                }
                else
                {
                    base.Navigation.Navigate(typeof(SeriesView));
                }
                base.Navigation.BackStack.Clear();
            }
        }
        public override void Deactivate(System.Collections.Generic.Dictionary<string, object> state)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= new TypedEventHandler<DataTransferManager, DataRequestedEventArgs>(this.OnDataRequested);
        }
        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            if (this.SeriesModel == null) return;

            var deferal = args.Request.GetDeferral();

            var request = args.Request;
            request.Data.SetWebLink(new Uri(this.SeriesModel.IMDB));
            request.Data.Properties.Title = this.SeriesModel.Name;
            request.Data.Properties.Description = this.SeriesModel.Overview;
            request.Data.SetText(this.SeriesModel.Overview);

            try
            {
                var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri(this.SeriesModel.FanArt.Image));

                var fanArtRef = RandomAccessStreamReference.CreateFromFile(file);
                request.Data.SetBitmap(fanArtRef);

                var thumb = await file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView, 240, Windows.Storage.FileProperties.ThumbnailOptions.ResizeThumbnail);
                request.Data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromStream(thumb);
            }
            catch (FileNotFoundException) { }
            finally { deferal.Complete(); }
        }
    }
}
