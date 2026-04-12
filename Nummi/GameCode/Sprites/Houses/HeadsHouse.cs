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
        // Need to scale the houses up and fix the collisions so can walk in front of the houses
        public HeadsHouse(Game1 gameRoot, Texture2D texture, Vector2 position)
                : base(gameRoot, texture, position, false, true) 
        {
            CollisionLayer = CollisionLayer.Solid;
            CollisionMask = CollisionLayer.Player;
            _layerDepth = 0.31f;

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
