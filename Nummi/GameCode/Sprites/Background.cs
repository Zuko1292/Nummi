using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class Background : Sprite
    {
        public Background(Game1 gameRoot, Texture2D currentbg)
            : base(gameRoot, currentbg, new Vector2(gameRoot._screenBounds.Width / 2, gameRoot._screenBounds.Height / 2), false, false)
        {
            _layerDepth = 1f;
            _texture = currentbg;
        }

        public override void Update(GameTime gameTime)
        {
            if(_gameRoot._player != null) _position = _gameRoot._player._position;

            base.Update(gameTime);
        }
    }
}
