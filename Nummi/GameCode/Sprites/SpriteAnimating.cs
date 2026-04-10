using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    public class SpriteAnimating : Sprite
    {
        #region ***** Member variables *****

        protected List<List<Rectangle>> _animations;

        protected List<int> _nextAnim;

        public int _animIndex, _frameIndex;

        protected float _frameDuration, _frameTimer;

        #endregion ***** Member variables *****

        #region ***** Constructors *****

        public SpriteAnimating(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove, bool canCollide)
            : base(gameRoot, texture, position, canMove, canCollide)
        {
            _animations = BuildAnimations();
            _txrSourceBounds = _animations[0][0];

            InitTexture(texture, _txrSourceBounds);
            InitBounds(position, canCollide);
            InitMovement(canMove);
        }

        #endregion ***** Constructors *****

        #region ***** Member methods: Initialisation *****
        // Builds the animations for sprites to make lists of animations
        protected virtual List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle());

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        #endregion ***** Member methods: Initialisation *****

        #region ***** Member methods: Update *****

        public override void Update(GameTime gameTime)
        {
            UpdateAnimation(gameTime);
            base.Update(gameTime);
        }

        #endregion ***** Member methods: Update *****

        #region ***** Member methods: Animation *****
        // Updating Animation
        protected void UpdateAnimation(GameTime gameTime)
        {
            _frameTimer += GBL.DeltaTime;
            if (_frameTimer >= _frameDuration)
            {
                _frameTimer = 0f;
                _frameIndex++;
            }

            if (_frameIndex >= _animations[_animIndex].Count)
            {
                if (_nextAnim[_animIndex] == _animIndex) _frameIndex = 0;
                else
                {
                    SetAnimation(_nextAnim[_animIndex]);
                }
            }

            ApplyFrame(_animations[_animIndex][_frameIndex]);

        }

        protected void ApplyFrame(Rectangle frame)
        {
            _txrSourceBounds = frame;

            _origin = new Vector2(frame.Width / 2f, frame.Height / 2f);

            _collisionBounds.Width = (int)(frame.Width * _collisionScale.X);
            _collisionBounds.Height = (int)(frame.Height * _collisionScale.Y);
        }
        // For setting what animation you want
        protected void SetAnimation(int animIndex)
        {

            if (animIndex == _animIndex) return;

            _animIndex = Math.Clamp(animIndex, 0, _animations.Count - 1);
            _frameIndex = 0;
            _frameTimer = 0f;
        }

        #endregion ***** Member methods: Animation *****
    }
}
