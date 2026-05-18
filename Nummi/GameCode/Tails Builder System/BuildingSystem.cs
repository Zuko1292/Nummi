using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    public class BuildingSystem
    {
        private GridSystem grid;

        // This class manages the building placement system. It keeps track of placed buildings, hotkeys for selecting buildings, and limits on how many of each building can be placed.
        public List<PlacedBuilding> placedBuildings = new();
        public Dictionary<Keys, BuildingType> hotkeys = new(); // Key to building type mapping for quick selection (Was only being used during development, but could be useful for debug mode or future features)
        public Dictionary<string, int> buildingLimits = new();   // Max per building
        public Dictionary<string, int> buildingCounts = new();   // Current count

        public BuildingType selectedBuilding = null;
        public bool buildMode = false;
        private bool _leftWasReleased = true;

        public int _housesPlaced= 0, _barracksPlaced = 0, _farmsPlaced = 0, _nuclearReactorsPlaced = 0, _blacksmithsPlaced;

        public BuildingSystem(GridSystem grid)
        {
            this.grid = grid;
        }
        // Call this to set a limit on how many of a certain building can be placed. If not set, there is no limit.
        public void SetLimit(string buildingName, int max)
        {
            buildingLimits[buildingName] = max;
            if (!buildingCounts.ContainsKey(buildingName))
                buildingCounts[buildingName] = 0;
        }
        // Call this to reset all building counts back to 0, useful for starting a new game or resetting the state.
        public void ResetCounts()
        {
            var keys = new List<string>(buildingCounts.Keys);
            foreach (var key in keys)
                buildingCounts[key] = 0;
        }
        // Checks if you can place more of a building
        public bool CanPlaceMore(string buildingName)
        {
            if (!buildingLimits.ContainsKey(buildingName)) return true;
            if (!buildingCounts.ContainsKey(buildingName)) return true;
            return buildingCounts[buildingName] < buildingLimits[buildingName];
        }
        // The remaining count is how many more of a building you can place before hitting the limit. Returns -1 if there is no limit.(This is used by the UI to show how many you have left)
        public int RemainingCount(string buildingName)
        {
            if (!buildingLimits.ContainsKey(buildingName)) return -1;
            int placed = buildingCounts.ContainsKey(buildingName) ? buildingCounts[buildingName] : 0;
            return buildingLimits[buildingName] - placed;
        }

        public void Update(Vector2 mouseWorld, Shop shop = null)
        {
            KeyboardState keyboard = Keyboard.GetState();
            MouseState mouse = Mouse.GetState();

            _housesPlaced = GetPlacedCount("House");
            _barracksPlaced = GetPlacedCount("Barracks");
            _farmsPlaced = GetPlacedCount("Farm");
            _nuclearReactorsPlaced = GetPlacedCount("Nuclear Reactor");
            _blacksmithsPlaced = GetPlacedCount("Blacksmith");
            // For hotkey placement aka testing as hotkey placement will not be in the game
            foreach (var pair in hotkeys)
            {
                if (keyboard.IsKeyDown(pair.Key))
                {
                    selectedBuilding = pair.Value;
                    buildMode = true;
                }
            }
            // for exiting building mode and deselecting building
            if (GBL.KeyPress(Keys.Escape))
            {
                buildMode = false;
                selectedBuilding = null;
            }
            // If not in build mode or no building selected, skip the rest of the update
            if (!buildMode || selectedBuilding == null)
                return;
            // Convert mouse world position to grid coordinates
            Point gridPos = grid.WorldToGrid(mouseWorld);
            // Check if the grid position is valid before trying to place a building
            if (!grid.IsInside(gridPos))
                return;
            // Handle left mouse button input for placing buildings. We check for release first to prevent multiple placements from a single click.
            if (mouse.LeftButton == ButtonState.Released)
                _leftWasReleased = true;
            // If left button is pressed and it was previously released, attempt to place the building
            if (mouse.LeftButton == ButtonState.Pressed && _leftWasReleased)
            {
                _leftWasReleased = false;

                if (CanPlace(gridPos, selectedBuilding.Size))
                {
                    // Check shop and deduct coins before placing
                    if (shop != null && !shop.TryPayForPlacement(selectedBuilding.Name))
                        return; // Cant afford, dont place

                    for (int x = 0; x < selectedBuilding.Size.X; x++)
                    {
                        for (int y = 0; y < selectedBuilding.Size.Y; y++)
                        {
                            grid.SetOccupied(new Point(gridPos.X + x, gridPos.Y + y), true);
                        }
                    }

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
        // Checks if a building can be placed at the given grid position, considering its size and whether the tiles are buildable and unoccupied. Also checks if we can place more of this building type based on limits.
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
        // Draws the building 
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
            // This draw makes it so the building you are trying to place is drawn on the cursor, tinted green if you can place it there and red if you cant. It also snaps to the grid.
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

        // Call this from shop to select a building
        public void SelectBuilding(BuildingType building)
        {
            selectedBuilding = building;
            buildMode = true;
        }
        // Get the placed amount of a building type, used for UI and tracking
        public int GetPlacedCount(string buildingName)
        {
            if (buildingCounts.ContainsKey(buildingName))
                return buildingCounts[buildingName];
            return 0;
        }
        // This is used by the UI to check if we are in build mode, which can affect how the UI looks or what options are available.
        public bool IsBuildMode => buildMode;
    }
}
