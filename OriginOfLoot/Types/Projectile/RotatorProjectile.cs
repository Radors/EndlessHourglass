using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.StaticMethods;
using OriginOfLoot.Types.Enemy;
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
        public List<IActiveEnemy> EnemiesHit { get; set; } = new();
        public float Speed { get; set; } = 350f;

        public int CurrentFrame { get; set; } = 0;
        public float CurrentFrameTime { get; set; } = 0f;
        public float TimePerFrame { get; set; } = 0.03f;
        public int TotalFrames { get; set; } = 6;

        public RotatorProjectile(Texture2D texture, Vector2 position, Vector2 direction)
        {
            Texture = texture;
            Position = position;

            direction.Normalize();
            Velocity = direction * Speed;

            Rectangle = Geometry.NewRectangle(position, texture);
        }

        public void UpdateFrame(float deltaTime)
        {
            if (CurrentFrameTime > TimePerFrame)
            {
                Debug.WriteLine("first path");
                CurrentFrame = (CurrentFrame < TotalFrames - 1) ? CurrentFrame + 1 : 0;
                CurrentFrameTime = 0;
                Debug.WriteLine("frame:" + CurrentFrame);
            }
            else
            {
                Debug.WriteLine("second path");
                CurrentFrameTime += deltaTime;
            }
        }
    }
}
