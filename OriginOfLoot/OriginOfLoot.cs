using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using System;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace OriginOfLoot
{
    public class OriginOfLoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private BoxingViewportAdapter _viewportAdapter;

        Texture2D map;
        Texture2D character;
        Vector2 characterPosition;
        Vector2 characterVelocity;
        Vector2 characterMovement;
        bool characterFacingRight;
        float characterFriction;
        float characterAcceleration;
        float maxCharacterSpeed;
        Texture2D hammer;
        Vector2 hammerPosition;
        Vector2 hammerOffsetLeft = new Vector2(-7, 8);
        Vector2 hammerOffsetRight = new Vector2(7, 8);

        public OriginOfLoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = false;
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 640, 360);
            _camera = new OrthographicCamera(_viewportAdapter);

            characterVelocity = new(0, 0);
            characterAcceleration = 950f;
            characterFriction = 0.0000001f;
            maxCharacterSpeed = 180f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            map = Content.Load<Texture2D>("ase_prod/map");
            character = Content.Load<Texture2D>("ase_prod/character");
            hammer = Content.Load<Texture2D>("ase_prod/hammer");
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
                     Player Movement
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

                characterFacingRight = characterVelocity.X > 0 ? true : false;
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

            // Make the hammer follow the position of the character.
            hammerPosition = characterPosition + (characterFacingRight ? hammerOffsetRight : hammerOffsetLeft);


            /* ==========================
                  Boundary Enforcement
               ========================== */

            // In this section, we currently force the screen boundary, but nothing related to in-game walls.
            int Xmax = _viewportAdapter.VirtualWidth - character.Width;
            int Ymax = _viewportAdapter.VirtualHeight - character.Height;

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
                          TBA
               ========================== */


            base.Update(gameTime);
        }

        // `Draw()` is also called once every frame, right after `Update()`
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Aquamarine);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(map, new Vector2(0, 0), Color.White);
            _spriteBatch.Draw(
                texture: character,
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
                texture: hammer,
                position: hammerPosition,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: characterFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0f
            );
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
