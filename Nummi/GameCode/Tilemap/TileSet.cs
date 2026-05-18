using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Nummi
{
    // This class handles the tileset and gives each 32 x 32 part of an image an xml number that can be used to reference it. This is used by the TileMap class to draw the correct tile at each position in the map. The tileset is created by taking a texture and dividing it into a grid of tiles, where each tile is a TextureRegion that represents a portion of the original texture. The TileSet class also provides methods for getting the TextureRegion for a specific tile index or for a specific column and row in the grid.
    public class TileSet
    {
        private readonly TextureRegion[] _tiles;

        public int TileWidth;
        public int TileHeight;
        public int Columns;
        public int Rows;
        public int Count;

        // Creates a new tileset based on the given texture region with the specified
        // tile width and height.
        public TileSet(Texture2D texture, int tileWidth, int tileHeight)
        {
            TileWidth = tileWidth;
            TileHeight = tileHeight;
            Columns = texture.Width / tileWidth;
            Rows = texture.Height / tileHeight;
            Count = Columns * Rows;

            _tiles = new TextureRegion[Count];

            // Creates the texture regions that make up each infividual tile.
            for (int i = 0; i < Count; i++)
            {
                int x = i % Columns * tileWidth;
                int y = i / Columns * tileHeight;
                _tiles[i] = new TextureRegion(texture, x, y, tileWidth, tileHeight);
            }
        }
        // Gets the texture region for the tile from this tileset at the given index
        public TextureRegion GetTile(int index) => _tiles[index];

        // Gets the texture region for the tile from this tileset at the given location.
        public TextureRegion GetTile(int column, int row)
        {
            int index = row * Columns + column;
            return GetTile(index);
        }
    }
}
