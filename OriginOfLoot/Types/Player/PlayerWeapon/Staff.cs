using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Player.PlayerWeapon
{
    public class Staff : IPlayerWeapon
    {
        public Vector2 BaseOffset { get; set; } = new Vector2(7, 8);
        public Vector2 BaseOffsetProjectile { get; set; } = new Vector2(17, 8);
        public Vector2 LeftProjectileAdjustment { get; set; } = new Vector2(9, 0);
        public Vector2 ProjectileDirectionOffset { get; set; } = new Vector2(3, 3);
        public float FireRate { get; set; } = 0.25f;
        public float TimeSinceFired { get; set; } = 0f;
        public int Damage { get; set; } = 30;
    }
}
