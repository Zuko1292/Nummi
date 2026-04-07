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
    }
}
