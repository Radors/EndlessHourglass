using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.Collisions.Layers;
using OriginOfLoot.Types.Enemy;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OriginOfLoot.Types.Projectile
{
    public class ProjectileManager
    {
        public List<IActiveProjectile> Projectiles { get; set; } = new();
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

            IActiveProjectile projectile = _player.Weapon switch
            {
                Rotator => new RotatorProjectile(TextureStore.RotatorProjectile, position, direction, _player.FacingRight),
                Staff => new StaffProjectile(TextureStore.StaffProjectile, position, direction, _player.FacingRight),
                _ => throw new ArgumentOutOfRangeException()
            };
            Projectiles.Add(projectile);
        }

        public void Update(float deltaTime, Vector2 pointerPos)
        {
            // Update
            foreach (var projectile in Projectiles)
            {
                projectile.Update(deltaTime);
            }

            // Collision
            foreach (var projectile in Projectiles)
            {
                foreach (var enemy in _enemyManager.Enemies)
                {
                    if (!projectile.EnemiesHit.Contains(enemy) &&
                        Geometry.RectangularCollision(projectile.Rectangle, enemy.Rectangle))
                    {
                        projectile.EnemiesHit.Add(enemy);
                        enemy.CurrentHealth -= projectile.Damage;
                    }
                }
            }

            // Boundary
            Projectiles.RemoveAll(n =>
                n.Position.X < 0 ||
                n.Position.Y < 0 ||
                n.Position.X > ConstConfig.ViewPixelsX ||
                n.Position.Y > ConstConfig.ViewPixelsY
            );
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var projectile in Projectiles)
            {
                projectile.Draw(spriteBatch);
            }
        }
    }
}
