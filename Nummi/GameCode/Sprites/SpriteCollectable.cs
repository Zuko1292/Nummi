using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // this class is for the collectable items in the game, like coins and gems. It inherits from SpriteAnimating because it has a simple animation of spinning in place. The collision layer is set to Collectable and the collision mask is set to Player, so that the player can collide with it and collect it. The layer depth is set to 0.31f so that it is drawn above the background but below the player and other solid objects.
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
