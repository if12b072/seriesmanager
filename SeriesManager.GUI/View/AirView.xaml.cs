using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

using SeriesManager.ViewModel;

namespace SeriesManager.GUI.View
{
    public sealed partial class AirView : NavigationView
    {
        public AirView()
        {
            this.InitializeComponent();
            this.DataContext = new AirViewModel();
        }

        protected override void UpdateVisualState(Size windowSize)
        {
            if (windowSize.Width < 900)
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
