using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Static;
using System;

namespace OriginOfLoot.Types.Effect
{
    public class RedMeleeEffect : IEffect
    {
        public Texture2D Texture { get; set; } = TextureStore.RedMeleeEffect;
        public Vector2 Direction { get; set; }

        public float TotalTimeToLive { get; set; } = 0.30f;
        public int TotalFrames { get; set; } = 11;
        public int CurrentFrame { get; set; } = 1;
        public float CurrentFrameTime { get; set; } = 0f;

        private readonly ActivePlayer _player;

        public RedMeleeEffect(Vector2 direction, ActivePlayer player)
        {
            Direction = direction;
            _player = player;
        }

        public void Update(float deltaTime)
        {
            if (CurrentFrameTime > TotalTimeToLive / TotalFrames)
            {
                CurrentFrame += 1;
                CurrentFrameTime = 0;
            }
            else
            {
                CurrentFrameTime += deltaTime;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: TextureStore.RedMeleeEffect,
                position: _player.Rectangle.Center.ToVector2() + (Direction * 10),
                sourceRectangle: TextureStore.RedMeleeEffectRectangles[Math.Clamp(CurrentFrame-1, 0, TotalFrames-1)],
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
