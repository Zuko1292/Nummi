using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class SpriteCollectable : SpriteAnimating
    {
        public SpriteCollectable(Game1 gameRoot, Texture2D texture, Vector2 position)
                : base(gameRoot, texture, position, false, false) { }

        protected override void OnCollideEvent(Sprite otherSprite)
        {
            if (otherSprite.GetType() == typeof(SpritePlayer))
            {
                _dead = true;
            }
        }
}
}
