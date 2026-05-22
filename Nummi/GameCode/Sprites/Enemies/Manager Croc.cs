using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class Manager_Croc : SpriteEnemy
    {
        // This class is so janky and scuffed

        public enum TempState
        {
            Frozen,
            Thawed
        }

        // Boss phase state machine for the thawed fight.
        private enum Phase
        {
            Mode1Approach,   // walking up to the player on the path
            Mode1Wail,       // performing the wide arm-wailing attack
            Mode1Tired,      // exposed; player can punish
            Mode1Retreat,    // running back to water on all fours
            Mode2WaterIdle,  // napping in hammock between slot waves
            Mode2Charge,     // dashing along the path to fence the player
            Mode2Slots       // slot machines firing coin patterns
        }

        public TempState _tempState;
        private Phase _phase = Phase.Mode2WaterIdle;

        // ── Frozen growl ───────────────────────────────────────────────────
        private float _growlCooldown = 0f;
        private const float GrowlCooldownDuration = 1f;

        // ── Mode 1 ─────────────────────────────────────────────────────────
        private float _wailWindupTimer = 0f;
        private const float WailWindupDuration = 0.7f;
        private float _wailTimer = 0f;
        private const float WailDuration = 1.4f;
        private float _tiredTimer = 0f;
        private const float TiredDuration = 3.5f;
        private float _tiredHits = 0f;            // damage points absorbed during the opening
        private const float TiredHitsToShove = 60f;

        private const float ModeShoveStep = 16f;

        // ── Mode 1 -> Mode 2 transitions ───────────────────────────────────
        private float _retreatSpeed = 200f;

        // ── Mode 2 ─────────────────────────────────────────────────────────
        private float _slotWaveTimer = 0f;
        private const float SlotWaveDuration = 6f;
        private float _chargeTimer = 0f;
        private const float ChargeDuration = 5f;
        private float _chargeSpeed = 320f;
        private int _chargeDirection = 1;          // +1 = right, -1 = left
        private float _chargeFlipTimer = 0f;
        private const float ChargeFlipInterval = 1.25f;
        private float _grabThrowImpulse = 700f;

        private Vector2? _moveTarget = null;
        private float _transitSpeed = 120f;
        private const float ArriveRadius = 16f;
        private Vector2 _spawnPos;

        private const int PathTileID  = 5;
        private const int WaterTileID = 23;

        public Manager_Croc(Game1 gameRoot, Vector2 position, TempState tempState)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Croc Boss Sheet"), position, true, 1500, 300, 30, true, 80f, 800f, 1000f, 300)
        {
            _tempState = tempState;
            _spawnPos = position;
            _gameRoot._bossName = "THE MANAGER";
            _gameRoot._currentBoss = this;
            _gameRoot._bossDead = false;
            _flipEffect = SpriteEffects.FlipHorizontally;

            if (_tempState == TempState.Frozen)
            {
                SetAnimation(0); // Sleeping (frozen)
                _isIndestructible = true;
                _canMove = true;
                _isStationary = true;
                _aggrorange = 0f;
            }
            else
            {
                // Boss stays exactly where LevelData spawned it - no teleport.
                _phase = Phase.Mode2WaterIdle;
                _slotWaveTimer = 0f;
                SetAnimation(7); // Nap
                _isIndestructible = true; // out of damage range while in water
            }

            _canPatrol = false;
        }

        // Placeholder rectangles — every slot is 64x64 on the placeholder sheet.
        // Replace with the real Manager sheet when the art is ready.
        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            List<List<Rectangle>> a = new List<List<Rectangle>>();

            // 0 - Frozen sleep
            a.Add(new List<Rectangle>());
            a[0].Add(new Rectangle(320, 0, 64, 96));

            // 1 - Sleepy growl reaction (frozen)
            a.Add(new List<Rectangle>());
            a[1].Add(new Rectangle(320, 0, 64, 96));

            // 2 - Mode 1 walk (toothy grin, bipedal)
            a.Add(new List<Rectangle>());
            a[2].Add(new Rectangle(0, 208, 96, 64));
            a[2].Add(new Rectangle(96, 208, 96, 64));
            a[2].Add(new Rectangle(192, 208, 96, 64));
            a[2].Add(new Rectangle(288, 208, 96, 64));
            a[2].Add(new Rectangle(384, 208, 96, 64));
            a[2].Add(new Rectangle(480, 208, 96, 64));
            a[2].Add(new Rectangle(576, 208, 96, 64));
            a[2].Add(new Rectangle(672, 208, 96, 64));

            // 3 - Mode 1 wail-arms attack
            a.Add(new List<Rectangle>());
            a[3].Add(new Rectangle(0, 96, 64, 96));
            a[3].Add(new Rectangle(64, 96, 64, 96));
            a[3].Add(new Rectangle(128, 96, 64, 96));
            a[3].Add(new Rectangle(192, 96, 64, 96));
            a[3].Add(new Rectangle(256, 96, 64, 96));
            a[3].Add(new Rectangle(320, 96, 64, 96));

            // 4 - Mode 1 tired/opening (scared face when hit)
            a.Add(new List<Rectangle>());
            a[4].Add(new Rectangle(0, 0, 64, 96));
            a[4].Add(new Rectangle(320, 0, 64, 96));

            // 5 - Mode 1 retreat (running on all fours, scared/angry)
            a.Add(new List<Rectangle>());
            a[5].Add(new Rectangle(0, 208, 96, 64));
            a[5].Add(new Rectangle(96, 208, 96, 64));
            a[5].Add(new Rectangle(192, 208, 96, 64));
            a[5].Add(new Rectangle(288, 208, 96, 64));
            a[5].Add(new Rectangle(384, 208, 96, 64));
            a[5].Add(new Rectangle(480, 208, 96, 64));
            a[5].Add(new Rectangle(576, 208, 96, 64));
            a[5].Add(new Rectangle(672, 208, 96, 64));


            // 6 - Mode 2 charge (frantic run along path)
            a.Add(new List<Rectangle>());
            a[6].Add(new Rectangle(0, 208, 96, 64));
            a[6].Add(new Rectangle(96, 208, 96, 64));
            a[6].Add(new Rectangle(192, 208, 96, 64));
            a[6].Add(new Rectangle(288, 208, 96, 64));
            a[6].Add(new Rectangle(384, 208, 96, 64));
            a[6].Add(new Rectangle(480, 208, 96, 64));
            a[6].Add(new Rectangle(576, 208, 96, 64));
            a[6].Add(new Rectangle(672, 208, 96, 64));

            // 7 - Mode 2 nap on hammock
            a.Add(new List<Rectangle>());
            a[7].Add(new Rectangle(640, 0, 64, 96));
            a[7].Add(new Rectangle(704, 0, 64, 96));


            _nextAnim = new List<int>();
            for (int i = 0; i < a.Count; i++) _nextAnim.Add(i);
            return a;
        }

        public override bool Dead
        {
            set
            {
                OnDeath();
                _gameRoot._bossDead = true;
                _gameRoot._isNextLevelTails = true;
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
                _dead = value;
            }
            get => _dead;
        }

        public override void Update(GameTime gameTime)
        {
            _isPatrolling = true;

            if (_gameRoot._bossDead) Dead = true;

            _growlCooldown -= GBL.DeltaTime;
            if (_tempState == TempState.Frozen) UpdateFrozen();
            else UpdateThawed();

            base.Update(gameTime);
        }

        // ── Frozen ──────────────────────────────────────────────────────────

        private void UpdateFrozen()
        {
            if (!_isKnockedback) _velocity = Vector2.Zero;
            if (_animIndex != 1) SetAnimation(0);
        }

        // ── Thawed boss state machine ───────────────────────────────────────

        private void UpdateThawed()
        {
            _isIndestructible = !IsOnPath(_position);

            if (_moveTarget.HasValue)
            {
                Vector2 to = _moveTarget.Value - _position;
                if (to.LengthSquared() <= ArriveRadius * ArriveRadius)
                {
                    _moveTarget = null;
                    _velocity = Vector2.Zero;
                }
                else
                {
                    to.Normalize();
                    _velocity = to * _transitSpeed;
                    return;
                }
            }

            switch (_phase)
            {
                case Phase.Mode1Approach:UpdateApproach();  break;
                case Phase.Mode1Wail:   UpdateWail();      break;
                case Phase.Mode1Tired:  UpdateTired();     break;
                case Phase.Mode1Retreat:UpdateRetreat();   break;
                case Phase.Mode2WaterIdle:UpdateWaterIdle(); break;
                case Phase.Mode2Charge: UpdateCharge();    break;
                case Phase.Mode2Slots:  UpdateSlots();     break;
            }
        }

        // ── Mode 1 ──────────────────────────────────────────────────────────

        private void UpdateApproach()
        {
            SetAnimation(2);
            Vector2 dir = _gameRoot._player._position - _position;
            if (dir != Vector2.Zero) dir.Normalize();
            _velocity = dir * _moveSpeed;

            if (Vector2.Distance(_position, _gameRoot._player._position) < 64f)
            {
                _phase = Phase.Mode1Wail;
                _wailWindupTimer = WailWindupDuration;
                _wailTimer = WailDuration;
                SetAnimation(3);
                _velocity = Vector2.Zero;
            }
        }

        private void UpdateWail()
        {
            _velocity = Vector2.Zero;

            if (_wailWindupTimer > 0f)
            {
                _wailWindupTimer -= GBL.DeltaTime;
                return;
            }

            _wailTimer -= GBL.DeltaTime;
            SpawnWailHitbox();

            if (_wailTimer <= 0f)
            {
                _phase = Phase.Mode1Tired;
                _tiredTimer = TiredDuration;
                _tiredHits = 0f;
                SetAnimation(4);
            }
        }

        private void UpdateTired()
        {
            _velocity = Vector2.Zero;
            _tiredTimer -= GBL.DeltaTime;

            if (_tiredHits >= TiredHitsToShove)
            {
                _phase = Phase.Mode2WaterIdle;
                _moveTarget = HammockSpot();   
                _slotWaveTimer = 0f;
                SetAnimation(5);
                return;
            }

            if (_tiredTimer <= 0f)
            {
                _phase = Phase.Mode1Retreat;
                SetAnimation(5);
            }
        }

        private void UpdateRetreat()
        {
            Vector2 target = NearestWaterTile(_position);
            Vector2 dir = target - _position;
            if (dir != Vector2.Zero) dir.Normalize();
            _velocity = dir * _retreatSpeed;
            SetAnimation(5);

            if (IsInWater(_position))
            {
                _phase = Phase.Mode2WaterIdle;
                _moveTarget = HammockSpot();
                _slotWaveTimer = 0f;
                SetAnimation(7);
            }
        }

        // ── Mode 2 ──────────────────────────────────────────────────────────

        private void UpdateWaterIdle()
        {
            _velocity = Vector2.Zero;
            SetAnimation(7);

            _slotWaveTimer += GBL.DeltaTime;

            if (_slotWaveTimer >= 1.5f)
            {
                _slotWaveTimer = 0f;
                _phase = Phase.Mode2Slots;
                SpawnSlotMachines();
            }
        }

        private void UpdateSlots()
        {
            _velocity = Vector2.Zero;
            SetAnimation(7);

            _slotWaveTimer += GBL.DeltaTime;
            if (_slotWaveTimer >= SlotWaveDuration)
            {
                _slotWaveTimer = 0f;
                _phase = Phase.Mode2Charge;
                _chargeTimer = ChargeDuration;
                _chargeFlipTimer = 0f;

                _chargeDirection = (_gameRoot._player._position.X >= _spawnPos.X) ? -1 : 1;
                _moveTarget = ChargeStartTarget();
                SetAnimation(6);
            }
        }

        private void UpdateCharge()
        {
            _chargeTimer -= GBL.DeltaTime;
            _chargeFlipTimer += GBL.DeltaTime;
            SetAnimation(6);

            _velocity = new Vector2(_chargeDirection * _chargeSpeed, 0f);

            float chargeHalfWidth = 160f;
            float minX = _spawnPos.X - chargeHalfWidth;
            float maxX = _spawnPos.X + chargeHalfWidth;
            if (_position.X <= minX) _chargeDirection = 1;
            else if (_position.X >= maxX) _chargeDirection = -1;
            else if (_chargeFlipTimer >= ChargeFlipInterval)
            {
                _chargeFlipTimer = 0f;
                _chargeDirection = -_chargeDirection;
            }

            // Grab-and-throw on contact.
            if (_collisionBounds.Intersects(_gameRoot._player._collisionBounds))
            {
                GrabAndThrow();
            }

            if (_chargeTimer <= 0f)
            {
                if (new Random().NextDouble() < 0.5)
                {
                    _phase = Phase.Mode2WaterIdle;
                    _moveTarget = HammockSpot();
                    SetAnimation(7);
                }
                else
                {
                    _phase = Phase.Mode1Approach;
                    _moveTarget = SurfaceTarget();
                    SetAnimation(2);
                }
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private void SpawnWailHitbox()
        {
            Vector2 facing = _gameRoot._player._position - _position;
            if (facing == Vector2.Zero) facing = new Vector2(1f, 0f);
            facing.Normalize();
            Vector2 spawn = _position + facing * 36f;
            _gameRoot._newSpriteList.Add(new ManagerWailHit(_gameRoot, spawn));
        }

        private void SpawnSlotMachines()
        {
            int count = 3;
            Vector2 pos = LevelData.TilePos(9, 5);
            for (int i = 0; i < count; i++)
            {
                float offset = (i - 1) * 96f; // -96, 0, +96
                _gameRoot._newSpriteList.Add(
                    new ManagerSlotMachine(_gameRoot, new Vector2(pos.X + offset, pos.Y)));
            }
        }

        private void GrabAndThrow()
        {
            Vector2 throwDir = new Vector2(_chargeDirection, 0f);
            _gameRoot._player._velocity += throwDir * _grabThrowImpulse;
            _gameRoot._health -= _damageStrength;
        }

        private Vector2 SurfaceTarget()
        {
            Vector2 toPlayer = _gameRoot._player._position - _spawnPos;
            if (toPlayer == Vector2.Zero) toPlayer = new Vector2(1f, 0f);
            toPlayer.Normalize();
            return _spawnPos + toPlayer * 48f;
        }

        private Vector2 ChargeStartTarget()
        {
            return _spawnPos + new Vector2(_chargeDirection * -160f, 0f);
        }

        private Vector2 HammockSpot()
        {
            return _spawnPos;
        }

        private int TileIdAt(Vector2 worldPos)
        {
            return _gameRoot._tilemap.Layers[0].GetTileIDAtWorld(
                (int)worldPos.X, (int)worldPos.Y);
        }

        private bool IsOnPath(Vector2 worldPos)  => TileIdAt(worldPos) == PathTileID;
        private bool IsInWater(Vector2 worldPos) => TileIdAt(worldPos) == WaterTileID;

        private Vector2 NearestWaterTile(Vector2 worldPos)
        {
            const int maxSteps = 32;
            float ts = 32f;

            for (int i = 1; i <= maxSteps; i++)
            {
                Vector2 up   = worldPos + new Vector2(0f, -i * ts);
                Vector2 down = worldPos + new Vector2(0f,  i * ts);
                if (IsInWater(up))   return up;
                if (IsInWater(down)) return down;
            }

            return _spawnPos;
        }

        // ── Collision events ────────────────────────────────────────────────

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            if (_tempState == TempState.Frozen && otherSprite is Attack && _growlCooldown <= 0f)
            {
                _growlCooldown = GrowlCooldownDuration;
                SetAnimation(1);
                return;
            }

            if (_tempState == TempState.Thawed
                && _phase == Phase.Mode1Tired
                && otherSprite is Attack atk)
            {
                _tiredHits += atk._weaponDamage;
                Vector2 toWater = NearestWaterTile(_position) - _position;
                if (toWater != Vector2.Zero) toWater.Normalize();
                _position += toWater * ModeShoveStep;
            }

            base.OnCollideEvent(otherSprite);
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();
            if (_animIndex == 1) SetAnimation(0);
        }

        public void OnDeath()
        {

            var groundLayer = _gameRoot._tilemap.Layers[1];
            int tx = (int)(_position.X / 32f);
            int ty = (int)(_position.Y / 32f);

            groundLayer.SetTile(tx, ty, 24);    // chest
            groundLayer.SetTile(tx + 2, ty, 47);    // mirror exit
            groundLayer.SetTile(tx + 2, ty - 1, 39);
        }
    }

    // ── Wail-arms hitbox ────────────────────────────────────────────────────
    public class ManagerWailHit : SpriteEnemy
    {
        float _life = 0.2f;

        public ManagerWailHit(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Waiter Smash"), position, false, 10000, 400, 30, false, 0, 400f, 0f, 0)
        {
            SetAnimation(0);
            _drawScale = new Vector2(2.5f, 2.5f);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;
            var a = new List<List<Rectangle>>();
            a.Add(new List<Rectangle>());
            a[0].Add(new Rectangle(0, 0, 16, 16));
            a[0].Add(new Rectangle(16, 0, 16, 16));
            a[0].Add(new Rectangle(32, 0, 16, 16));
            a[0].Add(new Rectangle(0, 0, 16, 16));
            a[0].Add(new Rectangle(16, 0, 16, 16));
            a[0].Add(new Rectangle(32, 0, 16, 16));
            a[0].Add(new Rectangle(0, 0, 16, 16));
            a[0].Add(new Rectangle(16, 0, 16, 16));
            a[0].Add(new Rectangle(32, 0, 16, 16));
            a[0].Add(new Rectangle(0, 0, 16, 16));
            a[0].Add(new Rectangle(16, 0, 16, 16));
            a[0].Add(new Rectangle(32, 0, 16, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < a.Count; i++) _nextAnim.Add(i);
            return a;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _life -= GBL.DeltaTime;
            if (_life <= 0f) _dead = true;
        }
    }

    // ── Slot machine that rises from the water and shoots coin patterns ────
    public class ManagerSlotMachine : SpriteEnemy
    {
        private float _lifeTimer = 0f;
        private const float Lifetime = 6f;
        private float _fireTimer = 0f;
        private const float FireInterval = 1.6f;
        private int _patternStep = 0;

        public ManagerSlotMachine(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Slots"), position, false, 10000, 0, 0, false, 0, 400f, 0f, 0)
        {
            SetAnimation(0);
            _isIndestructible = true;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 10f;
            var a = new List<List<Rectangle>>();
            a.Add(new List<Rectangle>());
            a[0].Add(new Rectangle(0, 0, 32, 48));
            _nextAnim = new List<int>();
            for (int i = 0; i < a.Count; i++) _nextAnim.Add(i);
            return a;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _velocity = Vector2.Zero;

            _lifeTimer += GBL.DeltaTime;
            _fireTimer += GBL.DeltaTime;

            if (_fireTimer >= FireInterval)
            {
                _fireTimer = 0f;
                FireCoinPattern();
                _patternStep++;
            }

            if (_lifeTimer >= Lifetime) _dead = true;
        }

        private void FireCoinPattern()
        {
            bool diagonal = _patternStep >= 4;
            float[] angles = diagonal
                ? new float[] { MathHelper.PiOver4, 3 * MathHelper.PiOver4, 5 * MathHelper.PiOver4, 7 * MathHelper.PiOver4 }
                : new float[] { 0f, MathHelper.PiOver2, MathHelper.Pi, 3 * MathHelper.PiOver2 };

            foreach (float a in angles)
            {
                Vector2 dir = new Vector2(MathF.Cos(a), MathF.Sin(a));
                _gameRoot._newSpriteList.Add(new ManagerCoin(_gameRoot, _position, dir));
            }
        }
    }

    // ── Coin projectile fired by the slot machines ─────────────────────────
    public class ManagerCoin : SpriteEnemyProjectile
    {
        public ManagerCoin(Game1 gameRoot, Vector2 position, Vector2 direction)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Tree Boss Projectile"), position, 220f, 12)
        {
            if (direction != Vector2.Zero) direction.Normalize();
            _velocity = direction * _moveSpeed;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            var a = new List<List<Rectangle>>();
            a.Add(new List<Rectangle>());
            a[0].Add(new Rectangle(0, 0, 8, 8));
            _nextAnim = new List<int>();
            for (int i = 0; i < a.Count; i++) _nextAnim.Add(i);
            return a;
        }
    }
}
