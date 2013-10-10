using GUI.Common;
using GUI.MVVM.ViewModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace GUI.MVVM.View
{
    public sealed partial class SearchView : NavigationView
    {
        public SearchView()
        {
            this.InitializeComponent();
            this.DataContext = new SearchViewModel();
        }

        protected override void UpdateVisualState(Windows.Foundation.Size windowSize)
        {
            if (windowSize.Width < 768)
            {
                VisualStateManager.GoToState(this, "Landscape500", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Landscape768", true);
            }
        }
    }
}
