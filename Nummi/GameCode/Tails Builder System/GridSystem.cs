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
    public class GridSystem
    {
        public int TileSize { get; }
        public int Width { get; }
        public int Height { get; }

        public Vector2 Origin { get; set; } = Vector2.Zero;

        private bool[,] occupied;

        public GridSystem(int width, int height, int tileSize)
        {
            Width = width;
            Height = height;
            TileSize = tileSize;
            occupied = new bool[width, height];
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
    }
}
