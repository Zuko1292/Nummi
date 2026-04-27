using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    public class BuildingSystem
    {
        private GridSystem grid;

        public List<PlacedBuilding> placedBuildings = new();
        public Dictionary<Keys, BuildingType> hotkeys = new();
        public Dictionary<string, int> buildingLimits = new();   // Max per building name
        public Dictionary<string, int> buildingCounts = new();   // Current count per name

        private BuildingType selectedBuilding = null;
        private bool buildMode = false;
        private bool _leftWasReleased = true;

        public BuildingSystem(GridSystem grid)
        {
            this.grid = grid;
        }

        public void SetLimit(string buildingName, int max)
        {
            buildingLimits[buildingName] = max;
            if (!buildingCounts.ContainsKey(buildingName))
                buildingCounts[buildingName] = 0;
        }

        public void ResetCounts()
        {
            var keys = new List<string>(buildingCounts.Keys);
            foreach (var key in keys)
                buildingCounts[key] = 0;
        }

        public bool CanPlaceMore(string buildingName)
        {
            if (!buildingLimits.ContainsKey(buildingName)) return true; // No limit set = unlimited
            if (!buildingCounts.ContainsKey(buildingName)) return true;
            return buildingCounts[buildingName] < buildingLimits[buildingName];
        }

        public int RemainingCount(string buildingName)
        {
            if (!buildingLimits.ContainsKey(buildingName)) return -1; // -1 = unlimited
            int placed = buildingCounts.ContainsKey(buildingName) ? buildingCounts[buildingName] : 0;
            return buildingLimits[buildingName] - placed;
        }

        public void Update(Vector2 mouseWorld)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            foreach (var pair in hotkeys)
            {
                if (keyboard.IsKeyDown(pair.Key))
                {
                    selectedBuilding = pair.Value;
                    buildMode = true;
                }
            }

            if (GBL.KeyPress(Keys.Escape))
            {
                buildMode = false;
                selectedBuilding = null;
            }

            if (!buildMode || selectedBuilding == null)
                return;

            Point gridPos = grid.WorldToGrid(mouseWorld);

            if (!grid.IsInside(gridPos))
                return;

            if (mouse.LeftButton == ButtonState.Released)
                _leftWasReleased = true;

            if (mouse.LeftButton == ButtonState.Pressed && _leftWasReleased)
            {
                _leftWasReleased = false;

                if (CanPlace(gridPos, selectedBuilding.Size))
                {
                    for (int x = 0; x < selectedBuilding.Size.X; x++)
                    {
                        for (int y = 0; y < selectedBuilding.Size.Y; y++)
                        {
                            grid.SetOccupied(new Point(gridPos.X + x, gridPos.Y + y), true);
                        }
                    }

                    // Track by name instead of on BuildingType itself
                    if (!buildingCounts.ContainsKey(selectedBuilding.Name))
                        buildingCounts[selectedBuilding.Name] = 0;
                    buildingCounts[selectedBuilding.Name]++;

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
            if (!CanPlaceMore(selectedBuilding.Name)) return false;

            for (int x = 0; x < size.X; x++)
            {
                for (int y = 0; y < size.Y; y++)
                {
                    Point check = new Point(pos.X + x, pos.Y + y);
                    if (!grid.IsInside(check) || grid.IsOccupied(check) || !grid.IsBuildable(check))
                        return false;
                }
            }
            return true;
        }

        public void Draw(Vector2 mouseWorld)
        {
            foreach (var b in placedBuildings)
            {
                Vector2 worldPos = grid.GridToWorld(b.Position);
                GBL.spriteBatch.Draw(
                    b.Type.Texture,
                    new Rectangle(
                        (int)worldPos.X,
                        (int)worldPos.Y,
                        b.Type.Size.X * grid.TileSize,
                        b.Type.Size.Y * grid.TileSize),
                    null,
                    Color.White,
                    0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    0.87f
                );
            }

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
                    selectedBuilding.Size.Y * grid.TileSize),
                null,
                tint,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0.86f
            );
        }

        // Call this from your shop to select a building
        public void SelectBuilding(BuildingType building)
        {
            selectedBuilding = building;
            buildMode = true;
        }

        public bool IsBuildMode => buildMode;
    }
}
