using Microsoft.Xna.Framework.Graphics;

namespace EndlessHourglass.Gameplay.Interfaces
{
    public interface IAttachedEffect
    {
        bool IsFinished();

        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
