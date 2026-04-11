using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{

    public class PossessedOak : SpriteEnemy
    {

        public override bool Dead
        {
            set
            {
                _dead = value;
                if (_dead) OnDeath();
            }
            get
            {
                return _dead;
            }
        }

        public PossessedOak(Game1 gameRoot, Texture2D texture, Vector2 position)
            : base(gameRoot, texture, position, true, 300, 220, 10, true, 0)
        {
        }

        public void OnDeath()
        {
            // Drop loot, play death animation, etc. 
        }
    }
}
