using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.StaticMethods;
using OriginOfLoot.Types.Enemy;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Projectile
{
    public class StaffProjectile : IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }
        public List<IActiveEnemy> EnemiesHit { get; set; } = new();
        public float Speed { get; set; } = 250f;

        public StaffProjectile(Texture2D texture, Vector2 position, Vector2 direction)
        {
            Texture = texture;
            Position = position;

            direction.Normalize();
            Velocity = direction * Speed;

            Rectangle = Geometry.NewRectangle(position, texture);
        }
    }
}
