using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class Bartender : SpriteNPC
    {
        public Bartender(Game1 gameRoot, Vector2 position) : 
            base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Bartender-Sheet"), position, false, 0f, 0f, 0f, 
                new List<string>() 
                { 
                    "I hate my life..." 
                })
        {

        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 10f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 32, 64));
            animations[0].Add(new Rectangle(32, 0, 32, 64));
            animations[0].Add(new Rectangle(64, 0, 32, 64));
            animations[0].Add(new Rectangle(96, 0, 32, 64));
            animations[0].Add(new Rectangle(128, 0, 32, 64));
            animations[0].Add(new Rectangle(160, 0, 32, 64));
            animations[0].Add(new Rectangle(192, 0, 32, 64));
            animations[0].Add(new Rectangle(224, 0, 32, 64));
            animations[0].Add(new Rectangle(256, 0, 32, 64));
            animations[0].Add(new Rectangle(288, 0, 32, 64));
            animations[0].Add(new Rectangle(320, 0, 32, 64));
            animations[0].Add(new Rectangle(352, 0, 32, 64));


            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }
    }
}
