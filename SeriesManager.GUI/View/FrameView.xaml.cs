using GUI.Common;
using GUI.MVVM.ViewModel;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;

namespace GUI.MVVM.View
{
    public sealed partial class FrameView : NavigationView
    {
        public FrameView(LaunchActivatedEventArgs e)
        {
            this.InitializeComponent();
            this.DataContext = new FrameViewModel(new NavigationService(this.NavigationFrame), e, this.Dispatcher);
            this.NavigationBar.SetBinding(CommandBar.IsOpenProperty, new Binding() { Source = AppX.Instance, Path = new PropertyPath("NavigationBarIsOpen"), Mode = BindingMode.TwoWay });
            this.SetBinding(Page.RequestedThemeProperty, new Binding() { Source = AppX.Instance, Path = new PropertyPath("Theme") });
        }

        public Frame Navi
        {
            get { return this.NavigationFrame; }
        }

        protected override void UpdateVisualState(Size windowSize)
        {
            if (windowSize.Width < 920)
            {
                VisualStateManager.GoToState(this, "Width500", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "Width920", true);
            }
        }

        private void Popup_Opened(object sender, object e)
        {
            popUpChild.Width = (Window.Current.Bounds.Width - (Window.Current.Bounds.Width / 3));
            popUp.HorizontalOffset = (Window.Current.Bounds.Width - popUpChild.Width) / 2;
            popUp.VerticalOffset = (Window.Current.Bounds.Height - popUpChild.ActualHeight - 100) / 2;
            searchBox.Focus(FocusState.Programmatic);
        }
    }
}
