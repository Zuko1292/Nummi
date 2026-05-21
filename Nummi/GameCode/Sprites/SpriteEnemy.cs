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
        // the _health is current health and the max health is well max health
        public int _health;
        public int _maxHealth;
        // this is used to store the last position the enemy saw the player, so that they can move towards it even if they can't see the player anymore. This makes the enemy feel more intelligent and less frustrating to fight, as they will continue to chase you for a short time after losing sight of you, rather than immediately giving up and going back to patrolling.
        public Vector2 _lastSeenPos;
        public int _knockbackStrength;
        public int _damageStrength;
        public bool _isBoss;
        public float _aggrorange;
        public bool _isIndestructible = false;
        private Attack _lastAttack = null;

        // Patrolling variables
        public bool _isPatrolling = true;
        protected float _walkingArea = 50f;
        public bool _canPatrol = false;

        // Dashing variables
        protected bool _hasDashed = true;
        public bool _isChargingDash = false;

        protected virtual bool IsDashing() => false;

        // Invincibility frames variables
        protected float _damageCooldown = 0.5f;
        protected float _damageTimer = 0f;
        public bool _isInvincible = false;

        // Knockback variables
        public bool _isKnockedback = false;
        protected float _knockbackTimer = 0f;
        protected float _knockbackDuration = 0.2f;

        // Base xp value for killing the enemy, can be modified by the type of enemy or if it's a boss
        public float _xpValue = 10f;

        public SpriteEffects _lockedFlipEffect;

        private Vector2 Direction;

        // override like this and if you want to do anything before it dies then do the _dead = value after and if you want to play an animation put the _dead value into the animation finished function
        public override bool Dead
        {
            set
            {
                _dead = value;
                _gameRoot._player.OnEnemyKilled(_xpValue);
            }

            get
            {
                return _dead;
            }
        }

        public SpriteEnemy(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove, int health, int knockbackStrength, int damageStrength, bool isBoss, float moveSpeed, float aggroRange, float xpValue, int goldValue)
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
            _aggrorange = aggroRange;
            _canFlip = true;
            _xpValue = xpValue;
            _maxHealth = health;
        }

        public override void Update(GameTime gameTime)
        {
            // This checks if the enemy can see the player, and if they can, it updates the last seen position to the player's current position. This allows the enemy to continue moving towards the player even if they lose sight of them, creating a more dynamic and engaging combat experience.
            // This handles the patrolling behavior of the enemy, making them move back and forth within a certain area. The enemy will switch directions when they reach the edge of their walking area, creating a simple but effective patrolling pattern.
            if (_isKnockedback)
            {
                _knockbackTimer -= GBL.DeltaTime;

                if (_knockbackTimer <= 0f)
                {
                    _isKnockedback = false;
                }
            }
            // This handles the invincibility frames of the enemy, making them temporarily invincible after taking damage. This prevents the enemy from taking multiple hits in quick succession, creating a more balanced and fair combat experience.
            if (_isInvincible)
            {
                _damageTimer -= GBL.DeltaTime;
                if (_damageTimer <= 0f)
                {
                    _isInvincible = false;
                }
            }
            if (_health <= 0)
            {
                Dead = true;
            }
            if(_gameRoot._headsLevel == 4)
            {
                foreach (Waiter w in _gameRoot._spriteList)
                {
                    if (w._tempState == Waiter.TempState.Frozen) return;
                }

            }
            if (_gameRoot.canSeePlayer)
            {
                _lastSeenPos = _gameRoot._player._position;
            }

            // This makes the enemy move towards the last seen position of the player if they are not patrolling, and it also handles the knockback and invincibility timers. The enemy will continue to move towards the last seen position for a short time after losing sight of the player, making them feel more intelligent and less frustrating to fight.
            if (!_isPatrolling)
            { 
                Direction = _lastSeenPos - _position;

                if (Direction != Vector2.Zero)
                {
                    Direction.Normalize();
                }
                if (!_isKnockedback && !IsDashing())
                {
                    _velocity = Direction * _moveSpeed;
                }
            }

            base.Update(gameTime);
        }
        // Use this when you want enemy to take damage
        public void TakeDamage(int amount)
        {
            _health -= amount;
        }
        // do the oncollide event like this for every class
        protected override void OnCollideEvent(Sprite otherSprite)
        {
            base.OnCollideEvent(otherSprite);
            if (otherSprite is Attack weapon)
            {
                if (weapon != _lastAttack && _gameRoot._player._currentWeapon != 4 && !_isIndestructible)
                {
                    _lastAttack = weapon;

                    TakeDamage((int)weapon._weaponDamage);

                    _isKnockedback = true;
                    _knockbackTimer = _knockbackDuration;

                    _lockedFlipEffect = _flipEffect;

                    Vector2 diff = _position - _gameRoot._player._position;
                    Vector2 knockbackDirection = diff == Vector2.Zero ? new Vector2(0f, -1f) : Vector2.Normalize(diff);
                    _velocity += knockbackDirection * 230;
                }
            }
        }
    }
    // Class for enemy projectiles
    public class SpriteEnemyProjectile : SpriteAnimating
    {
        public int _damageStrength;
        protected float _moveSpeed;

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
            _moveSpeed = moveSpeed;
        }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            base.OnCollideEvent(otherSprite);

            if (otherSprite is SpritePlayer player)
            {
                // TODO prolly make a function for taking damage so you arent hard coding it with the variable
                _gameRoot._health -= _damageStrength;
            }

            Dead = true;
        }
        // so that the projectile will also die if it hits a wall, you can change this to make it bounce or something if you want by overriding it in the child class
        protected override void OnTileCollideEvent(int tileX, int tileY)
        {
            Dead = true;
        }
    }
}
