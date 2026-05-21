using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // TODO make a security guard class
    public class Security_Guard : SpriteEnemy
    {
        
        public Security_Guard(Game1 gameRoot, Texture2D texture, Vector2 position)
        : base(gameRoot, texture, position, true, 50, 500, 5, false, 50, 600f, 100f, 40) 
        {

        }
        enum TempState
        {
            Frozen,
            Thawed
        }
    }
}
