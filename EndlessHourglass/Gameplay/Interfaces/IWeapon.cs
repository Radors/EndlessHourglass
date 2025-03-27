using Microsoft.Xna.Framework;

namespace EndlessHourglass.Gameplay.Interfaces
{
    public interface IWeapon
    {
        Vector2 BaseOffset { get; }
        Vector2 BaseOffsetProjectile { get; }
        Vector2 LeftProjectileAdjustment { get; }
        Vector2 ProjectileDirectionOffset { get; }
        float FireRate { get; }
        float TimeSinceFired { get; set; }
    }
}
