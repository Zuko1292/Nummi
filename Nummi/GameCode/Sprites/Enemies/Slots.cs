using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // TODO create slots enemy do it with the frozen and thawed states 
    public class Slots : SpriteEnemy
    {
        public enum TempState
        {
            Frozen,
            Thawed
        }
        TempState _tempState;
        protected float _lifeDuration;
        protected float _throwTimer = 0f;
        protected float _throwCooldown;
        protected float _goldMoveSpeed;

        bool _throwing = false;

        public Slots(Game1 gameRoot, Vector2 pos, TempState tempState)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slots"), pos, true, 100, 200, 10, false, 0, 400, 70f, 25)
        {
            _tempState = tempState;

            if (_tempState == TempState.Frozen)
            {
                SetAnimation(0);
                _aggrorange = 200f;
                _lifeDuration = 2f;
                _throwCooldown = 3f;
                _goldMoveSpeed = 50f;
                _aggrorange = 200f;
            }
            else
            {
                SetAnimation(2);
                _lifeDuration = 10f;
                _throwCooldown = 1.5f;
                _goldMoveSpeed = 150f;
            }
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 21f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sleeping Frozen
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(64, 224, 32, 32));

            // Thawed
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 32, 32));
            animations[1].Add(new Rectangle(32, 0, 32, 32));
            animations[1].Add(new Rectangle(64, 0, 32, 32));
            animations[1].Add(new Rectangle(96, 0, 32, 32));
            animations[1].Add(new Rectangle(128, 0, 32, 32));
            animations[1].Add(new Rectangle(160, 0, 32, 32));
            animations[1].Add(new Rectangle(192, 0, 32, 32));
            animations[1].Add(new Rectangle(0, 32, 32, 32));
            animations[1].Add(new Rectangle(32, 32, 32, 32));
            animations[1].Add(new Rectangle(64, 32, 32, 32));
            animations[1].Add(new Rectangle(96, 32, 32, 32));
            animations[1].Add(new Rectangle(128, 32, 32, 32));
            animations[1].Add(new Rectangle(160, 32, 32, 32));
            animations[1].Add(new Rectangle(192, 32, 32, 32));
            animations[1].Add(new Rectangle(0, 32, 32, 32));
            animations[1].Add(new Rectangle(32, 64, 32, 32));
            animations[1].Add(new Rectangle(64, 64, 32, 32));
            animations[1].Add(new Rectangle(96, 64, 32, 32));
            animations[1].Add(new Rectangle(128, 64, 32, 32));
            animations[1].Add(new Rectangle(160, 64, 32, 32));
            animations[1].Add(new Rectangle(192, 64, 32, 32));
            animations[1].Add(new Rectangle(0, 96, 32, 32));
            animations[1].Add(new Rectangle(32, 96, 32, 32));
            animations[1].Add(new Rectangle(64, 96, 32, 32));
            animations[1].Add(new Rectangle(96, 96, 32, 32));
            animations[1].Add(new Rectangle(128, 96, 32, 32));
            animations[1].Add(new Rectangle(160, 96, 32, 32));
            animations[1].Add(new Rectangle(192, 96, 32, 32));
            animations[1].Add(new Rectangle(0, 128, 32, 32));
            animations[1].Add(new Rectangle(32, 128, 32, 32));
            animations[1].Add(new Rectangle(64, 128, 32, 32));
            animations[1].Add(new Rectangle(96, 128, 32, 32));
            animations[1].Add(new Rectangle(128, 128, 32, 32));
            animations[1].Add(new Rectangle(160, 128, 32, 32));
            animations[1].Add(new Rectangle(192, 128, 32, 32));
            animations[1].Add(new Rectangle(0, 160, 32, 32));
            animations[1].Add(new Rectangle(32, 160, 32, 32));
            animations[1].Add(new Rectangle(64, 160, 32, 32));
            animations[1].Add(new Rectangle(96, 160, 32, 32));
            animations[1].Add(new Rectangle(128, 160, 32, 32));
            animations[1].Add(new Rectangle(160, 160, 32, 32));
            animations[1].Add(new Rectangle(192, 160, 32, 32));
            animations[1].Add(new Rectangle(0, 192, 32, 32));
            animations[1].Add(new Rectangle(32, 192, 32, 32));
            animations[1].Add(new Rectangle(64, 192, 32, 32));
            animations[1].Add(new Rectangle(96, 192, 32, 32));
            animations[1].Add(new Rectangle(128, 192, 32, 32));
            animations[1].Add(new Rectangle(160, 192, 32, 32));
            animations[1].Add(new Rectangle(192, 192, 32, 32));


            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_isKnockedback) _velocity = Vector2.Zero;
            // Gets distance to player and checks if it should stop throwing
            float distanceToPlayer = Vector2.Distance(_gameRoot._player._position, _position);

            switch (_tempState)
            {
                case TempState.Frozen:
                    if (distanceToPlayer > _aggrorange)
                    {
                        _throwing = false;
                        _throwTimer = 0f;
                    }
                    break;
                case TempState.Thawed:
                    if (distanceToPlayer > _aggrorange)
                    {
                        _throwing = false;
                        _throwTimer = 0f;
                    }
                    break;
            }

            if (!_throwing) _throwTimer += GBL.DeltaTime;

            if (_throwTimer >= _throwCooldown)
            {
                _throwTimer = 0f;
                _throwCooldown = (_tempState == TempState.Frozen) ? 3f : 1.5f; // Reset cooldown based on state
                _throwing = true;
            }

            if (_throwing)
            {
                _throwing = false;
                ThrowChip();
            }
        }


        public void ThrowChip()
        {
            GoldProjectile projectile = new GoldProjectile(_gameRoot, _position, _gameRoot._player._position, _lifeDuration, _goldMoveSpeed);
            _gameRoot._newSpriteList.Add(projectile);
        }
    }

    public class GoldProjectile : SpriteEnemyProjectile
    {
        float _lifeDuration;
        float _lifeTimer = 0f;

        public GoldProjectile(Game1 gameRoot, Vector2 position, Vector2 target, float lifeDuration, float moveSpeed)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Boss Projectile"), position, moveSpeed, 10)
        {
            _velocity = Vector2.Normalize(target - _position) * _moveSpeed;
            _lifeDuration = lifeDuration;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Flying
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 8, 8));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _lifeTimer += GBL.DeltaTime;
            if (_lifeTimer >= _lifeDuration)
            {
                Dead = true;
            }
        }
    }
}
