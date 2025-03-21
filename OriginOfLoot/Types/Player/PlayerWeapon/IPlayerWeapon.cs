using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Player.PlayerWeapon
{
    public interface IPlayerWeapon
    {
        public Vector2 BaseOffset { get; set; }
        public Vector2 BaseOffsetProjectile { get; set; }
        public Vector2 LeftProjectileAdjustment { get; set; }
        public Vector2 ProjectileDirectionOffset { get; set; }
        public float FireRate { get; set; }
        public float TimeSinceFired { get; set; }
    }
}
