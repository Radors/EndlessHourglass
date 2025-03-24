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
        private const float _totalTimePerStage = 12f;
        private readonly ActivePlayer _player;
        private readonly Random _random = new Random();
        private readonly int _xSpawnMax;
        private readonly int _ySpawnMax;

        public List<IActiveEnemy> Enemies { get; private set; } = new();
        public List<IAttachedEffect> AttachedEffects { get; private set; } = new();
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

            var choice = _random.Next(10);
            IActiveEnemy enemy = GameStage >= 5 && choice == 0 ? new RedRanged(spawnPosition, spawnDirection) :
                                                                 new RedMelee(spawnPosition, spawnDirection);

            Enemies.Add(enemy);
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
            AttachedEffects.RemoveAll(n => n.CurrentFrame > n.TotalFrames);

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
