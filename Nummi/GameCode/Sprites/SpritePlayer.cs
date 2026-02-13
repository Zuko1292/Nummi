using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi.GameCode.Sprites
{
    public class SpritePlayer : SpriteCharacter
    {
        #region ***** Member variables *****

        protected float _jumpSpeed = 750f;
        protected float _moveSpeed = 100f;
        protected bool _hasFloor;
        protected Sprite _floor;
        public Vector2 _playerPos;
        public float _posture = 50f;

        protected float _damageCooldown = 0.5f;
        protected float _damageTimer = 0f;
        protected bool _isInvincible = false;

        public bool _isKnockedback = false;
        protected float _knockbackTimer = 0f;
        protected float _knockbackDuration = 0.2f;

        public bool _isBlocking = false;
        public bool _isMoving = false;

        public SpriteEffects _lockedFlipEffect;

        #endregion ***** Member variables *****

        #region ***** Constructors *****

        public SpritePlayer(Game1 gameRoot, Vector2 position, bool canMove)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Player"), position, canMove, true)
        {
            _restitution = 0.0f;
            _friction = 0.25f;
            _drag = 0.0f;

            _layerDepth = 0.3f;

            _canFlip = true;

            CollisionLayer = CollisionLayer.Player;
            CollisionMask = CollisionLayer.All;
        }

        #endregion ***** Constructors *****

        #region ***** Member methods: Initialisation *****

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle UP
            animations.Add(new List<Rectangle>());

            // Idle DOWN
             animations.Add(new List<Rectangle>());

            // Idle SIDE

            // Walking UP
            animations.Add(new List<Rectangle>());

            // Walking DOWN
            animations.Add(new List<Rectangle>());

            // Walking SIDE
            animations.Add(new List<Rectangle>());

            // Dead
            animations.Add(new List<Rectangle>());

            // Blocking
            animations.Add(new List<Rectangle>());

            // Maybe have attacking animations here too? depending on how we want to handle attacking, whether it's a separate sprite or not
            animations.Add(new List<Rectangle>());

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        #endregion ***** Member methods: Initialisation *****

        #region ***** Member methods: Update *****

        public override void Update(GameTime gameTime)
        {
            // For Blocking
            if (GBL.KeyHold(Keys.F))
            {
                SetAnimation(4);
                _isBlocking = true;
                _velocity.X = 0f;
            }
            else if (_animIndex == 4 && !GBL.KeyHold(Keys.F))
            {
                SetAnimation(0);
                _isBlocking = false;
            }

            if (_isKnockedback)
            {
                _knockbackTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_knockbackTimer <= 0f)
                {
                    _isKnockedback = false;
                }
            }

            if (_isInvincible)
            {
                _damageTimer -= GBL.DeltaTime;
                if (_damageTimer <= 0f)
                {
                    _isInvincible = false;
                }
            }


            if (Dead) return;

            if (_gameRoot._health <= 0)
            {
                SetAnimation(3);
                _canMove = false;
                Dead = true;
            }




            float inputX = 0f;
            // Movement
            if (!_isKnockedback && !_isBlocking)
            {
                if (GBL.KeyHold(Keys.A) || GBL.KeyHold(Keys.Left))
                {
                    inputX -= 1f;
                    _isMoving = true;
                }
                if (GBL.KeyHold(Keys.D) || GBL.KeyHold(Keys.Right))
                {
                    inputX += 1f;
                    _isMoving = true;
                }
            }

            // Clamp horizontal speed
            _velocity.X = MathHelper.Clamp(_velocity.X, -_moveSpeed, _moveSpeed);


            base.Update(gameTime);
        }
        #endregion ***** Member methods: Update *****
    }
}