using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi.GameCode.Sprites;

namespace Nummi
{
    public class SpriteNPC: SpriteCharacter
    {
        protected bool _isTalking = false;
        protected float _speechTimer = 0f;

        public SpriteNPC(Game1 gameRoot, Vector2 position, bool canMove,float speechTimer)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Player_SpriteSheet"), position, canMove)
        {
            _canFlip = true;
            _layerDepth = 0.31f;
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
    
            List<List<Rectangle>> animations = new List<List<Rectangle>>();
    
            // Idle animation
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 64, 64));
    
            // Walk animation
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(64, 0, 64, 64));
            animations[1].Add(new Rectangle(128, 0, 64, 64));
            animations[1].Add(new Rectangle(192, 0, 64, 64));
            animations[1].Add(new Rectangle(256, 0, 64, 64));
    
            return animations;
        }
}
