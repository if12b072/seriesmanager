using GUI.Common;
using GUI.MVVM.ViewModel;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace GUI.MVVM.View
{
    public sealed partial class SubscriptionView : NavigationView
    {
        public SubscriptionView()
        {
            this.InitializeComponent();
            this.DataContext = new SubscriptionViewModel();
            this.CommandBar.SetBinding(CommandBar.IsOpenProperty, new Binding() { Source = AppX.Instance, Path = new PropertyPath("NavigationBarIsOpen"), Mode = BindingMode.TwoWay });
        }

        protected override void UpdateVisualState(Size windowSize)
        {
            if (windowSize.Width < 768)
            {
                VisualStateManager.GoToState(this, "Landscape500", true);
            }
            else if (windowSize.Height == 900)
            {
                VisualStateManager.GoToState(this, "Landscape900", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Landscape768", true);
            }
        }
    }
}
