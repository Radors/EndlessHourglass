using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EndlessHourglass.Gameplay.Enemy;
using EndlessHourglass.Gameplay.Interfaces;
using EndlessHourglass.Gameplay.Player;
using EndlessHourglass.Gameplay.Player.Weapon;
using EndlessHourglass.Gameplay.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EndlessHourglass.Gameplay.Projectile
{
    public class ProjectileManager
    {
        public List<IProjectile> PlayerProjectiles { get; set; } = new();
        public List<IProjectile> EnemyProjectiles { get; set; } = new();
        private readonly ActivePlayer _player;
        private readonly EnemyManager _enemyManager;

        public ProjectileManager(ActivePlayer player, EnemyManager enemyManager)
        {
            _player = player;
            _enemyManager = enemyManager;
        }

        public void NewPlayerProjectile(Vector2 pointerPos)
        {
            var position = _player.Position + _player.ProjectileOffset();
            var direction = new Vector2(pointerPos.X - (position.X + _player.ProjectileDirectionOffset().X),
                                        pointerPos.Y - (position.Y + _player.ProjectileDirectionOffset().Y));

            IProjectile projectile = _player.Weapon switch
            {
                Rotator => new RotatorProjectile(position, direction, _player.FacingRight),
                Staff => new StaffProjectile(position, direction, _player.FacingRight),
                _ => throw new ArgumentOutOfRangeException()
            };
            PlayerProjectiles.Add(projectile);
        }

        public void Update(float deltaTime, Vector2 pointerPos)
        {
            EnemyProjectiles.AddRange(_enemyManager.EnemyProjectilesToSpawn);
            _enemyManager.EnemyProjectilesToSpawn.Clear();

            foreach (var projectile in PlayerProjectiles)
            {
                projectile.Update(deltaTime);
            }
            foreach (var projectile in EnemyProjectiles)
            {
                projectile.Update(deltaTime);
            }

            foreach (var projectile in PlayerProjectiles)
            {
                foreach (var enemy in _enemyManager.Enemies)
                {
                    if (!projectile.HasCollidedWith.Contains(enemy) &&
                        Geometry.RectangularCollisionWithOverlap(enemy.Rectangle, 5, 8, projectile.Rectangle))
                    {
                        projectile.HasCollidedWith.Add(enemy);
                        enemy.CurrentHealth -= projectile.Damage;
                    }
                }
            }
            foreach (var projectile in EnemyProjectiles)
            {
                if (!projectile.HasCollidedWith.Contains(_player) &&
                        Geometry.CircularCollision(projectile.Rectangle, 20, _player.Rectangle))
                {
                    projectile.HasCollidedWith.Add(_player);
                    _player.CurrentHealth -= projectile.Damage;
                }
            }

            EnforceBoundary(PlayerProjectiles);
            EnforceBoundary(EnemyProjectiles);
        }

        public void EnforceBoundary(List<IProjectile> projectiles)
        {
            projectiles.RemoveAll(n =>
                n.Position.X < 0 ||
                n.Position.Y < 0 ||
                n.Position.X > ConstConfig.ViewPixelsX ||
                n.Position.Y > ConstConfig.ViewPixelsY
            );
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var projectile in PlayerProjectiles)
            {
                projectile.Draw(spriteBatch);
            }
            foreach (var projectile in EnemyProjectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
    }
}
