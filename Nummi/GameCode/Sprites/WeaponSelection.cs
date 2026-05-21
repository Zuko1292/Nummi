using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    // UI sprite that lets the player pick a weapon. Opened by the blacksmith NPC
    // after their dialogue ends. Click an entry (or press 1-5) to equip it.
    public class WeaponSelection : Sprite
    {
        private readonly List<string> _weapons = new List<string>
        {
            "Sword", "Great Sword", "Mace", "Great Hammer", "Bow"
        };

        private int _boxWidth = 320;
        private int _rowHeight = 36;
        private int _padding = 16;

        public WeaponSelection(Game1 gameRoot)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\UI\\Dialog Box"),
                   Vector2.Zero, false, false)
        {
            _gameRoot = gameRoot;
            _gameRoot._player._canMove = false;
            PositionToScreen();
        }

        private void PositionToScreen()
        {
            int sw = GBL.GD.Viewport.Width;
            int sh = GBL.GD.Viewport.Height;
            _position = new Vector2(sw / 2f, sh / 2f);
        }

        private Rectangle GetBoxRect()
        {
            int h = _padding * 2 + _rowHeight * _weapons.Count;
            return new Rectangle(
                (int)(_position.X - _boxWidth / 2f),
                (int)(_position.Y - h / 2f),
                _boxWidth,
                h
            );
        }

        private Rectangle GetRowRect(Rectangle box, int i)
        {
            return new Rectangle(
                box.X + _padding,
                box.Y + _padding + i * _rowHeight,
                box.Width - _padding * 2,
                _rowHeight - 4
            );
        }

        public override void Update(GameTime gameTime)
        {
            PositionToScreen();
            _visibleBounds = GetBoxRect();

            int picked = -1;

            if (GBL.KeyPress(Keys.D1)) picked = 0;
            if (GBL.KeyPress(Keys.D2)) picked = 1;
            if (GBL.KeyPress(Keys.D3)) picked = 2;
            if (GBL.KeyPress(Keys.D4)) picked = 3;
            if (GBL.KeyPress(Keys.D5)) picked = 4;

            if (GBL.LeftClick)
            {
                Rectangle box = GetBoxRect();
                Point mp = Mouse.GetState().Position;
                for (int i = 0; i < _weapons.Count; i++)
                {
                    if (GetRowRect(box, i).Contains(mp))
                    {
                        picked = i;
                        break;
                    }
                }
            }

            if (picked >= 0)
            {
                _gameRoot._player._currentWeapon = picked;
                _dead = true;
                _gameRoot._player._canMove = true;
            }

            base.Update(gameTime);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (_dead || _isHidden) return;

            Rectangle box = GetBoxRect();

            GBL.spriteBatch.Draw(
                _texture,
                box,
                null,
                Color.White,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.09f
            );

            Point mp = Mouse.GetState().Position;

            for (int i = 0; i < _weapons.Count; i++)
            {
                Rectangle row = GetRowRect(box, i);
                bool hover = row.Contains(mp);

                string label = (i + 1) + ". " + _weapons[i];
                Vector2 size = _gameRoot.font.MeasureString(label);
                Vector2 pos = new Vector2(
                    row.X + 12f,
                    row.Y + (row.Height - size.Y) / 2f
                );

                GBL.spriteBatch.DrawString(
                    _gameRoot.font,
                    label,
                    pos,
                    hover ? Color.Yellow : Color.White,
                    0f,
                    Vector2.Zero,
                    1f,
                    SpriteEffects.None,
                    0.08f
                );
            }
        }
    }
}
