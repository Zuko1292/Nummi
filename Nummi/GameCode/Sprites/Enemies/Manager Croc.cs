using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // TODO make Croc boss
    public class Manager_Croc : SpriteEnemy
    {
        enum TempState
        {
            Frozen,
            Thawed
        }

        public Manager_Croc(Game1 gameRoot, Texture2D texture, Vector2 position)
            : base(gameRoot, texture, position, true, 100, 200, 30, true, 50f, 400f, 300f, 300)
        {

        }
    }
}
