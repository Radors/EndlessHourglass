using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Methods
{
    public static class Geometry
    {
        public static Rectangle NewRectangle(Vector2 position, Texture2D texture)
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }
    }
}
