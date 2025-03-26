using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using EndlessHourglass.Types.Enemy;
using EndlessHourglass.Types.Player.Weapon;
using EndlessHourglass.Types.Projectile;
using EndlessHourglass.Types.Static;

namespace EndlessHourglass.Types.Player
{
    public class InputManager
    {
        private readonly ActivePlayer _player;
        private readonly ProjectileManager _projectileManager;
        private readonly EndlessHourglass _game;

        private readonly Rectangle RotatorEquipRectangle = new Rectangle(269, 45, 1, 1);
        private readonly Rectangle StaffEquipRectangle = new Rectangle(388, 45, 1, 1);

        public InputManager(ActivePlayer player, ProjectileManager projectileManager, EndlessHourglass game)
        {
            _player = player;
            _projectileManager = projectileManager;
            _game = game;
        }

        public void Update(float deltaTime, KeyboardState kstate, MouseState mstate, Vector2 pointerPos)
        {
            if (_game.IsGameOver() && (kstate.IsKeyDown(Keys.Space) || kstate.IsKeyDown(Keys.Enter)))
            {
                _game.Restart();
            }

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
            if (kstate.IsKeyDown(Keys.E))
            {
                if (Geometry.CircularCollision(_player.Rectangle, 30, RotatorEquipRectangle))
                {
                    _player.Weapon = new Rotator();
                }
                else if (Geometry.CircularCollision(_player.Rectangle, 30, StaffEquipRectangle))
                {
                    _player.Weapon = new Staff();
                }
            }

            // -------------------------------------------------
            if (mstate.LeftButton == ButtonState.Pressed &&
                _player.Weapon.TimeSinceFired > _player.Weapon.FireRate)
            {
                _projectileManager.NewPlayerProjectile(pointerPos);
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
