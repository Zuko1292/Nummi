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
    public class BuildingSystem
    {
        private GridSystem grid;

        public List<PlacedBuilding> placedBuildings = new();

        public Dictionary<Keys, BuildingType> hotkeys = new();

        private BuildingType selectedBuilding = null;
        private bool buildMode = false;

        public BuildingSystem(GridSystem grid)
        {
            this.grid = grid;
        }

        public void Update(Vector2 mouseWorld)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            // Select building with keys
            foreach (var pair in hotkeys)
            {
                if (keyboard.IsKeyDown(pair.Key))
                {
                    selectedBuilding = pair.Value;
                    buildMode = true;
                }
            }

            if (!buildMode || selectedBuilding == null)
                return;

            Point gridPos = grid.WorldToGrid(mouseWorld);

            if (!grid.IsInside(gridPos))
                return;

            // Place building
            if (mouse.LeftButton == ButtonState.Pressed)
            {
                if (CanPlace(gridPos, selectedBuilding.Size))
                {
                    for (int x = 0; x < selectedBuilding.Size.X; x++)
                    {
                        for (int y = 0; y < selectedBuilding.Size.Y; y++)
                        {
                            grid.SetOccupied(
                                new Point(gridPos.X + x, gridPos.Y + y), true);
                        }
                    }

                    placedBuildings.Add(new PlacedBuilding
                    {
                        Position = gridPos,
                        Type = selectedBuilding
                    });
                }
            }
        }

        private bool CanPlace(Point pos, Point size)
        {
            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    Point check = new Point(pos.X + x, pos.Y + y);

                    if (!grid.IsInside(check) || grid.IsOccupied(check))
                        return false;
                }
            }
            return true;
        }

        public void Draw(Vector2 mouseWorld)
        {
            // Draw placed
            foreach (var b in placedBuildings)
            {
                GBL.spriteBatch.Draw(
                    b.Type.Texture,
                    new Rectangle(
                        b.Position.X * grid.TileSize,
                        b.Position.Y * grid.TileSize,
                        b.Type.Size.X * grid.TileSize,
                        b.Type.Size.Y * grid.TileSize
                    ),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.87f
                );
            }

            // Draw ghost
            if (!buildMode || selectedBuilding == null)
                return;

            Point gridPos = grid.WorldToGrid(mouseWorld);
            Vector2 snapped = grid.GridToWorld(gridPos);

            bool valid = CanPlace(gridPos, selectedBuilding.Size);
            Color tint = valid ? Color.Green * 0.5f : Color.Red * 0.5f;

            GBL.spriteBatch.Draw(
                selectedBuilding.Texture,
                new Rectangle(
                    (int)snapped.X,
                    (int)snapped.Y,
                    selectedBuilding.Size.X * grid.TileSize,
                    selectedBuilding.Size.Y * grid.TileSize
                ),
                null,
                tint,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.86f
            );
        }

        public bool IsBuildMode => buildMode;
    }
}
