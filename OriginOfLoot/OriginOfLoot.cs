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
        Texture2D characterTexture;
        Texture2D hammerTexture;
        Texture2D swordTexture;
        Texture2D bowTexture;
        bool characterFacingRight;
        float characterFriction;
        float characterAcceleration;
        float maxCharacterSpeed;
        CharacterWeapon characterWeapon;
        Vector2 characterPosition;
        Vector2 characterVelocity;
        Vector2 characterMovement;
        Vector2 characterWeaponPosition;
        Vector2 hammerOffset;
        Vector2 swordOffset;
        Vector2 bowOffset;
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

            characterVelocity = new(0, 0);
            characterAcceleration = 950f;
            characterFriction = 0.0000001f;
            maxCharacterSpeed = 180f;
            hammerOffset = new Vector2(7, 8);
            swordOffset = new Vector2(7, 8);
            bowOffset = new Vector2(7, 8);
            characterWeapon = CharacterWeapon.Hammer;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mapTexture = Content.Load<Texture2D>("ase_prod/map");
            characterTexture = Content.Load<Texture2D>("ase_prod/character");
            hammerTexture = Content.Load<Texture2D>("ase_prod/hammer");
            swordTexture = Content.Load<Texture2D>("ase_prod/sword");
            bowTexture = Content.Load<Texture2D>("ase_prod/sword"); // <----- change
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
                   Character Movement
               ========================== */

            Vector2 characterInputDirection = Vector2.Zero;

            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                characterInputDirection.X += 1;
            }
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
            {
                characterInputDirection.X -= 1;
            }
            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
            {
                characterInputDirection.Y -= 1;
            }
            if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
            {
                characterInputDirection.Y += 1;
            }

            if (characterInputDirection != Vector2.Zero) // The player inputs keys to actively move
            {
                // In general, we always normalize the direction vector, into a "Unit vector".
                // This turns a diagonal (1, -1) into (0.707, -0.707) and length == 1.
                characterInputDirection.Normalize();

                // --> Acceleration has been disabled for now, to ensure movement feels responsive.
                // characterVelocity += characterInputDirection * characterAcceleration * deltaTime;
                characterVelocity = characterInputDirection * maxCharacterSpeed;

                characterFacingRight = characterInputDirection.X switch
                {
                    > 0 => true,
                    < 0 => false,
                    _ => characterFacingRight
                };
            }
            else
            {
                characterVelocity.X = 0;
                characterVelocity.Y = 0;
                // --> Friction has been disabled for now, to ensure movement feels responsive.
                //if (characterInputDirection.X == 0)
                //{
                //    characterVelocity.X *= MathF.Pow(characterFriction, deltaTime);
                //}
                //if (characterInputDirection.Y == 0)
                //{
                //    characterVelocity.Y *= MathF.Pow(characterFriction, deltaTime);
                //}
            }

            // Ensure that we are not breaking speed limits in any direction
            if (characterVelocity.LengthSquared() > maxCharacterSpeed * maxCharacterSpeed)
            {
                characterVelocity = Vector2.Normalize(characterVelocity) * maxCharacterSpeed;
            }

            // Compute final movement vector and add it to current position vector
            characterMovement = characterVelocity * deltaTime;
            characterPosition += characterMovement;


            /* ==========================
                  Boundary Enforcement
               ========================== */

            // In this section, we currently force the screen boundary, but nothing related to in-game walls.
            int Xmax = _viewportAdapter.VirtualWidth - characterTexture.Width;
            int Ymax = _viewportAdapter.VirtualHeight - characterTexture.Height;

            if (characterPosition.X > Xmax)
            {
                characterPosition.X = Xmax;
            }
            else if (characterPosition.X < 0)
            {
                characterPosition.X = 0;
            }

            if (characterPosition.Y > Ymax)
            {
                characterPosition.Y = Ymax;
            }
            else if (characterPosition.Y < 0)
            {
                characterPosition.Y = 0;
            }


            /* ==========================
                    Character Weapon
               ========================== */

            if (kstate.IsKeyDown(Keys.NumPad1))
            {
                characterWeapon = CharacterWeapon.Hammer;
            }
            if (kstate.IsKeyDown(Keys.NumPad2))
            {
                characterWeapon = CharacterWeapon.Sword;
            }
            if (kstate.IsKeyDown(Keys.NumPad3))
            {
                characterWeapon = CharacterWeapon.Bow;
            }
                
            currentWeaponOffset = characterWeapon switch
            {
                CharacterWeapon.Hammer => hammerOffset,
                CharacterWeapon.Sword => swordOffset,
                CharacterWeapon.Bow => bowOffset,
                _ => throw new ArgumentOutOfRangeException()
            };
            if (!characterFacingRight)
            {
                currentWeaponOffset = new Vector2(-currentWeaponOffset.X, currentWeaponOffset.Y);
            }
            characterWeaponPosition = characterPosition + currentWeaponOffset;


            base.Update(gameTime);
        }

        // `Draw()` is also called once every frame, right after `Update()`
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aquamarine);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(mapTexture, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(
                texture: characterTexture,
                position: characterPosition,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: characterFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0f
            );
            _spriteBatch.Draw(
                texture: characterWeapon switch { 
                    CharacterWeapon.Hammer => hammerTexture,
                    CharacterWeapon.Sword => swordTexture,
                    CharacterWeapon.Bow => bowTexture,
                    _ => throw new ArgumentOutOfRangeException()
                },
                position: characterWeaponPosition,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: characterFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: 0f
            );
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
