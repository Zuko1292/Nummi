using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class DialogBox : Sprite
    {
        protected Vector2 _txtPos;
        private string _firstText, _secondText;
        private bool _textChanged = false;
        private bool _justOpened = true;

        private int _boxWidth = 600;
        private int _boxHeight = 120;

        public DialogBox(Game1 gameRoot, string first, string second)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\UI\\Dialog Box"),
                   Vector2.Zero, false, false)
        {
            _firstText = first;
            _secondText = second;
            PositionToScreen();
        }

        private void PositionToScreen()
        {
            int screenWidth = GBL.GD.Viewport.Width;
            int screenHeight = GBL.GD.Viewport.Height;

            _position = new Vector2(
                screenWidth / 2f,
                screenHeight * 0.18f
            );
        }

        private Rectangle GetBoxRect()
        {
            return new Rectangle(
                (int)(_position.X - _boxWidth / 2f),
                (int)(_position.Y - _boxHeight / 2f),
                _boxWidth,
                _boxHeight
            );
        }

        public override void Update(GameTime gameTime)
        {
            PositionToScreen();

            _visibleBounds = GetBoxRect();

            if (_justOpened)
            {
                _justOpened = false;
                base.Update(gameTime);
                return;
            }

            if (GBL.KeyPress(Keys.E))
            {
                if (!_textChanged)
                {
                    _textChanged = true;
                }
                else
                {
                    _dead = true;
                    _gameRoot._player._canMove = true;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_dead || _isHidden) return;

            Rectangle boxRect = GetBoxRect();

            GBL.spriteBatch.Draw(
                _texture,
                boxRect,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.09f
            );

            Vector2 textStart = new Vector2(
                boxRect.X + 20f,
                boxRect.Y + 20f
            );

            string textToDisplay = _textChanged ? _secondText : _firstText;
            string[] lines = textToDisplay.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var line in lines)
            {
                GBL.spriteBatch.DrawString(_gameRoot.font, line, textStart, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.08f);
                textStart.Y += _gameRoot.font.LineSpacing;
            }
        }
    }
}