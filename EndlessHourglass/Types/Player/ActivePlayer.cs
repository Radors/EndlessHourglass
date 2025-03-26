using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EndlessHourglass.Types.Interfaces;
using EndlessHourglass.Types.Player.Weapon;
using EndlessHourglass.Types.Static;
using System;

namespace EndlessHourglass.Types.Player
{
    public class ActivePlayer : IEntity, ICollidable, IDamageReceiver
    {
        private Vector2 _healthBarOffset = new Vector2(0, 32);
        private float _speed = 180f;
        private Vector2 _velocity = new();
        private EndlessHourglass _game;

        public Vector2 InputDirection { get; set; } = new();
        public Vector2 Position { get; private set; } = new Vector2(250, 100);
        public Rectangle Rectangle { get; private set; } = new();
        public IWeapon Weapon { get; set; } = new Rotator();
        public bool FacingRight { get; set; } = true;
        public int MaxHealth { get; } = 140;
        public int CurrentHealth { get; set; }
        public float TotalInvincibilityAfterHit { get; } = 0.30f;
        public float TimeSinceHit { get; private set; } = 0f;

        public ActivePlayer(EndlessHourglass game)
        {
            _game = game;
            CurrentHealth = MaxHealth;
        }

        public void TakeDamage(int damage)
        {
            if (TimeSinceHit > TotalInvincibilityAfterHit)
            {
                CurrentHealth -= damage;
                TimeSinceHit = 0;
            }
        }

        public int HealthBarIndex()
        {
            int frame = (int)(CurrentHealth / (MaxHealth / 14f));
            return ConstConfig.StandardHealthBarTotalFrames - frame - 1;
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

        public void Update(float deltaTime)
        {
            if (CurrentHealth <= 0)
            {
                _game.GameOver();
            }

            // Position
            _velocity = InputDirection * _speed;
            Position += _velocity * deltaTime;

            // Rectangle
            Rectangle = Geometry.NewRectangle(Position, TextureStore.Player);

            // Invincibility
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
                position: Position + _healthBarOffset,
                sourceRectangle: TextureStore.HealthBarGreenRectangles[Math.Clamp(HealthBarIndex(), 0, TextureStore.HealthBarRedRectangles.Count-1)],
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision)
            );
        }
    }
}