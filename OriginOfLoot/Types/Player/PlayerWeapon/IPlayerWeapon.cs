using Microsoft.Xna.Framework;

namespace OriginOfLoot.Types.Player.PlayerWeapon
{
    public interface IPlayerWeapon
    {
        public Vector2 Offset { get; set; }
        public float FireRate { get; set; }
        public float TimeSinceFired { get; set; }
    }
}
