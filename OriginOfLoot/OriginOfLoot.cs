using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OriginOfLoot.Methods;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Player.PlayerWeapon;
using OriginOfLoot.Types.Projectile;
using OriginOfLoot.Types.Enemy;
using System.Linq;

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
        float timeToNextEnemySpawn;

        Player player;
        Texture2D mapTexture;
        Texture2D playerSpawnTexture;
        Texture2D redRangedTexture;

        Texture2D swordTexture;
        Texture2D staffTexture;
        Texture2D swordProjectileTexture;
        Texture2D staffProjectileTexture;
        List<IActiveEnemy> activeEnemies = new();
        List<StaffProjectile> staffProjectiles = new();
        List<SwordProjectile> swordProjectiles = new();
        List<Rectangle> swordProjectileRectangles = new();
        int swordProjectileFrames;


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

            player = new Player();

            swordProjectileFrames = 13;
            for (int i = 0; i < swordProjectileFrames; i++)
            {
                swordProjectileRectangles.Add(
                    new Rectangle(new Point(i * 16, 0), new Point(16, 16))
                );
            }
            timeToNextEnemySpawn = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mapTexture = Content.Load<Texture2D>("ase_prod/map");
            playerSpawnTexture = Content.Load<Texture2D>("ase_prod/player");
            staffTexture = Content.Load<Texture2D>("ase_prod/staff");
            staffProjectileTexture = Content.Load<Texture2D>("ase_prod/staffProjectile");
            swordTexture = Content.Load<Texture2D>("ase_prod/sword");
            swordProjectileTexture = Content.Load<Texture2D>("ase_prod/swordProjectile");
            redRangedTexture = Content.Load<Texture2D>("ase_prod/redRanged");

            player.Texture = playerSpawnTexture;
        }

        // `Update()` is called once every frame.
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            MouseState mstate = Mouse.GetState();
            Vector2 pointerPos = _camera.ScreenToWorld(mstate.X, mstate.Y);
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
                playerInputDirection.Normalize();

                player.Velocity = playerInputDirection * player.Speed;
            }
            else
            {
                player.Velocity = new Vector2(0, 0);
            }

            // Update position
            player.Position += player.Velocity * deltaTime;

            // Determine player facing direction
            if (pointerPos.X > player.Position.X + viewTileWidth / 2)
            {
                player.FacingRight = true;
            }
            else
            {
                player.FacingRight = false;
            }

            /* =======================================
                    Boundary Enforcement: Player
               ======================================= */

            // In this section, we currently force the screen boundary, but nothing related to in-game walls.
            int Xmax = _viewportAdapter.VirtualWidth - player.Texture.Width;
            int Ymax = _viewportAdapter.VirtualHeight - player.Texture.Height;

            if (player.Position.X > Xmax)
            {
                player.Position = new Vector2(Xmax, player.Position.Y);
            }
            else if (player.Position.X < 0)
            {
                player.Position = new Vector2(0, player.Position.Y);
            }
            if (player.Position.Y > Ymax)
            {
                player.Position = new Vector2(player.Position.X, Ymax);
            }
            else if (player.Position.Y < 0)
            {
                player.Position = new Vector2(player.Position.X, 0);
            }

            /* =======================================
                  Player Weapon: Equip and Position
               ======================================= */

            if (kstate.IsKeyDown(Keys.NumPad1))
            {
                player.Weapon = new Sword();
            }
            if (kstate.IsKeyDown(Keys.NumPad2))
            {
                player.Weapon = new Staff();
            }

            /* =======================================
                      Player Weapon Activation
               ======================================= */

            if (mstate.LeftButton == ButtonState.Pressed &&
                player.Weapon.TimeSinceFired > player.Weapon.FireRate)
            {
                var position = player.Position + player.CurrentProjectileSpawnOffset();

                var direction = new Vector2(0, 0);

                if (player.Weapon is Sword)
                {
                    direction = new Vector2(pointerPos.X - (position.X + 8),
                                            pointerPos.Y - (position.Y + 8));
                    var projectile = new SwordProjectile(swordProjectileTexture, position, direction);
                    swordProjectiles.Add(projectile);
                }
                else if (player.Weapon is Staff)
                {
                    direction = new Vector2(pointerPos.X - position.X,
                                            pointerPos.Y - position.Y);
                    var projectile = new StaffProjectile(staffProjectileTexture, position, direction);
                    staffProjectiles.Add(projectile);
                }

                player.Weapon.TimeSinceFired = 0;  
            }
            else
            {
                player.Weapon.TimeSinceFired += deltaTime;
            }

            // Update position of Staff projectiles
            foreach (var projectile in staffProjectiles)
            {
                projectile.Position += projectile.Velocity * deltaTime;
                projectile.Rectangle = Geometry.NewRectangle(projectile.Position, projectile.Texture);
            }

            // Update position of Sword projectiles, and progress animation
            foreach (var projectile in swordProjectiles)
            {
                projectile.Position += projectile.Velocity * deltaTime;
                projectile.Rectangle = Geometry.NewRectangle(projectile.Position, projectile.Texture);

                projectile.TimeAlive += deltaTime;

                float timePerFrame = projectile.Lifetime / swordProjectileRectangles.Count;
                projectile.CurrentFrame = Math.Min(
                    (int)Math.Floor(projectile.TimeAlive / timePerFrame),
                    swordProjectileRectangles.Count - 1);
            }


            /* =============================
                     Remove Projectiles
               ============================= */

            staffProjectiles.RemoveAll(n =>
                n.Position.X < 0 || 
                n.Position.Y < 0 ||
                n.Position.X > viewPixelsX ||
                n.Position.Y > viewPixelsY
            );
            swordProjectiles.RemoveAll(n =>
                n.TimeAlive > n.Lifetime
            );

            /* =============================
                     Enemy Creation
               ============================= */

            if (timeToNextEnemySpawn <= 0)
            {
                var spawnPosition = new Vector2(0, viewPixelsY / 2);
                var direction = new Vector2(1, 0);
                direction.Normalize();

                var redRanged = new RedRanged(
                                    redRangedTexture,
                                    spawnPosition,
                                    direction * 50f,
                                    Geometry.NewRectangle(spawnPosition, redRangedTexture)
                                );

                activeEnemies.Add(redRanged);
                timeToNextEnemySpawn = 1;
            }
            else
            {
                timeToNextEnemySpawn -= deltaTime;
            }

            /* =============================
                     Enemy Movement
               ============================= */

            foreach (var enemy in activeEnemies)
            {
                enemy.Position += enemy.Velocity * deltaTime;
                enemy.Rectangle = Geometry.NewRectangle(enemy.Position, enemy.Texture);
            }

            /* =============================
                     Enemy Screen Boundary
               ============================= */

            activeEnemies.RemoveAll(n =>
                n.Position.X < 0 ||
                n.Position.Y < 0 ||
                n.Position.X > viewPixelsX ||
                n.Position.Y > viewPixelsY
            );


            /* ===================================
                 Enemy and Projectile Collision
               =================================== */

            activeEnemies.RemoveAll(
                    enemy => swordProjectiles.Any(proj => proj.Rectangle.Intersects(enemy.Rectangle)) ||
                             staffProjectiles.Any(proj => proj.Rectangle.Intersects(enemy.Rectangle))
                          );


            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GraphicsDevice.Clear(Color.Aquamarine);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), 
                               samplerState: SamplerState.PointClamp,
                               sortMode: SpriteSortMode.BackToFront,
                               blendState: BlendState.NonPremultiplied);
            _spriteBatch.Draw(
                texture: mapTexture,
                position: default,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: 1f
            );

            _spriteBatch.Draw(
                texture: player.Texture,
                position: player.Position,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: player.FacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0.5f
            );
            _spriteBatch.Draw(
                texture: player.Weapon switch { 
                    Sword => swordTexture,
                    Staff => staffTexture,
                    _ => throw new ArgumentOutOfRangeException()
                },
                position: player.Position + player.CurrentWeaponOffset(),
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: 0.49f
            );
            foreach (var projectile in staffProjectiles)
            {
                for (int i = 0; i < 3; i++)
                {
                    _spriteBatch.Draw(
                        texture: staffProjectileTexture,
                        position: projectile.Position + i * (projectile.Velocity * deltaTime),
                        sourceRectangle: default,
                        color: Color.White,
                        rotation: 0f,
                        origin: default,
                        scale: 1f,
                        effects: default,
                        layerDepth: 0.48f
                    );
                }
            }
            foreach (var projectile in swordProjectiles)
            {
                _spriteBatch.Draw(
                    texture: swordProjectileTexture,
                    position: projectile.Position,
                    sourceRectangle: swordProjectileRectangles[projectile.CurrentFrame],
                    color: Color.White,
                    rotation: 0f,
                    origin: default,
                    scale: 1f,
                    effects: player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    layerDepth: 0.48f
                );
            }
            foreach (var enemy in activeEnemies)
            {
                _spriteBatch.Draw(
                    texture: enemy.Texture,
                    position: enemy.Position,
                    sourceRectangle: default,
                    color: Color.White,
                    rotation: 0f,
                    origin: default,
                    scale: 1f,
                    effects: default,
                    layerDepth: 0.49f
                );
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
