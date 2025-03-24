using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Player.PlayerWeapon
{
    public class Staff : IPlayerWeapon
    {
        public Vector2 BaseOffset { get; set; } = new Vector2(7, 8);
        public Vector2 BaseOffsetProjectile { get; set; } = new Vector2(15, 4);
        public Vector2 LeftProjectileAdjustment { get; set; } = new Vector2(5, 1);
        public Vector2 ProjectileDirectionOffset { get; set; } = new Vector2(6, 6);
        public float FireRate { get; set; } = 0.25f;
        public float TimeSinceFired { get; set; }

        public Staff()
        {
            TimeSinceFired = FireRate;
        }
    }
}
