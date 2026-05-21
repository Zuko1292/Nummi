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
        public int _goldValue;

        // Patrolling variables
        public bool _isPatrolling = true;
        protected float _walkingArea = 50f;
        public bool _canPatrol = false;

        // When true the enemy never self-propels (chase/patrol movement is skipped and its
        // velocity is forced to zero each frame), but it can still be hit and knocked back.
        // Used for frozen enemies that must hold their position.
        public bool _isStationary = false;

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
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
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
            _goldValue = goldValue;
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
            if (_gameRoot.canSeePlayer)
            {
                _lastSeenPos = _gameRoot._player._position;
            }

            // This makes the enemy move towards the last seen position of the player if they are not patrolling, and it also handles the knockback and invincibility timers. The enemy will continue to move towards the last seen position for a short time after losing sight of the player, making them feel more intelligent and less frustrating to fight.
            if (!_isPatrolling && !_isStationary)
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

            // Stationary enemies hold their position, but knockback impulses are still allowed
            // to play out (this runs before the movement integration in base.Update).
            if (_isStationary && !_isKnockedback)
            {
                _velocity = Vector2.Zero;
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
                if (weapon != _lastAttack && !_isIndestructible && (weapon is Arrow || _gameRoot._player._currentWeapon != 4))
                {
                    _lastAttack = weapon;

                    TakeDamage((int)weapon._weaponDamage);

                    _isKnockedback = true;
                    _knockbackTimer = _knockbackDuration;

                    _lockedFlipEffect = _flipEffect;

                    Vector2 diff = _position - _gameRoot._player._position;
                    Vector2 knockbackDirection = diff == Vector2.Zero ? new Vector2(0f, -1f) : Vector2.Normalize(diff);
                    _velocity += knockbackDirection * 230;

                    if (weapon is Arrow) weapon.Dead = true;
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
    // trigger zone is an invisible rectangle that detects when a SpriteEnemy enters it.
    public class TriggerZone : Sprite
    {
        // Tracks which enemies are currently inside the zone so we can fire
        // enter/exit events
        private readonly HashSet<SpriteEnemy> _enemiesInside = new HashSet<SpriteEnemy>();

        // Width and height of the trigger in world pixels.
        public int Width { get; private set; }
        public int Height { get; private set; }

        public TriggerZone(Game1 gameRoot, Vector2 position, int width, int height)
            : base(gameRoot)
        {
            Width = width;
            Height = height;

            InitTexture(gameRoot._defaultTxr);

            InitBounds(position, canCollide: true, collisionScale: Vector2.One);

            _collisionBounds = new Rectangle(
                (int)(position.X - width / 2f),
                (int)(position.Y - height / 2f),
                width,
                height
            );

            _visibleBounds = _collisionBounds; // not drawn anyway

            InitMovement(canMove: false);

            _isHidden = true;
            _canMove = false;
            _canCollide = true; 

            CollisionLayer = CollisionLayer.None;  
            CollisionMask = CollisionLayer.Enemy;  
        }
        public override void Update(GameTime gameTime)
        {
            if (_dead) return;

            // Re-sync bounds in case the zone was moved at runtime.
            _collisionBounds = new Rectangle(
                (int)(_position.X - Width / 2f),
                (int)(_position.Y - Height / 2f),
                Width,
                Height
            );

            HashSet<SpriteEnemy> currentlyInside = new HashSet<SpriteEnemy>();

            foreach (Sprite sprite in _gameRoot._spriteList)
            {
                if (sprite is SpriteEnemy enemy && !enemy.Dead)
                {
                    if (_collisionBounds.Intersects(enemy._collisionBounds))
                    {
                        currentlyInside.Add(enemy);

                        if (!_enemiesInside.Contains(enemy))
                        {
                            // Enemy just entered this frame.
                            OnEnemyEntered(enemy);
                        }
                        else
                        {
                            // Enemy is staying inside.
                            OnEnemyStay(enemy);
                        }
                    }
                }
            }

            // Any enemy that was inside last frame but isn't now has left.
            foreach (SpriteEnemy enemy in _enemiesInside)
            {
                if (!currentlyInside.Contains(enemy))
                {
                    OnEnemyExited(enemy);
                }
            }

            _enemiesInside.Clear();
            foreach (SpriteEnemy e in currentlyInside) _enemiesInside.Add(e);
        }

        public bool HasEnemyInside => _enemiesInside.Count > 0;

        // Convenience: get a snapshot of all enemies currently inside.
        public IReadOnlyCollection<SpriteEnemy> EnemiesInside => _enemiesInside;

        // ------------------------------------------------------------------
        // Override these in subclasses to add custom behaviour.
        // ------------------------------------------------------------------

        // Called once when an enemy first enters the zone.
        protected virtual void OnEnemyEntered(SpriteEnemy enemy) { }

        // Called every frame while an enemy remains inside the zone.
        protected virtual void OnEnemyStay(SpriteEnemy enemy) { }

        // Called once when an enemy leaves the zone.
        protected virtual void OnEnemyExited(SpriteEnemy enemy) { }

        // Zone is invisible — drawing is intentionally skipped.
        public override void Draw(SpriteBatch spriteBatch) { }
    }

    public class DetectionZone : TriggerZone
    {
        private bool _triggered = false;
        int _roomNum;
        public DetectionZone(Game1 gameRoot, Vector2 position, int width, int height, int roomNum)
            : base(gameRoot, position, width, height)
        {
            _roomNum = roomNum;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            Debug.WriteLine($"Enemies Inside: {EnemiesInside.Count}");

            if(EnemiesInside.Count <= 0)
            {
                var map = _gameRoot._tilemap.Layers[0];
                // These will take down the walls when all enemies inside the room are killed
                if (_roomNum == 1)
                {
                    if (_gameRoot._headsLevel == 6)
                    {
                        map.SetTile(46, 17, 1);
                        map.SetTile(46, 18, 1);
                        map.SetTile(46, 19, 1);
                        map.SetTile(46, 20, 1);
                        map.SetTile(46, 21, 1);
                        map.SetTile(46, 22, 1);
                        map.SetTile(46, 23, 1);
                        map.SetTile(46, 24, 1);
                        map.SetTile(46, 25, 1);
                        map.SetTile(46, 26, 1);
                        map.SetTile(46, 27, 1);
                        map.SetTile(46, 28, 1);
                        map.SetTile(46, 29, 1);
                        map.SetTile(46, 30, 1);
                        map.SetTile(46, 31, 1);
                        map.SetTile(46, 32, 1);
                        map.SetTile(46, 33, 1);
                        map.SetTile(46, 34, 1);
                        map.SetTile(46, 35, 1);
                        map.SetTile(46, 36, 1);
                        map.SetTile(46, 37, 1);
                    }
                    if (_gameRoot._headsLevel == 7)
                    {
                        map.SetTile(84, 17, 1);
                        map.SetTile(84, 18, 1);
                        map.SetTile(84, 19, 1);
                        map.SetTile(84, 20, 1);
                        map.SetTile(84, 21, 1);
                        map.SetTile(84, 22, 1);
                        map.SetTile(84, 23, 1);
                        map.SetTile(84, 24, 1);
                        map.SetTile(84, 25, 1);
                        map.SetTile(84, 26, 1);
                        map.SetTile(84, 27, 1);
                        map.SetTile(84, 28, 1);
                        map.SetTile(84, 29, 1);
                        map.SetTile(84, 30, 1);
                        map.SetTile(84, 31, 1);
                        map.SetTile(84, 32, 1);
                        map.SetTile(84, 33, 1);
                        map.SetTile(84, 34, 1);
                        map.SetTile(84, 35, 1);
                        map.SetTile(84, 36, 1);
                        map.SetTile(84, 37, 1);
                    }
                }
                else if (_roomNum == 2)
                {
                    map.SetTile(65, 17, 1);
                    map.SetTile(65, 18, 1);
                    map.SetTile(65, 19, 1);
                    map.SetTile(65, 20, 1);
                    map.SetTile(65, 21, 1);
                    map.SetTile(65, 22, 1);
                    map.SetTile(65, 23, 1);
                    map.SetTile(65, 24, 1);
                    map.SetTile(65, 25, 1);
                    map.SetTile(65, 26, 1);
                    map.SetTile(65, 27, 1);
                    map.SetTile(65, 28, 1);
                    map.SetTile(65, 29, 1);
                    map.SetTile(65, 30, 1);
                    map.SetTile(65, 31, 1);
                    map.SetTile(65, 32, 1);
                    map.SetTile(65, 33, 1);
                    map.SetTile(65, 34, 1);
                    map.SetTile(65, 35, 1);
                    map.SetTile(65, 36, 1);
                    map.SetTile(65, 37, 1);
                }
                else if (_roomNum == 3)
                {
                    if (_gameRoot._headsLevel == 6)
                    {
                        map.SetTile(84, 17, 1);
                        map.SetTile(84, 18, 1);
                        map.SetTile(84, 19, 1);
                        map.SetTile(84, 20, 1);
                        map.SetTile(84, 21, 1);
                        map.SetTile(84, 22, 1);
                        map.SetTile(84, 23, 1);
                        map.SetTile(84, 24, 1);
                        map.SetTile(84, 25, 1);
                        map.SetTile(84, 26, 1);
                        map.SetTile(84, 27, 1);
                        map.SetTile(84, 28, 1);
                        map.SetTile(84, 29, 1);
                        map.SetTile(84, 30, 1);
                        map.SetTile(84, 31, 1);
                        map.SetTile(84, 32, 1);
                        map.SetTile(84, 33, 1);
                        map.SetTile(84, 34, 1);
                        map.SetTile(84, 35, 1);
                        map.SetTile(84, 36, 1);
                        map.SetTile(84, 37, 1);
                    }
                    if (_gameRoot._headsLevel == 7)
                    {
                        map.SetTile(46, 17, 1);
                        map.SetTile(46, 18, 1);
                        map.SetTile(46, 19, 1);
                        map.SetTile(46, 20, 1);
                        map.SetTile(46, 21, 1);
                        map.SetTile(46, 22, 1);
                        map.SetTile(46, 23, 1);
                        map.SetTile(46, 24, 1);
                        map.SetTile(46, 25, 1);
                        map.SetTile(46, 26, 1);
                        map.SetTile(46, 27, 1);
                        map.SetTile(46, 28, 1);
                        map.SetTile(46, 29, 1);
                        map.SetTile(46, 30, 1);
                        map.SetTile(46, 31, 1);
                        map.SetTile(46, 32, 1);
                        map.SetTile(46, 33, 1);
                        map.SetTile(46, 34, 1);
                        map.SetTile(46, 35, 1);
                        map.SetTile(46, 36, 1);
                        map.SetTile(46, 37, 1);
                    }
                }
                else if (_roomNum == 4)
                {
                    if (_gameRoot._headsLevel == 6)
                    {
                        map.SetTile(103, 17, 1);
                        map.SetTile(103, 18, 1);
                        map.SetTile(103, 19, 1);
                        map.SetTile(103, 20, 1);
                        map.SetTile(103, 21, 1);
                        map.SetTile(103, 22, 1);
                        map.SetTile(103, 23, 1);
                        map.SetTile(103, 24, 1);
                        map.SetTile(103, 25, 1);
                        map.SetTile(103, 26, 1);
                        map.SetTile(103, 27, 1);
                        map.SetTile(103, 28, 1);
                        map.SetTile(103, 29, 1);
                        map.SetTile(103, 30, 1);
                        map.SetTile(103, 31, 1);
                        map.SetTile(103, 32, 1);
                        map.SetTile(103, 33, 1);
                        map.SetTile(103, 34, 1);
                        map.SetTile(103, 35, 1);
                        map.SetTile(103, 36, 1);
                        map.SetTile(103, 37, 1);
                    }
                }
            }
        }

        protected override void OnEnemyEntered(SpriteEnemy enemy)
        {
            if (_triggered) return;
            _triggered = true;
        }

        protected override void OnEnemyExited(SpriteEnemy enemy)
        {
            if (!HasEnemyInside) _triggered = false;

            
        }
    }
}
