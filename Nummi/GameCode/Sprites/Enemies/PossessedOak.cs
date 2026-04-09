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
        public PossessedOak(Game1 gameRoot, Texture2D texture, Vector2 position)
            : base(gameRoot, texture, position, true, 200, 220, 10, true, 0)
        { 
        }
    }
}
