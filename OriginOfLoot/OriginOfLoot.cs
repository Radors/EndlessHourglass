using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;


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
        float characterSpeed;
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
            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 720;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, 640, 360);
            _camera = new OrthographicCamera(_viewportAdapter); 

            characterSpeed = 100f;


            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            map = Content.Load<Texture2D>("MapA25");
            character = Content.Load<Texture2D>("CharacterA8");
            hammer = Content.Load<Texture2D>("HammerA1");
        }

        protected override void Update(GameTime gameTime)
        {
            var kstate = Keyboard.GetState();
            if (kstate.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            // Movement:

            Vector2 movement = Vector2.Zero;

            if (kstate.IsKeyDown(Keys.Right))
            {
                movement.X += 1;
            }
            if (kstate.IsKeyDown(Keys.Left))
            {
                movement.X -= 1;
            }
            if (kstate.IsKeyDown(Keys.Up))
            {
                movement.Y -= 1;
            }
            if (kstate.IsKeyDown(Keys.Down))
            {
                movement.Y += 1;
            }

            if (movement.Length() > 1)
            {
                movement.Normalize();
            }

            Vector2 finalMovement = movement * characterSpeed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            characterPosition += finalMovement;

            // Boundary enforcement:

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

            // Other

            hammerPosition = characterPosition + hammerOffset;

            base.Update(gameTime);
        }

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
