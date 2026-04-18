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

        public Dealer(Game1 gameRoot, Vector2 position, TempState tempState)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Dealer_PH"), position, false, 200, 220, 10, false, 0, 400f)
        {
            _tempState = tempState;

            if(_tempState == TempState.Frozen)
            {
                SetAnimation(0);
                _lifeDuration = 2f;
                _throwCooldown = 3f;
                _chipMoveSpeed = 50f;
                _aggrorange = 100f;
            }
            else
            {
                SetAnimation(2);
                _lifeDuration = 10f;
                _throwCooldown = 1.5f;
                _chipMoveSpeed = 150f;
            }
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sleeping Frozen
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 48));
            animations[0].Add(new Rectangle(32, 0, 32, 48));

            // Throwing Frozen
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 48, 32, 48));

            // Idle Thawed
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 96, 32, 48));

            // Throwing Thawed
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 144, 32, 48));


            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(!_throwing) _throwTimer += GBL.DeltaTime;

            if (_throwTimer >= _throwCooldown)
            {
                _throwTimer = 0f;
                SetAnimation(_animIndex + 1);
            }

            if(_animIndex == 1 || _animIndex == 3)
            {
                _throwing = true;
            }

            switch(_tempState)
            {
                case TempState.Frozen:
                    if (_gameRoot._player._position.X < _position.X - _aggrorange || _gameRoot._player._position.X > _position.X + _aggrorange)
                    {
                        SetAnimation(0);
                        _throwing = false;
                    }
                    break;
                case TempState.Thawed:
                    if (_gameRoot._player._position.X < _position.X - _aggrorange || _gameRoot._player._position.X > _position.X + _aggrorange)
                    {
                        SetAnimation(2);
                        _throwing = false;
                    }
                    break;
            }
        }

        protected override void OnAnimationFinished()
        {
            if(_animIndex == 1 || _animIndex == 3)
            {
                ThrowChip();
                SetAnimation(_animIndex - 1);
                _throwing = false;
            }
        }

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
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\ChipProjectile_PH"), position, moveSpeed, 10)
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
            animations[0].Add(new Rectangle(8, 0, 8, 8));
            animations[0].Add(new Rectangle(16, 0, 8, 8));
            animations[0].Add(new Rectangle(24, 0, 8, 8));
            animations[0].Add(new Rectangle(32, 0, 8, 8));
            animations[0].Add(new Rectangle(40, 0, 8, 8));
            animations[0].Add(new Rectangle(48, 0, 8, 8));
            animations[0].Add(new Rectangle(56, 0, 8, 8));

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
