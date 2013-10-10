using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace SeriesManager.Model
{
    public sealed class AirModel : IGrouping<string, EpisodeModel>
    {
        private readonly IEnumerable<EpisodeModel> episodes;

        public string Key { get; set; }

        public AirModel(string key, IEnumerable<EpisodeModel> episodes)
        {
            this.Key = key;
            this.episodes = episodes;
        }

        public IEnumerator<EpisodeModel> GetEnumerator()
        {
            return this.episodes.GetEnumerator();
        }

        IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
