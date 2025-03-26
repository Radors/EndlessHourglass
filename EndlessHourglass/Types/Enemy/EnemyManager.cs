using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EndlessHourglass.Types.Static;
using EndlessHourglass.Types.Projectile;
using System.Collections.Generic;
using System;
using EndlessHourglass.Types.Player;
using System.Diagnostics;
using EndlessHourglass.Types.Interfaces;
using System.Linq;

namespace EndlessHourglass.Types.Enemy
{
    public class EnemyManager
    {
        private const float _totalTimePerStage = 13f;
        private readonly ActivePlayer _player;
        private readonly Random _random = new Random();
        private readonly int _xSpawnMax;
        private readonly int _ySpawnMax;

        public List<IEnemy> Enemies { get; private set; } = new();
        public List<IAttachedEffect> AttachedEffects { get; private set; } = new();
        public List<IProjectile> EnemyProjectilesToSpawn { get; private set; } = new();
        public int GameStage { get; private set; } = 1;
        public float GameStageTimeLeft { get; private set; }
        public float TimeToNextSpawn { get; private set; } = 0;

        public EnemyManager(ActivePlayer player)
        {
            _player = player;
            _xSpawnMax = ConstConfig.ViewPixelsX - ConstConfig.TileStandard;
            _ySpawnMax = ConstConfig.ViewPixelsY - (2 * ConstConfig.TileStandard);
            GameStageTimeLeft = _totalTimePerStage;
        }

        public Vector2 NewSpawnPosition()
        {
            int spawnCorridor = _random.Next(3);
            Vector2 spawnPosition = spawnCorridor switch
            {
                0 => new Vector2(0, _random.Next(_ySpawnMax)),
                1 => new Vector2(_random.Next(_xSpawnMax), _ySpawnMax),
                2 => new Vector2(_xSpawnMax, _random.Next(_ySpawnMax)),
                _ => throw new ArgumentOutOfRangeException()
            };
            return spawnPosition;
        }

        public void SpawnEnemy()
        {
            var spawnPosition = NewSpawnPosition();
            var spawnDirection = Geometry.Direction(spawnPosition, _player.Position);

            int random = _random.Next(10);
            IEnemy enemy;
            if (Enemies.Where(n => n is RedRanged).Count() < 3)
            {
                enemy = (GameStage, random) switch
                {
                    ( >= 10, < 4) => new RedRanged(spawnPosition, spawnDirection, _player, this),
                    ( >= 4, < 3) => new RedRanged(spawnPosition, spawnDirection, _player, this),
                    (_, < 2) => new RedRanged(spawnPosition, spawnDirection, _player, this),
                    (_, _) => new RedMelee(spawnPosition, spawnDirection, _player)
                };
            }
            else
            {
                enemy = new RedMelee(spawnPosition, spawnDirection, _player);
            }
            Enemies.Add(enemy);
        }

        public void NewEnemyProjectile(IEnemy enemy)
        {
            var direction = Geometry.Direction(enemy.Position, _player.Position);
            var projectile = new RedRangedProjectile(enemy.Position, direction);

            EnemyProjectilesToSpawn.Add(projectile);
        }

        public void Update(float deltaTime)
        {
            // Progress GameStage
            if (GameStageTimeLeft <= 0)
            {
                GameStage += 1;
                GameStageTimeLeft = _totalTimePerStage;
            }
            else
            {
                GameStageTimeLeft -= deltaTime;
            }

            // Spawn
            if (TimeToNextSpawn <= 0)
            {
                SpawnEnemy();
                TimeToNextSpawn = GameStage < 4 ? 2f / GameStage : 2f / 4;
            }
            else
            {
                TimeToNextSpawn -= deltaTime;
            }

            // Update
            foreach (var enemy in Enemies)
            {
                enemy.Update(deltaTime);
            }
            foreach (var effect in AttachedEffects)
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
                    AttachedEffects.Add(new RedMeleeEffect(direction, _player));
                    break;
                }
            }

            // Remove effects
            AttachedEffects.RemoveAll(n => n.IsFinished());

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
            foreach (var effect in AttachedEffects)
            {
                effect.Draw(spriteBatch);
            }
        }
    }
}
