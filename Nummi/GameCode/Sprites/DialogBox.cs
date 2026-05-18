using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    // Class for the Dialog box
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
        // This method positions the dialog box at the center of the screen horizontally and at 18% of the screen height vertically. This is a common position for dialog boxes in games, as it allows them to be easily noticed by the player without obstructing too much of the gameplay area.
        private void PositionToScreen()
        {
            int screenWidth = GBL.GD.Viewport.Width;
            int screenHeight = GBL.GD.Viewport.Height;

            _position = new Vector2(
                screenWidth / 2f,
                screenHeight * 0.18f
            );
        }
        // This method calculates the rectangle that represents the area of the dialog box on the screen. It uses the current position of the dialog box and its width and height to create a rectangle that can be used for drawing and collision detection. The rectangle is centered around the position, so it subtracts half of the width and height from the position to get the top-left corner of the rectangle.
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
            // This updates the visible bounds of the dialog box to match its current position and size. The visible bounds are used for drawing the dialog box on the screen and for any interactions that may occur with it. By calling GetBoxRect(), it ensures that the visible bounds are always accurate based on the current position of the dialog box.
            _visibleBounds = GetBoxRect();
            // This checks if the dialog box has just been opened. If it has, it sets the _justOpened flag to false and returns early from the update method. This allows the dialog box to be displayed for at least one frame before any input is processed, ensuring that the player has a chance to see the dialog box before it can be interacted with.
            if (_justOpened)
            {
                _justOpened = false;
                base.Update(gameTime);
                return;
            }
            // This checks if the E key has been pressed. If it has, it toggles the _textChanged flag. If _textChanged was previously false, it sets it to true, which will cause the second text to be displayed. If _textChanged was already true, it sets the _dead flag to true, which will cause the dialog box to be removed from the game. Additionally, if the current game state is HeadsLevel, it allows the player to move again by setting _canMove to true.
            if (GBL.KeyPress(Keys.E))
            {
                if (!_textChanged)
                {
                    _textChanged = true;
                }
                else
                {
                    _dead = true;
                    if(_gameRoot._gameState == GameState.HeadsLevel)
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
            // This splits the text to be displayed into lines based on the newline character. It then iterates through each line and draws it on the screen using the sprite batch. The text is drawn starting from the calculated textStart position, and after each line is drawn, the Y position is incremented by the line spacing of the font to ensure that the lines are properly spaced vertically.
            foreach (var line in lines)
            {
                GBL.spriteBatch.DrawString(_gameRoot.font, line, textStart, Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.08f);
                textStart.Y += _gameRoot.font.LineSpacing;
            }
        }
    }
}