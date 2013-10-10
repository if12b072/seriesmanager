using GUI.Common;
using GUI.MVVM.ViewModel;
using Windows.UI.Xaml;

namespace GUI.MVVM.View
{
    public sealed partial class MediaView : NavigationView
    {
        public MediaView()
        {
            this.InitializeComponent();
            this.DataContext = new MediaViewModel();
        }

        protected override void UpdateVisualState(Windows.Foundation.Size windowSize)
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
