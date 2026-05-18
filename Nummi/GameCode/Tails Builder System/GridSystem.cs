using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nummi;

namespace Nummi
{
    public class GridSystem
    {
        // This class manages the grid for building placement. It tracks which tiles are occupied and which are buildable.
        public int TileSize { get; }
        public int Width { get; }
        public int Height { get; }

        public Vector2 Origin { get; set; } = Vector2.Zero;

        private bool[,] occupied;
        private bool[,] buildable;

        public GridSystem(int width, int height, int tileSize)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            occupied = new bool[width, height];
            buildable = new bool[width, height];
        }
        // This converts world coordinates (like mouse position) to grid coordinates (tile indices)
        public Point WorldToGrid(Vector2 worldPos)
        {
            return new Point(
                (int)((worldPos.X - Origin.X) / TileSize),
                (int)((worldPos.Y - Origin.Y) / TileSize)
            );
        }
        // This converts grid coordinates back to world coordinates, useful for drawing buildings at the correct position
        public Vector2 GridToWorld(Point gridPos)
        {
            return new Vector2(
                Origin.X + gridPos.X * TileSize,
                Origin.Y + gridPos.Y * TileSize
            );
        }

        //This checks if its within the grid bounds
        public bool IsInside(Point pos)
        {
            return pos.X >= 0 && pos.Y >= 0 &&
                   pos.X < Width && pos.Y < Height;
        }
        // This checks if the tile is occupied by a building
        public bool IsOccupied(Point pos)
        {
            return occupied[pos.X, pos.Y];
        }
        // This sets the occupied state of a tile, used when placing or removing buildings
        public void SetOccupied(Point pos, bool value)
        {
            occupied[pos.X, pos.Y] = value;
        }
        // This sets whether a tile is buildable
        public void SetBuildable(Point pos, bool value)
        {
            if (pos.X >= 0 && pos.Y >= 0 && pos.X < Width && pos.Y < Height)
                buildable[pos.X, pos.Y] = value;
        }
        // This checks if a tile is buildable, meaning it can have a building placed on it
        public bool IsBuildable(Point pos)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.X >= Width || pos.Y >= Height)
                return false;
            return buildable[pos.X, pos.Y];
        }

        // Dont really need this but it can be used to reset the buildable state of the grid if needed
        public void ResetBuildable()
        {
            buildable = new bool[Width, Height];
        }
    }
}