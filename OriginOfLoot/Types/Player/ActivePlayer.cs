using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using MonoGame.Extended.ViewportAdapters;
using OriginOfLoot.Types.Enemy;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Static;
using System;

namespace OriginOfLoot.Types.Player
{
    public class ActivePlayer
    {
        public Vector2 InputDirection { get; set; } = new Vector2(0, 0);
        public Vector2 Position { get; set; } = new Vector2(0, 0);
        public Rectangle Rectangle { get; set; } = new Rectangle(0, 0, 0, 0);
        public Vector2 Velocity { get; set; } = new Vector2(0, 0);
        public IPlayerWeapon Weapon { get; set; } = new Rotator();
        public bool FacingRight { get; set; } = true;
        public float Speed { get; set; } = 180f;
        public int MaxHealth { get; set; } = 140;
        public int CurrentHealth { get; set; } = 140;
        public Vector2 HealthBarOffset { get; set; } = new Vector2(0, 32);
        public float TotalInvincibilityAfterHit = 0.30f;
        public float TimeSinceHit = 0f;

        public void Update(float deltaTime)
        {
            // Position
            Velocity = InputDirection * Speed;
            Position += Velocity * deltaTime;

            // Rectangle
            Rectangle = Geometry.NewRectangle(Position, TextureStore.Player);

            // Time
            TimeSinceHit += deltaTime;

            // Boundary
            int Xmax = ConstConfig.ViewPixelsX - TextureStore.Player.Width;
            int Ymax = ConstConfig.ViewPixelsY - TextureStore.Player.Height;
            if (Position.X > Xmax)
            {
                Position = new Vector2(Xmax, Position.Y);
            }
            else if (Position.X < 0)
            {
                Position = new Vector2(0, Position.Y);
            }
            if (Position.Y > Ymax)
            {
                Position = new Vector2(Position.X, Ymax);
            }
            else if (Position.Y < 0)
            {
                Position = new Vector2(Position.X, 0);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: TextureStore.Player,
                position: Position,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: FacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision)
            );
            spriteBatch.Draw(
                texture: Weapon switch
                {
                    Rotator => TextureStore.Rotator,
                    Staff => TextureStore.Staff,
                    _ => throw new ArgumentOutOfRangeException()
                },
                position: Position + WeaponOffset(),
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision) + 0.000001f
            );
            spriteBatch.Draw(
                texture: TextureStore.HealthBarGreen,
                position: Position + HealthBarOffset,
                sourceRectangle: TextureStore.HealthBarGreenRectangles[Math.Clamp(HealthbarFrame(), 0, TextureStore.HealthBarRedRectangles.Count-1)],
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / 100000)
            );
        }

        public void TakeDamage(int damage)
        {
            if (TimeSinceHit > TotalInvincibilityAfterHit)
            {
                CurrentHealth -= damage;
                TimeSinceHit = 0;
            }
        }
        public int HealthbarFrame()
        {
            int frame = (int)(CurrentHealth / 10f);
            return 14 - frame;
        }
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