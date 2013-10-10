using System;
using System.Collections.Generic;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SeriesManager.Common
{
    public sealed class NavigationService
    {
        private Frame frame;

        public NavigationService()
        {
            this.frame = Window.Current.Content as Frame;
        }

        public NavigationService(Frame frame)
        {
            this.frame = frame;
        }

        public bool Navigate(Type sourcePageType)
        {
            return this.frame.Navigate(sourcePageType);
        }

        public bool Navigate(Type sourcePageType, object parameter)
        {
            return this.frame.Navigate(sourcePageType, parameter);
        }

        public bool GoBack()
        {
            if (this.frame.CanGoBack)
            {
                this.frame.GoBack();
                return true;
            }
            return false;
        }

        public bool Register(string key)
        {
            return SuspensionManager.RegisterFrame(this.frame, key);
        }

        public int BackStackDepth
        {
            get { return this.frame.BackStackDepth; }
        }

        public IList<PageStackEntry> BackStack
        {
            get { return this.frame.BackStack; }
        }

        public bool IsEmpty
        {
            get { return this.frame.Content == null; }
        }

        public Type ContentType
        {
            get
            {
                return this.frame.Content.GetType();
            }
        }
    }
}
