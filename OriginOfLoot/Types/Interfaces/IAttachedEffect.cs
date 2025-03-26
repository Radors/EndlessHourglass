using Microsoft.Xna.Framework.Graphics;

namespace OriginOfLoot.Types.Interfaces
{
    public interface IAttachedEffect
    {
        bool IsFinished();

        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
