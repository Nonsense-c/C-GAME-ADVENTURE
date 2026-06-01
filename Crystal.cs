using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Gamaa
{
    public class Crystal
    {
        public Rectangle Bounds;
        public string Type; 
        public bool IsCollected;

        public Crystal(Rectangle bounds, string type)
        {
            Bounds = bounds;
            Type = type;
            IsCollected = false;
        }

        public void Draw(SpriteBatch spriteBatch, Texture2D fireTexture, Texture2D waterTexture)
        {
            if (!IsCollected)
            {
                Texture2D texture = (Type == "fire") ? fireTexture : waterTexture;
                spriteBatch.Draw(texture, Bounds, Color.White);
            }
        }
    }
}