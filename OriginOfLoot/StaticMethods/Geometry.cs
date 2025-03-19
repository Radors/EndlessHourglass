using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;

namespace OriginOfLoot.StaticMethods
{
    public static class Geometry
    {
        public static Rectangle NewRectangle(Vector2 position, Texture2D texture)
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
        }

        public static List<Rectangle> SetupAnimationRectangles(Texture2D texture, int viewTileWidth)
        {
            var rectangles = new List<Rectangle>();
            var frameCount = texture.Width / viewTileWidth;
            for (int i = 0; i < frameCount; i++)
            {
                rectangles.Add(
                    new Rectangle(new Point(i * viewTileWidth, 0), new Point(viewTileWidth, viewTileWidth))
                );
            }
            return rectangles;
        }
    }
}
