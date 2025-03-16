using Microsoft.Xna.Framework;

namespace OriginOfLoot.Types
{
    public class ActiveSwordProjectile
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public float TimeAlive { get; set; }
        public int CurrentFrame { get; set; }

        public ActiveSwordProjectile(Vector2 position, Vector2 velocity, float timeAlive, int currentFrame)
        {
            Position = position;
            Velocity = velocity;
            TimeAlive = timeAlive;
            CurrentFrame = currentFrame;
        }
    }
}
