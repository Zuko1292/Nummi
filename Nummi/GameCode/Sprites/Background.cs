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
            animations[0].Add(new Rectangle(0, 0, 800, 480));
            animations[0].Add(new Rectangle(800, 0, 800, 480));
            animations[0].Add(new Rectangle(1600, 0, 800, 480));
            animations[0].Add(new Rectangle(2400, 0, 800, 480));
            animations[0].Add(new Rectangle(3200, 0, 800, 480));
            animations[0].Add(new Rectangle(4000, 0, 800, 480));
            animations[0].Add(new Rectangle(4800, 0, 800, 480));
            animations[0].Add(new Rectangle(5600, 0, 800, 480));
            animations[0].Add(new Rectangle(6400, 0, 800, 480));
            animations[0].Add(new Rectangle(7200, 0, 800, 480));
            animations[0].Add(new Rectangle(8000, 0, 800, 480));
            animations[0].Add(new Rectangle(8800, 0, 800, 480));
            animations[0].Add(new Rectangle(9600, 0, 800, 480));
            animations[0].Add(new Rectangle(10400, 0, 800, 480));


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
