using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EndlessHourglass.Types.Interfaces;
using EndlessHourglass.Types.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EndlessHourglass.Types.Projectile
{
    public class RedRangedProjectile : IProjectile
    {
        private Texture2D _texture = TextureStore.RedRangedProjectile;
        private Vector2 _velocity = new();
        private float _speed = 270f;
        private float _timePerFrame = 0.08f;
        private float _currentFrameTime = 0f;
        private int _totalFrames = 4;
        private int _currentFrame = 1;

        public List<IEntity> HasCollidedWith { get; set; } = new();
        public Vector2 Position { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public int Damage { get; private set; } = 40;

        public RedRangedProjectile(Vector2 position, Vector2 direction)
        {
            Position = position;

            direction.Normalize();
            _velocity = direction * _speed;

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
                sourceRectangle: TextureStore.RedRangedProjectileRectangles[Math.Clamp(_currentFrame - 1, 0, _totalFrames - 1)],
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision) + 0.1f
            );
        }
    }
}
