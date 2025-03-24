﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Player.PlayerWeapon
{
    public class Rotator : IPlayerWeapon
    {
        public Vector2 BaseOffset { get; set; } = new Vector2(4, 11);
        public Vector2 BaseOffsetProjectile { get; set; } = new Vector2(4, 11);
        public Vector2 LeftProjectileAdjustment { get; set; } = new Vector2(0, 0);
        public Vector2 ProjectileDirectionOffset { get; set; } = new Vector2(8, 8);
        public float FireRate { get; set; } = 0.25f;
        public float TimeSinceFired { get; set; }

        public Rotator()
        {
            TimeSinceFired = FireRate;
        }
    }
}
