using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
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

        public bool IsHovering { get; private set; }
        public bool IsClicked { get; private set; }

        public TextButton(SpriteFont font, string text, Vector2 position)
        {
            _font = font;
            _text = text;
            _position = position;

            _scale = _defaultScale;
            _currentColor = _defaultColor;
        }

        public void Update()
        {
            MouseState mouse = Mouse.GetState();
            Point mousePos = mouse.Position;

            Rectangle bounds = GetBounds();

            IsHovering = bounds.Contains(mousePos);

            if(IsHovering && GBL.LeftClick)
            {
                IsClicked = true;
            }
             else
            {
                IsClicked = false;
            }

            float targetScale = IsHovering ? _hoverScale : _defaultScale;
            _scale = MathHelper.Lerp(_scale, targetScale, 0.15f);

            Color targetColor = IsHovering ? _hoverColor : _defaultColor;
            _currentColor = Color.Lerp(_currentColor, targetColor, 0.15f);
        }

        public void Draw()
        {
            Vector2 textSize = _font.MeasureString(_text);
            Vector2 origin = textSize / 2f; 

            GBL.spriteBatch.DrawString(
                _font,
                _text,
                _position,
                _currentColor,
                0f,
                origin,
                _scale,
                SpriteEffects.None,
                0.01f
            );
        }

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
