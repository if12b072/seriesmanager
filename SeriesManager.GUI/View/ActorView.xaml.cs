using GUI.MVVM.ViewModel;
using Windows.UI.Xaml;

namespace SeriesManager.GUI.View
{
    public sealed partial class ActorView : NavigationView
    {
        public ActorView()
        {
            this.InitializeComponent();
            this.DataContext = new ActorViewModel();
        }

        protected override void UpdateVisualState(Windows.Foundation.Size windowSize)
        {
            if (windowSize.Width < 768)
            {
                VisualStateManager.GoToState(this, "Landscape500", true);
            }
            else if (windowSize.Width >= 768)
            {
                VisualStateManager.GoToState(this, "Landscape768", true);
            }
        }
    }
}
