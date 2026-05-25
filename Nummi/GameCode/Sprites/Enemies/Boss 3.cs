using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{

    public class Anaconda : SpriteEnemy
    {
        // Swipe cooldown
        private float _swipeCooldown = 0f;
        private float _swipeCooldownDuration = 1.5f;
        bool _attacking = false;

        public override bool Dead
        {
            set
            {
                if (_dead) return;
                OnDeath();
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
                if (_gameRoot._bossesDeadNum <= 2)
                {
                    _gameRoot._bossDead = true;
                    _gameRoot._isNextLevelTails = true;
                }
                _dead = value;
            }
            get
            {
                return _dead;
            }
        }
        public Anaconda(Game1 gameRoot, Vector2 pos)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Anaconda"), pos, true, 1200, 340, 50, true, 50f, 1000f, 1500f, 1500) 
        {

        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 64));
            animations[0].Add(new Rectangle(32, 0, 32, 64));
            animations[0].Add(new Rectangle(64, 0, 32, 64));
            animations[0].Add(new Rectangle(96, 0, 32, 64));
            animations[0].Add(new Rectangle(128, 0, 32, 64));
            animations[0].Add(new Rectangle(160, 0, 32, 64));
            animations[0].Add(new Rectangle(0, 64, 32, 64));
            animations[0].Add(new Rectangle(32, 64, 32, 64));
            animations[0].Add(new Rectangle(64, 64, 32, 64));
            animations[0].Add(new Rectangle(96, 64, 32, 64));
            animations[0].Add(new Rectangle(128, 64, 32, 64));

            // Swiping
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(160, 64, 32, 64));
            animations[1].Add(new Rectangle(0, 128, 32, 64));
            animations[1].Add(new Rectangle(32, 128, 32, 64));
            animations[1].Add(new Rectangle(64, 128, 32, 64));
            animations[1].Add(new Rectangle(96, 128, 32, 64));
            animations[1].Add(new Rectangle(128, 128, 32, 64));
            animations[1].Add(new Rectangle(160, 128, 32, 64));

            _canPatrol = true;

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if(!_attacking)
            {
                if (_velocity == Vector2.Zero) SetAnimation(0);
                else SetAnimation(1);
                // Makes it so goes back to being idle before stopping update should put this in all enemies that dont patrol however if they have idle animation then make their velocity 0 when not seeing player
                if (_lastSeenTimer <= 0.2f) SetAnimation(0);
            }

            if (_gameRoot._bossDead) Dead = true;

            // Swipe when close
            if (Vector2.Distance(_gameRoot._player._position, _position) < 48f
                && _swipeCooldown <= 0f && !_attacking)
            {
                TriggerSwipe();
                return;
            }
        }

        private void TriggerSwipe()
        {
            _attacking = true;
            _swipeCooldown = _swipeCooldownDuration;

            SetAnimation(1);
        }

        public void PawSwipe()
        {

            float direction = (_gameRoot._player._position.X < _position.X) ? -16f : 16f;
            WaiterPawSwipe swipe = new WaiterPawSwipe(_gameRoot, _position + new Vector2(direction, 0));
            _gameRoot._newSpriteList.Add(swipe);
        }

        public void OnDeath()
        {
            _gameRoot._bossesDeadNum += 1;
        }


    }

    public class Pig : SpriteEnemy
    {
        // Punch cooldown
        private float _swipeCooldown = 0f;
        private float _swipeCooldownDuration = 1.5f;
        bool _attacking = false;

        // Sliding
        bool _sliding = false;
        private float _slidingDuration = 6f;
        private float _slidingTimer = 0f;
        private float _slidingCooldown = 0f;
        private float _slidingIntermission = 10f;
        private Vector2 _slideDirection = Vector2.Zero;
        private const float SlideSpeed = 400f;
        private int _bounceCount = 0;
        private const int MaxBounces = 2;
        private float _bounceCooldownTimer = 0f;
        private const float BounceCooldown = 0.2f;

        public override bool Dead
        {
            set
            {
                if (_dead) return;
                OnDeath();
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);

                if (_gameRoot._bossesDeadNum <= 2)
                {
                    _gameRoot._bossDead = true;
                    _gameRoot._isNextLevelTails = true;
                }
                _dead = value;
            }
            get
            {
                return _dead;
            }
        }

        public Pig(Game1 gameRoot, Vector2 pos)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Pig Spritesheet"), pos, true, 1200, 250, 40, true, 50f, 1000f, 1500f, 1500)
        {
            _slidingCooldown = _slidingIntermission;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(224, 48, 32, 48));

            // Walking
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(128, 48, 32, 48));
            animations[1].Add(new Rectangle(160, 48, 32, 48));
            animations[1].Add(new Rectangle(192, 48, 32, 48));
            animations[1].Add(new Rectangle(224, 48, 32, 48));
            animations[1].Add(new Rectangle(0, 96, 32, 48));
            animations[1].Add(new Rectangle(32, 96, 32, 48));
            animations[1].Add(new Rectangle(64, 96, 32, 48));
            animations[1].Add(new Rectangle(96, 96, 32, 48));
            animations[1].Add(new Rectangle(128, 96, 32, 48));
            animations[1].Add(new Rectangle(160, 96, 32, 48));
            animations[1].Add(new Rectangle(192, 96, 32, 48));
            animations[1].Add(new Rectangle(224, 96, 32, 48));
            animations[1].Add(new Rectangle(0, 144, 32, 48));

            // Sliding
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(224, 217, 32, 23));

            // Punching
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(160, 192, 32, 48));
            animations[3].Add(new Rectangle(192, 192, 32, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            if (_sliding)
            {
                _isPatrolling = true;
                if (_bounceCooldownTimer > 0f) _bounceCooldownTimer -= GBL.DeltaTime;
                _velocity = _slideDirection * SlideSpeed;
                SetAnimation(2);

                _slidingTimer -= GBL.DeltaTime;
                if (_slidingTimer <= 0f)
                {
                    _sliding = false;
                    _velocity = Vector2.Zero;
                    _slidingCooldown = 0f;
                    SetAnimation(0);
                }

                base.Update(gameTime);
                if (_sliding) _velocity = _slideDirection * SlideSpeed;
                return;
            }

            if (_gameRoot._bossDead) Dead = true;

            if (!_attacking)
            {
                if (_velocity == Vector2.Zero) SetAnimation(0);
                else SetAnimation(1);
                if (_lastSeenTimer <= 0.2f) SetAnimation(0);
            }

            _slidingCooldown += GBL.DeltaTime;
            if (_slidingCooldown > _slidingIntermission)
            {
                _slidingCooldown = 0f;
                StartSlide();
                base.Update(gameTime);
                return;
            }

            if (Vector2.Distance(_gameRoot._player._position, _position) < 48f
                && _swipeCooldown <= 0f && !_attacking)
            {
                TriggerPunch();
                base.Update(gameTime);
                return;
            }

            base.Update(gameTime);
        }

        private void StartSlide()
        {
            _sliding = true;
            _slidingTimer = _slidingDuration;
            _bounceCount = 0;
            _bounceCooldownTimer = 0f;

            Vector2 dir = _gameRoot._player._position - _position;
            _slideDirection = dir == Vector2.Zero ? new Vector2(1f, 0f) : Vector2.Normalize(dir);
            _velocity = _slideDirection * SlideSpeed;
            SetAnimation(2);
        }

        private void TriggerPunch()
        {
            _attacking = true;
            _swipeCooldown = _swipeCooldownDuration;
            _velocity = Vector2.Zero;

            SetAnimation(3);
        }

        public void PawSwipe()
        {

            float direction = (_gameRoot._player._position.X < _position.X) ? -16f : 16f;
            WaiterPawSwipe swipe = new WaiterPawSwipe(_gameRoot, _position + new Vector2(direction, 0));
            _gameRoot._newSpriteList.Add(swipe);
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            if (_animIndex == 3)
            {
                PawSwipe();
                _attacking = false;
                SetAnimation(0);
            }
        }

        public void OnDeath()
        {
            _gameRoot._bossesDeadNum += 1;

        }

        protected override void OnTileCollideEvent(int tileX, int tileY)
        {
            base.OnTileCollideEvent(tileX, tileY);

            if (!_sliding) return;
            if (_bounceCooldownTimer > 0f) return;

            if (_bounceCount < MaxBounces)
            {
                _bounceCount++;
                _bounceCooldownTimer = BounceCooldown;

                float tileCentreX = tileX * 32f + 16f;
                float tileCentreY = tileY * 32f + 16f;
                float dx = Math.Abs(_position.X - tileCentreX);
                float dy = Math.Abs(_position.Y - tileCentreY);

                if (dx > dy) _slideDirection.X = -_slideDirection.X;
                else         _slideDirection.Y = -_slideDirection.Y;

                _velocity = _slideDirection * SlideSpeed;
                _slidingTimer = _slidingDuration;
            }
            else
            {
                _sliding = false;
                _slidingCooldown = 0f;
                _velocity = Vector2.Zero;
                _bounceCount = 0;
                _bounceCooldownTimer = 0f;
                SetAnimation(0);
            }
        }
    }
}
