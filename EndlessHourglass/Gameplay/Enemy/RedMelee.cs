﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EndlessHourglass.Gameplay.Interfaces;
using EndlessHourglass.Gameplay.Player;
using EndlessHourglass.Gameplay.Static;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace EndlessHourglass.Gameplay.Enemy
{
    public class RedMelee : IEnemy
    {
        private Texture2D _texture = TextureStore.RedMelee;
        private readonly Vector2 _healthBarOffset = new Vector2(0, 32);
        private Vector2 _velocity;
        private float _speed = 55f;
        private readonly ActivePlayer _player;

        public Vector2 Position { get; private set; }
        public Rectangle Rectangle { get; private set; }
        public int Damage { get; } = 40;
        public int MaxHealth { get; } = 140;
        public int CurrentHealth { get; set; }

        public RedMelee(Vector2 position, Vector2 direction, ActivePlayer player)
        {
            Position = position;
            _velocity = direction * _speed;
            Rectangle = Geometry.NewRectangle(position, _texture);
            CurrentHealth = MaxHealth;
            _player = player;
        }

        public int HealthBarIndex()
        {
            int frame = (int)(CurrentHealth / (MaxHealth/14f));
            return ConstConfig.StandardHealthBarTotalFrames - frame - 1;
        }

        public void Update(float deltaTime)
        {
            var direction = Geometry.Direction(Position, _player.Position);
            _velocity = direction * _speed;
            Position += _velocity * deltaTime;
            Rectangle = Geometry.NewRectangle(Position, _texture);
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
                sourceRectangle: TextureStore.HealthBarRedRectangles[Math.Clamp(HealthBarIndex(), 0, TextureStore.HealthBarRedRectangles.Count-1)],
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
