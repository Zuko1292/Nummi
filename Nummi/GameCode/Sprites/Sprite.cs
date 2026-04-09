using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    
    [Flags]
    public enum CollisionLayer
    {
        None = 0,
        Player = 1 << 0,
        Enemy = 1 << 1,
        Collectable = 1 << 2,
        Solid = 1 << 3,
        All = ~0,
    }

    public class Sprite
    {
        #region ***** Member Variables *****
        public bool _isHidden;
        public bool _canMove;
        public bool _canCollide;
        protected bool _hasContact;
        protected bool _hascontactPrev;
        protected bool _canFlip;
        protected bool _dead;
        public float _layerDepth = 0.5f;
        public float _rotation = 0f;

        public float _lastSeenTimer = 2f;

        protected Game1 _gameRoot;
        protected Texture2D _texture;

        public Vector2 _position;
        public Vector2 _velocity;

        protected float _maxSpeed;
        protected float _friction;
        protected float _drag;
        protected float _restitution;

        protected Vector2 _origin;
        protected Vector2 _drawScale;
        protected Vector2 _collisionScale;

        protected Rectangle _visibleBounds;
        protected Rectangle _txrSourceBounds;
        public Rectangle _collisionBounds;

        public SpriteEffects _flipEffect;

        public CollisionLayer CollisionLayer { get; protected set; } = CollisionLayer.None;
        public CollisionLayer CollisionMask { get; protected set;} = CollisionLayer.All;
            
        public virtual bool Dead
        {
            set
            {
                _dead = value;
            }

            get
            {
                return _dead;
            }
        }

        #endregion ***** Member Variables *****

        #region ***** Constructors *****

        public Sprite(Game1 gameRoot)
        {
            _gameRoot = gameRoot;
        }

        public Sprite(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove = false, bool canCollide = false)
        {
            _gameRoot = gameRoot;

            InitTexture(texture);
            InitBounds(position, canCollide);
            InitMovement(canMove);
        }

        public Sprite(Game1 gameRoot, Texture2D texture, Vector2 position, Rectangle txrSourceBounds = default, bool canMove = false, bool canCollide = false)
        {
            _gameRoot = gameRoot;

            InitTexture(texture, txrSourceBounds);
            InitBounds(position, canCollide);
            InitMovement(canMove);
        }

        #endregion ***** Constructors *****

        #region ***** Initialisation *****
        // Initalising the Textures bounds and position to the middle of sprite
        public virtual void InitTexture(Texture2D texture = null, Rectangle txrSourceBounds = default)
        {
            _texture = texture ?? _gameRoot._defaultTxr;

            _txrSourceBounds = !txrSourceBounds.IsEmpty
                ? txrSourceBounds
                : new Rectangle(0, 0, _texture.Width, _texture.Height);

            _origin = new Vector2(_txrSourceBounds.Width / 2f, _txrSourceBounds.Height / 2f);
        }
        // For initalising the collision bounds
        public virtual void InitBounds(Vector2 position = default, bool canCollide = false, Vector2 collisionScale = default, Vector2 drawScale = default)
        {
            if (position != Vector2.Zero)
                _position = position;

            _canCollide = canCollide;

            _collisionScale = collisionScale != Vector2.Zero ? collisionScale : Vector2.One;
            _drawScale = drawScale != Vector2.Zero ? drawScale : Vector2.One;

            _visibleBounds = new Rectangle(
                (_position - _origin).ToPoint(),
                (_txrSourceBounds.Size.ToVector2() * _drawScale).ToPoint()
            );

            _collisionBounds = new Rectangle(
                (_position - (_origin * _collisionScale)).ToPoint(),
                (_txrSourceBounds.Size.ToVector2() * _drawScale * _collisionScale).ToPoint()
            );
        }
        // For initalising the movement of all sprites 
        public virtual void InitMovement(
            bool canMove = false,
            bool canFlip = false,
            Vector2 velocity = default,
            Vector2 gravity = default,
            float friction = default,
            float drag = default,
            float restitution = default,
            float maxSpeed = default)
        {
            _canMove = canMove;

            if (_canMove)
            {
                _canFlip = canFlip;

                if (velocity != Vector2.Zero) _velocity = velocity;

                _friction = friction != 0f ? friction : 0.004f; // FIX
                _drag = drag;
                _restitution = restitution;
                _maxSpeed = maxSpeed != 0f ? maxSpeed : 5000f;
            }
            else
            {
                _velocity = Vector2.Zero;
                _maxSpeed = 5000f;
                _canFlip = false;
            }

            _flipEffect = (_canMove && _canFlip && _velocity.X < 0f)
                ? SpriteEffects.FlipHorizontally
                : SpriteEffects.None;
        }

        #endregion

        #region ***** Update *****

        public virtual void Update(GameTime gameTime)
        {
            if (_dead) return;

            UpdateMovement(gameTime);
            ResolveTilemapCollision(_gameRoot._tilemap);
            UpdateBounds(gameTime);
            UpdateCollision(gameTime);
        }
        // Updates movement of all sprites
        protected virtual void UpdateMovement(GameTime gameTime)
        {
            if (!_canMove) return;

            _velocity = Game1.ClampVec2(_velocity, _maxSpeed);
            _velocity *= 1f - _drag;

            if (Math.Abs(_velocity.X) < 4) _velocity.X = 0f;
            if (Math.Abs(_velocity.Y) < 4) _velocity.Y = 0f;

            if (_hasContact && !(this is SpritePlayer p && p._isKnockedback))
            {
                _velocity *= 1f - _friction;
            }

            _position += _velocity * GBL.DeltaTime;

            if (_canFlip)
            {
                // locks character so it doesnt flip when knockedback
                if (this is SpritePlayer player && player._isKnockedback)
                {
                    _flipEffect = player._lockedFlipEffect;
                }
                else
                {
                    if (_velocity.X < 0f) _flipEffect = SpriteEffects.FlipHorizontally;
                    else if (_velocity.X > 0f) _flipEffect = SpriteEffects.None;
                }
                if (this is SpritePlayer pl && pl._isMoving == false)
                {
                    _flipEffect = pl._lockedFlipEffect;
                }
            }
        }

        protected void UpdateBounds(GameTime gameTime)
        {
            _visibleBounds.Location = (_position - _origin).ToPoint();

            if (!_canCollide) return;

            _collisionBounds.Location = (_position - (_origin * _collisionScale)).ToPoint();
        }
        // for updating collision
        protected void UpdateCollision(GameTime gameTime)
        {
            if (!_canCollide || !_canMove) return;

            _hascontactPrev = _hasContact;
            _hasContact = false;

            foreach (Sprite other in _gameRoot._spriteList)
            {
                if (IsColliding(other, gameTime))
                {
                    _hasContact = true;
                    OnCollideEvent(other);
                }
            }
        }
        public void ResolveTilemapCollision(TilemapGroup group)
        {
            var map = group.Layers[0];

            if (!_canCollide) return;

            Rectangle bounds = _collisionBounds;

            int leftTile = (int)(bounds.Left / map.TileWidth);
            int rightTile = (int)(bounds.Right / map.TileWidth);
            int topTile = (int)(bounds.Top / map.TileHeight);
            int bottomTile = (int)(bounds.Bottom / map.TileHeight);

            for (int y = topTile; y <= bottomTile; y++)
            {
                for (int x = leftTile; x <= rightTile; x++)
                {
                    int worldX = (int)(x * map.TileWidth);
                    int worldY = (int)(y * map.TileHeight);

                    if (!group.IsSolidAtWorld(worldX, worldY))
                        continue;

                    if (group.IsExitAtWorld(worldX, worldY))
                    {
                        _gameRoot.NextLevel();
                        return;
                    }
                    if(group.IsChestAtWorld(worldX, worldY))
                    {
                        return;
                    }

                    Rectangle tileRect = new Rectangle(
                        worldX,
                        worldY,
                        (int)map.TileWidth,
                        (int)map.TileHeight
                    );

                    Rectangle intersection = Rectangle.Intersect(_collisionBounds, tileRect);

                    if (!intersection.IsEmpty)
                    {
                        Vector2 depenetration = Vector2.Zero;

                        if (intersection.Height < intersection.Width)
                        {
                            depenetration.Y = (_position.Y < tileRect.Center.Y)
                                ? -intersection.Height
                                : intersection.Height;

                            _velocity.Y = 0;
                        }
                        else
                        {
                            depenetration.X = (_position.X < tileRect.Center.X)
                                ? -intersection.Width
                                : intersection.Width;

                            _velocity.X = 0;
                        }

                        _position += depenetration;
                        _collisionBounds.Location = (_position - (_origin * _collisionScale)).ToPoint();
                    }
                }
            }
        }

        #endregion

        #region ***** Collision *****
        // checking if something can collide
        public virtual bool CanCollide(Sprite other)
        {
            if (!_canCollide || _dead || other == null || other._dead)
                return false;

            if ((CollisionMask & other.CollisionLayer) == 0)
                return false;

            if ((other.CollisionMask & CollisionLayer) == 0)
                return false;

            return true;
        }
        // Checking if something is already colliding with another sprite
        public bool IsColliding(Sprite other, GameTime gameTime)
        {
            if (!CanCollide(other) || other == this)
                return false;

            Rectangle intersection = Rectangle.Intersect(_collisionBounds, other._collisionBounds);
            if (intersection.IsEmpty)
                return false;

            Vector2 depenetration = Vector2.Zero;
            
            if (intersection.Height < intersection.Width)
            {
                depenetration.Y = (_position.Y < other._position.Y)
                    ? -intersection.Height
                    : intersection.Height;

                _velocity.Y = 0f;
            }
            else
            {
                depenetration.X = (_position.X < other._position.X)
                    ? -intersection.Width
                    : intersection.Width;

                _velocity.X = 0f;
            }
            

            if (!other._canMove)
            {
                _position += depenetration;
                UpdateBounds(gameTime);

                // Only zero out velocities if not knockedback
                if (_gameRoot._player._isKnockedback == false)
                {
                    if (depenetration.Y != 0f) _velocity.Y = 0f;
                    if (depenetration.X != 0f) _velocity.X = 0f;
                }
            }

            return true;
        }

        // for when collision happens
        protected virtual void OnCollideEvent(Sprite otherSprite) { }

        #endregion

        #region ***** Draw *****

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (_dead || _isHidden) return;

            spriteBatch.Draw(
                _texture,
                _visibleBounds,
                _txrSourceBounds,
                Color.White,
                _rotation,
                Vector2.Zero,
                _flipEffect,
                _layerDepth
            );
        }

        #endregion
    }
}

