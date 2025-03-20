using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Enemy;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Static;
using System;
using System.Collections.Generic;

namespace OriginOfLoot.Types.Projectile
{
    public class ProjectileManager
    {
        public List<IActiveProjectile> Projectiles { get; set; } = new();
        public List<ProjectileID> playerProjectilesToSpawn { get; set; } = new();
        private readonly ActivePlayer _player;
        private readonly EnemyManager _enemyManager;

        public ProjectileManager(ActivePlayer player, EnemyManager enemyManager)
        {
            _player = player;
            _enemyManager = enemyManager;
        }

        public void NewPlayerProjectile()
        {
            var projectileID = _player.Weapon switch
            {
                Rotator => ProjectileID.Rotator,
                Staff => ProjectileID.Staff,
                _ => throw new ArgumentOutOfRangeException()
            };
            playerProjectilesToSpawn.Add(projectileID);
        }

        public void Update(float deltaTime, Vector2 pointerPos)
        {
            // Creation
            foreach (var projectileID in playerProjectilesToSpawn)
            {
                var position = _player.Position + _player.ProjectileOffset();

                var direction = new Vector2(pointerPos.X - (position.X + _player.ProjectileDirectionOffset().X),
                                            pointerPos.Y - (position.Y + _player.ProjectileDirectionOffset().Y));

                IActiveProjectile projectile = projectileID switch
                {
                    ProjectileID.Rotator => new RotatorProjectile(TextureStore.RotatorProjectile, position, direction, _player.FacingRight),
                    ProjectileID.Staff => new StaffProjectile(TextureStore.StaffProjectile, position, direction, _player.FacingRight),
                    _ => throw new ArgumentOutOfRangeException()
                };

                Projectiles.Add(projectile);

            }
            playerProjectilesToSpawn.Clear();

            // Movement
            foreach (var projectile in Projectiles)
            {
                projectile.Position += projectile.Velocity * deltaTime;
                projectile.Rectangle = Geometry.NewRectangle(projectile.Position, projectile.Texture);

                if (projectile is RotatorProjectile rotatorProjectile)
                {
                    rotatorProjectile.UpdateFrame(deltaTime);
                }
            }

            // Collision
            foreach (var projectile in Projectiles)
            {
                foreach (var enemy in _enemyManager.ActiveEnemies)
                {
                    if (!projectile.EnemiesHit.Contains(enemy) &&
                        projectile.Rectangle.Intersects(enemy.Rectangle))
                    {
                        var damage = projectile switch
                        {
                            RotatorProjectile => new Rotator().Damage,
                            StaffProjectile => new Staff().Damage,
                            _ => throw new ArgumentOutOfRangeException()
                        };

                        enemy.CurrentHealth -= damage;
                        projectile.EnemiesHit.Add(enemy);
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
                if (projectile is StaffProjectile staffProjectile)
                {
                    spriteBatch.Draw(
                        texture: TextureStore.StaffProjectile,
                        position: staffProjectile.Position,
                        sourceRectangle: default,
                        color: Color.White,
                        rotation: 0f,
                        origin: default,
                    scale: 1f,
                        effects: projectile.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        layerDepth: ConstConfig.StandardDepth + (_player.Position.Y / ConstConfig.StandardDepthDivision) + 0.000002f
                    );
                }
                else if (projectile is RotatorProjectile rotatorProjectile)
                {
                    spriteBatch.Draw(
                        texture: TextureStore.RotatorProjectile,
                        position: projectile.Position,
                        sourceRectangle: TextureStore.RotatorProjectileRectangles[rotatorProjectile.CurrentFrame],
                        color: Color.White,
                        rotation: 0f,
                        origin: default,
                    scale: 1f,
                        effects: projectile.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        layerDepth: ConstConfig.StandardDepth + (_player.Position.Y / ConstConfig.StandardDepthDivision) + 0.000002f
                    );
                }
            }
        }
    }
}
