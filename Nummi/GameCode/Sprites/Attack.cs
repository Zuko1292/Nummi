using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class Attack : SpriteAnimating
    {
        public float _weaponDamage;
        private float _lifetime = 0.5f;

        public Attack(Game1 gameRoot, Texture2D weaponTxr, Vector2 position, bool canMove, int currentWeapon) : 
            base(gameRoot,weaponTxr, position, canMove, true)
        {
            CollisionLayer = CollisionLayer.Player;
            CollisionMask = CollisionLayer.Enemy & ~CollisionLayer.Player;
        }

        public override void Update(GameTime gameTime)
        {
            _weaponDamage = 10;

            _lifetime -= GBL.DeltaTime;

            if(_lifetime <= 0)
            {
                _gameRoot._player._attacking = false;
                Dead = true;
            }

            _velocity = _gameRoot._player._velocity;
            base.Update(gameTime);
        }
    }
    public class Up_Attack : Attack
    {

        public Up_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Up Slash-Sheet"), position, true, currentWeapon)
        {
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 16));
            animations[0].Add(new Rectangle(48, 0, 48, 16));
            animations[0].Add(new Rectangle(96, 0, 48, 16));
            animations[0].Add(new Rectangle(144, 0, 48, 16));
            animations[0].Add(new Rectangle(192, 0, 48, 16));
            animations[0].Add(new Rectangle(240, 0, 48, 16));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 64, 16));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 32, 48, 16));

            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 48, 64, 16));

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 64, 48, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SetAnimation(_gameRoot._player._currentWeapon);
        }
    }
    public class Down_Attack : Attack
    {

        public Down_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Down Slash-Sheet"), position, true, currentWeapon)
        {
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 16));
            animations[0].Add(new Rectangle(48, 0, 48, 16));
            animations[0].Add(new Rectangle(96, 0, 48, 16));
            animations[0].Add(new Rectangle(144, 0, 48, 16));
            animations[0].Add(new Rectangle(192, 0, 48, 16));
            animations[0].Add(new Rectangle(240, 0, 48, 16));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 64, 16));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 32, 48, 16));

            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 48, 64, 16));

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 64, 48, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SetAnimation(_gameRoot._player._currentWeapon);
        }
    }
    public class Right_Attack : Attack
    {

        public Right_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Right Slash-Sheet"), position, true, currentWeapon)
        {
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 48));
            animations[0].Add(new Rectangle(16, 0, 16, 48));
            animations[0].Add(new Rectangle(32, 0, 16, 48));
            animations[0].Add(new Rectangle(48, 0, 16, 48));
            animations[0].Add(new Rectangle(64, 0, 16, 48));
            animations[0].Add(new Rectangle(80, 0, 16, 48));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 64, 16));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 32, 48, 16));

            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 48, 64, 16));

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 64, 48, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SetAnimation(_gameRoot._player._currentWeapon);
        }
    }
    public class Left_Attack : Attack
    {

        public Left_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Left Slash-Sheet"), position, true, currentWeapon)
        {
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 48));
            animations[0].Add(new Rectangle(16, 0, 16, 48));
            animations[0].Add(new Rectangle(32, 0, 16, 48));
            animations[0].Add(new Rectangle(48, 0, 16, 48));
            animations[0].Add(new Rectangle(64, 0, 16, 48));
            animations[0].Add(new Rectangle(80, 0, 16, 48));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 64, 16));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 32, 48, 16));

            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 48, 64, 16));

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 64, 48, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            SetAnimation(_gameRoot._player._currentWeapon);
        }
    }

    public class Arrow : Attack
    {

        public Arrow(Game1 gameRoot, Vector2 position, int currentWeapon)
            : base(gameRoot, GBL.Content.Load<Texture2D>(""), position, true, currentWeapon)
        {
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Vector2 Direction = Vector2.Normalize(_gameRoot._player._position + new Vector2(GBL._camera.ScreenToWorld(GBL.mousePos).X, GBL._camera.ScreenToWorld(GBL.mousePos).Y));
            _velocity = Direction * 200f * GBL.DeltaTime;
        }
    }
}
