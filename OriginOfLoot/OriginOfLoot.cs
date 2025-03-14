using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using OriginOfLoot.Enums;
using System;


namespace OriginOfLoot
{
    public class OriginOfLoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private BoxingViewportAdapter _viewportAdapter;

        Texture2D mapTexture;
        Texture2D playerTexture;
        Texture2D hammerTexture;
        Texture2D swordTexture;
        Texture2D staffTexture;
        bool playerFacingRight;
        float playerFriction;
        float playerAcceleration;
        float maxplayerSpeed;
        PlayerWeapon playerWeapon;
        Vector2 playerPosition;
        Vector2 playerVelocity;
        Vector2 playerMovement;
        Vector2 playerWeaponPosition;
        Vector2 hammerOffset;
        Vector2 swordOffset;
        Vector2 staffOffset;
        Vector2 currentWeaponOffset;

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

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 640, 360);
            _camera = new OrthographicCamera(_viewportAdapter);

            playerVelocity = new(0, 0);
            playerAcceleration = 950f;
            playerFriction = 0.0000001f;
            maxplayerSpeed = 180f;
            hammerOffset = new Vector2(7, 8);
            swordOffset = new Vector2(7, 8);
            staffOffset = new Vector2(7, 8);
            playerWeapon = PlayerWeapon.Hammer;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mapTexture = Content.Load<Texture2D>("ase_prod/map");
            playerTexture = Content.Load<Texture2D>("ase_prod/player");
            hammerTexture = Content.Load<Texture2D>("ase_prod/hammer");
            swordTexture = Content.Load<Texture2D>("ase_prod/sword");
            staffTexture = Content.Load<Texture2D>("ase_prod/sword"); // <----- change
        }

        // `Update()` is called once every frame.
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            /* ==========================
                   player Movement
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


            /* ==========================
                  Boundary Enforcement
               ========================== */

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


            /* ==========================
                    player Weapon
               ========================== */

            if (kstate.IsKeyDown(Keys.NumPad1))
            {
                playerWeapon = PlayerWeapon.Hammer;
            }
            if (kstate.IsKeyDown(Keys.NumPad2))
            {
                playerWeapon = PlayerWeapon.Sword;
            }
            if (kstate.IsKeyDown(Keys.NumPad3))
            {
                playerWeapon = PlayerWeapon.Staff;
            }
                
            currentWeaponOffset = playerWeapon switch
            {
                PlayerWeapon.Hammer => hammerOffset,
                PlayerWeapon.Sword => swordOffset,
                PlayerWeapon.Staff => staffOffset,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (!playerFacingRight)
            {
                currentWeaponOffset = new Vector2(-currentWeaponOffset.X, currentWeaponOffset.Y);
            }
            playerWeaponPosition = playerPosition + currentWeaponOffset;


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
                    PlayerWeapon.Hammer => hammerTexture,
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
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
