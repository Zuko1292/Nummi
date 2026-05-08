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
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\TreeBoss_PH"), position, false, 10000, 220, 10, true, 0, 400f, 0f)// The tree is a stationary enemy That cant be killed
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

            Vector2 dir = _gameRoot._player._position - treeCenter;

            Branch branch = new Branch(_gameRoot, treeCenter, dir);
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
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Root Slap-Sheet"), position, true, 100, 220, 10, true, 0, 400f, 0f)
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

            // Slapping (retracted)
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

            _nextAnim = new List<int>() { 0, 0 }; 

            return animations;
        }

        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }


    }
}

