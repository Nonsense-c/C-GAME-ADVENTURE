using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gamaa
{
    public class Gate
    {
        public Rectangle Bounds;
        public bool IsActive;

        public Gate(Rectangle bounds)
        {
            Bounds = bounds;
            IsActive = true;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D texture)
        {
            if (IsActive)
            {
                spriteBatch.Draw(texture, Bounds, Color.White);
            }
        }
    }
}