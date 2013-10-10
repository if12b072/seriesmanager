using System;

using Windows.UI.Xaml.Controls;

using SeriesManager.Enum;

namespace SeriesManager.GUI.View
{
    public sealed partial class SettingsView : SettingsFlyout
    {
        public SettingsView()
        {
            this.InitializeComponent();
            LanguageList.ItemsSource = System.Enum.GetValues(typeof(Languages));
            this.DataContext = AppX.Instance;
        }
    }
}
