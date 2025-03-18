using Microsoft.Xna.Framework;

namespace OriginOfLoot.Types.Player.PlayerWeapon
{
    public class Staff : IPlayerWeapon
    {
        public Vector2 Offset { get; set; } = new Vector2(7, 8);
        public Vector2 ProjectileSpawnOffset { get; set; } = new Vector2(19, 8);
        public float FireRate { get; set; } = 0.25f;
        public float TimeSinceFired { get; set; } = 0f;
    }
}
