using Microsoft.Xna.Framework;

namespace OriginOfLoot.Types
{
    public class ActiveEnemy
    {
        public EnemyCategory EnemyCategory { get; set; }
        public Vector2 Position { get; set; }
        public Rectangle Rectangle { get; set; }
        public Vector2 Velocity { get; set; }
        public float TimeUntilNextAction { get; set; }

        public ActiveEnemy(EnemyCategory enemyCategory, Vector2 position, Vector2 velocity, float timeUntilNextAction)
        {
            EnemyCategory = enemyCategory;
            Position = position;
            Velocity = velocity;
            TimeUntilNextAction = timeUntilNextAction;
        }

    }
}
