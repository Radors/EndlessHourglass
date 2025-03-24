using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Static;
using System;

namespace OriginOfLoot.Types.Effect
{
    public class RedMeleeEffect : IAttachedEffect
    {
        private readonly Vector2 _direction;
        private readonly ActivePlayer _player;
        private readonly float _totalTimeToLive = 0.30f;
        private float _currentTimeAlive = 0f;

        public int TotalFrames { get; } = 11;
        public int CurrentFrame { get; private set; } = 1;

        public RedMeleeEffect(Vector2 direction, ActivePlayer player)
        {
            _direction = direction;
            _player = player;
        }

        public void Update(float deltaTime)
        {
            CurrentFrame = (int)(_currentTimeAlive / (_totalTimeToLive / TotalFrames));

            _currentTimeAlive += deltaTime;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: TextureStore.RedMeleeEffect,
                position: _player.Rectangle.Center.ToVector2() + (_direction * 10),
                sourceRectangle: TextureStore.RedMeleeEffectRectangles[Math.Clamp(CurrentFrame-1, 0, TotalFrames - 1)],
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
