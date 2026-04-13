using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

namespace Nummi
{
    public class GridRenderer
    {
        private Texture2D pixel;
        private GridSystem grid;

        public bool Visible = true;

        public GridRenderer(GraphicsDevice device, GridSystem grid)
        {
            this.grid = grid;

            pixel = new Texture2D(device, 1, 1);
            pixel.SetData(new[] { Color.White });
        }

        public void Draw()
        {
            if (!Visible) return;

            Color color = Color.Blue * 0.4f;

            for (int x = 0; x <= grid.Width; x++)
            {
                GBL.spriteBatch.Draw(
                    pixel,
                    new Rectangle(x * grid.TileSize, 0, 1, grid.Height * grid.TileSize),
                    null,
                    color,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.88f
                );
            }

            for (int y = 0; y <= grid.Height; y++)
            {
                GBL.spriteBatch.Draw(
                    pixel,
                    new Rectangle(0, y * grid.TileSize, grid.Width * grid.TileSize, 1),
                    null,
                    color,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.88f
                );
            }
        }
    }
}
