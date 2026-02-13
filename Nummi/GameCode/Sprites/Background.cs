using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi.GameCode.Sprites;

namespace Nummi.GameCode.Sprites
{
    public class Background : Sprite
    {
        public Background(Game1 gameRoot, Texture2D currentbg)
            : base(gameRoot, currentbg, new Vector2(gameRoot._screenBounds.Width / 2, gameRoot._screenBounds.Height / 2), false, false)
        {
            _layerDepth = 0.9f;
            _texture = currentbg;
        }
    }
}
