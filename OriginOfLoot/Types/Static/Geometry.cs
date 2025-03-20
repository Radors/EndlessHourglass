using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;

namespace OriginOfLoot.Types.Static
{
    public static class Geometry
    {
        public static Rectangle NewRectangle(Vector2 position, Texture2D texture)
        {
            return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height);
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
    }
}
