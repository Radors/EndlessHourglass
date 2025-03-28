﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EndlessHourglass.Gameplay.Interfaces
{
    public interface IEntity
    {
        Vector2 Position { get; }

        void Update(float deltaTime);
        void Draw(SpriteBatch spriteBatch);
    }
}
