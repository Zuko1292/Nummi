using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class Background : SpriteAnimating
    {
        int _currentbg;
        // 1 = Main Menu
        // 2 = Play Menu
        // 3 = Guide Menu

        public Background(Game1 gameRoot, Texture2D currentbg, int currentbgnum)
            : base(gameRoot, currentbg, new Vector2(gameRoot._screenBounds.Width / 2, gameRoot._screenBounds.Height / 2), false, false)
        {
            _layerDepth = 1f;
            _texture = currentbg;
            _currentbg = currentbgnum;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;

            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Spinning Coin
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 320, 320));
            animations[0].Add(new Rectangle(320, 0, 320, 320));
            animations[0].Add(new Rectangle(640, 0, 320, 320));
            animations[0].Add(new Rectangle(960, 0, 320, 320));
            animations[0].Add(new Rectangle(1280, 0, 320, 320));
            animations[0].Add(new Rectangle(1600, 0, 320, 320));
            animations[0].Add(new Rectangle(1920, 0, 320, 320));
            animations[0].Add(new Rectangle(2240, 0, 320, 320));
            animations[0].Add(new Rectangle(2560, 0, 320, 320));
            animations[0].Add(new Rectangle(2880, 0, 320, 320));
            animations[0].Add(new Rectangle(3200, 0, 320, 320));
            animations[0].Add(new Rectangle(3520, 0, 320, 320));
            animations[0].Add(new Rectangle(3840, 0, 320, 320));
            animations[0].Add(new Rectangle(3160, 0, 320, 320));


            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);

            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            if(_gameRoot._player != null) _position = _gameRoot._player._position;

            base.Update(gameTime);
        }
    }
}
