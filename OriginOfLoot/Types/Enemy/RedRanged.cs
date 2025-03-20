using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OriginOfLoot.Types.Enemy
{
    public class RedRanged : IActiveEnemy
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle { get; set; }
        public Vector2 Velocity { get; set; }
        public float Speed { get; set; } = 50f;
        public int MaxHealth { get; set; } = 140;
        public int CurrentHealth { get; set; } = 140;
        public Vector2 HealthbarOffset { get; set; } = new Vector2(0, 32);

        public RedRanged(Texture2D texture, Vector2 position, Vector2 direction)
        {
            Texture = texture;
            Position = position;

            direction.Normalize();
            Velocity = direction * Speed;

            Rectangle = Geometry.NewRectangle(position, texture);
        }

        public int HealthbarFrame()
        {
            int frame = (int)(CurrentHealth / 10f);
            return 14 - frame;
        }
    }
}
