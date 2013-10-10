using Windows.UI.Xaml.Controls;

using GUI.Common;
using GUI.MVVM.ViewModel;
using GUI.MVVM.Model;
using Windows.UI.Xaml;

namespace GUI.MVVM.View
{
    public sealed partial class SeriesFrameView : NavigationView
    {
        public SeriesFrameView()
        {
            this.InitializeComponent();
            this.DataContext = new SeriesFrameViewModel(new NavigationService(this.NavigationFrame));
        }
    }
}
