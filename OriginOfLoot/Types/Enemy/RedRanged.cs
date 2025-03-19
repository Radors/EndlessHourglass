using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Enemy
{
    public class RedRanged : IActiveEnemy
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle { get; set; }
        public Vector2 Velocity { get; set; }
        public int MaxHealth { get; set; } = 200;
        public int CurrentHealth { get; set; } = 200;

        public float StartingSpeed { get; set; } = 50f; // Reminder to rethink this later

        public RedRanged(Texture2D texture, Vector2 position, Vector2 velocity, Rectangle rectangle)
        {
            Texture = texture;
            Position = position;
            Velocity = velocity;
            Rectangle = rectangle;
        }
    }
}
