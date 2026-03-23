using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    public class SpriteCharacter : SpriteAnimating
    {


        #region ***** Constructors *****

        public SpriteCharacter(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove)
            : base(gameRoot, texture, position, canMove, true) 
        { 
        }

        public SpriteCharacter(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove, bool canCollide)
            : base(gameRoot, texture, position, canMove, canCollide)
        {
        }

        #endregion ***** Constructors *****

    }
}
