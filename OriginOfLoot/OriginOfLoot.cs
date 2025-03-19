using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using OriginOfLoot.StaticMethods; 
using OriginOfLoot.Types.Player; 
using OriginOfLoot.Types.Player.PlayerWeapon; 
using OriginOfLoot.Types.Projectile; 
using OriginOfLoot.Types.Enemy; 
using System.Linq;
using System.Transactions;

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
        int viewTileStandard;
        float entityStandardDepth;
        float timeToNextEnemySpawn;

        Player player;

        Texture2D mapTexture;
        Texture2D playerTexture;
        Texture2D redRangedTexture;
        Texture2D healthbarTexture;
        Texture2D swordTexture;
        Texture2D staffTexture;
        Texture2D swordProjectileTexture;
        Texture2D staffProjectileTexture;

        List<IActiveEnemy> activeEnemies = new();
        List<StaffProjectile> staffProjectiles = new();
        List<SwordProjectile> swordProjectiles = new();
        List<Rectangle> swordProjectileRectangles = new();
        List<Rectangle> healthbarRectangles = new();


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
            viewTileStandard = 16;
            entityStandardDepth = 0.5f;
            _viewportAdapter = new BoxingViewportAdapter(Window, GraphicsDevice, viewPixelsX, viewPixelsY);
            _camera = new OrthographicCamera(_viewportAdapter);

            player = new Player();

            timeToNextEnemySpawn = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            mapTexture = Content.Load<Texture2D>("ase_prod/map");
            playerTexture = Content.Load<Texture2D>("ase_prod/player");
            swordTexture = Content.Load<Texture2D>("ase_prod/sword");
            staffTexture = Content.Load<Texture2D>("ase_prod/staff");
            swordProjectileTexture = Content.Load<Texture2D>("ase_prod/swordProjectile");
            staffProjectileTexture = Content.Load<Texture2D>("ase_prod/staffProjectile");
            redRangedTexture = Content.Load<Texture2D>("ase_prod/redRanged");
            healthbarTexture = Content.Load<Texture2D>("ase_prod/healthbar");

            swordProjectileRectangles = Geometry.SetupAnimationRectangles(swordProjectileTexture, viewTileStandard, viewTileStandard);
            healthbarRectangles = Geometry.SetupAnimationRectangles(healthbarTexture, viewTileStandard, 4);
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
            if (pointerPos.X > player.Position.X + viewTileStandard / 2)
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
            int Xmax = _viewportAdapter.VirtualWidth - playerTexture.Width;
            int Ymax = _viewportAdapter.VirtualHeight - playerTexture.Height;

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
                var position = player.Position + player.ProjectileOffset();

                var direction = new Vector2(pointerPos.X - (position.X + player.ProjectileDirectionOffset().X),
                                            pointerPos.Y - (position.Y + player.ProjectileDirectionOffset().Y));

                if (player.Weapon is Sword)
                {
                    var projectile = new SwordProjectile(swordProjectileTexture, position, direction);
                    swordProjectiles.Add(projectile);
                }
                else if (player.Weapon is Staff)
                {
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
                var redRanged = new RedRanged(
                                    redRangedTexture,
                                    new Vector2(0, viewPixelsY / 2),
                                    new Vector2(1, 0)
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

            foreach (var enemy in activeEnemies)
            {
                // Sword
                foreach (var swordProjectile in swordProjectiles)
                {
                    if (swordProjectile.Rectangle.Intersects(enemy.Rectangle))
                    {
                        enemy.CurrentHealth -= new Sword().Damage;
                        swordProjectile.RemainingHits -= 1;
                    }
                }
                // Staff
                foreach (var staffProjectile in staffProjectiles)
                {
                    if (staffProjectile.Rectangle.Intersects(enemy.Rectangle))
                    {
                        enemy.CurrentHealth -= new Staff().Damage;
                        staffProjectile.RemainingHits -= 1;
                    }
                }
            }

            activeEnemies.RemoveAll(n => n.CurrentHealth <= 0);
            swordProjectiles.RemoveAll(n => n.RemainingHits <= 0);
            staffProjectiles.RemoveAll(n => n.RemainingHits <= 0);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GraphicsDevice.Clear(Color.Aquamarine);

            _spriteBatch.Begin(transformMatrix: _camera.GetViewMatrix(), 
                               samplerState: SamplerState.PointClamp,
                               sortMode: SpriteSortMode.FrontToBack,
                               blendState: BlendState.NonPremultiplied);
            // Map
            _spriteBatch.Draw(
                texture: mapTexture,
                position: default,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: 0f
            );
           
            // Player
            _spriteBatch.Draw(
                texture: playerTexture,
                position: player.Position,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: player.FacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: entityStandardDepth + (player.Position.Y / 100000)
            );
            // Player Weapon
            _spriteBatch.Draw(
                texture: player.Weapon switch { 
                    Sword => swordTexture,
                    Staff => staffTexture,
                    _ => throw new ArgumentOutOfRangeException()
                },
                position: player.Position + player.WeaponOffset(),
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                layerDepth: entityStandardDepth + (player.Position.Y / 100000) + 0.000001f
            );
            foreach (var projectile in staffProjectiles)
            {
                for (int i = 0; i < 3; i++)
                {
                    // Staff Projectile
                    _spriteBatch.Draw(
                        texture: staffProjectileTexture,
                        position: projectile.Position + i * (projectile.Velocity * deltaTime),
                        sourceRectangle: default,
                        color: Color.White,
                        rotation: 0f,
                        origin: default,
                        scale: 1f,
                        effects: player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                        layerDepth: entityStandardDepth + (player.Position.Y / 100000) + 0.000002f
                    );
                }
            }
            foreach (var projectile in swordProjectiles)
            {
                // Sword Projectile
                _spriteBatch.Draw(
                    texture: swordProjectileTexture,
                    position: projectile.Position,
                    sourceRectangle: swordProjectileRectangles[projectile.CurrentFrame],
                    color: Color.White,
                    rotation: 0f,
                    origin: default,
                    scale: 1f,
                    effects: player.FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
                    layerDepth: entityStandardDepth + (player.Position.Y / 100000) + 0.000002f
                );
            }
            foreach (var enemy in activeEnemies)
            {
                // Enemy
                _spriteBatch.Draw(
                    texture: enemy.Texture,
                    position: enemy.Position,
                    sourceRectangle: default,
                    color: Color.White,
                    rotation: 0f,
                    origin: default,
                    scale: 1f,
                    effects: default,
                    layerDepth: entityStandardDepth + (enemy.Position.Y / 100000)
                );
                // Enemy Healthbar
                _spriteBatch.Draw(
                    texture: healthbarTexture,
                    position: enemy.Position + enemy.HealthbarOffset,
                    sourceRectangle: healthbarRectangles[enemy.HealthbarFrame()],
                    color: Color.White,
                    rotation: 0f,
                    origin: default,
                    scale: 1f,
                    effects: default,
                    layerDepth: entityStandardDepth + (enemy.Position.Y / 100000)
                );
            }
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
