using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi.GameCode.Sprites;

namespace Nummi
{
    public class DialogBox : Sprite
    {
        protected Vector2 _txtPos;
        string _firstText, _secondText;
        bool _textChanged = false;

        public DialogBox(Game1 gameRoot, Vector2 position, string first, string second)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Dialog Box"), position, false, false)
        {
            _position = position;
            _firstText = first;
            _secondText = second;
        }

        public override void Update(GameTime gameTime)
        {
            _txtPos = (_position + new Vector2(16, 16));

            _visibleBounds = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width, _texture.Height);

            if(!_textChanged && GBL.KeyPress(Keys.E))
            {
                _textChanged = true;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            string _textToDisplay = _textChanged ? _secondText : _firstText;

            string[] lines = _textToDisplay.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            if(!_dead && !_isHidden)
            {
                foreach(var line in lines)
                {
                    GBL.spriteBatch.DrawString(_gameRoot.font, line, _txtPos, Color.White);
                    _txtPos.Y += _gameRoot.font.LineSpacing;
                }
            }
        }
    }
}
