using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using OriginOfLoot.Types.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OriginOfLoot.Types.Enemy
{
    public class RedMelee : IActiveEnemy
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle { get; set; }
        public Vector2 Velocity { get; set; }
        public bool FacingRight { get; set; } = true;
        public float Speed { get; set; } = 50f;
        public int MaxHealth { get; set; } = 140;
        public int CurrentHealth { get; set; } = 140;
        public int Damage { get; set; } = 40;
        public Vector2 HealthBarOffset { get; set; } = new Vector2(0, 32);

        public RedMelee(Texture2D texture, Vector2 position, Vector2 direction)
        {
            Texture = texture;
            Position = position;
            Velocity = direction * Speed;
            Rectangle = Geometry.NewRectangle(position, texture);
        }

        public void Update(float deltaTime, Vector2 playerPosition)
        {
            var direction = Geometry.Direction(Position, playerPosition);
            Velocity = direction * Speed;
            Position += Velocity * deltaTime;
            Rectangle = Geometry.NewRectangle(Position, Texture);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: Texture,
                position: Position,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / 100000)
            );
            spriteBatch.Draw(
                texture: TextureStore.HealthBarRed,
                position: Position + HealthBarOffset,
                sourceRectangle: TextureStore.HealthBarRedRectangles[Math.Clamp(HealthbarFrame(), 0, TextureStore.HealthBarRedRectangles.Count-1)],
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / 100000)
            );
        }

        public int HealthbarFrame()
        {
            int frame = (int)(CurrentHealth / 10f);
            return 14 - frame;
        }
    }
}
