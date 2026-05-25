using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class HeadsHouse : Sprite
    {
        // This makes the heads houses and scales them so you can walk infront of them but they still look big and not like a tiny house. The collision bounds are also adjusted so you can walk infront of the top part of the house but not the bottom part, which makes it feel more natural and less frustrating to navigate around them.
        public HeadsHouse(Game1 gameRoot, Texture2D texture, Vector2 position, Vector2 drawScale)
                : base(gameRoot, texture, position, false, true) 
        {
            CollisionLayer = CollisionLayer.Solid;
            CollisionMask = CollisionLayer.Player | CollisionLayer.Enemy;
            _layerDepth = 0.32f;

            _drawScale = drawScale;

        }

        protected override void UpdateBounds(GameTime gameTime)
        {
            _visibleBounds = new Rectangle(
                (_position - _origin).ToPoint(),
                (_txrSourceBounds.Size.ToVector2() * _drawScale).ToPoint()
            );

            if (!_canCollide) return;

            int width = (int)(_txrSourceBounds.Width * _drawScale.X);
            int fullHeight = (int)(_txrSourceBounds.Height * _drawScale.Y);

            int collisionHeight = (int)(fullHeight * 0.75f);

            _collisionBounds = new Rectangle(
                (int)(_position.X - width / 2f),
                (int)(_position.Y - fullHeight / 2f), 
                width,
                collisionHeight
            );
        }
    }
}
