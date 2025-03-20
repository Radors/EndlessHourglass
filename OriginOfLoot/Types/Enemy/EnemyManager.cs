using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Static;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Projectile;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Enemy
{
    public class EnemyManager
    {
        public List<IActiveEnemy> ActiveEnemies { get; set; } = new();
        public int GameStage { get; set; } = 0;
        public float GameStageTimeLeft { get; set; } = 10;
        public float TimeSinceLastSpawn { get; set; } = 0;

        public EnemyManager()
        {
        }

        public void Update(float deltaTime)
        {
            // Spawn
            if (TimeSinceLastSpawn <= 0)
            {
                var redRanged = new RedRanged(
                                    TextureStore.RedRanged,
                                    new Vector2(0, 200),
                                    new Vector2(1, 0)
                                );

                ActiveEnemies.Add(redRanged);
                TimeSinceLastSpawn = 1;
            }
            else
            {
                TimeSinceLastSpawn -= deltaTime;
            }

            // Movement
            foreach (var enemy in ActiveEnemies)
            {
                enemy.Position += enemy.Velocity * deltaTime;
                enemy.Rectangle = Geometry.NewRectangle(enemy.Position, enemy.Texture);
            }

            // Remove
            ActiveEnemies.RemoveAll(n => n.CurrentHealth <= 0);
            ActiveEnemies.RemoveAll(n =>
                n.Position.X < 0 ||
                n.Position.Y < 0 ||
                n.Position.X > ConstConfig.ViewPixelsX ||
                n.Position.Y > ConstConfig.ViewPixelsY
            );
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var enemy in ActiveEnemies)
            {
                // Enemy
                spriteBatch.Draw(
                    texture: enemy.Texture,
                    position: enemy.Position,
                    sourceRectangle: default,
                    color: Color.White,
                    rotation: 0f,
                    origin: default,
                    scale: 1f,
                    effects: default,
                    layerDepth: ConstConfig.StandardDepth + (enemy.Position.Y / 100000)
                );
                // Enemy healthbar
                spriteBatch.Draw(
                    texture: TextureStore.HealthBar,
                    position: enemy.Position + enemy.HealthbarOffset,
                    sourceRectangle: TextureStore.HealthBarRectangles[enemy.HealthbarFrame()],
                    color: Color.White,
                    rotation: 0f,
                    origin: default,
                    scale: 1f,
                    effects: default,
                    layerDepth: ConstConfig.StandardDepth + (enemy.Position.Y / 100000)
                );
            }
        }
    }
}
