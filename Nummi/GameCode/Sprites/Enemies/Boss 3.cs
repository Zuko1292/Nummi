using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{

    public class Anaconda : SpriteEnemy
    {
        public override bool Dead
        {
            set
            {
                OnDeath();
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
                _dead = value;
            }
            get
            {
                return _dead;
            }
        }
        public Anaconda(Game1 gameRoot, Vector2 pos)
            : base(gameRoot, GBL.Content.Load<Texture2D>(""), pos, true, 1200, 340, 50, true, 200f, 350f, 1500f, 1500) 
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if(_gameRoot._bossesDeadNum <= 2)
            {
                _gameRoot._bossDead = true;
                _gameRoot._isNextLevelTails = true;
            }
        }

        public void OnDeath()
        {
            _gameRoot._bossesDeadNum += 1;
        }
    }

    public class Pig : SpriteEnemy
    {
        public override bool Dead
        {
            set
            {
                OnDeath();
                _gameRoot._player.OnEnemyKilled(_xpValue, _goldValue);
                _dead = value;
            }
            get
            {
                return _dead;
            }
        }

        public Pig(Game1 gameRoot, Vector2 pos)
            : base(gameRoot, GBL.Content.Load<Texture2D>(""), pos, true, 1200, 250, 40, true, 450f, 350f, 1500f, 1500)
        {

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_gameRoot._bossesDeadNum <= 2)
            {
                _gameRoot._bossDead = true;
                _gameRoot._isNextLevelTails = true;
            }
        }

        public void OnDeath()
        {
            _gameRoot._bossesDeadNum += 1;
        }
    }
}
