using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{

    public class Dealer : SpriteEnemy
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
        protected float _chipMoveSpeed;

        bool _throwing = false;

        public Dealer(Game1 gameRoot, Vector2 position, TempState tempState, int health = 200, int knockbackStrength = 220, int damageStrength = 10, float xpValue = 70f)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Snake Dealer"), position, false, health, knockbackStrength, damageStrength, false, 0, 400f, xpValue, 25)
        {
            _tempState = tempState;

            if(_tempState == TempState.Frozen)
            {
                SetAnimation(0);
                _lifeDuration = 2f;
                _throwCooldown = 3f;
                _chipMoveSpeed = 50f;
                _aggrorange = 200f;
                _frameDuration = 1f / 4f;
                _canMove = true;
            }
            else
            {
                _frameDuration = 1f / 8f;
                SetAnimation(2);
                _lifeDuration = 10f;
                _throwCooldown = 1.5f;
                _chipMoveSpeed = 150f;
            }

            _canPatrol = true;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sleeping Frozen
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 64));
            animations[0].Add(new Rectangle(32, 0, 32, 64));
            animations[0].Add(new Rectangle(64, 0, 32, 64));
            animations[0].Add(new Rectangle(96, 0, 32, 64));
            animations[0].Add(new Rectangle(128, 0, 32, 64));
            animations[0].Add(new Rectangle(160, 0, 32, 64));
            animations[0].Add(new Rectangle(192, 0, 32, 64));
            animations[0].Add(new Rectangle(224, 0, 32, 64));
            animations[0].Add(new Rectangle(256, 0, 32, 64));
            animations[0].Add(new Rectangle(288, 0, 32, 64));
            animations[0].Add(new Rectangle(320, 0, 32, 64));
            animations[0].Add(new Rectangle(352, 0, 32, 64));
            animations[0].Add(new Rectangle(384, 0, 32, 64));
            animations[0].Add(new Rectangle(416, 0, 32, 64));

            // Throwing Frozen
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 64, 32, 64));
            animations[1].Add(new Rectangle(32, 64, 32, 64));
            animations[1].Add(new Rectangle(64, 64, 32, 64));
            animations[1].Add(new Rectangle(96, 64, 32, 64));
            animations[1].Add(new Rectangle(128, 64, 32, 64));
            animations[1].Add(new Rectangle(160, 64, 32, 64));
            animations[1].Add(new Rectangle(192, 64, 32, 64));
            animations[1].Add(new Rectangle(224, 64, 32, 64));
            animations[1].Add(new Rectangle(256, 64, 32, 64));
            animations[1].Add(new Rectangle(288, 64, 32, 64));
            animations[1].Add(new Rectangle(320, 64, 32, 64));
            animations[1].Add(new Rectangle(352, 64, 32, 64));


            // Idle Thawed
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 0, 32, 64));
            animations[2].Add(new Rectangle(32, 0, 32, 64));
            animations[2].Add(new Rectangle(64, 0, 32, 64));
            animations[2].Add(new Rectangle(96, 0, 32, 64));
            animations[2].Add(new Rectangle(128, 0, 32, 64));
            animations[2].Add(new Rectangle(160, 0, 32, 64));
            animations[2].Add(new Rectangle(192, 0, 32, 64));
            animations[2].Add(new Rectangle(224, 0, 32, 64));
            animations[2].Add(new Rectangle(256, 0, 32, 64));
            animations[2].Add(new Rectangle(288, 0, 32, 64));
            animations[2].Add(new Rectangle(320, 0, 32, 64));
            animations[2].Add(new Rectangle(352, 0, 32, 64));
            animations[2].Add(new Rectangle(384, 0, 32, 64));
            animations[2].Add(new Rectangle(416, 0, 32, 64));

            // Throwing Thawed
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 64, 32, 64));
            animations[3].Add(new Rectangle(32, 64, 32, 64));
            animations[3].Add(new Rectangle(64, 64, 32, 64));
            animations[3].Add(new Rectangle(96, 64, 32, 64));
            animations[3].Add(new Rectangle(128, 64, 32, 64));
            animations[3].Add(new Rectangle(160, 64, 32, 64));
            animations[3].Add(new Rectangle(192, 64, 32, 64));
            animations[3].Add(new Rectangle(224, 64, 32, 64));
            animations[3].Add(new Rectangle(256, 64, 32, 64));
            animations[3].Add(new Rectangle(288, 64, 32, 64));
            animations[3].Add(new Rectangle(320, 64, 32, 64));
            animations[3].Add(new Rectangle(352, 64, 32, 64));


            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_tempState == TempState.Frozen && !_isKnockedback) _velocity = Vector2.Zero;

            // Gets distance to player and checks if it should stop throwing
            float distanceToPlayer = Vector2.Distance(_gameRoot._player._position, _position);

            switch (_tempState)
            {
                case TempState.Frozen:
                    if (distanceToPlayer > _aggrorange)
                    {
                        SetAnimation(0);
                        _throwing = false;
                        _throwTimer = 0f;
                    }
                    break;
                case TempState.Thawed:
                    if (distanceToPlayer > _aggrorange)
                    {
                        SetAnimation(2);
                        _throwing = false;
                        _throwTimer = 0f;
                    }
                    break;
            }

            if (!_throwing) _throwTimer += GBL.DeltaTime;

            if (_throwTimer >= _throwCooldown)
            {
                _throwTimer = 0f;
                SetAnimation(_animIndex + 1);
            }

            if(_animIndex == 1 || _animIndex == 3)
            {
                _throwing = true;
            }

        }

        protected override void OnAnimationFinished()
        {
            // makes it so chip throws at end of throwing animation and then goes back to idle animation
            if (_animIndex == 1 || _animIndex == 3)
            {
                ThrowChip();
                SetAnimation(_animIndex - 1);
                _throwing = false;
            }
        }
        // used for throwing chip
        public void ThrowChip()
        {
            ChipProjectile projectile = new ChipProjectile(_gameRoot, _position, _gameRoot._player._position, _lifeDuration, _chipMoveSpeed);
            _gameRoot._newSpriteList.Add(projectile);
        }

    }

    public class ChipProjectile : SpriteEnemyProjectile
    {
        float _lifeDuration;
        float _lifeTimer = 0f;

        public ChipProjectile(Game1 gameRoot, Vector2 position, Vector2 target, float lifeDuration, float moveSpeed)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Chip Projectile"), position, moveSpeed, 10)
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
            animations[0].Add(new Rectangle(0, 0, 3, 3));

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
