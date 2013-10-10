using System.Collections.Generic;

using SeriesManager.Model;
using SeriesManager.Enum;

namespace SeriesManager.ViewModel
{
    public sealed class ActorViewModel : ViewModelBase
    {
        #region Properties

        public IEnumerable<ActorModel> Actors 
        { 
            get; 
            private set; 
        }

        public WatchState State 
        { 
            get; 
            private set; 
        }

        #endregion

        public override void Activate(object parameter, Dictionary<string, object> state)
        {
            var series = AppX.Instance.Current;
            if (series != null)
            {
                this.Actors = series.Actors;
                OnPropertyChanged("Actors");
                this.State = series.State;
                OnPropertyChanged("State");
            }
        }
    }
}
