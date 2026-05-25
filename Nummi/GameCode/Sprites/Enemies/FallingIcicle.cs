using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nummi;

namespace Nummi
{
    public class FallingIcicle : SpriteEnemy
    {
        private const float FallSpeed = 280f;
        private const float FallDistance = 7f * 32f;

        private Vector2 _spawnPos;

        public FallingIcicle(Game1 gameRoot, Vector2 position)
            : base(gameRoot,
                   GBL.Content.Load<Texture2D>("Textures\\TileSets\\Dungeon 2 TileSheet"),
                   position,
                   canMove: true,
                   health: 10000,
                   knockbackStrength: 200,
                   damageStrength: 20,
                   isBoss: false,
                   moveSpeed: 0f,
                   aggroRange: 0f,
                   xpValue: 0f,
                   goldValue: 0)
        {
            _spawnPos = position;
            _isIndestructible = true;
            _canPatrol = true;
            _isPatrolling = true;
            _velocity = new Vector2(0f, FallSpeed);
            SetAnimation(0);
        }

        protected override List<List<Rectangle>> BuildAnimations()
        {
            _frameDuration = 1f / 8f;
            var a = new List<List<Rectangle>>();
            a.Add(new List<Rectangle>());
            a[0].Add(new Rectangle(224, 32, 32, 32));

            _nextAnim = new List<int>();
            for (int i = 0; i < a.Count; i++) _nextAnim.Add(i);
            return a;
        }

        public override void Update(GameTime gameTime)
        {
            _isPatrolling = true;
            _velocity = new Vector2(0f, FallSpeed);

            if (_position.Y - _spawnPos.Y >= FallDistance)
            {
                _dead = true;
            }

            base.Update(gameTime);
        }
    }
}
