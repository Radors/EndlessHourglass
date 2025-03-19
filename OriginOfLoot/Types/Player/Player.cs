using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Player.PlayerWeapon;
using System;

namespace OriginOfLoot.Types.Player
{
    public class Player
    {
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Vector2 Velocity { get; set; } = new Vector2(0, 0);
        public IPlayerWeapon Weapon { get; set; } = new Rotator();
        public bool FacingRight { get; set; } = true;
        public float Speed { get; set; } = 180f;
        public int MaxHealth { get; set; } = 140;
        public int CurrentHealth { get; set; } = 140;

        public Vector2 WeaponOffset()
        {
            return FacingRight ?
                   Weapon.BaseOffset :
                   new Vector2(-Weapon.BaseOffset.X, Weapon.BaseOffset.Y);
        }
        public Vector2 ProjectileOffset()
        {
            return FacingRight ?
                   Weapon.BaseOffsetProjectile :
                   new Vector2(-Weapon.BaseOffsetProjectile.X + Weapon.LeftProjectileAdjustment.X, 
                                Weapon.BaseOffsetProjectile.Y + Weapon.LeftProjectileAdjustment.Y);
        }
        public Vector2 ProjectileDirectionOffset()
        {
            return Weapon.ProjectileDirectionOffset;
        }
    }
}