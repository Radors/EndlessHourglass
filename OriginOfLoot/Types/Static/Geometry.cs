using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace OriginOfLoot.Types.Static
{
    public static class Geometry
    {
        public static Rectangle NewRectangle(Vector2 position, Texture2D texture)
        {
            return new Rectangle((int)MathF.Round(position.X), (int)MathF.Round(position.Y), texture.Width, texture.Height);
        }

        public static bool RectangularCollision(Rectangle rectangleA, Rectangle rectangleB)
        {
            bool aRight = rectangleB.Left <= rectangleA.Right;
            bool aLeft = rectangleB.Right >= rectangleA.Left;
            bool aTop = rectangleB.Bottom >= rectangleA.Top;
            bool aBottom = rectangleB.Top <= rectangleA.Bottom;

            return aRight && aLeft && aTop && aBottom;
        }

        public static bool CircularCollision(Rectangle rectangleA, float radius, Rectangle rectangleB)
        {
            float deltaX = rectangleB.X + rectangleB.Width / 2f -
                           (rectangleA.X + rectangleA.Width / 2f);
            float deltaY = rectangleB.Y + rectangleB.Height / 2f -
                           (rectangleA.Y + rectangleA.Height / 2f);

            float squaredDistance = deltaX * deltaX + deltaY * deltaY;

            return squaredDistance <= radius * radius;
        }

        public static Vector2 Direction(Vector2 start, Vector2 end)
        {
            var direction = new Vector2(end.X - start.X, end.Y - start.Y);
            direction.Normalize();
            return direction;
        }
    }
}
