using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Nummi.GameCode.Sprites.NPCs
{
    public class Bartender : SpriteNPC
    {
        public Bartender(Game1 gameRoot, Texture2D texture, Vector2 position) : 
            base(gameRoot, texture, position, false, 0f, 0f, 0f, null)
        {

        }
    }
}
