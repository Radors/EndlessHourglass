using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Effect
{
    public interface IAttachedEffect
    {
        public int TotalFrames { get; } 
        public int CurrentFrame { get; }

        public void Update(float deltaTime);
        public void Draw(SpriteBatch spriteBatch);
    }
}
