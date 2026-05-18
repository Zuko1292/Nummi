using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    // This is the base class for all characters in the game, including the player and NPCs. It inherits from SpriteAnimating, which means it has all the functionality of a sprite that can animate, but it also has some additional functionality specific to characters, such as movement and collision handling. The constructors allow you to specify whether the character can move and whether it can collide with other objects, which gives you flexibility in how you want to use this class for different types of characters.
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
