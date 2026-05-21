using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // TODO make Croc boss
    public class Manager_Croc : SpriteEnemy
    {
        TempState _state;
        public enum TempState
        {
            Frozen,
            Thawed
        }

        public Manager_Croc(Game1 gameRoot, Vector2 position, TempState state)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\Animations\\Croc_Boss"), position, true, 100, 200, 30, true, 50f, 400f, 300f, 300)
        {

        }

        public override void Update(GameTime gameTime)
        {
            if (_state == TempState.Frozen)
                return;
            else if (_state == TempState.Thawed)
            {
                base.Update(gameTime);
            } 
        }
    }
}
