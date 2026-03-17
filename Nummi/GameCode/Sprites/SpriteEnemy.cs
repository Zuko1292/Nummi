using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Nummi;

namespace Nummi
{
    public class SpriteEnemy : SpriteCharacter
    {
        protected float _moveSpeed = 3f;
        public int _health;

        private Vector2 Direction;

        public override bool Dead
        {
            set
            {
                _dead = value;
            }

            get
            {
                return _dead;
            }
        }

        public SpriteEnemy(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove, int health)
            : base(gameRoot, texture, position, canMove)
        {
            CollisionLayer = CollisionLayer.Enemy;
            CollisionMask = CollisionLayer.All & ~CollisionLayer.Enemy;
            InitBounds(position, true, new Vector2(1f,1f));
            _layerDepth = 0.1f;
            _health = health;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();
            // Idle animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 32));
            animations[0].Add(new Rectangle(32, 0, 32, 32));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            Direction = _gameRoot._player._position - _position;

            if (Direction.LengthSquared() > 0f)
            {
                Direction.Normalize();
            }

            _velocity = Direction * _moveSpeed * GBL.DeltaTime;

            base.Update(gameTime);
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
            if (_health <= 0)
            {
                Dead = true;
            }
        }
    }
}
