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
    public class StaffProjectile : IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }
        public bool FacingRight { get; set; }
        public List<IActiveEnemy> EnemiesHit { get; set; } = new();
        public float Speed { get; set; } = 250f;
        public int Damage { get; set; } = 40;

        public StaffProjectile(Texture2D texture, Vector2 position, Vector2 direction, bool facingRight)
        {
            Texture = texture;
            Position = position;

            direction.Normalize();
            Velocity = direction * Speed;

            FacingRight = facingRight;

            Rectangle = Geometry.NewRectangle(position, texture);
        }

        public void Update(float deltaTime) 
        {
            Position += Velocity * deltaTime;
            Rectangle = Geometry.NewRectangle(Position, Texture);
        }

        public void Draw(SpriteBatch spriteBatch) 
        {
            spriteBatch.Draw(
                texture: TextureStore.StaffProjectile,
                position: Position,
                sourceRectangle: default,
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
