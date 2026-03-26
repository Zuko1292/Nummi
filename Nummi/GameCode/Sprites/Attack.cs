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
    public class Attack : SpriteAnimating
    {
        public float _weaponDamage;


        public Attack(Game1 gameRoot, Texture2D weaponTxr, Vector2 position, bool canMove, int currentWeapon) : 
            base(gameRoot,weaponTxr, position, canMove, true)
        {
        }

        public override void Update(GameTime gameTime)
        {
            _weaponDamage = 10;

            _velocity = _gameRoot._player._velocity;
            base.Update(gameTime);
        }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            base.OnCollideEvent(otherSprite);

            if(otherSprite is SpriteEnemy enemy)
            {
                Dead = true;
            }
        }
    }
    public class Up_Attack : Attack
    {

        public Up_Attack(Game1 gameRoot, Vector2 position, int currentWeapon) :
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Up Slash-Sheet"), position, true, currentWeapon)
        {
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 16));
            animations[0].Add(new Rectangle(48, 0, 48, 16));
            animations[0].Add(new Rectangle(96, 0, 48, 16));
            animations[0].Add(new Rectangle(144, 0, 48, 16));
            animations[0].Add(new Rectangle(192, 0, 48, 16));
            animations[0].Add(new Rectangle(240, 0, 48, 16));

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
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 16));
            animations[0].Add(new Rectangle(48, 0, 48, 16));
            animations[0].Add(new Rectangle(96, 0, 48, 16));
            animations[0].Add(new Rectangle(144, 0, 48, 16));
            animations[0].Add(new Rectangle(192, 0, 48, 16));
            animations[0].Add(new Rectangle(240, 0, 48, 16));

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
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 48));
            animations[0].Add(new Rectangle(0, 16, 16, 48));
            animations[0].Add(new Rectangle(0, 32, 16, 48));
            animations[0].Add(new Rectangle(0, 48, 16, 48));
            animations[0].Add(new Rectangle(0, 64, 16, 48));
            animations[0].Add(new Rectangle(0, 92, 16, 48));

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
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword attack animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 16, 48));
            animations[0].Add(new Rectangle(0, 16, 16, 48));
            animations[0].Add(new Rectangle(0, 32, 16, 48));
            animations[0].Add(new Rectangle(0, 48, 16, 48));
            animations[0].Add(new Rectangle(0, 64, 16, 48));
            animations[0].Add(new Rectangle(0, 92, 16, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }
    }
}
