
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Enemy;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Effect
{
    public interface IEffect
    {
        public Texture2D Texture { get; set; }
        public Vector2 Direction { get; set; }

        public float TotalTimeToLive { get; set; }
        public int TotalFrames { get; set; }
        public int CurrentFrame { get; set; }
        public float CurrentFrameTime { get; set; }

        public void Update(float deltaTime);
        public void Draw(SpriteBatch spriteBatch);
    }
}
