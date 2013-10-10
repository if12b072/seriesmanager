using System.Windows.Input;
using Windows.UI.Xaml.Controls;
using System.Linq;

using GUI.Common;
using GUI.Controls;
using GUI.MVVM.Model;
using GUI.MVVM.View;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.UI.ApplicationSettings;
using Windows.ApplicationModel.Resources;
using GUI.Enums;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.System;
using Windows.ApplicationModel.Activation;
using System.Threading.Tasks;
using Windows.Storage;
using System;
using System.Xml.Linq;
using Windows.UI.Core;

namespace GUI.MVVM.ViewModel
{
    public sealed class FrameViewModel : ViewModelBase
    {
        private bool searchBarIsOpen;
        private readonly LaunchActivatedEventArgs launchArgs;
        private readonly CoreDispatcher dispatcher;
        private bool loading = true;

        #region DataProperties

        public AppX Model { get; private set; }
        public bool SearchBarIsOpen
        {
            get { return this.searchBarIsOpen; }
            set { SetProperty(ref this.searchBarIsOpen, value); }
        }
        public bool Loading
        {
            get { return this.loading; }
            private set { SetProperty(ref this.loading, value); }
        }

        #endregion

        #region CommandProperties

        public RelayCommand HomeCommand { get; private set; }
        public RelayCommand SearchCommand { get; private set; }
        public RelayCommand SearchBarCommand { get; private set; }
        public RelayCommand AirDayCommand { get; private set; }
        public RelayCommand AirWeekCommand { get; private set; }
        public RelayCommand AirMonthCommand { get; private set; }
        public RelayCommand AirYearCommand { get; private set; }
        public RelayCommand SettingsPaneCommand { get; private set; }
        public RelayCommand NavigatedCommand { get; private set; }

        #endregion

        #region Contructor

        public FrameViewModel(NavigationService navigation, LaunchActivatedEventArgs e, CoreDispatcher dispatcher)
        {
            base.Navigation = navigation;
            this.Model = AppX.Instance;
            this.Model.PropertyChanged += Model_PropertyChanged;
            this.launchArgs = e;
            this.dispatcher = dispatcher;
            e.SplashScreen.Dismissed += Splash_Dismissed;

            this.HomeCommand = new RelayCommand(OnHomeExecuted);
            this.SearchCommand = new RelayCommand(OnSearchExecuted);
            this.SearchBarCommand = new RelayCommand(OnSearchBarExecuted);
            this.AirDayCommand = new RelayCommand(OnAirDayExecuted, OnAirDayCanExecute);
            this.AirWeekCommand = new RelayCommand(OnAirWeekExecuted, OnAirWeekCanExecute);
            this.AirMonthCommand = new RelayCommand(OnAirMonthExecuted, OnAirMonthCanExecute);
            this.AirYearCommand = new RelayCommand(OnAirYearExecuted, OnAirYearCanExecute);
            this.SettingsPaneCommand = new RelayCommand(OnSettingsPaneExecuted);
            this.NavigatedCommand = new RelayCommand(OnNavigatedExecuted);
        }

        #endregion

        #region DelegateCommands

        private void OnNavigatedExecuted(object parameter)
        {
            AppX.Instance.NavigationBarIsOpen = false;
        }
        private void OnSearchBarExecuted(object parameter)
        {
            this.SearchBarIsOpen = true;
        }
        private void OnHomeExecuted(object parameter)
        {
            if (base.Navigation.ContentType != typeof(SubscriptionView))
            {
                base.Navigation.Navigate(typeof(SubscriptionView));
            }
        }
        private void OnSettingsPaneExecuted(object parameter)
        {
            new SettingsView().ShowIndependent();
        }
        private void OnSearchExecuted(object parameter)
        {
            var args = parameter as SearchBoxQuerySubmittedEventArgs;
            base.Navigation.Navigate(typeof(SearchView), args.QueryText);
            this.SearchBarIsOpen = false;
        }
        private void OnAirDayExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(AirView), -1);
        }
        private bool OnAirDayCanExecute(object parameter)
        {
            return this.Model.AirDay.Any();
        }
        private void OnAirWeekExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(AirView), -2);
        }
        private bool OnAirWeekCanExecute(object parameter)
        {
            return this.Model.AirWeek.Any();
        }
        private void OnAirMonthExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(AirView), -3);
        }
        private bool OnAirMonthCanExecute(object parameter)
        {
            return this.Model.AirMonth.Any();
        }
        private void OnAirYearExecuted(object parameter)
        {
            base.Navigation.Navigate(typeof(AirView), -4);
        }
        private bool OnAirYearCanExecute(object parameter)
        {
            return this.Model.AirYear.Any();
        }

        #endregion

        private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "AirDay":
                    this.AirDayCommand.Invalidate();
                    break;
                case "AirWeek":
                    this.AirWeekCommand.Invalidate();
                    break;
                case "AirMonth":
                    this.AirMonthCommand.Invalidate();
                    break;
                case "AirYear":
                    this.AirYearCommand.Invalidate();
                    break;

            }
        }
        private async void Splash_Dismissed(SplashScreen sender, object args)
        {
            await dispatcher.RunAsync(CoreDispatcherPriority.High, async () =>
            {
                this.Loading = true;
                await Load();
                this.Loading = false;

                bool register = base.Navigation.Register("MainFrame");
                if (!LoadArguments(this.launchArgs.Arguments) && !register)
                {
                    base.Navigation.Navigate(typeof(SubscriptionView));
                }
            });            
        }
        private async Task Load()
        {
            var seriesCol = AppX.Instance.Series;

            if (seriesCol.Any()) return;
            await this.Clear();

            var folders = await ApplicationData.Current.LocalFolder.GetFoldersAsync();
            var language = AppX.Instance.Language;
            foreach (var folder in folders)
            {

                XDocument seriesContent = await API.Parse(folder, language);
                if (seriesContent == null)
                    continue;

                SeriesModel series = new SeriesModel(seriesContent);
                seriesCol.Add(series);
            }

            var subscriptions = ApplicationData.Current.RoamingSettings.Containers;
            foreach (var sub in subscriptions)
            {
                if (!seriesCol.Any(S => S.ID.ToString() == sub.Key))
                {
                    var series = new SeriesModel(Int32.Parse(sub.Key), language);
                    await series.Load();
                    seriesCol.Add(series);
                }
            }
        }
        private async Task Clear()
        {
            foreach (var folder in await ApplicationData.Current.LocalFolder.GetFoldersAsync())
            {
                int seriesID;
                if (!Int32.TryParse(folder.Name, out seriesID))
                    continue;

                if (!Roaming.Exists(seriesID))
                {
                    await Local.Delete(seriesID);
                    if (ApplicationData.Current.LocalSettings.Values.ContainsKey(seriesID.ToString()))
                    {
                        ApplicationData.Current.LocalSettings.Values.Remove(seriesID.ToString());
                    }
                }
            }
        }
        public bool LoadArguments(string args)
        {
            if (!string.IsNullOrEmpty(args))
            {
                foreach (var series in AppX.Instance.Series)
                {
                    if (series.ID.ToString() == args)
                    {
                        base.Navigation.Navigate(typeof(SeriesFrameView), series.ID);
                        return true;
                    }
                }
            }

            return false;
        }
    }
}