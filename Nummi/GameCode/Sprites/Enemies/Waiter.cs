using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // TODO this class lowk a mess gotta clean it up and make it more understandable, also add comments to explain what the different states and timers do. The waiter has two main states: Frozen and Thawed. When frozen, it will stay still and only attack when the player gets close, with a build up animation. When thawed, it will move around and attack more aggressively, with different behaviors if it's in the backroom or outside. The timers are used for controlling the hopping movement, drink throwing, swipe cooldown, and frozen wake up process.
    public class Waiter : SpriteEnemy
    {
        bool _attacking = false;
        bool _inBackRoom;

        // Hopping( did the hopping so to create zig zag so normal movement isnt effected
        private float _hopTimer = 0f;
        private float _hopInterval = 0.4f;
        private float _hopSpeed = 180f;
        private float _zigzagDirection = 1f;
        private int _hopCount = 0;

        // Drink throwing
        private float _throwTimer = 0f;
        private float _throwCooldown = 2.5f;

        // Swipe cooldown
        private float _swipeCooldown = 0f;
        private float _swipeCooldownDuration = 1.5f;

        // Frozen wake up
        private bool _awake = false;
        private float _wakeUpTimer = 0f;
        private float _wakeUpDuration = 1.2f; 

        public enum TempState
        {
            Frozen,
            Thawed
        }

        TempState _tempState;

        public Waiter(Game1 gameRoot, Vector2 position, TempState tempState, bool inBackRoom = false)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Bunny Guard"), position, true, 150, 220, 20, false, 50, 400f, 80f)
        {
            _tempState = tempState;
            _inBackRoom = inBackRoom;

            if (_tempState == TempState.Frozen)
            {
                SetAnimation(0);
                _aggrorange = 100f;
                _canMove = false;
            }
            else
            {
                if (_inBackRoom)
                    SetAnimation(2); // Idle backroom
                else
                    SetAnimation(3); // Walking with tray
            }

            _canPatrol = true;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // 0 - Sleeping Frozen
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 128, 32, 64));
            animations[0].Add(new Rectangle(32, 128, 32, 64));
            animations[0].Add(new Rectangle(64, 128, 32, 64));
            animations[0].Add(new Rectangle(96, 128, 32, 64));
            animations[0].Add(new Rectangle(128, 128, 32, 64));
            animations[0].Add(new Rectangle(160, 128, 32, 64));
            animations[0].Add(new Rectangle(192, 128, 32, 64));
            animations[0].Add(new Rectangle(224, 128, 32, 64));
            animations[0].Add(new Rectangle(256, 128, 32, 64));
            animations[0].Add(new Rectangle(288, 128, 32, 64));
            animations[0].Add(new Rectangle(320, 128, 32, 64));
            animations[0].Add(new Rectangle(352, 128, 32, 64));
            animations[0].Add(new Rectangle(384, 128, 32, 64));
            animations[0].Add(new Rectangle(416, 128, 32, 64));
            animations[0].Add(new Rectangle(448, 128, 32, 64));
            animations[0].Add(new Rectangle(480, 128, 32, 64));
            animations[0].Add(new Rectangle(512, 128, 32, 64));

            // 1 - Swiping Frozen 
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 32, 64));
            animations[1].Add(new Rectangle(32, 0, 32, 64));
            animations[1].Add(new Rectangle(64, 0, 32, 64));
            animations[1].Add(new Rectangle(96, 0, 32, 64));
            animations[1].Add(new Rectangle(128, 0, 32, 64));
            animations[1].Add(new Rectangle(160, 0, 32, 64));
            animations[1].Add(new Rectangle(192, 0, 32, 64));
            animations[1].Add(new Rectangle(224, 0, 32, 64));
            animations[1].Add(new Rectangle(256, 0, 32, 64));

            // 2 - Idle Thawed 
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(224, 128, 32, 64));
            animations[2].Add(new Rectangle(256, 128, 32, 64));
            animations[2].Add(new Rectangle(288, 128, 32, 64));
            animations[2].Add(new Rectangle(320, 128, 32, 64));
            animations[2].Add(new Rectangle(352, 128, 32, 64));
            animations[2].Add(new Rectangle(384, 128, 32, 64));
            animations[2].Add(new Rectangle(416, 128, 32, 64));
            animations[2].Add(new Rectangle(448, 128, 32, 64));
            animations[2].Add(new Rectangle(480, 128, 32, 64));
            animations[2].Add(new Rectangle(512, 128, 32, 64));

            // 3 - Walking/Hopping Thawed 
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 64, 32, 64));
            animations[3].Add(new Rectangle(32, 64, 32, 64));
            animations[3].Add(new Rectangle(64, 64, 32, 64));
            animations[3].Add(new Rectangle(96, 64, 32, 64));
            animations[3].Add(new Rectangle(128, 64, 32, 64));
            animations[3].Add(new Rectangle(160, 64, 32, 64));
            animations[3].Add(new Rectangle(192, 64, 32, 64));
            animations[3].Add(new Rectangle(224, 64, 32, 64));

            // 4 - Swiping Thawed (outside backroom)
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 0, 32, 64));
            animations[4].Add(new Rectangle(32, 0, 32, 64));
            animations[4].Add(new Rectangle(64, 0, 32, 64));
            animations[4].Add(new Rectangle(96, 0, 32, 64));
            animations[4].Add(new Rectangle(128, 0, 32, 64));
            animations[4].Add(new Rectangle(160, 0, 32, 64));
            animations[4].Add(new Rectangle(192, 0, 32, 64));
            animations[4].Add(new Rectangle(224, 0, 32, 64));
            animations[4].Add(new Rectangle(256, 0, 32, 64));

            // 5 - Idle Thawed (outside backroom - with tray)
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(0, 96, 32, 48));

            // 6 - Swiping Thawed (backroom)
            animations.Add(new List<Rectangle>());
            animations[6].Add(new Rectangle(0, 128, 32, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            float distanceToPlayer = Vector2.Distance(_gameRoot._player._position, _position);
            bool playerInRange = distanceToPlayer <= _aggrorange;

            _swipeCooldown -= GBL.DeltaTime;

            switch (_tempState)
            {
                case TempState.Frozen:
                    UpdateFrozen(gameTime, playerInRange);
                    break;

                case TempState.Thawed:
                    if (_inBackRoom)
                        UpdateThawedBackRoom(gameTime, playerInRange);
                    else
                        UpdateThawedOutside(gameTime, playerInRange);
                    break;
            }
        }

        // ── Frozen ──────────────────────────────────────────────────────────

        private void UpdateFrozen(GameTime gameTime, bool playerInRange)
        {
            if (!playerInRange)
            {
                // Player left - go back to sleep
                _awake = false;
                _wakeUpTimer = 0f;
                _attacking = false;
                SetAnimation(0);
                return;
            }

            if (!_awake && !_attacking)
            {
                // Player got close - start build up
                _awake = true;
                _wakeUpTimer = 0f;
                SetAnimation(1); // One eye open / build up frame
            }

            if (_awake && !_attacking)
            {
                _wakeUpTimer += GBL.DeltaTime;

                if (_wakeUpTimer >= _wakeUpDuration)
                {
                    // Build up finished - swipe
                    TriggerSwipe();
                }
            }
        }

        // ── Thawed Backroom ─────────────────────────────────────────────────

        private void UpdateThawedBackRoom(GameTime gameTime, bool playerInRange)
        {
            // Stop moving during attack
            if (_animIndex == 6) _velocity = Vector2.Zero;

            if (!playerInRange)
            {
                SetAnimation(2); // Idle
                _velocity = Vector2.Zero;
                _attacking = false;
                return;
            }

            if (!_attacking)
            {
                // Move towards player
                Vector2 dir = _gameRoot._player._position - _position;
                if (dir != Vector2.Zero) dir.Normalize();
                _velocity = dir * 80f;

                SetAnimation(2);

                // Swipe when close enough
                if (Vector2.Distance(_gameRoot._player._position, _position) < 48f
                    && _swipeCooldown <= 0f)
                {
                    TriggerSwipe();
                }
            }
        }

        // ── Thawed Outside (zigzag hop + drink throw + swipe) ───────────────

        private void UpdateThawedOutside(GameTime gameTime, bool playerInRange)
        {
            if (_animIndex == 4) _velocity = Vector2.Zero;

            if (!playerInRange)
            {
                SetAnimation(5); // Idle with tray
                _velocity = Vector2.Zero;
                _attacking = false;
                _hopTimer = 0f;
                return;
            }

            // Throw drink on cooldown
            _throwTimer += GBL.DeltaTime;
            if (_throwTimer >= _throwCooldown && !_attacking)
            {
                _throwTimer = 0f;
                ThrowDrink();
            }

            // Swipe when close
            if (Vector2.Distance(_gameRoot._player._position, _position) < 48f
                && _swipeCooldown <= 0f && !_attacking)
            {
                TriggerSwipe();
                return;
            }

            if (!_attacking)
            {
                // Zigzag hop towards player
                _hopTimer += GBL.DeltaTime;

                if (_hopTimer >= _hopInterval)
                {
                    _hopTimer = 0f;
                    _hopCount++;

                    // Flip zigzag every hop
                    _zigzagDirection = (_hopCount % 2 == 0) ? 1f : -1f;

                    Vector2 toPlayer = _gameRoot._player._position - _position;
                    if (toPlayer != Vector2.Zero) toPlayer.Normalize();

                    // Perpendicular direction for zigzag
                    Vector2 perp = new Vector2(-toPlayer.Y, toPlayer.X) * _zigzagDirection * 0.5f;

                    _velocity = (toPlayer + perp) * _hopSpeed;
                }

                SetAnimation(3); // Hopping with tray
            }
        }

        // ── Attacks ─────────────────────────────────────────────────────────

        private void TriggerSwipe()
        {
            _attacking = true;
            _swipeCooldown = _swipeCooldownDuration;

            if (_tempState == TempState.Frozen)
                SetAnimation(1);
            else if (_inBackRoom)
                SetAnimation(6);
            else
                SetAnimation(4);
        }

        public void PawSwipe()
        {
            float direction = (_gameRoot._player._position.X < _position.X) ? -16f : 16f;
            WaiterPawSwipe swipe = new WaiterPawSwipe(_gameRoot, _position + new Vector2(direction, 0));
            _gameRoot._newSpriteList.Add(swipe);
        }

        public void ThrowDrink()
        {
            AcidDrink drink = new AcidDrink(_gameRoot, _position, _gameRoot._player._position);
            _gameRoot._newSpriteList.Add(drink);
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();

            if (_animIndex == 1 || _animIndex == 4 || _animIndex == 6)
            {
                PawSwipe();
                _attacking = false;

                // Return to idle after swipe
                if (_tempState == TempState.Frozen)
                {
                    _awake = false;
                    _wakeUpTimer = 0f;
                    SetAnimation(0);
                }
            }
        }
    }

    // ── Paw Swipe Hitbox ────────────────────────────────────────────────────

    public class WaiterPawSwipe : SpriteEnemy
    {
        float _lifeDuration = 0.15f;

        public WaiterPawSwipe(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Waiter Smash"), position, false, 10000, 300, 20, false, 0, 400f, 80f)
        {
            SetAnimation(0);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 16));
            animations[0].Add(new Rectangle(16, 0, 16, 16));
            animations[0].Add(new Rectangle(32, 0, 16, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _lifeDuration -= GBL.DeltaTime;
            if (_lifeDuration <= 0f) _dead = true;
        }
    }

    // ── Acid Drink Projectile ───────────────────────────────────────────────

    public class AcidDrink : SpriteEnemyProjectile
    {
        private float _lifeTimer = 0f;
        private float _lifeDuration = 3f;
        private bool _landed = false;
        private float _aoeRadius = 48f;
        private float _aoeDuration = 1.5f;
        private float _aoeTimer = 0f;

        public AcidDrink(Game1 gameRoot, Vector2 position, Vector2 target)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Acid Drink"), position, 120f, 15)
        {
            Vector2 dir = target - position;
            if (dir != Vector2.Zero) dir.Normalize();
            _velocity = dir * _moveSpeed;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Flying
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 16));

            // Splat / AOE
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 16, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _lifeTimer += GBL.DeltaTime;

            if (!_landed)
            {
                bool hitPlayer = Vector2.Distance(_position, _gameRoot._player._position) < 16f;
                bool timedOut = _lifeTimer >= _lifeDuration;

                if (hitPlayer || timedOut)
                {
                    Land();
                }
            }
            else
            {
                // AOE pool on the ground
                _aoeTimer += GBL.DeltaTime;
                _velocity = Vector2.Zero;

                // Damage player if standing in AOE
                if (Vector2.Distance(_position, _gameRoot._player._position) <= _aoeRadius)
                {
                    _gameRoot._health -= _damageStrength / 3;
                }

                if (_aoeTimer >= _aoeDuration)
                    _dead = true;
            }
        }

        private void Land()
        {
            _landed = true;
            _velocity = Vector2.Zero;
            _lifeTimer = 0f;
            SetAnimation(1); // Switch to splat sprite
        }
    }
}