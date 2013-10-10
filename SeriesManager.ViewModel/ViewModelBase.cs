using System.Collections.Generic;

using SeriesManager.Common;

namespace SeriesManager.ViewModel
{
    public abstract class ViewModelBase : BindableBase
    {
        #region Properties

        private NavigationService navigation;

        public NavigationService Navigation
        {
            get 
            { 
                return this.navigation; 
            }
            set 
            { 
                if (this.navigation == null) 
                {
                    this.navigation = value;
                }
            }
        }

        public RelayCommand CommandBarOpenedCommand
        {
            get;
            private set;
        }

        #endregion

        #region Contructor

        public ViewModelBase() 
        {
            this.CommandBarOpenedCommand = new RelayCommand(OnCommandBarOpenedExecuted);
        }

        public ViewModelBase(NavigationService navigation) : this()
        {
            this.Navigation = navigation;
        }

        #endregion

        private void OnCommandBarOpenedExecuted(object parameter)
        {
            AppX.Instance.NavigationBarIsOpen = true;
        }

        public virtual void Activate(object parameter, Dictionary<string, object> state) { }
        public virtual void Deactivate(Dictionary<string, object> state) { }
    }
}
