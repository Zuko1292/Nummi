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
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Anaconda"), pos, true, 1200, 340, 50, true, 200f, 350f, 1500f, 1500) 
        {

        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

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
            animations[1].Add(new Rectangle(192, 128, 32, 64));
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
        private float _slidingCooldown = 0f;
        private float _slidingIntermission = 10f;

        public override bool Dead
        {
            set
            {
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
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Pig Spritesheet"), pos, true, 1200, 250, 40, true, 450f, 350f, 1500f, 1500)
        {

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
            base.Update(gameTime);


            // Punch when close
            if (_sliding) return;
            if (Vector2.Distance(_gameRoot._player._position, _position) < 48f
                && _swipeCooldown <= 0f && !_attacking)
            {
                TriggerPunch();
                return;
            }
        }

        private void TriggerPunch()
        {
            _attacking = true;
            _swipeCooldown = _swipeCooldownDuration;

            SetAnimation(2);
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
}
