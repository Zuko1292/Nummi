using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class DroppedWeapon : SpriteCollectable
    {
        int _weaponType;

        public DroppedWeapon(Game1 gameRoot, Texture2D texture, Vector2 position, int WeaponType)
            : base(gameRoot, texture, position)
        {
            _weaponType = WeaponType;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {        
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Sword
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 32));

            // Great Sword
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(32, 0, 32, 32));

            // Mace
            animations.Add(new List<Rectangle>());
            animations[2].Add(new Rectangle(64, 0, 32, 32));

            // Great Hammer 
            animations.Add(new List<Rectangle>());
            animations[3].Add(new Rectangle(96, 0, 32, 32));

            // Bow
            animations.Add(new List<Rectangle>());
            animations[4].Add(new Rectangle(128, 0, 32, 32));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            SetAnimation(_weaponType);
        }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            if (otherSprite.GetType() == typeof(SpritePlayer))
            {
                SpritePlayer player = (SpritePlayer)otherSprite;
                player.PickupWeapon(_weaponType);
                _dead = true;
            }
        }
    }
}
