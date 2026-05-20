using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    // This class is for Text Button for example the play button so text highlights and you can click it
    public class TextButton
    {
        private SpriteFont _font;
        private string _text;
        private Vector2 _position;

        private float _scale;
        private float _defaultScale = 1f;
        private float _hoverScale = 1.2f;

        private Color _defaultColor = Color.White;
        private Color _hoverColor = Color.Yellow;
        private Color _currentColor;

        // These properties track whether the button is being hovered over or clicked, which can be used in the game logic to trigger actions when the button is interacted with.
        public bool IsHovering { get; private set; }
        public bool IsClicked { get; private set; }

        public TextButton(SpriteFont font, string text, Vector2 position, Color color)
        {
            _font = font;
            _text = text;
            _position = position;

            _scale = _defaultScale;
            _currentColor = color;
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Point mousePos = mouse.Position;

            Rectangle bounds = GetBounds();

            IsHovering = bounds.Contains(mousePos);

            // Handles the clicking
            if(IsHovering && GBL.LeftClick)
            {
                IsClicked = true;
            }
             else
            {
                IsClicked = false;
            }
            // Handles the hovering animation and color change
            float targetScale = IsHovering ? _hoverScale : _defaultScale;
            _scale = MathHelper.Lerp(_scale, targetScale, 0.15f);

            Color targetColor = IsHovering ? _hoverColor : _defaultColor;
            _currentColor = Color.Lerp(_currentColor, targetColor, 0.15f);
        }
        // The draw makes it so the position of the text is the middle of the text
        public void Draw()
        {
            Vector2 textSize = _font.MeasureString(_text);
            Vector2 origin = textSize / 2f;

            GBL.Game.FancyText(
                _font,
                _text,
                _position,
                _currentColor,
                Color.Black,
                _scale);
        }
        // Gets the rectangle of the text for the hovering and clicking detection, it also takes into account the scale of the text so that it works correctly when hovering and clicking
        private Rectangle GetBounds()
        {
            Vector2 textSize = _font.MeasureString(_text);
            Vector2 scaledSize = textSize * _scale;

            return new Rectangle(
                (int)(_position.X - scaledSize.X / 2f),
                (int)(_position.Y - scaledSize.Y / 2f),
                (int)scaledSize.X,
                (int)scaledSize.Y
            );
        }
    }

}
