
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using MonoGame.Extended.Collisions.Layers;
using OriginOfLoot.Types.Enemy;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Projectile;
using OriginOfLoot.Types.Static;
using System;

namespace OriginOfLoot.Types
{
    public class InputManager
    {
        private readonly ActivePlayer _player;
        private readonly ProjectileManager _projectileManager;
        private readonly EnemyManager _enemyManager;

        public InputManager(ActivePlayer player, ProjectileManager projectileManager, EnemyManager enemyManager)
        {
            _player = player;
            _projectileManager = projectileManager;
            _enemyManager = enemyManager;
        }

        public void Update(float deltaTime, KeyboardState kstate, MouseState mstate, Vector2 pointerPos)
        {

            // -------------------------------------------------
            Vector2 inputDirection = Vector2.Zero;
            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                inputDirection.X += 1;
            }
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
            {
                inputDirection.X -= 1;
            }
            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
            {
                inputDirection.Y -= 1;
            }
            if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
            {
                inputDirection.Y += 1;
            }
            if (inputDirection != Vector2.Zero)
            {
                inputDirection.Normalize();
            }
            _player.InputDirection = inputDirection;

            // -------------------------------------------------
            if (kstate.IsKeyDown(Keys.NumPad1))
            {
                _player.Weapon = new Rotator();
            }
            if (kstate.IsKeyDown(Keys.NumPad2))
            {
                _player.Weapon = new Staff();
            }

            // -------------------------------------------------
            if (mstate.LeftButton == ButtonState.Pressed &&
                _player.Weapon.TimeSinceFired > _player.Weapon.FireRate)
            {
                _projectileManager.NewPlayerProjectile();
                _player.Weapon.TimeSinceFired = 0;
            }
            else
            {
                _player.Weapon.TimeSinceFired += deltaTime;
            }

            // -------------------------------------------------
            if (pointerPos.X > _player.Position.X + ConstConfig.TileStandard / 2)
            {
                _player.FacingRight = true;
            }
            else
            {
                _player.FacingRight = false;
            }
        }
    }
}
