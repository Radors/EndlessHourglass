using System.Numerics;

namespace OriginOfLoot.Types
{
    public interface EnemyCategory
    {
        public float StartingSpeed { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class RedEnemy : EnemyCategory
    {
        public float StartingSpeed { get; set; } = 50f;
        public int Width { get; set; } = 16;
        public int Height { get; set; } = 32;
    }
}
