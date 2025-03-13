﻿using Microsoft.Xna.Framework;
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
        float characterFriction;
        float characterAcceleration;
        float maxCharacterSpeed;
        Texture2D hammer;
        Vector2 hammerPosition;
        Vector2 hammerOffset = new Vector2(-7, 8);

        public OriginOfLoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = false;
            _graphics.PreferredBackBufferWidth = 1920;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 640, 360);
            _camera = new OrthographicCamera(_viewportAdapter);

            // Velocity is both Direction and Speed (Direction * Acceleration * deltaTime)
            characterVelocity = new(0, 0);
            characterAcceleration = 950f;
            characterFriction = 0.0000001f;
            maxCharacterSpeed = 180f;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            map = Content.Load<Texture2D>("map");
            character = Content.Load<Texture2D>("character");
            hammer = Content.Load<Texture2D>("hammer");
        }

        // `Update()` is called once every frame.
        protected override void Update(GameTime gameTime)
        {
            KeyboardState kstate = Keyboard.GetState(); // `kstate` knows about all keys pressed in this particular frame
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (kstate.IsKeyDown(Keys.Escape))
            {
                Exit(); // Allows to exit the game with escape
            }

            /* ==========================
                     Player Movement
               ========================== */

            Vector2 characterDirection = Vector2.Zero; // We use a `Vector2` to capture direction

            if (kstate.IsKeyDown(Keys.Right) || kstate.IsKeyDown(Keys.D))
            {
                characterDirection.X += 1;
            }
            if (kstate.IsKeyDown(Keys.Left) || kstate.IsKeyDown(Keys.A))
            {
                characterDirection.X -= 1;
            }
            if (kstate.IsKeyDown(Keys.Up) || kstate.IsKeyDown(Keys.W))
            {
                characterDirection.Y -= 1;
            }
            if (kstate.IsKeyDown(Keys.Down) || kstate.IsKeyDown(Keys.S))
            {
                characterDirection.Y += 1;
            }

            if (characterDirection != Vector2.Zero) // The player inputs keys to actively move
            {
                // In general, we always normalize the direction vector, into a "Unit vector".
                // This turns a diagonal (1, -1) into (0.707, -0.707) and length == 1.
                characterDirection.Normalize();

                // Velocity += Direction *  (Acceleration * Time) <- Speed 
                characterVelocity += characterDirection * characterAcceleration * deltaTime;
            }
            if (characterDirection.X == 0) // The player has no input direction horizontally
            {
                characterVelocity.X *= MathF.Pow(characterFriction, deltaTime); // Apply friction X
            }
            if (characterDirection.Y == 0) // The player has no input direction vertically
            {
                characterVelocity.Y *= MathF.Pow(characterFriction, deltaTime); // Apply friction Y
            }

            // Ensure that we are not breaking speed limits in any direction
            if (characterVelocity.LengthSquared() > maxCharacterSpeed * maxCharacterSpeed)
            {
                characterVelocity = Vector2.Normalize(characterVelocity) * maxCharacterSpeed;
            }


            // Compute the final `characterMovement`
            Vector2 characterMovement = characterVelocity * deltaTime;
            // Add `characterMovement` to position
            characterPosition += characterMovement;


            // Make the hammer follow the position of the character.
            hammerPosition = characterPosition + hammerOffset;


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
            _spriteBatch.Draw(character, characterPosition, Color.White);
            _spriteBatch.Draw(hammer, hammerPosition, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
