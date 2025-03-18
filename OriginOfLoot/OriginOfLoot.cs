using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended.ViewportAdapters;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using OriginOfLoot.Types;
using System.Diagnostics;
using OriginOfLoot.Types.Enemy;
using OriginOfLoot.Types.Projectile;
using OriginOfLoot.Types.Weapon;
using OriginOfLoot.Types.Player;

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

        Texture2D mapTexture;
        Texture2D redRangedTexture;
        List<IActiveEnemy> activeEnemies = new();
        List<StaffProjectile> staffProjectiles = new();
        List<SwordProjectile> swordProjectiles = new();
        List<Rectangle> swordProjectileAnimationRectangles = new();

        IWeapon playerWeapon;
        Vector2 playerPosition;
        Vector2 playerVelocity;
        Vector2 playerMovement;
        Vector2 playerWeaponPosition;
        bool playerFacingRight;
        Texture2D playerTexture;
        float timeAfterWeaponActivation;
        float currentPlayerFireRate;
        float playerSpeed;
        Vector2 currentWeaponOffset;

        Texture2D staffTexture;
        Texture2D staffProjectileTexture;
        Vector2 staffOffset;
        Vector2 staffProjectileOffset;
        float staffProjectileSpeed;
        float staffFireRate;

        Texture2D swordTexture;
        Texture2D swordProjectileTexture;
        Vector2 swordOffset;
        float swordProjectileSpeed;
        float swordProjectileLifetime;
        float swordFireRate;



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
            playerSpeed = 180f;
            staffOffset = new Vector2(7, 8);
            swordOffset = new Vector2(12, 19);
            playerWeapon = new Staff();
            staffProjectileSpeed = 250f;
            swordProjectileSpeed = 350f;
            staffProjectileOffset = new Vector2(19, 8);
            staffFireRate = 0.25f;
            swordFireRate = 0.30f;
            swordProjectileLifetime = 0.40f;
            currentPlayerFireRate = swordFireRate;
            for (int i = 0; i < 13; i++)
            {
                swordProjectileAnimationRectangles.Add(
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
            playerTexture = Content.Load<Texture2D>("ase_prod/player");
            staffTexture = Content.Load<Texture2D>("ase_prod/staff");
            staffProjectileTexture = Content.Load<Texture2D>("ase_prod/staffProjectile");
            swordTexture = Content.Load<Texture2D>("ase_prod/sword");
            swordProjectileTexture = Content.Load<Texture2D>("ase_prod/swordProjectile");
            redRangedTexture = Content.Load<Texture2D>("ase_prod/redRanged");
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

                playerVelocity = playerInputDirection * playerSpeed;
            }
            else
            {
                playerVelocity.X = 0;
                playerVelocity.Y = 0;
            }

            // Ensure that we are not breaking speed limits in any direction
            if (playerVelocity.LengthSquared() > playerSpeed * playerSpeed)
            {
                playerVelocity = Vector2.Normalize(playerVelocity) * playerSpeed;
            }

            // Compute final movement vector and add it to current position vector
            playerMovement = playerVelocity * deltaTime;
            playerPosition += playerMovement;

            // Determine player facing direction
            if (pointerPos.X > playerPosition.X + viewTileWidth / 2)
            {
                playerFacingRight = true;
            }
            else
            {
                playerFacingRight = false;
            }

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
                playerWeapon = new Sword();
                currentPlayerFireRate = swordFireRate;
            }
            if (kstate.IsKeyDown(Keys.NumPad2))
            {
                playerWeapon = new Staff();
                currentPlayerFireRate = staffFireRate;
            }

            currentWeaponOffset = (playerFacingRight, playerWeapon) switch
            {
                (true, Sword) => swordOffset,
                (true, Staff) => staffOffset,
                (false, Sword) => new Vector2(viewTileWidth - swordOffset.X, swordOffset.Y),
                (false, Staff) => new Vector2(-staffOffset.X, staffOffset.Y),
                _ => throw new ArgumentOutOfRangeException()
            };

            playerWeaponPosition = playerPosition + currentWeaponOffset;

            /* =======================================
                      Player Weapon Activation
               ======================================= */

            // Activate Staff
            if (playerWeapon is Staff &&
                mstate.LeftButton == ButtonState.Pressed && 
                timeAfterWeaponActivation > currentPlayerFireRate)
            {
                Vector2 currentProjectileOffset = playerFacingRight ? 
                    staffProjectileOffset : 
                    new Vector2(viewTileWidth - staffProjectileTexture.Width - staffProjectileOffset.X, 
                                staffProjectileOffset.Y);
                var projectilePosition = playerPosition + currentProjectileOffset;
                var projectileDirection = new Vector2(pointerPos.X - projectilePosition.X, 
                                                      pointerPos.Y - projectilePosition.Y);
                projectileDirection.Normalize();
                var projectileVelocity = projectileDirection * staffProjectileSpeed;

                var newProjectile = new StaffProjectile(
                                            staffProjectileTexture,
                                            projectilePosition,
                                            projectileVelocity,
                                            new Rectangle(
                                                (int)projectilePosition.X,
                                                (int)projectilePosition.Y,
                                                staffProjectileTexture.Width,
                                                staffProjectileTexture.Height
                                            )
                                        );

                staffProjectiles.Add(newProjectile);
                timeAfterWeaponActivation = 0;
            }
            // Activate Sword
            else if (playerWeapon is Sword &&
                     mstate.LeftButton == ButtonState.Pressed &&
                     timeAfterWeaponActivation > currentPlayerFireRate)
            {
                var projectilePosition = playerPosition + currentWeaponOffset;

                var projectileDirection = new Vector2(pointerPos.X - projectilePosition.X,
                                                      pointerPos.Y - projectilePosition.Y);
                projectileDirection.Normalize();
                var projectileVelocity = projectileDirection * swordProjectileSpeed;

                var newProjectile = new SwordProjectile(
                                           swordProjectileTexture,
                                           projectilePosition,
                                           projectileVelocity,
                                           new Rectangle(
                                               (int)projectilePosition.X,
                                               (int)projectilePosition.Y,
                                               swordProjectileTexture.Width,
                                               swordProjectileTexture.Height
                                           ),
                                           0,
                                           0
                                       );

                swordProjectiles.Add(newProjectile);
                timeAfterWeaponActivation = 0;
            }
            else
            {
                timeAfterWeaponActivation += deltaTime;
            }

            // Update position of Staff projectiles
            foreach (var projectile in staffProjectiles)
            {
                projectile.Position += projectile.Velocity * deltaTime;
                projectile.Rectangle = new Rectangle(
                            (int)projectile.Position.X,
                            (int)projectile.Position.Y,
                            staffProjectileTexture.Width,
                            staffProjectileTexture.Height
                        );
            }

            // Update position of Sword projectiles, and progress animation
            foreach (var projectile in swordProjectiles)
            {
                projectile.Position += projectile.Velocity * deltaTime;
                projectile.Rectangle = new Rectangle(
                            (int)projectile.Position.X,
                            (int)projectile.Position.Y,
                            swordProjectileTexture.Width,
                            swordProjectileTexture.Height
                        );

                projectile.TimeAlive += deltaTime;

                float timePerFrame = swordProjectileLifetime / swordProjectileAnimationRectangles.Count;
                projectile.CurrentFrame = Math.Min(
                    (int)Math.Floor(projectile.TimeAlive / timePerFrame), 
                    swordProjectileAnimationRectangles.Count - 1);
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
                n.TimeAlive > swordProjectileLifetime
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
                                    new Rectangle(
                                        (int)spawnPosition.X,
                                        (int)spawnPosition.Y,
                                        swordProjectileTexture.Width,
                                        swordProjectileTexture.Height
                                    )
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
                enemy.Rectangle = new Rectangle(
                            (int)enemy.Position.X,
                            (int)enemy.Position.Y,
                            enemy.Texture.Width,
                            enemy.Texture.Height
                        );
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


            var removeIndexes = new List<int>();
            for (int i = 0; i < activeEnemies.Count; i++)
            {
                foreach (var projectile in staffProjectiles)
                {
                    if (activeEnemies[i].Rectangle.Intersects(projectile.Rectangle))
                    {
                        removeIndexes.Add(i);
                    }
                }
                foreach (var projectile in swordProjectiles)
                {
                    if (activeEnemies[i].Rectangle.Intersects(projectile.Rectangle))
                    {
                        removeIndexes.Add(i);
                    }
                }
            }
            foreach (var index in removeIndexes)
            {
                activeEnemies.RemoveAt(index);
            }


            base.Update(gameTime);
        }

        // `Draw()` is also called once every frame, right after `Update()`
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
                texture: playerTexture,
                position: playerPosition,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: playerFacingRight ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                layerDepth: 0.5f
            );
            _spriteBatch.Draw(
                texture: playerWeapon switch { 
                    Sword => swordTexture,
                    Staff => staffTexture,
                    _ => throw new ArgumentOutOfRangeException()
                },
                position: playerWeaponPosition,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: playerWeapon switch
                {
                    Sword => new Vector2(8, 8),
                    _ => default,
                },
                scale: 1f,
                effects: playerFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
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

                // int fadeToBlack = 255 - (int)(170f / swordProjectileLifetime * projectile.TimeAlive);
                // Color currentColor = new Color(fadeToBlack, fadeToBlack, fadeToBlack, 150);
                // Color solidifyAtEnd = new Color(255, 255, 255, 200);
                _spriteBatch.Draw(
                    texture: swordProjectileTexture,
                    position: projectile.Position,
                    sourceRectangle: swordProjectileAnimationRectangles[projectile.CurrentFrame],
                    color: Color.White,
                    rotation: 0f,
                    origin: new Vector2(8, 8),
                    scale: 1f,
                    effects: playerFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally,
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
