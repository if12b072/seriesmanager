using SeriesManager.Enum;

namespace GUI.Extension
{
    public static class WatchStateExtensions
    {
        public static WatchState Invert(this WatchState state)
        {
            switch (state)
            {
                case WatchState.Watched:
                case WatchState.UnAired:
                    return WatchState.Unwatched;
                case WatchState.Unwatched:
                    return WatchState.Watched;
            }

            return WatchState.UnAired;
        }
    }
}
