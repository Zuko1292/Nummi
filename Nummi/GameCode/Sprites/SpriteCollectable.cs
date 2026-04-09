using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class SpriteCollectable : SpriteAnimating
    {
        public SpriteCollectable(Game1 gameRoot, Texture2D texture, Vector2 position)
                : base(gameRoot, texture, position, false, true) 
        {
                CollisionLayer = CollisionLayer.Collectable;
                CollisionMask = CollisionLayer.Player;
                _layerDepth = 0.31f;
        }
    }
}
