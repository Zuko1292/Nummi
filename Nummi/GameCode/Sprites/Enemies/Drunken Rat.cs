using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{

    public class Drunken_Rat : SpriteEnemy
    {
        // Swipe cooldown
        private float _swipeCooldown = 0f;
        private float _swipeCooldownDuration = 1.5f;
        bool _attacking = false;

        public Drunken_Rat(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Drunken Rat Sheet"), position, true, 250, 200, 30, false, 100, 400f, 175f, 150)
        {
            _swipeCooldown = _swipeCooldownDuration;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 64));

            // Walking
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 64, 32, 64));
            animations[1].Add(new Rectangle(32, 64, 32, 64));
            animations[1].Add(new Rectangle(64, 64, 32, 64));
            animations[1].Add(new Rectangle(96, 64, 32, 64));
            animations[1].Add(new Rectangle(128, 64, 32, 64));
            animations[1].Add(new Rectangle(160, 64, 32, 64));
            animations[1].Add(new Rectangle(192, 64, 32, 64));

            // Swiping
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 128, 32, 64));
            animations[2].Add(new Rectangle(32, 128, 32, 64));
            animations[2].Add(new Rectangle(64, 128, 32, 64));
            animations[2].Add(new Rectangle(96, 128, 32, 64));
            animations[2].Add(new Rectangle(128, 128, 32, 64));
            animations[2].Add(new Rectangle(160, 128, 32, 64));
            animations[2].Add(new Rectangle(192, 128, 32, 64));

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

            if(!_attacking)
                _swipeCooldown -= GBL.DeltaTime;

            if (Vector2.Distance(_gameRoot._player._position, _position) < 48f
                && _swipeCooldown <= 0f && !_attacking)
            {
                _swipeCooldown = _swipeCooldownDuration;
                PawSwipe();
            }

        }
        public void PawSwipe()
        {
            _attacking = true;
            float direction = (_gameRoot._player._position.X < _position.X) ? -16f : 16f;
            WaiterPawSwipe swipe = new WaiterPawSwipe(_gameRoot, _position + new Vector2(direction, 0));
            _gameRoot._newSpriteList.Add(swipe);
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();

            if(_animIndex == 2) _attacking = false;
        }

    }
}


