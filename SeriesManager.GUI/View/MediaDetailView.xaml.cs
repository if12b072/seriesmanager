using GUI.Common;
using GUI.MVVM.ViewModel;

namespace GUI.MVVM.View
{
    public sealed partial class MediaDetailView : NavigationView
    {
        public MediaDetailView()
        {
            this.InitializeComponent();
            this.DataContext = new MediaDetailViewModel();
        }
    }
}
