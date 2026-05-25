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

        float _shootTimer = 0f;
        float _shootInterval = 1f;

        float _slimeSpawnTimer = 0f;
        float _slimeInterval = 10f;

        public override bool Dead
        {
            set
            {
                _gameRoot._bossDead = true;
                _gameRoot._isNextLevelTails = true;
                OnDeath();
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
                _dead = value;
            }
            get
            {
                return _dead;
            }
        }

        public PossessedTree(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Boss"), position, false, 10000, 220, 10, true, 0, 400f, 700f, 150)// The tree is a stationary enemy That cant be killed
        {
            _isIndestructible = true;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 141, 64, 72));
            animations[0].Add(new Rectangle(64, 141, 64, 72));
            animations[0].Add(new Rectangle(128, 141, 64, 72));
            animations[0].Add(new Rectangle(192, 141, 64, 72));
            animations[0].Add(new Rectangle(256, 141, 64, 72));
            animations[0].Add(new Rectangle(320, 141, 64, 72));
            animations[0].Add(new Rectangle(384, 141, 64, 72));
            animations[0].Add(new Rectangle(448, 141, 64, 72));

            // Shooting
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 64, 69));
            animations[1].Add(new Rectangle(64, 0, 64, 69));
            animations[1].Add(new Rectangle(128, 0, 64, 69));
            animations[1].Add(new Rectangle(192, 0, 64, 69));
            animations[1].Add(new Rectangle(256, 0, 64, 69));
            animations[1].Add(new Rectangle(320, 0, 64, 69));
            animations[1].Add(new Rectangle(384, 0, 64, 69));
            animations[1].Add(new Rectangle(448, 0, 64, 69));

            // Idle no Slime
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 69, 64, 72));
            animations[2].Add(new Rectangle(64, 69, 64, 72));
            animations[2].Add(new Rectangle(128, 69, 64, 72));
            animations[2].Add(new Rectangle(192, 69, 64, 72));
            animations[2].Add(new Rectangle(256, 69, 64, 72));
            animations[2].Add(new Rectangle(320, 69, 64, 72));
            animations[2].Add(new Rectangle(384, 69, 64, 72));
            animations[2].Add(new Rectangle(448, 69, 64, 72));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {

            if (_gameRoot._bossFightStarted && _gameRoot._bossDead)
            {
                Dead = true;
            }
            base.Update(gameTime);

            if (_gameRoot._slimeOffHead)
            {
                return; // Skip the rest of the update loop if the slime is active, since the tree should be idle during this time
            }

            _branchSpawnTimer += GBL.DeltaTime;
            if (_branchSpawnTimer >= _branchSpawnInterval)
            {
                _branchSpawnTimer = 0f;
                BranchSpawn();
            }


            _shootTimer += GBL.DeltaTime;
            if(_shootTimer >= _shootInterval)
            {
                _shootTimer = 0f;
                SetAnimation(1);
                SpawnProjectile();
            }

            _slimeSpawnTimer += GBL.DeltaTime;
            if(!_gameRoot._slimeOffHead && _slimeSpawnTimer >= _slimeInterval)
            {
                _slimeSpawnTimer = 0f;
                PossessingSlime slime = new PossessingSlime(_gameRoot, _position + new Vector2(0, 50));
                _gameRoot._newSpriteList.Add(slime);
                SetAnimation(2);
                _gameRoot._slimeOffHead = true;
            }
        }

        public void OnDeath()
        {
            // When getting tilemap tile layer is 0 object layer is 1
            // Spawns Mirror to next level
            var map1 = _gameRoot._tilemap.Layers[0];

            map1.SetTile(10, 14, 14);
            map1.SetTile(10, 15, 22);

            var map2 = _gameRoot._tilemap.Layers[1];

            map2.SetTile(10, 11, 7);

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

        public void SpawnProjectile()
        {
            Vector2 treeCenter = new Vector2(
                _collisionBounds.Center.X,
                _collisionBounds.Center.Y
            );
            Vector2 playerPos = _gameRoot._player._position;
            TreeBossProjectile projectile = new TreeBossProjectile(_gameRoot, treeCenter, playerPos);
            _gameRoot._newSpriteList.Add(projectile);
        }
    }

    public class PossessingSlime : SpriteEnemy
    {
        public override bool Dead
        {
            set
            {
                _gameRoot._currentBoss._health -= _gameRoot._currentBoss._maxHealth / 4;
                _gameRoot._slimeOffHead = false; // Reset the flag in Game1 to indicate the slime's head is back on
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
                _dead = value;
            }
            get
            {
                return _dead;
            }
        }

        public PossessingSlime(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\PossessingSlime"), position, true, 250, 220, 20, true, 100, 1000f, 0f, 0)
        {
            // The slime needs to physically bump into the tree (and any other
            // enemies), but the existing LOS raycast in Game1.CanSeePlayer
            // only checks tilemap tiles - sprites never block it - so the
            // slime keeps clear sight to the player through the tree.
            CollisionMask = CollisionLayer.All;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 64, 48));

            // Walking
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 64, 48));
            animations[1].Add(new Rectangle(64, 0, 64, 48));
            animations[1].Add(new Rectangle(128, 0, 64, 48));
            animations[1].Add(new Rectangle(192, 0, 64, 48));
            animations[1].Add(new Rectangle(256, 0, 64, 48));
            animations[1].Add(new Rectangle(320, 0, 64, 48));
            animations[1].Add(new Rectangle(384, 0, 64, 48));
            animations[1].Add(new Rectangle(448, 0, 64, 48));
            animations[1].Add(new Rectangle(512, 0, 64, 48));
            animations[1].Add(new Rectangle(576, 0, 64, 48));
            animations[1].Add(new Rectangle(640, 0, 64, 48));
            animations[1].Add(new Rectangle(704, 0, 64, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_velocity == Vector2.Zero) SetAnimation(0);
            else SetAnimation(1);
            // Makes it so goes back to being idle before stopping update should put this in all enemies that dont patrol however if they have idle animation then make their velocity 0 when not seeing player
            if (_lastSeenTimer <= 0.2f) SetAnimation(0);

            if (_gameRoot._bossDead)
            {
                Dead = true;
            }

            if (GBL.KeyPress(Keys.K))
            {
                Dead = true;
            }

        }
    }

    public class Branch : SpriteEnemy
    {
        private Vector2 _attackDirection;

        public Branch(Game1 gameRoot, Vector2 position, Vector2 attackDirection)
        : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Root Slap-Sheet"), position, false, 100, 220, 10, true, 0, 400f, 0f, 0)
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

    public class TreeBossProjectile : SpriteEnemyProjectile
    {
        public TreeBossProjectile(Game1 gameRoot, Vector2 position, Vector2 target)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Boss Projectile"), position, 150f, 10)
        {
            _velocity = Vector2.Normalize(target - _position) * _moveSpeed;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 8, 8));

            _nextAnim = new List<int>() { 0 };

            return animations;
        }
    }
}

