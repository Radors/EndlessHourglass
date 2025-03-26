using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OriginOfLoot.Types.Interfaces;
using OriginOfLoot.Types.Player;
using OriginOfLoot.Types.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace OriginOfLoot.Types.Enemy
{
    public class RedRanged : IEnemy
    {
        private Texture2D _texture = TextureStore.RedRanged;
        private readonly Vector2 _healthBarOffset = new Vector2(0, 32);
        private Vector2 _velocity;
        private float _speed = 45f;
        private float _fireRate = 3f;
        private float _timeSinceFired = 1f;
        private readonly ActivePlayer _player;
        private readonly EnemyManager _enemyManager;

        public Vector2 Position { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public int Damage { get; } = 40;
        public int MaxHealth { get; } = 140;
        public int CurrentHealth { get; set; }

        public RedRanged(Vector2 position, Vector2 direction, ActivePlayer player, EnemyManager enemyManager)
        {
            Position = position;
            _velocity = direction * _speed;
            Rectangle = Geometry.NewRectangle(position, _texture);
            CurrentHealth = MaxHealth;
            _enemyManager = enemyManager;
            _player = player;
        }

        public int HealthBarIndex()
        {
            int frame = (int)(CurrentHealth / (MaxHealth / 14f));
            return ConstConfig.StandardHealthBarTotalFrames - frame - 1;
        }

        public void Update(float deltaTime)
        {
            var direction = Geometry.Direction(Position, _player.Position);
            _velocity = direction * _speed;
            Position += _velocity * deltaTime;
            Rectangle = Geometry.NewRectangle(Position, _texture);

            if (_timeSinceFired > _fireRate)
            {
                _enemyManager.NewEnemyProjectile(this);
                _timeSinceFired = 0;
            }
            else
            {
                _timeSinceFired += deltaTime;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                texture: _texture,
                position: Position,
                sourceRectangle: default,
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision)
            );
            spriteBatch.Draw(
                texture: TextureStore.HealthBarRed,
                position: Position + _healthBarOffset,
                sourceRectangle: TextureStore.HealthBarRedRectangles[Math.Clamp(HealthBarIndex(), 0, TextureStore.HealthBarRedRectangles.Count - 1)],
                color: Color.White,
                rotation: 0f,
                origin: default,
                scale: 1f,
                effects: default,
                layerDepth: ConstConfig.StandardDepth + (Position.Y / ConstConfig.StandardDepthDivision)
            );
        }
    }
}
