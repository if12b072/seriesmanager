using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;

using GUI.Common;
using GUI.MVVM.Model;
using GUI.MVVM.View;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml.Linq;
using GUI.Extensions;

namespace GUI.MVVM.ViewModel
{
    public class SearchViewModel : ViewModelBase
    {
        private bool loading;
        private string searchQuery;

        #region DataProperties

        public ObservableCollection<SeriesModel> SearchResult { get; private set; }
        public bool Loading
        {
            get { return this.loading; }
            private set { SetProperty(ref this.loading, value); }
        }
        public string Name
        {
            get { return new ResourceLoader().GetString("SearchHeader") + this.searchQuery; }
        }

        #endregion

        #region CommandProperties

        public RelayCommand ItemClickedCommand { get; private set; }

        #endregion

        public SearchViewModel()
        {
            this.SearchResult = new ObservableCollection<SeriesModel>();
            this.ItemClickedCommand = new RelayCommand(OnItemClickExecuted);
        }

        #region CommandDelegates

        private async void OnItemClickExecuted(object parameter)
        {
            var clickedItem = (parameter as ItemClickEventArgs).ClickedItem as SeriesModel;

            this.Loading = true;
            await clickedItem.Load();
            this.Loading = false;

            base.Navigation.Navigate(typeof(SeriesFrameView), clickedItem.ID);
        }

        #endregion

        public async Task Search()
        {
            this.SearchResult.Clear();

            if (string.IsNullOrWhiteSpace(this.searchQuery))
                return;

            this.Loading = true;

            string shortLanguage = AppX.Instance.Language.ToShort();
            string content = API.SeriesPath + "GetSeries.php?seriesname=" + this.searchQuery + "&language=" + shortLanguage;

            try
            {
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.GetAsync(content);
                response.EnsureSuccessStatusCode();
                content = await response.Content.ReadAsStringAsync();
            }
            catch { return; }

            var doc = XDocument.Parse(content);
            var data = doc.Element("Data");
            var seriesData = data.Elements("Series");

            foreach (XElement series in seriesData)
            {
                // Check if existing
                if (series.Element("id") != null)
                {
                    int id;
                    if (!int.TryParse(series.Element("id").Value, out id))
                        continue;

                    var result = AppX.Instance.Series.FirstOrDefault(S => S.ID == id);
                    if (result != null)
                    {
                        if (!this.SearchResult.Any(S => S.ID == id))
                        {
                            this.SearchResult.Add(result);
                        }
                        continue;
                    }

                    XElement langElement = series.Element("Language") == null ? series.Element("language") : series.Element("Language");
                    if (langElement == null)
                        continue;

                    if (langElement.Value == shortLanguage)
                    {
                        XDocument tmpDoc = new XDocument(new XElement("Data", series));
                        SeriesModel s = new SeriesModel(tmpDoc);
                        if (s != null)
                        {
                            AppX.Instance.Series.Add(s);
                            this.SearchResult.Add(s);
                            await s.Load();
                        }
                    }
                }
            }

            this.Loading = false;
        }
        public async override void Activate(object parameter, Dictionary<string, object> state)
        {
            this.searchQuery = (state != null && state.ContainsKey("SearchQuery")) ? (string)state["SearchQuery"] : (string)parameter;
            OnPropertyChanged("Name");
            await this.Search();
        }
        public override void Deactivate(Dictionary<string, object> state)
        {
            if (!string.IsNullOrWhiteSpace(this.Name))
            {
                state["SearchQuery"] = this.searchQuery;
            }
        }
    }
}
