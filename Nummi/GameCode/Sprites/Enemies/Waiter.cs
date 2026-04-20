using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{

    public class Waiter : SpriteEnemy
    {
        public enum TempState
        {
            Frozen,
            Thawed
        }

        TempState _tempState;

        public Waiter(Game1 gameRoot, Vector2 position, TempState tempState)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Sprites/Enemies/Waiter"), position, true, 150, 220, 20, false, 100, 400f, 80f)
        {
            _tempState = tempState;

            if (_tempState == TempState.Frozen)
            {
                SetAnimation(0);
                _aggrorange = 100f;
            }
            else
            {
                SetAnimation(3);
            }
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sleeping Frozen
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 48));
            animations[0].Add(new Rectangle(32, 0, 32, 48));

            // Walking Frozen
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 48, 32, 48));

            // Swiping Frozen
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 48, 32, 48));

            // Idle Thawed
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 96, 32, 48));

            // Walking Thawed
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 144, 32, 48));

            // Swiping Thawed
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(0, 144, 32, 48));

            // Idle Thawed backRoom
            animations.Add(new List<Rectangle>());
            animations[6].Add(new Rectangle(0, 96, 32, 48));

            // Swiping Thawed backRoom
            animations.Add(new List<Rectangle>());
            animations[7].Add(new Rectangle(0, 144, 32, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_animIndex == 6 || _animIndex == 7) _velocity = Vector2.Zero;

            switch (_tempState)
            {
                case TempState.Frozen:
                    if (_velocity == Vector2.Zero) SetAnimation(0);
                    else SetAnimation(1);
                    if (_lastSeenTimer <= 0.2f) SetAnimation(0);
                    break;
                case TempState.Thawed:
                    if (_velocity == Vector2.Zero) SetAnimation(3);
                    else SetAnimation(4);
                    if (_lastSeenTimer <= 0.2f) SetAnimation(3);
                    break;
            }
        }
    }
}
