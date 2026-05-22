using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class Security_Guard : SpriteEnemy
    {
        public enum TempState
        {
            Frozen,
            Thawed
        }

        public TempState _tempState;

        bool _attacking = false;

        // ── Thawed: three-paw swipe combo (left, right, slam) ──────────────
        private float _swipeCooldown = 0f;
        private float _swipeCooldownDuration = 4f;
        private float _swipeRange = 60f;
        private int _comboStep = 0;        // 0 = not combo'ing, 1 = left, 2 = right, 3 = slam
        private float _comboStepTimer = 0f;
        private float _comboStepDuration = 0.35f;

        private float _chaseSpeed = 140f;

        public Security_Guard(Game1 gameRoot, Vector2 position, TempState tempState)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Bear"), position, true, 200, 500, 15, false, 50, 300f, 120f, 60)
        {
            _tempState = tempState;

            if (_tempState == TempState.Frozen)
            {
                
                SetAnimation(0);
                _isIndestructible = true;
                _canMove = true;
                _isStationary = true;
                _aggrorange = 0f; 
            }
            else
            {
                SetAnimation(2); // Thawed idle
            }

            _canPatrol = false;
        }


        protected override List<List<Rectangle>> BuildAnimations()
        {

            // the unused animations are because I added stuff from design but there was no asset for those so i removed them and didnt want to have to change values in the code
            _frameDuration = 1f / 8f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // 0 - Sleeping Frozen
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(64, 192, 32, 64));
            animations[0].Add(new Rectangle(96, 192, 32, 64));
            animations[0].Add(new Rectangle(128, 192, 32, 64));
            animations[0].Add(new Rectangle(160, 192, 32, 64));
            animations[0].Add(new Rectangle(192, 192, 32, 64));
            animations[0].Add(new Rectangle(224, 192, 32, 64));
            animations[0].Add(new Rectangle(256, 192, 32, 64));
            animations[0].Add(new Rectangle(288, 192, 32, 64));
            animations[0].Add(new Rectangle(0, 256, 32, 64));
            animations[0].Add(new Rectangle(32, 256, 32, 64));
            animations[0].Add(new Rectangle(64, 256, 32, 64));
            animations[0].Add(new Rectangle(96, 256, 32, 64));
            animations[0].Add(new Rectangle(128, 256, 32, 64));
            animations[0].Add(new Rectangle(160, 256, 32, 64));

            // 1 - (unused)
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 32, 64));

            // 2 - Idle Thawed
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 0, 32, 64));

            // 3 - Chase / Run Thawed
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(288, 0, 32, 64));
            animations[3].Add(new Rectangle(0, 64, 32, 64));
            animations[3].Add(new Rectangle(32, 64, 32, 64));
            animations[3].Add(new Rectangle(64, 64, 32, 64));
            animations[3].Add(new Rectangle(96, 64, 32, 64));
            animations[3].Add(new Rectangle(128, 64, 32, 64));
            animations[3].Add(new Rectangle(160, 64, 32, 64));
            animations[3].Add(new Rectangle(192, 64, 32, 64));

            // 4 - (unused)
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 0, 32, 64));

            // 5 - Left swipe
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(224, 128, 32, 64));

            // 6 - Right swipe
            animations.Add(new List<Rectangle>());
            animations[6].Add(new Rectangle(192, 128, 32, 64));

            // 7 - Two-paw slam
            animations.Add(new List<Rectangle>());
            animations[7].Add(new Rectangle(256, 128, 32, 64));
            animations[7].Add(new Rectangle(288, 128, 32, 64));
            animations[7].Add(new Rectangle(0, 192, 32, 64));
            animations[7].Add(new Rectangle(32, 192, 32, 64));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _swipeCooldown -= GBL.DeltaTime;

            float distance = Vector2.Distance(_gameRoot._player._position, _position);
            bool playerInRange = distance <= _aggrorange;

            switch (_tempState)
            {
                case TempState.Frozen:
                    UpdateFrozen();
                    break;
                case TempState.Thawed:
                    UpdateThawed(distance, playerInRange);
                    break;
            }
        }

        // ── Frozen ──────────────────────────────────────────────────────────

        private void UpdateFrozen()
        {
            if (!_isKnockedback) _velocity = Vector2.Zero;
            SetAnimation(0);
        }

        // ── Thawed (mini boss) ──────────────────────────────────────────────

        private void UpdateThawed(float distance, bool playerInRange)
        {
            
            if (_isKnockedback)
            {
                SetAnimation(3);
                return;
            }

            
            if (_comboStep > 0)
            {
                AdvanceCombo();
                return;
            }

            if (!playerInRange)
            {
                SetAnimation(2);
                _velocity = Vector2.Zero;
                _attacking = false;
                return;
            }

            if (distance < _swipeRange && _swipeCooldown <= 0f && !_attacking)
            {
                StartComboSwipe();
                return;
            }

            if (!_attacking)
            {
                Vector2 dir = _gameRoot._player._position - _position;
                if (dir != Vector2.Zero) dir.Normalize();
                _velocity = dir * _chaseSpeed;
                SetAnimation(3);
            }
        }

        // ── Three-paw swipe combo ───────────────────────────────────────────

        private void StartComboSwipe()
        {
            _attacking = true;
            _swipeCooldown = _swipeCooldownDuration;
            _comboStep = 1;
            _comboStepTimer = 0f;
            _velocity = Vector2.Zero;
            SetAnimation(5); // Left
            SpawnSwipe(side: -1);
        }

        private void AdvanceCombo()
        {
            if (!_isKnockedback) _velocity = Vector2.Zero;
            _comboStepTimer += GBL.DeltaTime;

            if (_comboStepTimer < _comboStepDuration) return;
            _comboStepTimer = 0f;

            _comboStep++;
            switch (_comboStep)
            {
                case 2:
                    SetAnimation(6); // Right
                    SpawnSwipe(side: +1);
                    break;
                case 3:
                    SetAnimation(7); // Slam (AOE)
                    SpawnSlamAOE();
                    break;
                default:
                    _comboStep = 0;
                    _attacking = false;
                    break;
            }
        }

        private void SpawnSwipe(int side)
        {
            Vector2 offset = new Vector2(side * 28f, 0f);
            GuardPawSwipe swipe = new GuardPawSwipe(_gameRoot, _position + offset);
            _gameRoot._newSpriteList.Add(swipe);
        }

        private void SpawnSlamAOE()
        {
            GuardSlamAOE slam = new GuardSlamAOE(_gameRoot, _position);
            _gameRoot._newSpriteList.Add(slam);
        }
    }

    // Short-lived AOE for the guard's paw swipes (combo steps 1 & 2).
    public class GuardPawSwipe : SpriteEnemy
    {
        float _lifeDuration = 0.18f;

        public GuardPawSwipe(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Waiter Smash"), position, false, 10000, 350, 25, false, 0, 400f, 80f, 0)
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

    // Two-paw slam AOE that closes the swipe combo. Wider radius, more damage.
    public class GuardSlamAOE : SpriteEnemy
    {
        float _lifeDuration = 0.25f;

        public GuardSlamAOE(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Waiter Smash"), position, false, 10000, 450, 40, false, 0, 400f, 120f, 0)
        {
            SetAnimation(0);
            _drawScale = new Vector2(3f, 3f);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;
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
}
