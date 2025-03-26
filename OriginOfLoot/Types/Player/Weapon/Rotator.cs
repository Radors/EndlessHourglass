using Microsoft.Xna.Framework;
using EndlessHourglass.Types.Interfaces;

namespace EndlessHourglass.Types.Player.Weapon
{
    public class Rotator : IWeapon
    {
        public Vector2 BaseOffset { get; } = new Vector2(5, 9);
        public Vector2 BaseOffsetProjectile { get; } = new Vector2(5, 9);
        public Vector2 LeftProjectileAdjustment { get; } = new Vector2(0, 0);
        public Vector2 ProjectileDirectionOffset { get; } = new Vector2(8, 8);
        public float FireRate { get; } = 0.25f;
        public float TimeSinceFired { get; set; }

        public Rotator()
        {
            TimeSinceFired = FireRate;
        }
    }
}
