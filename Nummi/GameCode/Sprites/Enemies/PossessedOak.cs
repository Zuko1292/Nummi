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

    public class PossessedTree : SpriteEnemy
    {

        float _branchSpawnTimer = 0f;
        float _branchSpawnInterval = 5f;

        public override bool Dead
        {
            set
            {
                _dead = value;
                if (_dead) OnDeath();
            }
            get
            {
                return _dead;
            }
        }

        public PossessedTree(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\TreeBoss_PH"), position, false, 10000, 220, 10, true, 0)// The tree is a stationary enemy That cant be killed
        {

        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 80));


            _nextAnim = new List<int>() { 0, 0 };

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_gameRoot._bossDead)
            {
                Dead = true;
            }

            _branchSpawnTimer += GBL.DeltaTime;
            if (_branchSpawnTimer >= _branchSpawnInterval)
            {
                _branchSpawnTimer = 0f;
                BranchSpawn();
            }
        }

        public void OnDeath()
        {
            // Drop loot, play death animation, etc. 
        }

        public void BranchSpawn()
        {
            Vector2 treeCenter = new Vector2(
                _collisionBounds.Center.X,
                _collisionBounds.Center.Y
            );

            Vector2 dir = _gameRoot._player._position - treeCenter;

            Branch branch = new Branch(_gameRoot, treeCenter, dir);
            _gameRoot._newSpriteList.Add(branch);
        }
    }

    public class PossessingSlime : SpriteEnemy
    {
        public PossessingSlime(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Enemies\\PossessingSlime"), position, true, 500, 220, 20, true, 100)
        {
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_gameRoot._bossDead)
            {
                Dead = true;
            }

        }
    }

    public class Branch : SpriteEnemy
    {
        private Vector2 _attackDirection;

        private float _extendScale = 0f; 
        private float _extendSpeed = 3f;

        private bool _extending = true;
        private float _idleTimer = 0f;
        private float _idleDuration = 3f;

        public Branch(Game1 gameRoot, Vector2 position, Vector2 attackDirection)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Branch"), position, true, 100, 220, 10, true, 0)
        {
            if (attackDirection != Vector2.Zero)
                _attackDirection = Vector2.Normalize(attackDirection);
            else
                _attackDirection = Vector2.UnitX;

            _rotation = (float)Math.Atan2(_attackDirection.Y, _attackDirection.X);
            _rotation -= MathHelper.PiOver2;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle (retracted)
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 80));

            // Extending animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 32, 80));
            animations[1].Add(new Rectangle(32, 0, 32, 80));
            animations[1].Add(new Rectangle(64, 0, 32, 80));

            _nextAnim = new List<int>() { 0, 0 }; 

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            float dt = GBL.DeltaTime;

            if (_extending)
            {
                SetAnimation(1);

                _extendScale += _extendSpeed * dt;

                if (_extendScale >= 1f)
                {
                    _extendScale = 1f;
                    _extending = false;
                    _idleTimer = _idleDuration;
                    SetAnimation(0);
                }
            }

            else if (_idleTimer > 0f)
            {
                _idleTimer -= dt;
            }
            else
            {
                _extendScale -= _extendSpeed * dt;

                if (_extendScale <= 0f)
                {
                    _extendScale = 0f;
                    Dead = true;
                }
            }

            base.Update(gameTime);
        }

        protected override void UpdateBounds(GameTime gameTime)
        {
            Vector2 baseSize = _txrSourceBounds.Size.ToVector2();

            float length = baseSize.Y * _extendScale;

            // Direction the branch extends in world space
            Vector2 dir = _attackDirection;

            // Perpendicular vector (for width)
            Vector2 perp = new Vector2(-dir.Y, dir.X);

            float halfWidth = baseSize.X * 0.5f;

            // Center of the branch (so it grows outward from start point)
            Vector2 center = _position + dir * (length * 0.5f);

            // Build rectangle manually
            _visibleBounds = new Rectangle(
                (center - new Vector2(halfWidth, length * 0.5f)).ToPoint(),
                new Point((int)(baseSize.X), (int)(length))
            );

            _collisionBounds = _visibleBounds;
        }
    }
}

