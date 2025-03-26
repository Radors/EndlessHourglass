using Microsoft.Xna.Framework.Graphics;

namespace EndlessHourglass.Types.Interfaces
{
    public interface IAttachedEffect
    {
        bool IsFinished();

        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
