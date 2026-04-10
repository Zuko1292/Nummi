using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class TallPurpleSlime : SpriteEnemy
    {
        public TallPurpleSlime(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Purple Slime Anim-Sheet-Tall"), position, true, 1000, 220, 10, false, 50)
        {
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(384, 0, 32, 80));
            // Walking
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 32, 80));
            animations[1].Add(new Rectangle(32, 0, 32, 80));
            animations[1].Add(new Rectangle(64, 0, 32, 80));
            animations[1].Add(new Rectangle(96, 0, 32, 80));
            animations[1].Add(new Rectangle(128, 0, 32, 80));
            animations[1].Add(new Rectangle(160, 0, 32, 80));
            animations[1].Add(new Rectangle(192, 0, 32, 80));
            animations[1].Add(new Rectangle(224, 0, 32, 80));
            animations[1].Add(new Rectangle(256, 0, 32, 80));
            animations[1].Add(new Rectangle(288, 0, 32, 80));
            animations[1].Add(new Rectangle(320, 0, 32, 80));
            animations[1].Add(new Rectangle(352, 0, 32, 80));

            _nextAnim = new List<int>();
            for (int i = 0; i < animations.Count; i++) _nextAnim.Add(i);
            return animations;
        }

        public override void Update(GameTime gameTime)
        {
            if (_velocity == Vector2.Zero) SetAnimation(0);
            else SetAnimation(1);

            if (_lastSeenTimer <= 1f) SetAnimation(0);

            base.Update(gameTime);
        }
    }
}
