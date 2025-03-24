using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Enemy
{
    public interface IActiveEnemy
    {
        public Vector2 Position { get; }
        public Rectangle Rectangle { get; }
        public int Damage { get; }
        public int MaxHealth { get; }
        public int CurrentHealth { get; set; }

        public void Update(float deltaTime, Vector2 playerPosition);
        public void Draw(SpriteBatch spriteBatch);
    }
}
