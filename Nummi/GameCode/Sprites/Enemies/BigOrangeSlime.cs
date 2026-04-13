using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class BigOrangeSlime : SpriteEnemy
    {
        protected bool _canShoot = true;
        float _shootTimer, _shootInterval;

        private Vector2 _dashDirection;

        public float _chargeDashTimer = 0f;
        public float _chargeDashDuration = 0.5f;
        private float _dashCooldown = 8f;
        private float _dashCooldownTimer = 0f;

        public bool _isDashing = false;
        protected float _dashTimer = 0f;
        protected float _dashDuration = 0.1f;
        private Vector2 _dashTarget;

        private float _dashSpeed = 200f;

        public BigOrangeSlime(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet-Big"), position, true, 100, 250, 20, false, 25)
        {

        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 10f;
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

            // Charging Dash
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 0, 64, 48));
            animations[2].Add(new Rectangle(64, 0, 64, 48));
            animations[2].Add(new Rectangle(128, 0, 64, 48));
            animations[2].Add(new Rectangle(192, 0, 64, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            if (_velocity == Vector2.Zero && !_isChargingDash) SetAnimation(0);
            else if (_velocity != Vector2.Zero)SetAnimation(1);

            if (_isDashing)
            {
                _isChargingDash = false;

                _dashTimer -= GBL.DeltaTime;

                Vector2 toTarget = _dashTarget - _position;

                if (toTarget.Length() < 40f)
                {
                    _isDashing = false;
                    _hasDashed = true;
                    _velocity = Vector2.Zero;
                }
                else
                {
                    _velocity = _dashDirection * _dashSpeed;
                }

                if (_dashTimer <= 0f)
                {
                    _isDashing = false;
                }
            }

            if(!_isDashing)
            {
                _dashCooldownTimer += GBL.DeltaTime;
                if(_dashCooldownTimer >= _dashCooldown)
                {
                    _isChargingDash = true;
                    _dashCooldownTimer = 8f;
                }
            }

            if (_lastSeenTimer <= 0.2f) SetAnimation(0);

            if(!_canShoot) 
            {
                _shootTimer += GBL.DeltaTime;
                if(_shootTimer >= _shootInterval)
                {
                    _canShoot = true;
                    _shootTimer = 0f;
                }
            }
            else if(_canShoot && !_isChargingDash && !_isDashing)
            {
                ShootProjectile();
                _canShoot = false;
                _shootInterval = 2f; 
            }

            if(_isChargingDash && !_isDashing)
            {
                _chargeDashTimer += GBL.DeltaTime;

                _velocity = Vector2.Zero;

                SetAnimation(2);
                if(_chargeDashTimer >= _chargeDashDuration)
                {
                    _isDashing = true;
                    _dashTimer = _dashDuration;

                    _dashTarget = _gameRoot._player._position;

                    Vector2 dashDirection = _dashTarget - _position;

                    if (dashDirection != Vector2.Zero)
                        _dashDirection = Vector2.Normalize(dashDirection);

                    _isChargingDash = false;
                }
            }

            base.Update(gameTime);
        }

        public void ShootProjectile()
        {

            Vector2 projectilePosition = _position; 
            BigOrangeSlimeProjectile projectile = new BigOrangeSlimeProjectile(_gameRoot, projectilePosition, _gameRoot._player._position);
            _gameRoot._newSpriteList.Add(projectile);
        }
    }

    public class BigOrangeSlimeProjectile : SpriteEnemyProjectile
    {

        public BigOrangeSlimeProjectile(Game1 gameRoot, Vector2 position, Vector2 target)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange-Slime-Projectile"), position, 200f, 10)
        {
            CollisionLayer = CollisionLayer.Enemy;
            CollisionMask = CollisionLayer.Player;
            InitBounds(position, true, new Vector2(1f, 1f));
            _layerDepth = 0.1f;
            _velocity = Vector2.Normalize(target - _position) * 200f;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Flying
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 8, 8));
            animations[0].Add(new Rectangle(8, 0, 8, 8));
            animations[0].Add(new Rectangle(16, 0, 8, 8));
            animations[0].Add(new Rectangle(24, 0, 8, 8));
            animations[0].Add(new Rectangle(32, 0, 8, 8));
            animations[0].Add(new Rectangle(40, 0, 8, 8));
            animations[0].Add(new Rectangle(48, 0, 8, 8));
            animations[0].Add(new Rectangle(56, 0, 8, 8));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            base.OnCollideEvent(otherSprite);

            if(otherSprite is SpritePlayer player)
            {
                _gameRoot._health -= _damageStrength;
            }

            Dead = true;
        }

        protected override void OnTileCollideEvent(int tileX, int tileY)
        {
            Dead = true;
        }
    }
}
