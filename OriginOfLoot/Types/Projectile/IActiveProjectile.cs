using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Projectile
{
    public interface IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }  // ----
    }

    public class SwordProjectile : IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }
        public float TimeAlive { get; set; }
        public int CurrentFrame { get; set; }

        public SwordProjectile(Texture2D texture, Vector2 position, Vector2 velocity, Rectangle rectangle,
                                     float timeAlive, int currentFrame)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Rectangle = rectangle;
            TimeAlive = timeAlive;
            CurrentFrame = currentFrame;
        }
    }

    public class StaffProjectile : IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }

        public StaffProjectile(Texture2D texture, Vector2 position, Vector2 velocity, Rectangle rectangle)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Rectangle = rectangle;
        }
    }
}
