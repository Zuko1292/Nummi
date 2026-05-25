using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // This class handles the player's attacks, it is a sprite that is spawned when the player attacks and it has a short lifetime. It also handles the animation of the attack and the damage it does to enemies. The attack is spawned in the direction the player is facing and it moves with the player, so if the player moves while attacking, the attack will move with them. The attack also has a collision layer that allows it to damage enemies but not collide with the player or other attacks.
    // TODO right now the attack collides with tilemap so give tilemap a collision layer and make it so attack cant see that so prolly add another layer called like non collidables for attacks and other stuff of the sort
    public class Attack : SpriteAnimating
    {
        public float _weaponDamage;
        protected float _lifetime = 0.5f;
        

        public Attack(Game1 gameRoot, Texture2D weaponTxr, Vector2 position, bool canMove, int currentWeapon) : 
            base(gameRoot,weaponTxr, position, canMove, true)
        {
            CollisionLayer = CollisionLayer.Attacks;
            CollisionMask = CollisionLayer.Enemy & ~CollisionLayer.Player;

            // Spawn an arrow projectile for the bow, but guard against an Arrow (itself an
            // Attack constructed with weapon 4) recursively spawning more arrows.
            if(currentWeapon == 4 && !(this is Arrow)) ArrowSpawn();

            if (currentWeapon == 5 && _gameRoot._isTesting) InitBounds(position - new Vector2(160, 160), true, new Vector2(10, 10), new Vector2(10, 10));
        }

        public override void Update(GameTime gameTime)
        {
            _weaponDamage = _gameRoot._player.Stats.WeaponDmg;

            _lifetime -= GBL.DeltaTime;

            if(_lifetime <= 0 && this is Arrow)
            {
                Dead = true;
            }
            
            if (!(this is Arrow)) _velocity = _gameRoot._player._velocity;
            base.Update(gameTime);
        }

        public void ArrowSpawn()
        {
            Vector2 arrowCenter = new Vector2(
                _collisionBounds.Center.X,
                _collisionBounds.Center.Y
            );

            // Get mouse position in world space (accounting for camera)
            Vector2 mouseWorld = new Vector2(GBL._camera.ScreenToWorld(GBL.mousePos).X, GBL._camera.ScreenToWorld(GBL.mousePos).Y);

            Vector2 dir = mouseWorld - arrowCenter;

            if (dir == Vector2.Zero) return;

            Vector2 dirNorm = Vector2.Normalize(dir);

            float spawnRadius = 50f;
            Vector2 spawnPos = arrowCenter + dirNorm * spawnRadius;

            Arrow arrow = new Arrow(_gameRoot, spawnPos, _gameRoot._player._currentWeapon, dir);
            _gameRoot._spriteList.Add(arrow);
        }

        protected override void OnAnimationFinished()
        {
            base.OnAnimationFinished();

            if(!(this is Arrow))
            {
                _gameRoot._player._attacking = false;
                Dead = true;
            }
        }
    }
    public class Up_Attack : Attack
    {

        public Up_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Up Slash-Sheet"), position, true, currentWeapon)
        {
            SetAnimation(_gameRoot._player._currentWeapon);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 16));
            animations[0].Add(new Rectangle(48, 0, 48, 16));
            animations[0].Add(new Rectangle(96, 0, 48, 16));
            animations[0].Add(new Rectangle(144, 0, 48, 16));
            animations[0].Add(new Rectangle(192, 0, 48, 16));
            animations[0].Add(new Rectangle(240, 0, 48, 16));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 96, 32));
            animations[1].Add(new Rectangle(96, 16, 96, 32));
            animations[1].Add(new Rectangle(192, 16, 96, 32));
            animations[1].Add(new Rectangle(288, 16, 96, 32));
            animations[1].Add(new Rectangle(384, 16, 96, 32));
            animations[1].Add(new Rectangle(480, 16, 96, 32));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 48, 32,32));
            animations[2].Add(new Rectangle(32, 48, 32, 32));
            animations[2].Add(new Rectangle(64, 48, 32, 32));


            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 80, 48, 48));
            animations[3].Add(new Rectangle(48, 80, 48, 48));
            animations[3].Add(new Rectangle(96, 80, 48, 48));

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(96, 48, 32, 32));

            // GODMODE for Testing purposes
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(0, 80, 48, 48));
            animations[5].Add(new Rectangle(48, 80, 48, 48));
            animations[5].Add(new Rectangle(96, 80, 48, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }
    }
    public class Down_Attack : Attack
    {

        public Down_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Down Slash-Sheet"), position, true, currentWeapon)
        {
            SetAnimation(_gameRoot._player._currentWeapon);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 16));
            animations[0].Add(new Rectangle(48, 0, 48, 16));
            animations[0].Add(new Rectangle(96, 0, 48, 16));
            animations[0].Add(new Rectangle(144, 0, 48, 16));
            animations[0].Add(new Rectangle(192, 0, 48, 16));
            animations[0].Add(new Rectangle(240, 0, 48, 16));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 96, 32));
            animations[1].Add(new Rectangle(96, 16, 96, 32));
            animations[1].Add(new Rectangle(192, 16, 96, 32));
            animations[1].Add(new Rectangle(288, 16, 96, 32));
            animations[1].Add(new Rectangle(384, 16, 96, 32));
            animations[1].Add(new Rectangle(480, 16, 96, 32));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 48, 32, 32));
            animations[2].Add(new Rectangle(32, 48, 32, 32));
            animations[2].Add(new Rectangle(64, 48, 32, 32));


            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 80, 48, 48));
            animations[3].Add(new Rectangle(48, 80, 48, 48));
            animations[3].Add(new Rectangle(96, 80, 48, 48)); ;

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(96, 48, 32, 32));

            // GODMODE for Testing purposes
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(0, 80, 48, 48));
            animations[5].Add(new Rectangle(48, 80, 48, 48));
            animations[5].Add(new Rectangle(96, 80, 48, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }
    }
    public class Right_Attack : Attack
    {

        public Right_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Right Slash-Sheet"), position, true, currentWeapon)
        {
            SetAnimation(_gameRoot._player._currentWeapon);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 48));
            animations[0].Add(new Rectangle(16, 0, 16, 48));
            animations[0].Add(new Rectangle(32, 0, 16, 48));
            animations[0].Add(new Rectangle(48, 0, 16, 48));
            animations[0].Add(new Rectangle(64, 0, 16, 48));
            animations[0].Add(new Rectangle(80, 0, 16, 48));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 48, 32, 96));
            animations[1].Add(new Rectangle(32, 48, 32, 96));
            animations[1].Add(new Rectangle(64, 48, 32, 96));
            animations[1].Add(new Rectangle(96, 48, 32, 96));
            animations[1].Add(new Rectangle(128, 48, 32, 96));
            animations[1].Add(new Rectangle(160, 48, 32, 96));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 144, 32, 32));
            animations[2].Add(new Rectangle(32, 144, 32, 32));
            animations[2].Add(new Rectangle(64, 144, 32, 32));

            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 176, 48, 48));
            animations[3].Add(new Rectangle(48, 176, 48, 48));
            animations[3].Add(new Rectangle(96, 176, 48, 48));

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(96, 144, 32, 32));

            // GODMODE for Testing purposes
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(0, 176, 48, 48));
            animations[5].Add(new Rectangle(48, 176, 48, 48));
            animations[5].Add(new Rectangle(96, 176, 48, 48));
            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }
    }
    public class Left_Attack : Attack
    {

        public Left_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Left Slash-Sheet"), position, true, currentWeapon)
        {
            SetAnimation(_gameRoot._player._currentWeapon);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 48));
            animations[0].Add(new Rectangle(16, 0, 16, 48));
            animations[0].Add(new Rectangle(32, 0, 16, 48));
            animations[0].Add(new Rectangle(48, 0, 16, 48));
            animations[0].Add(new Rectangle(64, 0, 16, 48));
            animations[0].Add(new Rectangle(80, 0, 16, 48));

            // Great Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 48, 32, 96));
            animations[1].Add(new Rectangle(32, 48, 32, 96));
            animations[1].Add(new Rectangle(64, 48, 32, 96));
            animations[1].Add(new Rectangle(96, 48, 32, 96));
            animations[1].Add(new Rectangle(128, 48, 32, 96));
            animations[1].Add(new Rectangle(160, 48, 32, 96));

            // Mace attack animation
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 144, 32, 32));
            animations[2].Add(new Rectangle(32, 144, 32, 32));
            animations[2].Add(new Rectangle(64, 144, 32, 32));

            // Great Hammer attack animation
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(0, 176, 48, 48));
            animations[3].Add(new Rectangle(48, 176, 48, 48));
            animations[3].Add(new Rectangle(96, 176, 48, 48));

            // Bow attack animation
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(96, 144, 32, 32));

            // GODMODE for Testing purposes
            animations.Add(new List<Rectangle>());
            animations[5].Add(new Rectangle(0, 176, 48, 48));
            animations[5].Add(new Rectangle(48, 176, 48, 48));
            animations[5].Add(new Rectangle(96, 176, 48, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }
    }

    public class Arrow : Attack
    {
        public Vector2 Direction;

        public Arrow(Game1 gameRoot, Vector2 position, int currentWeapon, Vector2 _direction)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Arrow"), position, true, currentWeapon)
        {

            if (_direction != Vector2.Zero)
                Direction = Vector2.Normalize(_direction);
            else
                Direction = Vector2.UnitX;

            float angle = (float)Math.Atan2(Direction.Y, Direction.X);

            _rotation = angle + MathHelper.Pi / 2;

            _lifetime = 5f;
            CollisionLayer = CollisionLayer.Player;
            CollisionMask = CollisionLayer.Enemy & ~CollisionLayer.Player;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 12f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 16));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;

        }

        public override void Update(GameTime gameTime)
        {
            _velocity = Direction * 200f;

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_dead || _isHidden) return;

            spriteBatch.Draw(
                _texture,
                _position,          //  Use _position not _visibleBounds when using origin
                _txrSourceBounds,
                Color.White,
                _rotation,
                _origin,            //  Pivot point for rotation
                _drawScale,
                _flipEffect,
                _layerDepth
            );
        }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            base.OnCollideEvent(otherSprite);
        }

        protected override void OnTileCollideEvent(int tileX, int tileY)
        {
            base.OnTileCollideEvent(tileX, tileY);
            Dead = true;
        }
    }
}
