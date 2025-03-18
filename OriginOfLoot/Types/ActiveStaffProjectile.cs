using Microsoft.Xna.Framework;

namespace OriginOfLoot.Types
{
    public class ActiveStaffProjectile
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }

        public ActiveStaffProjectile(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}
