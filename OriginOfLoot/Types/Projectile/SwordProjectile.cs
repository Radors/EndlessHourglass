using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.StaticMethods;

namespace OriginOfLoot.Types.Projectile
{
    public class SwordProjectile : IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }
        public float Speed { get; set; } = 350f;
        public int CurrentFrame { get; set; } = 0;
        public float Lifetime { get; set; } = 0.40f;
        public float TimeAlive { get; set; } = 0;

        public SwordProjectile(Texture2D texture, Vector2 position, Vector2 direction)
        {
            Texture = texture;
            Position = position;

            direction.Normalize();
            Velocity = direction * Speed;

            Rectangle = Geometry.NewRectangle(position, texture);
        }
    }
}
