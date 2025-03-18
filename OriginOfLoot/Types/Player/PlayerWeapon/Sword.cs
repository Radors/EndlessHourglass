using Microsoft.Xna.Framework;

namespace OriginOfLoot.Types.Player.PlayerWeapon
{
    public class Sword : IPlayerWeapon
    {
        public Vector2 Offset { get; set; } = new Vector2(12, 19);
        public float FireRate { get; set; } = 0.30f;
        public float TimeSinceFired { get; set; } = 0f;
    }
}
