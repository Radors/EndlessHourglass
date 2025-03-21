using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Projectile;
using OriginOfLoot.Types.Enemy;
using System.Linq;
using System.Transactions;
using OriginOfLoot.Types.Static;

namespace OriginOfLoot
{
    public class OriginOfLoot : Game
    {
        private GraphicsDeviceManager _graphics;
        private OrthographicCamera _camera;
        private SpriteBatch _spriteBatch;
        private BoxingViewportAdapter _viewportAdapter;

        private ActivePlayer _player;
        private EnemyManager _enemyManager;
        private ProjectileManager _projectileManager;
        private InputManager _inputManager;

        public OriginOfLoot()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Window.AllowUserResizing = false;
            DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;
            _graphics.PreferredBackBufferWidth = displayMode.Width;
            _graphics.PreferredBackBufferHeight = displayMode.Height;
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();
            IsFixedTimeStep = false;

            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, ConstConfig.ViewPixelsX, ConstConfig.ViewPixelsY);
            _camera = new OrthographicCamera(_viewportAdapter);

            _player = new ActivePlayer();
            _enemyManager = new EnemyManager(_player);
            _projectileManager = new ProjectileManager(_player, _enemyManager);
            _inputManager = new InputManager(_player, _projectileManager, _enemyManager);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            TextureStore.Setup(Content);
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            KeyboardState kstate = Keyboard.GetState();
            MouseState mstate = Mouse.GetState();
            Vector2 pointerPos = _camera.ScreenToWorld(mstate.X, mstate.Y);
            if (kstate.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            _inputManager.Update(deltaTime, kstate, mstate, pointerPos);
            _player.Update(deltaTime);
            _projectileManager.Update(deltaTime, pointerPos);
            _enemyManager.Update(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GraphicsDevice.Clear(Color.DarkSlateGray);
            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), 
                               samplerState: SamplerState.PointClamp,
                               sortMode: SpriteSortMode.FrontToBack,
                               blendState: BlendState.NonPremultiplied);
            _spriteBatch.Draw(
                texture: TextureStore.Map,
                position: default,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: 0f
            );
            
            _player.Draw(_spriteBatch);
            _projectileManager.Draw(_spriteBatch);
            _enemyManager.Draw(_spriteBatch);
            
            _spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
