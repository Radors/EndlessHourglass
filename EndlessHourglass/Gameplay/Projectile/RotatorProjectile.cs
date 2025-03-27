using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EndlessHourglass.Gameplay.Interfaces;
using EndlessHourglass.Gameplay.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EndlessHourglass.Gameplay.Projectile
{
    public class RotatorProjectile : IProjectile
    {
        private Texture2D _texture = TextureStore.RotatorProjectile;
        private Vector2 _velocity = new();
        private float _speed { get; set; } = 400f;
        private float _timePerFrame { get; set; } = 0.04f;
        private float _currentFrameTime { get; set; } = 0f;
        private int _totalFrames = 6;
        private int _currentFrame = 1;
        private bool _facingRight = true;

        public List<IEntity> HasCollidedWith { get; set; } = new();
        public Vector2 Position { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public int Damage { get; set; } = 40;

        public RotatorProjectile(Vector2 position, Vector2 direction, bool facingRight)
        {
            Position = position;

            direction.Normalize();
            _velocity = direction * _speed;

            _facingRight = facingRight;

            Rectangle = Geometry.NewRectangle(position, _texture);
        }

        public void Update(float deltaTime)
        {
            Position += _velocity * deltaTime;
            Rectangle = Geometry.NewRectangle(Position, _texture);

            if (_currentFrameTime > _timePerFrame)
            {
                _currentFrame = (_currentFrame < _totalFrames) ? _currentFrame + 1 : 0;
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
                sourceRectangle: TextureStore.RotatorProjectileRectangles[Math.Clamp(_currentFrame - 1, 0, _totalFrames - 1)],
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: _facingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision) + 0.1f
            );
        }
    }
}
