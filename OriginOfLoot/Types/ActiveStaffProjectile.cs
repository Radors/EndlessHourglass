using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Enums
{
    public class ActiveStaffProjectile
    {
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }

        public ActiveStaffProjectile(Vector2 position, Vector2 velocity)
        {
            Position = position;
            Velocity = velocity;
        }
    }
}
