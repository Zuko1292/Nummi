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
    // This top class is for drawing the grid however the camera is really weird when trying to build it with pixels and it looks weird so ignore the top class and just look at the bottom one, which is the one that is actually used in the game. The top class was an attempt to draw the grid with pixels, but it ended up looking really weird due to how the camera works in the game. The bottom class is a simple sprite that is drawn over the grid when in build mode, and it looks much better.
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
                    new Rectangle(
                        (int)(grid.Origin.X + x * grid.TileSize),
                        (int)grid.Origin.Y,
                        1,
                        grid.Height * grid.TileSize
                    ),
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
                    new Rectangle(
                        (int)grid.Origin.X,
                        (int)(grid.Origin.Y + y * grid.TileSize),
                        grid.Width * grid.TileSize,
                        1
                    ),
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

    public class Grid : Sprite
    {

        public Grid(Game1 gameRoot, Vector2 position)
            : base(gameRoot, GBL.Content.Load<Texture2D>("Textures\\UI\\Grid"), position, false, false)
        {
            _isHidden = true;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (!_gameRoot.buildingSystem.buildMode) _isHidden = true;
            else _isHidden = false;
        }
    }
}
