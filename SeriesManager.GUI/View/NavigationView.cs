using GUI.MVVM.View;
using GUI.MVVM.ViewModel;
using SeriesManager.Common;
using SeriesManager.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Navigation;

namespace SeriesManager.GUI.View
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public class NavigationView : Page
    {
        public RelayCommand BackCommand { get; private set; }
        public RelayCommand ForwardCommand { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="NavigationView"/> class.
        /// </summary>
        /// <param name="page">A reference to the current page used for navigation.  
        /// This reference allows for frame manipulation and to ensure that keyboard 
        /// navigation requests only occur when the page is occupying the entire window.</param>
        public NavigationView()
        {
            // When this page is part of the visual tree make two changes:
            // 1) Map application view state to visual state for the page
            // 2) Handle keyboard and mouse navigation requests
            this.Loaded += (sender, e) =>
            {
                // Keyboard and mouse navigation only apply when occupying the entire window
                if (this.ActualHeight == Window.Current.Bounds.Height &&
                    this.ActualWidth == Window.Current.Bounds.Width)
                {
                    // Listen to the window directly so focus isn't required
                    Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=
                        CoreDispatcher_AcceleratorKeyActivated;
                    Window.Current.CoreWindow.PointerPressed +=
                        this.CoreWindow_PointerPressed;
                }
            };

            // Undo the same changes when the page is no longer visible
            this.Unloaded += (sender, e) =>
            {
                Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated -=
                    CoreDispatcher_AcceleratorKeyActivated;
                Window.Current.CoreWindow.PointerPressed -=
                    this.CoreWindow_PointerPressed;
            };

            Window.Current.SizeChanged += delegate(object sender, WindowSizeChangedEventArgs e) { UpdateVisualState(e.Size); };
            
            this.BackCommand = new RelayCommand(OnBackExecuted, OnBackCanExecute);
            this.ForwardCommand = new RelayCommand(OnForwardExecuted, OnForwardCanExecute);

            this.Loaded += delegate { this.UpdateVisualState(new Size(Window.Current.Bounds.Width, Window.Current.Bounds.Height)); };
        }

        #region NavigationSupport

        public virtual bool OnBackCanExecute(object parameter)
        {
            return (this.Frame != null && this.Frame.CanGoBack) || (Window.Current != null && (Window.Current.Content as FrameView) != null && ((Window.Current.Content as FrameView).Navi.CanGoBack || !((Window.Current.Content as FrameView).Navi.Content is SubscriptionView)));
        }

        public virtual bool OnForwardCanExecute(object parameter)
        {
            return this.Frame != null && this.Frame.CanGoForward;
        }

        public virtual void OnBackExecuted(object parameter)
        {
            if (this.Frame != null && this.Frame.CanGoBack)
            {
                this.Frame.GoBack();
                return;
            }
            else if (Window.Current != null && (Window.Current.Content as FrameView) != null)
            {
                if ((Window.Current.Content as FrameView).Navi.CanGoBack)
                {
                    (Window.Current.Content as FrameView).Navi.GoBack();
                    return;
                }
                else
                {
                    (Window.Current.Content as FrameView).Navi.Navigate(typeof(SubscriptionView));
                }
            }
        }

        public virtual void OnForwardExecuted(object parameter)
        {
            if (this.Frame != null && this.Frame.CanGoForward) this.Frame.GoForward();
        }

        /// <summary>
        /// Invoked on every keystroke, including system keys such as Alt key combinations, when
        /// this page is active and occupies the entire window.  Used to detect keyboard navigation
        /// between pages even when the page itself doesn't have focus.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher sender,
            AcceleratorKeyEventArgs e)
        {
            var virtualKey = e.VirtualKey;

            // Only investigate further when Left, Right, or the dedicated Previous or Next keys
            // are pressed
            if ((e.EventType == CoreAcceleratorKeyEventType.SystemKeyDown ||
                e.EventType == CoreAcceleratorKeyEventType.KeyDown) &&
                (virtualKey == VirtualKey.Left || virtualKey == VirtualKey.Right ||
                (int)virtualKey == 166 || (int)virtualKey == 167))
            {
                var coreWindow = Window.Current.CoreWindow;
                var downState = CoreVirtualKeyStates.Down;
                bool menuKey = (coreWindow.GetKeyState(VirtualKey.Menu) & downState) == downState;
                bool controlKey = (coreWindow.GetKeyState(VirtualKey.Control) & downState) == downState;
                bool shiftKey = (coreWindow.GetKeyState(VirtualKey.Shift) & downState) == downState;
                bool noModifiers = !menuKey && !controlKey && !shiftKey;
                bool onlyAlt = menuKey && !controlKey && !shiftKey;

                if (((int)virtualKey == 166 && noModifiers) ||
                    (virtualKey == VirtualKey.Left && onlyAlt))
                {
                    // When the previous key or Alt+Left are pressed navigate back
                    e.Handled = true;
                    this.BackCommand.Execute(null);
                }
                else if (((int)virtualKey == 167 && noModifiers) ||
                    (virtualKey == VirtualKey.Right && onlyAlt))
                {
                    // When the next key or Alt+Right are pressed navigate forward
                    e.Handled = true;
                    this.ForwardCommand.Execute(null);
                }
            }
        }

        /// <summary>
        /// Invoked on every mouse click, touch screen tap, or equivalent interaction when this
        /// page is active and occupies the entire window.  Used to detect browser-style next and
        /// previous mouse button clicks to navigate between pages.
        /// </summary>
        /// <param name="sender">Instance that triggered the event.</param>
        /// <param name="e">Event data describing the conditions that led to the event.</param>
        private void CoreWindow_PointerPressed(CoreWindow sender,
            PointerEventArgs e)
        {
            var properties = e.CurrentPoint.Properties;

            // Ignore button chords with the left, right, and middle buttons
            if (properties.IsLeftButtonPressed || properties.IsRightButtonPressed ||
                properties.IsMiddleButtonPressed) return;

            // If back or foward are pressed (but not both) navigate appropriately
            bool backPressed = properties.IsXButton1Pressed;
            bool forwardPressed = properties.IsXButton2Pressed;
            if (backPressed ^ forwardPressed)
            {
                e.Handled = true;
                if (backPressed) this.BackCommand.Execute(null);
                if (forwardPressed) this.ForwardCommand.Execute(null);
            }
        }

        #endregion

        #region Process lifetime management

        private String _pageKey;

        private void LoadState(object navigationParameter, Dictionary<string, object> pageState)
        {
            var navigableViewModel = this.DataContext as ViewModelBase;
            if (navigableViewModel != null)
            {
                if (navigableViewModel.Navigation == null)
                {
                    navigableViewModel.Navigation = new NavigationService(this.Frame);
                }
                navigableViewModel.Activate(navigationParameter, pageState);
            }
        }

        private void SaveState(Dictionary<string, object> pageState)
        {
            var navigableViewModel = this.DataContext as ViewModelBase;
            if (navigableViewModel != null)
                navigableViewModel.Deactivate(pageState);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            this._pageKey = "Page-" + this.Frame.BackStackDepth;

            if (e.NavigationMode == NavigationMode.New)
            {
                // Clear existing state for forward navigation when adding a new page to the
                // navigation stack
                var nextPageKey = this._pageKey;
                int nextPageIndex = this.Frame.BackStackDepth;
                while (frameState.Remove(nextPageKey))
                {
                    nextPageIndex++;
                    nextPageKey = "Page-" + nextPageIndex;
                }

                // Pass the navigation parameter to the new page
                this.LoadState(e.Parameter, null);
            }
            else
            {
                // Pass the navigation parameter and preserved page state to the page, using
                // the same strategy for loading suspended state and recreating pages discarded
                // from cache
                this.LoadState(e.Parameter, (Dictionary<String, Object>)frameState[this._pageKey]);
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            var frameState = SuspensionManager.SessionStateForFrame(this.Frame);
            var pageState = new Dictionary<String, Object>();
            this.SaveState(pageState);
            frameState[_pageKey] = pageState;
        }

        #endregion
        
        protected virtual void UpdateVisualState(Size windowSize)
        {
        }
    }
}
