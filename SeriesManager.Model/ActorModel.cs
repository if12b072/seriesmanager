using System.Xml.Linq;

using SeriesManager.Model.Banner;

namespace SeriesManager.Model
{
    public sealed class ActorModel
    {
        #region Properties

        private readonly int seriesID;

        public string Name { get; private set; }
        public string Role { get; private set; }
        public BlankBannerModel Image { get; private set; }
        
        #endregion

        #region Contructor

        public ActorModel(XElement actor, int seriesID)
        {
            this.seriesID = seriesID;

            foreach (XElement elem in actor.Elements())
            {
                if (elem.Name == "Image")
                {
                    this.Image = new BlankBannerModel(seriesID, elem.Value);
                }
                else if (elem.Name == "Name")
                {
                    this.Name = elem.Value;
                }
                else if (elem.Name == "Role")
                {
                    this.Role = elem.Value;
                }
            }
        }

        #endregion
    }
}
