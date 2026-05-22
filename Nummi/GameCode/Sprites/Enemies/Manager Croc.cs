using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // Dungeon 2 boss "The Manager".
    //
    // Two attack modes (see design doc Dungeon 2 boss fight):
    //   Mode 1 - On the brick path. Croc wails its arms in a wide area then
    //            gets tired and exposes itself; player damage stuns/knockbacks
    //            the croc; sustained punishment shoves it into the water. If
    //            left alone it gives up the opening and runs back to water.
    //   Mode 2 - In the water. Croc charges left/right along the brick path
    //            to fence the player; if the croc touches the player it grabs
    //            and throws them further down the path. Slot machines rise
    //            from the water and shoot coin projectiles in patterns.
    //            Croc can only be damaged while out of the water.
    // Frozen variant: always asleep, invincible, growls when hit (matches
    // the other frozen casino enemies).
    public class Manager_Croc : SpriteEnemy
    {
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

        // ── Tilemap regions (in tile coords) ───────────────────────────────
        private static readonly Rectangle PathTiles      = new Rectangle(1, 7,  18, 7); // x 1..18, y 7..13
        private static readonly Rectangle WaterTopTiles  = new Rectangle(1, 3,  18, 4); // y 3..6
        private static readonly Rectangle WaterBotTiles  = new Rectangle(1, 14, 18, 4); // y 14..17

        public Manager_Croc(Game1 gameRoot, Texture2D texture, Vector2 position, TempState tempState)
            : base(gameRoot, texture, position, true, 600, 300, 30, true, 80f, 800f, 0f, 300)
        {
            _tempState = tempState;
            _gameRoot._bossName = "THE MANAGER";
            _gameRoot._currentBoss = this;
            _gameRoot._bossDead = false;

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
                // Boss starts in the water napping on the hammock.
                _phase = Phase.Mode2WaterIdle;
                _slotWaveTimer = 0f;
                SetAnimation(7); // Nap
                MoveToHammockNapSpot();
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
            for (int i = 0; i < 4; i++) a[0].Add(new Rectangle(i * 64, 0, 64, 64));

            // 1 - Sleepy growl reaction (frozen)
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 4; i++) a[1].Add(new Rectangle(i * 64, 64, 64, 64));

            // 2 - Mode 1 walk (toothy grin, bipedal)
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 6; i++) a[2].Add(new Rectangle(i * 64, 128, 64, 64));

            // 3 - Mode 1 wail-arms attack
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 8; i++) a[3].Add(new Rectangle(i * 64, 192, 64, 64));

            // 4 - Mode 1 tired/opening (scared face when hit)
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 4; i++) a[4].Add(new Rectangle(i * 64, 256, 64, 64));

            // 5 - Mode 1 retreat (running on all fours, scared/angry)
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 6; i++) a[5].Add(new Rectangle(i * 64, 320, 64, 64));

            // 6 - Mode 2 charge (frantic run along path)
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 6; i++) a[6].Add(new Rectangle(i * 64, 384, 64, 64));

            // 7 - Mode 2 nap on hammock
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 4; i++) a[7].Add(new Rectangle(i * 64, 448, 64, 64));

            // 8 - Death (falls backwards, snot bubble)
            a.Add(new List<Rectangle>());
            for (int i = 0; i < 6; i++) a[8].Add(new Rectangle(i * 64, 512, 64, 64));

            _nextAnim = new List<int>();
            for (int i = 0; i < a.Count; i++) _nextAnim.Add(i);
            return a;
        }

        public override bool Dead
        {
            set
            {
                _gameRoot._bossDead = true;
                _gameRoot._isNextLevelTails = true;
                OnDeath();
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
                _dead = value;
            }
            get => _dead;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            _growlCooldown -= GBL.DeltaTime;

            if (_tempState == TempState.Frozen)
            {
                UpdateFrozen();
                return;
            }

            UpdateThawed();
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
            // Only damageable while out of water (i.e. on the brick path).
            _isIndestructible = !IsOnPath(_position);

            switch (_phase)
            {
                case Phase.Mode1Approach:   UpdateApproach();  break;
                case Phase.Mode1Wail:       UpdateWail();      break;
                case Phase.Mode1Tired:      UpdateTired();     break;
                case Phase.Mode1Retreat:    UpdateRetreat();   break;
                case Phase.Mode2WaterIdle:  UpdateWaterIdle(); break;
                case Phase.Mode2Charge:     UpdateCharge();    break;
                case Phase.Mode2Slots:      UpdateSlots();     break;
            }
        }

        // ── Mode 1 ──────────────────────────────────────────────────────────

        private void UpdateApproach()
        {
            SetAnimation(2);
            Vector2 dir = _gameRoot._player._position - _position;
            if (dir != Vector2.Zero) dir.Normalize();
            _velocity = dir * _moveSpeed;

            // Once close enough, begin the wail wind-up.
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
                MoveToHammockNapSpot();
                _slotWaveTimer = 0f;
                SetAnimation(7);
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
            // Sprint toward the nearest water bank on all fours.
            Vector2 target = NearestWaterTile(_position);
            Vector2 dir = target - _position;
            if (dir != Vector2.Zero) dir.Normalize();
            _velocity = dir * _retreatSpeed;
            SetAnimation(5);

            if (!IsOnPath(_position))
            {
                _phase = Phase.Mode2WaterIdle;
                MoveToHammockNapSpot();
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

            // Open with a slot-wave + charge combination.
            if (_slotWaveTimer >= 1.5f)
            {
                _slotWaveTimer = 0f;
                _phase = Phase.Mode2Slots;
                SpawnSlotMachines();
            }
        }

        private void UpdateSlots()
        {
            // Stays napping while the slot wave plays out. The waking moment
            // is the cue the wave is ending (see Phase.Mode2Charge below).
            _velocity = Vector2.Zero;
            SetAnimation(7);

            _slotWaveTimer += GBL.DeltaTime;
            if (_slotWaveTimer >= SlotWaveDuration)
            {
                _slotWaveTimer = 0f;
                _phase = Phase.Mode2Charge;
                _chargeTimer = ChargeDuration;
                _chargeFlipTimer = 0f;
                _chargeDirection = _gameRoot._player._position.X > _position.X ? 1 : -1;
                EnterChargeStartPosition();
                SetAnimation(6);
            }
        }

        private void UpdateCharge()
        {
            _chargeTimer -= GBL.DeltaTime;
            _chargeFlipTimer += GBL.DeltaTime;
            SetAnimation(6);

            // Dash horizontally along the brick path.
            _velocity = new Vector2(_chargeDirection * _chargeSpeed, 0f);

            // Reverse on path edges or periodically to "cover the entire path".
            float minX = PathTiles.Left * 32f + 16f;
            float maxX = (PathTiles.Right) * 32f - 16f;
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
                // Decide next move: drop back to water for another slot wave,
                // or surface for a Mode 1 wail.
                if (new Random().NextDouble() < 0.5)
                {
                    _phase = Phase.Mode2WaterIdle;
                    MoveToHammockNapSpot();
                    SetAnimation(7);
                }
                else
                {
                    SurfaceForMode1();
                }
            }
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private void SpawnWailHitbox()
        {
            // Wide hitbox slightly in front of the croc.
            Vector2 facing = _gameRoot._player._position - _position;
            if (facing == Vector2.Zero) facing = new Vector2(1f, 0f);
            facing.Normalize();
            Vector2 spawn = _position + facing * 36f;
            _gameRoot._newSpriteList.Add(new ManagerWailHit(_gameRoot, spawn));
        }

        private void SpawnSlotMachines()
        {
            // A few machines spaced along the path.
            int count = 3;
            for (int i = 0; i < count; i++)
            {
                float t = (i + 1) / (float)(count + 1);
                float x = MathHelper.Lerp(PathTiles.Left * 32f, PathTiles.Right * 32f, t);
                float y = PathTiles.Center.Y * 32f;
                _gameRoot._newSpriteList.Add(new ManagerSlotMachine(_gameRoot, new Vector2(x, y)));
            }
        }

        private void GrabAndThrow()
        {
            // Throw the player further down the path in the direction the
            // croc is moving, so the player must dodge past again.
            Vector2 throwDir = new Vector2(_chargeDirection, 0f);
            _gameRoot._player._velocity += throwDir * _grabThrowImpulse;
            _gameRoot._health -= _damageStrength;
        }

        private void SurfaceForMode1()
        {
            // Climb back up onto the brick path and start Mode 1.
            float midY = PathTiles.Center.Y * 32f;
            _position = new Vector2(_gameRoot._player._position.X, midY);
            _phase = Phase.Mode1Approach;
            SetAnimation(2);
        }

        private void EnterChargeStartPosition()
        {
            // Position at one edge of the path on the player's row.
            float startX = _chargeDirection > 0
                ? PathTiles.Left * 32f + 16f
                : PathTiles.Right * 32f - 16f;
            float y = MathHelper.Clamp(_gameRoot._player._position.Y,
                PathTiles.Top * 32f + 16f,
                PathTiles.Bottom * 32f - 16f);
            _position = new Vector2(startX, y);
        }

        private void MoveToHammockNapSpot()
        {
            // Hammock napping spot - corner of the top water area.
            _position = new Vector2(WaterTopTiles.Left * 32f + 16f,
                                    WaterTopTiles.Top  * 32f + 16f);
        }

        private static bool IsInTileRect(Vector2 worldPos, Rectangle tileRect)
        {
            int tx = (int)(worldPos.X / 32f);
            int ty = (int)(worldPos.Y / 32f);
            return tx >= tileRect.Left && tx < tileRect.Right
                && ty >= tileRect.Top  && ty < tileRect.Bottom;
        }

        private static bool IsOnPath(Vector2 worldPos) => IsInTileRect(worldPos, PathTiles);

        private static Vector2 NearestWaterTile(Vector2 worldPos)
        {
            // Pick whichever water bank (top or bottom) is closer in Y.
            float topY = (WaterTopTiles.Top + WaterTopTiles.Height / 2f) * 32f;
            float botY = (WaterBotTiles.Top + WaterBotTiles.Height / 2f) * 32f;
            float targetY = Math.Abs(worldPos.Y - topY) <= Math.Abs(worldPos.Y - botY) ? topY : botY;
            return new Vector2(worldPos.X, targetY);
        }

        // ── Collision events ────────────────────────────────────────────────

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            // Frozen: bounce hits off and growl sleepily.
            if (_tempState == TempState.Frozen && otherSprite is Attack && _growlCooldown <= 0f)
            {
                _growlCooldown = GrowlCooldownDuration;
                SetAnimation(1);
                return;
            }

            // During the tired opening, count damage and shove the croc
            // toward the nearest water bank for every hit.
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
            if (_animIndex == 1) SetAnimation(0); // growl -> sleep
        }

        // ── Death ───────────────────────────────────────────────────────────

        public void OnDeath()
        {
            // Death animation (fall backwards + snot bubble) handled by anim 8.
            SetAnimation(8);

            // Spawn chest under the croc and the mirror exit two tiles next to it.
            var groundLayer = _gameRoot._tilemap.Layers[0];
            int tx = (int)(_position.X / 32f);
            int ty = (int)(_position.Y / 32f);

            // Use the same tile IDs the other bosses use for chest + mirror exit.
            groundLayer.SetTile(tx, ty, 14);          // chest
            groundLayer.SetTile(tx + 2, ty, 22);      // mirror exit
        }
    }

    // ── Wail-arms hitbox ────────────────────────────────────────────────────
    public class ManagerWailHit : SpriteEnemy
    {
        float _life = 0.2f;

        public ManagerWailHit(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Waiter Smash"), position, false, 10000, 400, 30, false, 0, 400f, 80f, 0)
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
        private const float FireInterval = 0.5f;
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
            // Simple straight-line pattern early, diagonal pattern later for
            // diamond safe-spots (per the spec).
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
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Coin"), position, 220f, 12)
        {
            if (direction != Vector2.Zero) direction.Normalize();
            _velocity = direction * _moveSpeed;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            var a = new List<List<Rectangle>>();
            a.Add(new List<Rectangle>());
            a[0].Add(new Rectangle(0, 0, 16, 16));
            _nextAnim = new List<int>();
            for (int i = 0; i < a.Count; i++) _nextAnim.Add(i);
            return a;
        }
    }
}
