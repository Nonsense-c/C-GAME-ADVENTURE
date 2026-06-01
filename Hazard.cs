using Microsoft.Xna.Framework;

namespace Gamaa
{
    public class Hazard
    {
        public Rectangle Area;
        public string Type;

        public Hazard(Rectangle area, string type)
        {
            Area = area;
            Type = type;
        }
    }
}
