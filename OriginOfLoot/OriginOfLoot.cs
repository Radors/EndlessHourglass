using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using OriginOfLoot.Enums;
using System;
using System.Collections.Generic;


namespace OriginOfLoot
{
    public class OriginOfLoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private BoxingViewportAdapter _viewportAdapter;
        int viewPixelsX;
        int viewPixelsY;
        int viewTileWidth;
        
        Texture2D mapTexture;
        Texture2D playerTexture;
        Texture2D swordTexture;
        Texture2D staffTexture;
        Texture2D staffProjectileTexture;
        bool playerFacingRight;
        float playerFriction;
        float playerAcceleration;
        float maxplayerSpeed;
        PlayerWeapon playerWeapon;
        Vector2 playerPosition;
        Vector2 playerVelocity;
        Vector2 playerMovement;
        Vector2 playerWeaponPosition;
        Vector2 swordOffset;
        Vector2 staffOffset;
        Vector2 currentWeaponOffset;
        List<ActiveStaffProjectile> activeStaffProjectiles;
        float staffProjectileSpeed;
        Vector2 staffProjectileOffset;

        public OriginOfLoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = false;
            //_graphics.PreferredBackBufferWidth = 1280;
            //_graphics.PreferredBackBufferHeight = 720;
            //_graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            viewPixelsX = 640;
            viewPixelsY = 360;
            viewTileWidth = 16;
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, viewPixelsX, viewPixelsY);
            _camera = new OrthographicCamera(_viewportAdapter);

            playerVelocity = new(0, 0);
            playerAcceleration = 950f;
            playerFriction = 0.0000001f;
            maxplayerSpeed = 180f;
            swordOffset = new Vector2(7, 8);
            staffOffset = new Vector2(7, 8);
            playerWeapon = PlayerWeapon.Sword;
            staffProjectileSpeed = 250f;
            activeStaffProjectiles = new List<ActiveStaffProjectile>();
            staffProjectileOffset = new Vector2(19, 8);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mapTexture = Content.Load<Texture2D>("ase_prod/map");
            playerTexture = Content.Load<Texture2D>("ase_prod/player");
            swordTexture = Content.Load<Texture2D>("ase_prod/sword");
            staffTexture = Content.Load<Texture2D>("ase_prod/staff");
            staffProjectileTexture = Content.Load<Texture2D>("ase_prod/staffProjectile");
        }

        // `Update()` is called once every frame.
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mstate = Mouse.GetState();
            KeyboardState kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            /* ==========================
                    Player Movement
               ========================== */

            Vector2 playerInputDirection = Vector2.Zero;

            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                playerInputDirection.X += 1;
            }
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
            {
                playerInputDirection.X -= 1;
            }
            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
            {
                playerInputDirection.Y -= 1;
            }
            if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
            {
                playerInputDirection.Y += 1;
            }

            if (playerInputDirection != Vector2.Zero) // The player inputs keys to actively move
            {
                // In general, we always normalize the direction vector, into a "Unit vector".
                // This turns a diagonal (1, -1) into (0.707, -0.707) and length == 1.
                playerInputDirection.Normalize();

                // --> Acceleration has been disabled for now, to ensure movement feels responsive.
                // playerVelocity += playerInputDirection * playerAcceleration * deltaTime;
                playerVelocity = playerInputDirection * maxplayerSpeed;

                playerFacingRight = playerInputDirection.X switch
                {
                    > 0 => true,
                    < 0 => false,
                    _ => playerFacingRight
                };
            }
            else
            {
                playerVelocity.X = 0;
                playerVelocity.Y = 0;
                // --> Friction has been disabled for now, to ensure movement feels responsive.
                //if (playerInputDirection.X == 0)
                //{
                //    playerVelocity.X *= MathF.Pow(playerFriction, deltaTime);
                //}
                //if (playerInputDirection.Y == 0)
                //{
                //    playerVelocity.Y *= MathF.Pow(playerFriction, deltaTime);
                //}
            }

            // Ensure that we are not breaking speed limits in any direction
            if (playerVelocity.LengthSquared() > maxplayerSpeed * maxplayerSpeed)
            {
                playerVelocity = Vector2.Normalize(playerVelocity) * maxplayerSpeed;
            }

            // Compute final movement vector and add it to current position vector
            playerMovement = playerVelocity * deltaTime;
            playerPosition += playerMovement;


            /* =======================================
                    Boundary Enforcement: Player
               ======================================= */

            // In this section, we currently force the screen boundary, but nothing related to in-game walls.
            int Xmax = _viewportAdapter.VirtualWidth - playerTexture.Width;
            int Ymax = _viewportAdapter.VirtualHeight - playerTexture.Height;

            if (playerPosition.X > Xmax)
            {
                playerPosition.X = Xmax;
            }
            else if (playerPosition.X < 0)
            {
                playerPosition.X = 0;
            }

            if (playerPosition.Y > Ymax)
            {
                playerPosition.Y = Ymax;
            }
            else if (playerPosition.Y < 0)
            {
                playerPosition.Y = 0;
            }


            /* =======================================
                  Player Weapon: Equip and Position
               ======================================= */

            if (kstate.IsKeyDown(Keys.NumPad1))
            {
                playerWeapon = PlayerWeapon.Sword;
            }
            if (kstate.IsKeyDown(Keys.NumPad2))
            {
                playerWeapon = PlayerWeapon.Staff;
            }
                
            currentWeaponOffset = playerWeapon switch
            {
                PlayerWeapon.Sword => swordOffset,
                PlayerWeapon.Staff => staffOffset,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (!playerFacingRight)
            {
                currentWeaponOffset = new Vector2(-currentWeaponOffset.X, currentWeaponOffset.Y);
            }
            playerWeaponPosition = playerPosition + currentWeaponOffset;

            /* =======================================
                      Player Weapon: Projectiles
               ======================================= */

            if (mstate.LeftButton == ButtonState.Pressed) // Create a new projectile
            {
                Vector2 pointerPos = _camera.ScreenToWorld(mstate.X, mstate.Y);
                Vector2 currentProjectileOffset = playerFacingRight ? 
                    staffProjectileOffset : 
                    new Vector2(viewTileWidth - staffProjectileTexture.Width - staffProjectileOffset.X, 
                                staffProjectileOffset.Y);

                var projectilePosition = playerPosition + currentProjectileOffset;

                var projectileDirection = new Vector2(pointerPos.X - projectilePosition.X, 
                                                      pointerPos.Y - projectilePosition.Y);
                projectileDirection.Normalize();
                var projectileVelocity = projectileDirection * staffProjectileSpeed;

                var newProjectile = new ActiveStaffProjectile(projectilePosition, projectileVelocity);

                activeStaffProjectiles.Add(newProjectile);
            }

            foreach (var projectile in activeStaffProjectiles) // Update position of all projectiles
            {
                projectile.Position += projectile.Velocity * deltaTime;
            }


            /* =======================================
                  Boundary Enforcement: Projectiles
               ======================================= */

            activeStaffProjectiles.RemoveAll(n =>
                n.Position.X < 0 || 
                n.Position.Y < 0 ||
                n.Position.X > viewPixelsX ||
                n.Position.Y > viewPixelsY
            );


            base.Update(gameTime);
        }

        // `Draw()` is also called once every frame, right after `Update()`
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aquamarine);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(mapTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(
                texture: playerTexture,
                position: playerPosition,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: playerFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0f
            );
            _spriteBatch.Draw(
                texture: playerWeapon switch { 
                    PlayerWeapon.Sword => swordTexture,
                    PlayerWeapon.Staff => staffTexture,
                    _ => throw new ArgumentOutOfRangeException()
                },
                position: playerWeaponPosition,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: playerFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: 0f
            );
            foreach (var projectile in activeStaffProjectiles)
            {
                _spriteBatch.Draw(
                texture: staffProjectileTexture,
                position: projectile.Position,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: 0f
            );
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
