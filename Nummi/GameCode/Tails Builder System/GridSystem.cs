using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Nummi;

namespace Nummi
{
    public class GridSystem
    {
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

        public Point WorldToGrid(Vector2 worldPos)
        {
            return new Point(
                (int)((worldPos.X - Origin.X) / TileSize),
                (int)((worldPos.Y - Origin.Y) / TileSize)
            );
        }

        public Vector2 GridToWorld(Point gridPos)
        {
            return new Vector2(
                Origin.X + gridPos.X * TileSize,
                Origin.Y + gridPos.Y * TileSize
            );
        }

        public bool IsInside(Point pos)
        {
            return pos.X >= 0 && pos.Y >= 0 &&
                   pos.X < Width && pos.Y < Height;
        }

        public bool IsOccupied(Point pos)
        {
            return occupied[pos.X, pos.Y];
        }

        public void SetOccupied(Point pos, bool value)
        {
            occupied[pos.X, pos.Y] = value;
        }

        public void SetBuildable(Point pos, bool value)
        {
            if (pos.X >= 0 && pos.Y >= 0 && pos.X < Width && pos.Y < Height)
                buildable[pos.X, pos.Y] = value;
        }

        public bool IsBuildable(Point pos)
        {
            if (pos.X < 0 || pos.Y < 0 || pos.X >= Width || pos.Y >= Height)
                return false;
            return buildable[pos.X, pos.Y];
        }

        // Call this when switching levels so old buildable data is cleared
        public void ResetBuildable()
        {
            buildable = new bool[Width, Height];
        }
    }
}