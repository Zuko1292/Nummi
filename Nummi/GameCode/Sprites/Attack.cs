using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Mime;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi.GameCode.Sprites
{
    public class Attack : SpriteAnimating
    {
        public float _weaponDamage;

        public Attack(Game1 gameRoot, Vector2 position, bool canMove, int currentWeapon) : 
            base(gameRoot,GBL.Content.Load<Texture2D>("Textures\\Animations\\Attacking_Anim_Sword"), position, canMove, true)
        {
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Up
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 48, 16));

            // Down
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 16, 48, 16));

            // Left
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(0, 32, 16, 48));

            // Right
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(16, 32, 16, 48));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            if(GBL.mousePos.X < _position.X && 
                (_gameRoot._player._animIndex == 5 || _gameRoot._player._animIndex == 6)) SetAnimation(2);
            else if(GBL.mousePos.X > _position.X &&
                (_gameRoot._player._animIndex == 5 || _gameRoot._player._animIndex == 2)) SetAnimation(3);
            else if(GBL.mousePos.Y < _position.Y &&
                (_gameRoot._player._animIndex == 3 || _gameRoot._player._animIndex == 1)) SetAnimation(0);
            else if(GBL.mousePos.Y > _position.Y &&
                (_gameRoot._player._animIndex == 4 || _gameRoot._player._animIndex == 0)) SetAnimation(1);

            if (_animIndex == 0) _position -= new Vector2(0, 16);
            else if(_animIndex == 1) _position += new Vector2(0, 16);
            else if(_animIndex == 2) _position -= new Vector2(16, 0);
            else if(_animIndex == 3) _position += new Vector2(16, 0);

            _weaponDamage = 10;
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
}
