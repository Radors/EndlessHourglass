using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Interfaces;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Static;
using System;

namespace OriginOfLoot.Types.Enemy
{
    public class RedMeleeEffect : IAttachedEffect
    {
        private readonly Vector2 _direction;
        private readonly ActivePlayer _player;
        private const float _totalTimeToLive = 0.30f;
        private float _currentTimeAlive = 0f;
        private const int _totalFrames = 11;
        private int _currentFrame = 1;

        public RedMeleeEffect(Vector2 direction, ActivePlayer player)
        {
            _direction = direction;
            _player = player;
        }

        public bool IsFinished()
        {
            return _currentFrame > _totalFrames;
        }

        public void Update(float deltaTime)
        {
            _currentFrame = (int)(_currentTimeAlive / (_totalTimeToLive / _totalFrames));

            _currentTimeAlive += deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: TextureStore.RedMeleeEffect,
                position: _player.Rectangle.Center.ToVector2() + _direction * 10,
                sourceRectangle: TextureStore.RedMeleeEffectRectangles[Math.Clamp(_currentFrame - 1, 0, _totalFrames - 1)],
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(8, 8),
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + 0.2f
            );
        }
    }
}
