using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Nummi;

namespace Nummi
{
    public class SpriteEnemy : SpriteCharacter
    {
        protected float _moveSpeed;
        public int _health;
        public Vector2 _lastSeenPos;
        public int _knockbackStrength;
        public int _damageStrength;
        public bool _isBoss;

        public bool _isPatrolling = true;
        protected float _walkingArea = 50f;
        public bool _canPatrol = false;  
        protected bool _hasDashed = false;
        public bool _isChargingDash = false;

        protected float _damageCooldown = 0.5f;
        protected float _damageTimer = 0f;
        public bool _isInvincible = false;

        public bool _isKnockedback = false;
        protected float _knockbackTimer = 0f;
        protected float _knockbackDuration = 0.2f;

        public SpriteEffects _lockedFlipEffect;

        private Vector2 Direction;

        public override bool Dead
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

        public SpriteEnemy(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove, int health, int knockbackStrength, int damageStrength, bool isBoss, float moveSpeed)
            : base(gameRoot, texture, position, canMove)
        {
            CollisionLayer = CollisionLayer.Enemy;
            CollisionMask = CollisionLayer.All & ~CollisionLayer.Enemy;
            InitBounds(position, true, new Vector2(1f, 1f));
            _layerDepth = 0.1f;
            _health = health;
            _knockbackStrength = knockbackStrength;
            _damageStrength = damageStrength;
            _isBoss = isBoss;
            _moveSpeed = moveSpeed;
            _canFlip = true;
        }

        public override void Update(GameTime gameTime)
        {

            if (_gameRoot.canSeePlayer)
            {
                _lastSeenPos = _gameRoot._player._position;
            }

            if (_health <= 0)
            {
                Dead = true;
            }

            if (!_isPatrolling)
            { 
                Direction = _lastSeenPos - _position;

                if (Direction != Vector2.Zero)
                {
                    Direction.Normalize();
                }
                if (!_isKnockedback && _hasDashed && !_isChargingDash)  _velocity = Direction * _moveSpeed;
            }

            if (_isKnockedback)
            {
                _knockbackTimer -= GBL.DeltaTime;

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

            base.Update(gameTime);
        }

        public void TakeDamage(int amount)
        {
            _health -= amount;
        }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            base.OnCollideEvent(otherSprite);
            if (otherSprite is Attack weapon)
            {
                if (!_isInvincible && _gameRoot._player._currentWeapon != 4 && !_isBoss)
                {
                    TakeDamage((int)weapon._weaponDamage);

                    _isInvincible = true;
                    _damageTimer = _damageCooldown;

                    _isKnockedback = true;
                    _knockbackTimer = _knockbackDuration;

                    _lockedFlipEffect = _flipEffect;

                    Vector2 knockbackDirection = Vector2.Normalize(_position - weapon._position);
                    _velocity += knockbackDirection * 200;                   
                }
            }
        }
    }

    public class SpriteEnemyProjectile : SpriteAnimating
    {
        public int _damageStrength;

        public override bool Dead
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
        public SpriteEnemyProjectile(Game1 gameRoot, Texture2D texture, Vector2 position, float moveSpeed, int damageStrength)
            : base(gameRoot, texture, position, true, true)
        {
            CollisionLayer = CollisionLayer.Enemy;
            CollisionMask = CollisionLayer.Player;
            InitBounds(position, true, new Vector2(1f, 1f));
            _layerDepth = 0.1f;
            _damageStrength = damageStrength;
        }
    }
}
