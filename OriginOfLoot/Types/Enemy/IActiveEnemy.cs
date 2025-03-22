using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Enemy
{
    public interface IActiveEnemy
    {
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle { get; set; }
        public Vector2 Velocity { get; set; }
        public bool FacingRight { get; set; }
        public float Speed { get; set; }
        public int MaxHealth { get; set; }
        public int CurrentHealth { get; set; }
        public int Damage { get; set; }
        public Vector2 HealthBarOffset { get; set; }

        public void Update(float deltaTime, Vector2 playerPosition);
        public void Draw(SpriteBatch spriteBatch);
        public int HealthbarFrame();
    }
}
