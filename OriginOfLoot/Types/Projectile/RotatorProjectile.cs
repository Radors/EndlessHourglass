using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using OriginOfLoot.Types.Enemy;
using OriginOfLoot.Types.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OriginOfLoot.Types.Projectile
{
    public class RotatorProjectile : IActiveProjectile
    {
        private Texture2D _texture = TextureStore.RotatorProjectile;
        private float _speed { get; set; } = 350f;
        private float _timePerFrame { get; set; } = 0.03f;
        private float _currentFrameTime { get; set; } = 0f;

        public List<IActiveEnemy> EnemiesHit { get; set; } = new();
        public Vector2 Position { get; private set; }
        public Vector2 Velocity { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public bool FacingRight { get; private set; }
        public int Damage { get; set; } = 40;
        public int TotalFrames { get; } = 6;
        public int CurrentFrame { get; private set; } = 1;

        public RotatorProjectile(Vector2 position, Vector2 direction, bool facingRight)
        {
            Position = position;

            direction.Normalize();
            Velocity = direction * _speed;

            FacingRight = facingRight;

            Rectangle = Geometry.NewRectangle(position, _texture);
        }

        public void Update(float deltaTime)
        {
            Position += Velocity * deltaTime;
            Rectangle = Geometry.NewRectangle(Position, _texture);

            if (_currentFrameTime > _timePerFrame)
            {
                CurrentFrame = (CurrentFrame < TotalFrames) ? CurrentFrame + 1 : 0;
                _currentFrameTime = 0;
            }
            else
            {
                _currentFrameTime += deltaTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: _texture,
                position: Position,
                sourceRectangle: TextureStore.RotatorProjectileRectangles[Math.Clamp(CurrentFrame - 1, 0, TotalFrames - 1)],
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision) + 0.1f
            );
        }
    }
}
