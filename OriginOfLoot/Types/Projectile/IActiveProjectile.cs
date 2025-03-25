using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using OriginOfLoot.Types.Enemy;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Projectile
{
    public interface IActiveProjectile
    {
        public List<IActiveEnemy> EnemiesHit { get; set; }
        public Vector2 Position { get; }
        public Vector2 Velocity { get; }
        public Rectangle Rectangle { get; }
        public int Damage { get; set; }
        public int TotalFrames { get; }
        public int CurrentFrame { get; }

        public void Update(float deltaTime);
        public void Draw(SpriteBatch spriteBatch);
    }
}
