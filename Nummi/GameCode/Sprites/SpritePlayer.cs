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
        private bool _facingLeft = true;

        public SpriteEffects _lockedFlipEffect;

        #endregion ***** Member variables *****

        #region ***** Constructors *****

        public SpritePlayer(Game1 gameRoot, Vector2 position, bool canMove)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Player_SpriteSheet"), position, canMove, true)
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

            // Idle DOWN
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 32));
            animations[0].Add(new Rectangle(128, 0, 32, 32));

            // Idle UP
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(64, 0, 32, 32));
            animations[1].Add(new Rectangle(192, 0, 32, 32));

            // Idle SIDE RIGHT
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(32, 0, 32, 32));
            animations[2].Add(new Rectangle(160, 0, 32, 32));

            // Walking UP
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 96, 32, 32));
            animations[3].Add(new Rectangle(32, 96, 32, 32));
            animations[3].Add(new Rectangle(64, 96, 32, 32));
            animations[3].Add(new Rectangle(96, 96, 32, 32));
            animations[3].Add(new Rectangle(128, 96, 32, 32));
            animations[3].Add(new Rectangle(160, 96, 32, 32));
            animations[3].Add(new Rectangle(192, 96, 32, 32));
            animations[3].Add(new Rectangle(224, 96, 32, 32));

            // Walking DOWN
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(0, 32, 32, 32));
            animations[4].Add(new Rectangle(32, 32, 32, 32));
            animations[4].Add(new Rectangle(64, 32, 32, 32));
            animations[4].Add(new Rectangle(96, 32, 32, 32));
            animations[4].Add(new Rectangle(128, 32, 32, 32));
            animations[4].Add(new Rectangle(160, 32, 32, 32));
            animations[4].Add(new Rectangle(192, 32, 32, 32));
            animations[4].Add(new Rectangle(224, 32, 32, 32));

            // Walking SIDE
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(0, 64, 32, 32));
            animations[5].Add(new Rectangle(32, 64, 32, 32));
            animations[5].Add(new Rectangle(64, 64, 32, 32));
            animations[5].Add(new Rectangle(96, 64, 32, 32));
            animations[5].Add(new Rectangle(128, 64, 32, 32));
            animations[5].Add(new Rectangle(160, 64, 32, 32));
            animations[5].Add(new Rectangle(192, 64, 32, 32));
            animations[5].Add(new Rectangle(224, 64, 32, 32));

            // IDLE SIDE LEFT
            animations.Add(new List<Rectangle>());
            animations[6].Add(new Rectangle(96, 0, 32, 32));
            animations[6].Add(new Rectangle(224, 0, 32, 32));

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
            float inputY = 0f;
            // Movement
            if (!_isKnockedback && !_isBlocking)
            {
                if (GBL.KeyHold(Keys.A) || GBL.KeyHold(Keys.Left))
                {
                    inputX -= 1f;
                    _facingLeft = true;
                }
                if (GBL.KeyHold(Keys.D) || GBL.KeyHold(Keys.Right))
                {
                    inputX += 1f;
                    _facingLeft = false;
                }
                if (GBL.KeyHold(Keys.W) || GBL.KeyHold(Keys.Up))
                {
                    inputY -= 1f;
                }
                if (GBL.KeyHold(Keys.S) || GBL.KeyHold(Keys.Down))
                {
                    inputY += 1f;
                }

                if (_velocity.X != 0 && _velocity.Y == 0) SetAnimation(5);
                if (_velocity.Y > 0) SetAnimation(4);
                else if (_velocity.Y < 0) SetAnimation(3);

                _velocity.X = inputX * _moveSpeed;

                _velocity.Y = inputY * _moveSpeed;

                if(_velocity.X != 0f || _velocity.Y != 0f)
                {
                    _isMoving = true;
                }

                if (_velocity.X == 0f && _velocity.Y == 0f)
                {
                    _isMoving = false;
                    if (_animIndex == 3) SetAnimation(1);
                    else if (_animIndex == 4) SetAnimation(0);
                    else if (_animIndex == 5 && !_facingLeft) SetAnimation(2);
                    else if (_animIndex == 5 && _facingLeft) SetAnimation(6);
                }

                int futureLeft = (int)_position.X - 16;
                int futureRight = (int)_position.X + 16;
                int futureTop = (int)_position.Y - 16;
                int futureBottom = (int)_position.Y + 16;

                if (_velocity.X > 0)
                {
                    if (_gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureTop) ||
                        _gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureBottom))
                    {
                        _velocity.X = 0;
                    }
                }
                if (_velocity.X < 0)
                {
                    if (_gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureTop) ||
                        _gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureBottom))
                    {
                        _velocity.X = 0;
                    }
                }
                if (_velocity.Y > 0)
                {
                    if (_gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureTop) ||
                        _gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureBottom))
                    {
                        _velocity.Y = 0;
                    }
                }
                if (_velocity.Y < 0)
                {
                    if (_gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureTop) ||
                        _gameRoot._tilemap.IsSolidAtWorld(futureLeft, futureBottom))
                    {
                        _velocity.Y = 0;
                    }
                }
            }

            // Clamp horizontal speed
            _velocity.X = MathHelper.Clamp(_velocity.X, -_moveSpeed, _moveSpeed);


            base.Update(gameTime);
        }
        #endregion ***** Member methods: Update *****
    }
}