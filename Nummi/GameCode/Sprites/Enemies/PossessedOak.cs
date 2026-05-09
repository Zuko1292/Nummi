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
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Boss Idle-Sheet"), position, false, 10000, 220, 10, true, 0, 400f, 0f)// The tree is a stationary enemy That cant be killed
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

            if(GBL.KeyPress(Keys.Space))
            {
                Dead = true;
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

            Vector2 playerPos = _gameRoot._player._position;
            Vector2 dir = playerPos - treeCenter;

            if (dir == Vector2.Zero) return;

            Vector2 dirNorm = Vector2.Normalize(dir);

            // Clamp spawn to within 50px of tree centre
            float spawnRadius = 50f;
            Vector2 spawnPos = treeCenter + dirNorm * spawnRadius;

            Branch branch = new Branch(_gameRoot, spawnPos, dir);
            _gameRoot._newSpriteList.Add(branch);
        }
    }

    public class PossessingSlime : SpriteEnemy
    {
        public PossessingSlime(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Enemies\\PossessingSlime"), position, true, 500, 220, 20, true, 100, 400f, 200f)
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

        public Branch(Game1 gameRoot, Vector2 position, Vector2 attackDirection)
        : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Root Slap-Sheet"), position, false, 100, 220, 10, true, 0, 400f, 0f)
        {
            if (attackDirection != Vector2.Zero)
                _attackDirection = Vector2.Normalize(attackDirection);
            else
                _attackDirection = Vector2.UnitX;

            float angle = (float)Math.Atan2(_attackDirection.Y, _attackDirection.X);

            _rotation = angle + MathHelper.Pi; 
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(116, 11, 11, 5));
            animations[0].Add(new Rectangle(179, 6, 12, 10));
            animations[0].Add(new Rectangle(234, 2, 21, 14));
            animations[0].Add(new Rectangle(276, 3, 43, 13));
            animations[0].Add(new Rectangle(322, 4, 61, 13));
            animations[0].Add(new Rectangle(386, 3, 61, 14));
            animations[0].Add(new Rectangle(468, 3, 43, 13));
            animations[0].Add(new Rectangle(554, 2, 21, 14));
            animations[0].Add(new Rectangle(628, 6, 12, 10));
            animations[0].Add(new Rectangle(691, 11, 12, 5));

            _nextAnim = new List<int>() { 0 };

            return animations;
        }

        protected override void OnAnimationFinished()
        {
            Dead = true;
        }

        public override void Update(GameTime gameTime)
        {

            UpdateAnimation(gameTime);
            // Manually update visible bounds since we skip base.Update
            _visibleBounds = new Rectangle(
                (int)(_position.X - _origin.X),
                (int)(_position.Y - _origin.Y),
                _txrSourceBounds.Width,
                _txrSourceBounds.Height
            );
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_dead || _isHidden) return;

            spriteBatch.Draw(
                _texture,
                _position,          // ← Use _position not _visibleBounds when using origin
                _txrSourceBounds,
                Color.White,
                _rotation,
                _origin,            // ← Pivot point for rotation
                _drawScale,
                _flipEffect,
                _layerDepth
            );
        }
    }
}

