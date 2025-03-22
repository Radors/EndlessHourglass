using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using OriginOfLoot.Types.Enemy;
using OriginOfLoot.Types.Static;
using System.Collections.Generic;
using System.Diagnostics;

namespace OriginOfLoot.Types.Projectile
{
    public class RotatorProjectile : IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }
        public bool FacingRight { get; set; }
        public List<IActiveEnemy> EnemiesHit { get; set; } = new();
        public float Speed { get; set; } = 350f;
        public int Damage { get; set; } = 40;

        public int CurrentFrame { get; set; } = 0;
        public float CurrentFrameTime { get; set; } = 0f;
        public float TimePerFrame { get; set; } = 0.03f;
        public int TotalFrames { get; set; } = 6;

        public RotatorProjectile(Texture2D texture, Vector2 position, Vector2 direction, bool facingRight)
        {
            Texture = texture;
            Position = position;

            direction.Normalize();
            Velocity = direction * Speed;

            FacingRight = facingRight;

            Rectangle = Geometry.NewRectangle(position, texture);
        }

        public void UpdateFrame(float deltaTime)
        {
            if (CurrentFrameTime > TimePerFrame)
            {
                CurrentFrame = (CurrentFrame < TotalFrames - 1) ? CurrentFrame + 1 : 0;
                CurrentFrameTime = 0;
            }
            else
            {
                CurrentFrameTime += deltaTime;
            }
        }

        public void Update(float deltaTime)
        {
            Position += Velocity * deltaTime;
            Rectangle = Geometry.NewRectangle(Position, Texture);

            UpdateFrame(deltaTime);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: TextureStore.RotatorProjectile,
                position: Position,
                sourceRectangle: TextureStore.RotatorProjectileRectangles[CurrentFrame],
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
