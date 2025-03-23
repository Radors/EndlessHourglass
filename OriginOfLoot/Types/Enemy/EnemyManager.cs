using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Static;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Projectile;
using System.Collections.Generic;
using System;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Effect;
using System.Diagnostics;

namespace OriginOfLoot.Types.Enemy
{
    public class EnemyManager
    {
        public List<IActiveEnemy> Enemies { get; set; } = new();
        public List<IEffect> EnemyEffects { get; set; } = new();
        public int GameStage { get; set; } = 1;
        public const float TotalTimePerStage = 12f;
        public float GameStageTimeLeft { get; set; } = 12f;
        public float TimeToNextSpawn { get; set; } = 0;
        private readonly ActivePlayer _player;
        private readonly Random _random = new Random();
        private readonly int _xSpawnMax;
        private readonly int _ySpawnMax;

        public EnemyManager(ActivePlayer player)
        {
            _player = player;
            _xSpawnMax = ConstConfig.ViewPixelsX - ConstConfig.TileStandard;
            _ySpawnMax = ConstConfig.ViewPixelsY - (2 * ConstConfig.TileStandard);
        }

        public void SpawnEnemy()
        {
            int spawnCorridor = _random.Next(3);
            Vector2 spawnPosition = spawnCorridor switch
            {
                0 => new Vector2(0, _random.Next(_ySpawnMax)),
                1 => new Vector2(_random.Next(_xSpawnMax), _ySpawnMax),
                2 => new Vector2(_xSpawnMax, _random.Next(_ySpawnMax)),
                _ => throw new ArgumentOutOfRangeException()
            };

            var spawnDirection = Geometry.Direction(spawnPosition, _player.Position);
            var redMelee = new RedMelee(
                                    TextureStore.RedMelee,
                                    spawnPosition,
                                    spawnDirection
                                );

            Enemies.Add(redMelee);
        }

        public void Update(float deltaTime)
        {
            // Progress GameStage
            if (GameStageTimeLeft <= 0)
            {
                GameStage += 1;
                GameStageTimeLeft = TotalTimePerStage;
            }
            else
            {
                GameStageTimeLeft -= deltaTime;
            }

            // Spawn
            if (TimeToNextSpawn <= 0)
            {
                SpawnEnemy();
                TimeToNextSpawn = 2f / (1f + GameStage);
            }
            else
            {
                TimeToNextSpawn -= deltaTime;
            }

            // Update
            foreach (var enemy in Enemies)
            {
                enemy.Update(deltaTime, _player.Position);
            }
            foreach (var effect in EnemyEffects)
            {
                effect.Update(deltaTime);
            }

            // Collision
            foreach (var enemy in Enemies)
            {
                if (_player.TimeSinceHit > _player.TotalInvincibilityAfterHit && Geometry.CircularCollision(enemy.Rectangle, 22, _player.Rectangle))
                {
                    _player.TakeDamage(enemy.Damage);
                    var direction = Geometry.Direction(_player.Position, enemy.Position);
                    EnemyEffects.Add(new RedMeleeEffect(direction, _player));
                    break;
                }
            }

            // Remove effects
            EnemyEffects.RemoveAll(n => n.CurrentFrame > n.TotalFrames);

            // Remove enemies
            Enemies.RemoveAll(n => n.CurrentHealth <= 0);
            Enemies.RemoveAll(n =>
                n.Position.X < 0 ||
                n.Position.Y < 0 ||
                n.Position.X > ConstConfig.ViewPixelsX ||
                n.Position.Y > ConstConfig.ViewPixelsY
            );
        }


        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var enemy in Enemies)
            {
                enemy.Draw(spriteBatch);
            }
            foreach (var effect in EnemyEffects)
            {
                effect.Draw(spriteBatch);
            }
        }
    }
}
