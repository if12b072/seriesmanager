using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.Resources;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;

using SeriesManager.Common;
using SeriesManager.GUI.View;

namespace SeriesManager.GUI
{
    sealed partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            SuspensionManager.KnownTypes.Add(typeof(int[]));
        }

        private async void OnSuspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            await SuspensionManager.SaveAsync();
            deferral.Complete();
        }

        protected override async void OnLaunched(LaunchActivatedEventArgs args)
        {
            var frame = Window.Current.Content as FrameView;

            if (args.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
                try { await SuspensionManager.RestoreAsync(); }
                catch (SuspensionManagerException) { }
            }

            if (frame == null)
            {
                frame = new FrameView(args);
                Window.Current.Content = frame;
                SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
            }
            else
            {
                frame.LoadArguments(args.Arguments);
            }

            Window.Current.Activate();
        }

        private void OnCommandsRequested(SettingsPane settingsPane, SettingsPaneCommandsRequestedEventArgs eventArgs)
        {
            SettingsCommand defaultsCommand = new SettingsCommand("settingsID", new ResourceLoader().GetString("Settings"),
                (handler) =>
                {
                    new SettingsView().Show();
                });
            eventArgs.Request.ApplicationCommands.Add(defaultsCommand);
        }
    }
}
