using Microsoft.Xna.Framework;
using EndlessHourglass.Types.Interfaces;

namespace EndlessHourglass.Types.Player.Weapon
{
    public class Staff : IWeapon
    {
        public Vector2 BaseOffset { get; } = new Vector2(7, 8);
        public Vector2 BaseOffsetProjectile { get; } = new Vector2(15, 4);
        public Vector2 LeftProjectileAdjustment { get; } = new Vector2(5, 1);
        public Vector2 ProjectileDirectionOffset { get; } = new Vector2(6, 6);
        public float FireRate { get; } = 0.25f;
        public float TimeSinceFired { get; set; }

        public Staff()
        {
            TimeSinceFired = FireRate;
        }
    }
}
