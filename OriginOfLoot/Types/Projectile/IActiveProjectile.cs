using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using OriginOfLoot.Types.Enemy;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Projectile
{
    public interface IActiveProjectile
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Velocity { get; set; }
        public Rectangle Rectangle { get; set; }
        public bool FacingRight { get; set; }
        public List<IActiveEnemy> EnemiesHit { get; set; }
        public float Speed { get; set; }
        public int Damage { get; set; }

        public void Update(float deltaTime);
        public void Draw(SpriteBatch spriteBatch);
    }
}
