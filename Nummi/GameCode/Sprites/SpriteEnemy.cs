using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    public class SpriteEnemy : SpriteCharacter
    {
        public override bool Dead
        {
            set
            {
                _dead = value;
            }

            get
            {
                return _dead;
            }
        }

        public SpriteEnemy(Game1 gameRoot, Texture2D texture, Vector2 position, bool canMove)
            : base(gameRoot, texture, position, canMove)
        {
            CollisionLayer = CollisionLayer.Enemy;
            CollisionMask = CollisionLayer.All & ~CollisionLayer.Enemy;
            InitBounds(position, true, new Vector2(1f,1f));
            _layerDepth = 0.1f;
        }
    }
}
