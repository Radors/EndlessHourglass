using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Player.PlayerWeapon;
using System;

namespace OriginOfLoot.Types.Player
{
    public class Player
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Velocity { get; set; } = new Vector2(0, 0);
        public IPlayerWeapon Weapon { get; set; } = new Sword();
        public bool FacingRight { get; set; } = true;
        public float Speed { get; set; } = 180f;

        public Vector2 CurrentWeaponOffset()
        {
            var vector = (FacingRight, Weapon) switch
            {
                (true, _) => Weapon.Offset,
                (false, Sword) => new Vector2(-Weapon.Offset.X, Weapon.Offset.Y),
                (false, Staff) => new Vector2(-Weapon.Offset.X, Weapon.Offset.Y),
                _ => throw new ArgumentOutOfRangeException()
            };
            return vector;
        }

        public Vector2 CurrentProjectileSpawnOffset()
        {
            var vector = (FacingRight, Weapon) switch
            {
                (_, Sword) => CurrentWeaponOffset(),
                (true, Staff staff) => staff.ProjectileSpawnOffset,
                (false, Staff) => new Vector2(16 - Texture.Width - Weapon.Offset.X, Weapon.Offset.Y),
                _ => throw new ArgumentOutOfRangeException()
            };
            return vector;
        }
    }
}