using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Static;
using System;

namespace OriginOfLoot.Types.Effect
{
    public class RedMeleeEffect : IEffect
    {
        public Texture2D Texture { get; set; } = TextureStore.RedMeleeEffect;
        public Vector2 CenterPosition { get; set; }
        public bool FacingRight { get; set; }

        public float TotalTimeToLive { get; set; } = 0.3f;
        public int CurrentFrame { get; set; } = 1;
        public float CurrentFrameTime { get; set; } = 0f;


        public RedMeleeEffect(Vector2 centerPosition, bool facingRight)
        {
            CenterPosition = centerPosition;
            FacingRight = facingRight;
        }

        public void Update(float deltaTime)
        {
            if (CurrentFrameTime > TotalTimeToLive / TextureStore.RedMeleeEffectRectangles.Count)
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
                position: CenterPosition,
                sourceRectangle: TextureStore.RedMeleeEffectRectangles[Math.Clamp(CurrentFrame-1, 0, 2)],
                color: Color.White,
                rotation: 0f,
                origin: new Vector2(6, 6),
                scale: 1f,
                effects: FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: ConstConfig.StandardDepth + 0.2f
            );
        }
    }
}
