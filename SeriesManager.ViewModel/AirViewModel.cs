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
using GUI.MVVM.Model;
using GUI.MVVM.View;
using GUI.Enums;
using Windows.ApplicationModel.Resources;
using GUI.Controls;

namespace GUI.MVVM.ViewModel
{
    public class AirViewModel : ViewModelBase
    {
        private int state;

        #region DataProperties

        public CollectionViewSource ViewSource { get; private set; }
        
        #endregion

        #region CommandProperties

        public RelayCommand ItemClickedCommand { get; private set; }

        #endregion

        public AirViewModel()
        {
            this.ViewSource = new CollectionViewSource() { IsSourceGrouped = true };
            this.ItemClickedCommand = new RelayCommand(OnItemClickExecuted);
        }

        #region CommandDelegates

        private void OnItemClickExecuted(object parameter)
        {
            var clickedItem = (parameter as ItemClickEventArgs).ClickedItem as EpisodeModel;
            if (clickedItem == null) return;

            if (this.state == 0)
            {
                base.Navigation.Navigate(typeof(EpisodeView), clickedItem.ID);
            }
            else
            {
                var arr = new int[] { clickedItem.Season.Series.ID, clickedItem.ID };
                base.Navigation.Navigate(typeof(SeriesFrameView), arr);
            }
            
        }

        #endregion

        public override void Activate(object parameter, Dictionary<string, object> state)
        {
            var res = new ResourceLoader();
            this.state = (state != null && state.ContainsKey("WatchState")) ? (int)state["WatchState"] : (int)parameter;

            switch (this.state)
            {
                case -4:
                    this.ViewSource.Source = AppX.Instance.AirYear.GroupBy(E => (MonthOfYear)E.FirstAired.Date.Month);
                    break;
                case -3:
                    this.ViewSource.Source = AppX.Instance.AirMonth.GroupBy(E => (WeekOfMonth)E.FirstAired.Date.GetWeekOfMonth());
                    break;
                case -2:
                    this.ViewSource.Source = AppX.Instance.AirWeek.GroupBy(E => E.FirstAired.Date.DayOfWeek);
                    break;
                case -1:
                    this.ViewSource.Source = new List<AirModel>(new AirModel[] { new AirModel(res.GetString("AirToday"), AppX.Instance.AirDay) });
                    break;
                case 0:
                    var airCol = new List<AirModel>(new AirModel[] {
                        new AirModel(res.GetString("AirToday"), AppX.Instance.Current.AirToday),
                        new AirModel(res.GetString("AirWeek"), AppX.Instance.Current.AirWeek),
                        new AirModel(res.GetString("AirMonth"), AppX.Instance.Current.AirMonth),
                        new AirModel(res.GetString("AirYear"), AppX.Instance.Current.AirYear),
                        new AirModel(res.GetString("AirAll"), AppX.Instance.Current.AirOther)
                    });
                    this.ViewSource.Source = airCol;
                    break;
            }

            OnPropertyChanged("ViewSource");
        }
    }
}
