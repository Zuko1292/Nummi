using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class BigOrangeSlime : SpriteEnemy
    {
        public BigOrangeSlime(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Orange Slime Anim-Sheet-Big"), position, true, 60, 250, 20, false, 25)
        {

        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 15f;
            List<List<Rectangle>> animations = new List<List<Rectangle>>();

            // Idle
            animations.Add(new List<Rectangle>());
            animations[0].Add(new Rectangle(0, 0, 64, 48));
            // Walking
            animations.Add(new List<Rectangle>());
            animations[1].Add(new Rectangle(0, 0, 64, 48));
            animations[1].Add(new Rectangle(64, 0, 64, 48));
            animations[1].Add(new Rectangle(128, 0, 64, 48));
            animations[1].Add(new Rectangle(192, 0, 64, 48));
            animations[1].Add(new Rectangle(256, 0, 64, 48));
            animations[1].Add(new Rectangle(320, 0, 64, 48));
            animations[1].Add(new Rectangle(384, 0, 64, 48));
            animations[1].Add(new Rectangle(448, 0, 64, 48));
            animations[1].Add(new Rectangle(512, 0, 64, 48));
            animations[1].Add(new Rectangle(576, 0, 64, 48));
            animations[1].Add(new Rectangle(640, 0, 64, 48));
            animations[1].Add(new Rectangle(704, 0, 64, 48));

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

    public class BigOrangeSlimeProjectile : SpriteEnemyProjectile
    {

        public BigOrangeSlimeProjectile(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>(""), position, 200f, 10)
        {
            CollisionLayer = CollisionLayer.Enemy;
            CollisionMask = CollisionLayer.Player;
            InitBounds(position, true, new Vector2(1f, 1f));
            _layerDepth = 0.1f;
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
