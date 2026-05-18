using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
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

        // Variable for checking if the player is in the trap room
        public bool _isInTrapRoom = false;

        public int _currentWeapon = 0; // 0 = Sword, 1 = Great Sword, 2 = Mace, 3 = Great Hammer, 4 = Bow
        // The damage cooldown and invincibility variables are used to give the player a brief period of invincibility after taking damage, which is a common mechanic in many games to prevent the player from taking rapid consecutive hits. The knockback variables are used to apply a knockback effect when the player takes damage, which can add a sense of impact and make combat feel more dynamic. The dash variables are used to implement a dash mechanic, allowing the player to quickly move in a direction for a short duration, which can be useful for dodging attacks or quickly closing distance with enemies. The blocking variable allows the player to block incoming attacks, which can add an additional layer of strategy to combat.
        protected float _damageCooldown = 0.5f;
        protected float _damageTimer = 0f;
        protected bool _isInvincible = false;

        public bool _isKnockedback = false;
        protected float _knockbackTimer = 0f;
        protected float _knockbackDuration = 0.2f;

        public bool _isDashing = false;
        protected float _dashTimer = 0f;
        protected float _dashDuration = 0.3f;

        private float _dashCooldown = 1f;
        private float _dashCooldownTimer = 0f;

        private Vector2 _dashDirection;

        private float _dashSpeed = 400f;

        public bool _isBlocking = false;
        public bool _isMoving = false;
        private bool _facingLeft = false;

        // checks mouse direction for attacking
        public Vector2 _mouseDirection;

        // This variable is used to determine whether the player's current animation is based on vertical or horizontal movement, which can be useful for determining the direction of attacks or other actions that depend on the player's orientation. By checking whether the current animation index corresponds to a vertical or horizontal movement animation, the game can adjust the player's actions accordingly, such as changing the direction of an attack or adjusting the hitbox for a weapon swing.
        private bool _yORx;
        // Checks if player is attacking
        public bool _attacking = false;

        public SpriteEffects _lockedFlipEffect;
        // Loads stats
        public CharacterStats Stats { get; private set; }
        // Loads level
        public LevelSystem LevelSystem { get; private set; }

        #endregion ***** Member variables *****

        #region ***** Constructors *****

        // This Constructor is used when starting a new game, so the player starts with default stats and level progression
        public SpritePlayer(Game1 gameRoot, Vector2 position, bool canMove)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), position, canMove, true)
        {
            _restitution = 0.0f;
            _friction = 0.25f;
            _drag = 0.0f;

            _layerDepth = 0.3f;

            _canFlip = true;

            CollisionLayer = CollisionLayer.Player;
            CollisionMask = CollisionLayer.All;

            Stats = new CharacterStats(str: 5, vit: 5);
            LevelSystem = new LevelSystem();

            LevelSystem.OnLevelUp += HandleLevelUp;
        }

        // This Constructor is used when going to a new level, so the player keeps their stats and level progression
        public SpritePlayer(Game1 gameRoot, Vector2 position, bool canMove, CharacterStats existingStats, LevelSystem existingLevel)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Player_SpriteSheet"), position, canMove, true)
        {
            _restitution = 0.0f;
            _friction = 0.25f;
            _drag = 0.0f;

            _layerDepth = 0.3f;

            _canFlip = true;

            CollisionLayer = CollisionLayer.Player;
            CollisionMask = CollisionLayer.All;

            Stats = existingStats;
            LevelSystem = existingLevel;
            LevelSystem.OnLevelUp += HandleLevelUp;
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
            animations[7].Add(new Rectangle(0, 128, 32, 32));

            // Blocking
            animations.Add(new List<Rectangle>());
            animations[8].Add(new Rectangle(96, 0, 32, 32));

            // Dash Up
            animations.Add(new List<Rectangle>());
            animations[9].Add(new Rectangle(256, 64, 32, 32));

            // Dash Down
            animations.Add(new List<Rectangle>());
            animations[10].Add(new Rectangle(256, 0, 32, 32));

            // Dash Side
            animations.Add(new List<Rectangle>());
            animations[11].Add(new Rectangle(256, 32, 32, 32));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        #endregion ***** Member methods: Initialisation *****

        #region ***** Member methods: Update *****

        public override void Update(GameTime gameTime)
        {
            // Blocking
            if (GBL.KeyHold(Keys.F))
            {
                SetAnimation(8);
                _isBlocking = true;
                _velocity = Vector2.Zero;
            }
            else if (_animIndex == 8 && !GBL.KeyHold(Keys.F))
            {
                SetAnimation(0);
                _isBlocking = false;
            }
            // Dashing
            if (_isDashing)
            {
                _dashTimer -= GBL.DeltaTime;

                _velocity = _dashDirection * _dashSpeed;

                if (_dashTimer <= 0f)
                {
                    _isDashing = false;
                }
            }
            
            if (GBL.KeyPress(Keys.Q) && !_isDashing && _dashCooldownTimer <= 0f)
            {
                _isDashing = true;
                _dashTimer = _dashDuration;

                Vector2 dashDirection = Vector2.Zero;

                if (_velocity.X > 0) dashDirection.X = 1f;
                else if (_velocity.X < 0) dashDirection.X = -1f;

                if (_velocity.Y > 0) dashDirection.Y = 1f;
                else if (_velocity.Y < 0) dashDirection.Y = -1f;

                if (dashDirection == Vector2.Zero)
                {
                    dashDirection.X = _facingLeft ? -1f : 1f;
                }

                if (dashDirection != Vector2.Zero)
                    _dashDirection = Vector2.Normalize(dashDirection);

                if (Math.Abs(_dashDirection.X) >= Math.Abs(_dashDirection.Y))
                {
                    SetAnimation(11); 
                }
                else
                {
                    if (_dashDirection.Y < 0)
                        SetAnimation(9); 
                    else
                        SetAnimation(10); 
                }

                _dashCooldownTimer = _dashCooldown;
            }

            if(!_isDashing) _dashCooldownTimer -= GBL.DeltaTime;
            // Attacking
            if (_animIndex == 5 || _animIndex == 2 || _animIndex == 6) _yORx = false;
            if (_animIndex == 0 || _animIndex == 1 || _animIndex == 3 || _animIndex == 4) _yORx = true;

            if(GBL.LeftClick && !_attacking && !_isBlocking)
            {
                if (GBL._camera.ScreenToWorld(GBL.mousePos).X > _position.X && !_yORx) Right_Attacking();
                if (GBL._camera.ScreenToWorld(GBL.mousePos).X < _position.X && !_yORx) Left_Attacking();
                if (GBL._camera.ScreenToWorld(GBL.mousePos).Y > _position.Y && _yORx) Down_Attacking();
                if (GBL._camera.ScreenToWorld(GBL.mousePos).Y < _position.Y && _yORx) Up_Attacking();
            }
            // Knockback and invincibility timers
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
            if (!_isKnockedback && !_isBlocking && !_isDashing)
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

                _velocity.X = inputX * _moveSpeed;
                _velocity.Y = inputY * _moveSpeed;

                if (_velocity.X != 0 || _velocity.Y != 0)
                {
                    if (_velocity.Y > 0)
                        SetAnimation(4);
                    else if (_velocity.Y < 0)
                        SetAnimation(3);
                    else
                        SetAnimation(5);
                }


                if (_velocity.X != 0f || _velocity.Y != 0f)
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
            }

            // Clamp horizontal speed
            if(!_isDashing) _velocity.X = MathHelper.Clamp(_velocity.X, -_moveSpeed, _moveSpeed);


            base.Update(gameTime);
        }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            base.OnCollideEvent(otherSprite);
            // Enemy collision
            if (otherSprite is SpriteEnemy enemy)
            {
                if (!_isInvincible && !_isBlocking)
                {
                    _gameRoot._health -= enemy._damageStrength;
                    _isInvincible = true;
                    _damageTimer = _damageCooldown;

                    // Apply knockback
                    _isKnockedback = true;
                    _knockbackTimer = _knockbackDuration;

                    _lockedFlipEffect = _flipEffect;

                    Vector2 knockbackDirection = Vector2.Normalize(_position - enemy._position);
                    _velocity += knockbackDirection * enemy._knockbackStrength;
                }
                else if(!_isInvincible && _isBlocking)
                {
                    _isKnockedback = true;
                    _knockbackTimer = _knockbackDuration;

                    Vector2 knockbackDirection = Vector2.Normalize(_position - enemy._position);
                    _velocity += knockbackDirection * enemy._knockbackStrength;

                    _lockedFlipEffect = _flipEffect;

                }
            }
            // Weapon pickup collision
            if (otherSprite is DroppedWeapon droppedWeapon)
            {

                if (droppedWeapon._pickupCD < 0)
                {
                    PickupWeapon(droppedWeapon._animIndex);
                    droppedWeapon.Dead = true;
                }
            }
        }

        protected override void OnTileCollideEvent(int tileX, int tileY)
        {
            // Stops dashing if you hit wall
            if (_isDashing)
            {
                _isDashing = false;
            }
        }

        public void Up_Attacking()
        {
            if (_gameRoot._player == null) return;

            _attacking = true;

            Up_Attack atk = new Up_Attack(_gameRoot, _position - new Vector2(0, 24), _currentWeapon);
            _gameRoot._newSpriteList.Add(atk);
        }
        public void Down_Attacking()
        {
            if (_gameRoot._player == null) return;

            _attacking = true;
            Down_Attack atk = new Down_Attack(_gameRoot, _position + new Vector2(0, 24), _currentWeapon);
            _gameRoot._newSpriteList.Add(atk);
        }
        public void Right_Attacking()
        {
            if (_gameRoot._player == null) return;

            _attacking = true;
            Right_Attack atk = new Right_Attack(_gameRoot, _position + new Vector2(24, 0), _currentWeapon);
            _gameRoot._newSpriteList.Add(atk);
        }
        public void Left_Attacking()
        {
            if (_gameRoot._player == null) return;

            _attacking = true;
            Left_Attack atk = new Left_Attack(_gameRoot, _position - new Vector2(24, 0), _currentWeapon);
            _gameRoot._newSpriteList.Add(atk);
        }

        public void PickupWeapon(int weaponType)
        {
            _currentWeapon = weaponType;
        } 

        public void ChestOpened(Vector2 pos)
        {
            DroppedWeapon droppedWeapon = new DroppedWeapon(_gameRoot, _position, new Random().Next(0, 5));
            _gameRoot._newSpriteList.Add(droppedWeapon);
        }

        private void HandleLevelUp()
        {
            Stats.Strength.Modify(+1);
            Stats.Vitality.Modify(+1);
        }
        public void OnEnemyKilled(float xpValue)
        {
            LevelSystem.AddXP(xpValue);
        }
        #endregion ***** Member methods: Update *****
    }

    // Creates stat
    public class Stat
    {
        public string Name { get; private set; }
        public float BaseValue { get; private set; }
        public float CurrentValue { get; private set; }
        public float MaxValue { get; private set; }

        public Stat(string name, float baseValue, float maxValue)
        {
            Name = name;
            BaseValue = baseValue;
            CurrentValue = baseValue;
            MaxValue = maxValue;
        }

        public void Modify(float amount)
        {
            CurrentValue = Math.Clamp(CurrentValue + amount, 0, MaxValue);
        }

        public void Reset() => CurrentValue = BaseValue;
    }
    // Players stats
    public class CharacterStats
    {

        public Stat Strength { get; private set; }
        public Stat Vitality { get; private set; }

        public CharacterStats(int str, int vit)
        {
            Strength = new Stat("Strength", str, 99);
            Vitality = new Stat("Vitality", vit, 99);
        }

        public int MaxHP => (int)(Vitality.CurrentValue * 15);
        public int WeaponDmg => (int)(Strength.CurrentValue * 2.5f);
    }
    // Level system for player progression, it uses a simple exponential growth formula for XP requirements, where each level requires 8% more XP than the previous one. The LevelUp method is called when the player has enough XP to level up, which increases the player's level and updates the XP required for the next level. The OnLevelUp event allows other parts of the game to respond to the player leveling up, such as updating the HUD or unlocking new abilities.
    // TODO I think the scaling is a bit crazy right now so need to fix that
    public class LevelSystem
    {
        public int Level { get; private set; }
        public float CurrentXP { get; private set; }
        public float XPToNextLevel { get; private set; }

        private const float BaseXP = 100f;       
        private const float ScaleRate = 1.08f;   

        public event Action OnLevelUp;           

        public LevelSystem()
        {
            Level = 1;
            CurrentXP = 0;
            XPToNextLevel = BaseXP;
        }

        public void AddXP(float amount)
        {
            CurrentXP += amount;

            while (CurrentXP >= XPToNextLevel)
            {
                CurrentXP -= XPToNextLevel;       
                LevelUp();
            }
        }

        private void LevelUp()
        {
            Level++;
            XPToNextLevel *= ScaleRate;           
            OnLevelUp?.Invoke();                  
        }

        public float XPForLevel(int targetLevel)
        {
            float xp = BaseXP;
            for (int i = 1; i < targetLevel; i++)
                xp *= ScaleRate;
            return xp;
        }
    }
    // For Basic HUD elements like health and XP bars, and boss health bar
    public class HUD
    {
        private Texture2D _pixel;
        private SpriteFont _font;

        Game1 _gameRoot;

        public HUD(Game1 gameRoot, SpriteFont font)
        {
            _gameRoot = gameRoot;
            _font = font;
            _pixel = new Texture2D(GBL.GD, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        public void Draw(SpritePlayer player)
        {
            DrawHealthBar(player);
            DrawXPBar(player.LevelSystem);

            SpriteEnemy boss = _gameRoot._currentBoss;

            if (!_gameRoot._bossDead)
            {
                DrawBossBar(boss);
            }
        }

        private void DrawHealthBar(SpritePlayer player)
        {
            int barWidth = 225; 
            int barHeight = 9;  
            int x = 20, y = 30; 

            float fill = _gameRoot._health / player.Stats.MaxHP;
            fill = Math.Clamp(fill, 0f, 1f);

            float uiLayer = 0.1f;

            // Background
            GBL.spriteBatch.Draw(
                _pixel,
                new Rectangle(x, y, barWidth, barHeight),
                null,
                Color.DarkRed,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                uiLayer
            );

            // Fill - changes colour based on health
            Color fillColor = fill > 0.5f ? Color.LimeGreen : fill > 0.25f ? Color.Orange : Color.Red;
            GBL.spriteBatch.Draw(
                _pixel,
                new Rectangle(x, y, (int)(barWidth * fill), barHeight),
                null,
                fillColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                uiLayer - 0.001f
            );

            // Text
            GBL.spriteBatch.DrawString(
                _font,
                $"HP {(int)_gameRoot._health}/{player.Stats.MaxHP}",
                new Vector2(x, y - 14),
                Color.White,
                0f,
                Vector2.Zero,
                0.75f,   // 75% text scale
                SpriteEffects.None,
                uiLayer - 0.002f
            );
        }

        private void DrawXPBar(LevelSystem lvl)
        {
            int barWidth = 225; 
            int barHeight = 9;  
            int x = 20, y = 55; 

            float fill = lvl.CurrentXP / lvl.XPToNextLevel;
            float uiLayer = 0.1f;

            // Background
            GBL.spriteBatch.Draw(
                _pixel,
                new Rectangle(x, y, barWidth, barHeight),
                null,
                Color.DarkGray,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                uiLayer
            );

            // Fill
            GBL.spriteBatch.Draw(
                _pixel,
                new Rectangle(x, y, (int)(barWidth * fill), barHeight),
                null,
                Color.Gold,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                uiLayer - 0.001f
            );

            // Text
            GBL.spriteBatch.DrawString(
                _font,
                $"LVL {lvl.Level}  {lvl.CurrentXP:0}/{lvl.XPToNextLevel:0} XP",
                new Vector2(x, y - 14),
                Color.White,
                0f,
                Vector2.Zero,
                0.75f, 
                SpriteEffects.None,
                uiLayer - 0.002f
            );
        }

        private void DrawBossBar(SpriteEnemy boss)
        {
            int barWidth = 350; 
            int barHeight = 25;  
            int x = 240, y = 420; 

            float fill = boss._health / boss._maxHealth;
            fill = Math.Clamp(fill, 0f, 1f);

            float uiLayer = 0.1f;

            // Background
            GBL.spriteBatch.Draw(
                _pixel,
                new Rectangle(x, y, barWidth, barHeight),
                null,
                Color.DarkRed,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                uiLayer
            );

            // Fill
            Color fillColor = Color.Red;
            GBL.spriteBatch.Draw(
                _pixel,
                new Rectangle(x, y, (int)(barWidth * fill), barHeight),
                null,
                fillColor,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                uiLayer - 0.001f
            );

            // Text
            GBL.spriteBatch.DrawString(
                _font,
                $"HP {(int)_gameRoot._health}/{boss._maxHealth}",
                new Vector2(x, y - 14),
                Color.White,
                0f,
                Vector2.Zero,
                0.75f,   // 75% text scale
                SpriteEffects.None,
                uiLayer - 0.002f
            );
        }
    }
}