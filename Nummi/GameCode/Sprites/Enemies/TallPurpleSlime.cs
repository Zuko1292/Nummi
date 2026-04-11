using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class TallPurpleSlime : SpriteEnemy
    {
        public TallPurpleSlime(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet-Tall"), position, true, 1000, 220, 10, false, 50)
        {
            _walkingArea = 100f;
            _canPatrol = true;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(384, 0, 32, 80));
            // Walking
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 32, 80));
            animations[1].Add(new Rectangle(32, 0, 32, 80));
            animations[1].Add(new Rectangle(64, 0, 32, 80));
            animations[1].Add(new Rectangle(96, 0, 32, 80));
            animations[1].Add(new Rectangle(128, 0, 32, 80));
            animations[1].Add(new Rectangle(160, 0, 32, 80));
            animations[1].Add(new Rectangle(192, 0, 32, 80));
            animations[1].Add(new Rectangle(224, 0, 32, 80));
            animations[1].Add(new Rectangle(256, 0, 32, 80));
            animations[1].Add(new Rectangle(288, 0, 32, 80));
            animations[1].Add(new Rectangle(320, 0, 32, 80));
            animations[1].Add(new Rectangle(352, 0, 32, 80));

            // Patrol
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 80, 32, 80));
            animations[2].Add(new Rectangle(32, 80, 32, 80));
            animations[2].Add(new Rectangle(64, 80, 32, 80));
            animations[2].Add(new Rectangle(96, 80, 32, 80));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {

            if (_lastSeenTimer <= 1f)
            {
                SetAnimation(2);
                _isPatrolling = true;
            }
            if(_isPatrolling) 
            {
                _velocity.X = _walkingArea * (float)Math.Sin(gameTime.TotalGameTime.TotalSeconds) * 0.5f;
                _velocity.Y = 0f;
                SetAnimation(2);
                Debug.WriteLine(_frameDuration);
            }

            if (_animIndex == 2) _frameDuration = 1f / 8f;
            else _frameDuration = 1f / 15f;

            if (!_isPatrolling)
            {
                SetAnimation(1);
            }

            base.Update(gameTime);
        }

        protected override void OnTileCollideEvent(int tileX, int tileY)
        {
            base.OnTileCollideEvent(tileX, tileY);

            if(_isPatrolling)
            {
                 _velocity.X = -_velocity.X;
                _position.X += _velocity.X * 0.1f; // Move a bit away from the tile to prevent sticking
            }    
        }
    }
}
