using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Nummi
{
    public class Tilemap
    {
        private readonly TileSet _tileSet;
        private readonly int[] _tiles;

        public int Rows;
        public int Columns;
        public int Count;
        public Vector2 Scale = Vector2.One;

        public float TileWidth => _tileSet.TileWidth;
        public float TileHeight => _tileSet.TileHeight;


        public Tilemap(TileSet tileSet, int columns, int rows)
        {
            _tileSet = tileSet;
            Rows = rows;
            Columns = columns;
            Count = Columns * Rows;
            _tiles = new int[Count];

        }
        // Sets the tile at the given index in this tilemap to use the tile from
        // the tileset at the specified tileset id.
        public void SetTile(int index, int tilesetID)
        {
            _tiles[index] = tilesetID;
        }
        // Sets the tile at the given column and row in this tilemap to use the tile
        /// from the tileset at the specified tileset id.
        public void SetTile(int column, int row, int tilesetID)
        {
            int index = row * Columns + column;
            SetTile(index, tilesetID);
        }
        // Gets the texture region of the tile from this tilemap at the specified index.
        public TextureRegion GetTile(int index)
        {
            return _tileSet.GetTile(_tiles[index]);
        }
        //Gets the texture region of the tile from this tilemap at the specified
        // column and row.
        public TextureRegion GetTile(int column, int row)
        {
            int index = row * Columns + column;
            return GetTile(index);
        }
        // Draws this tilemap using the GBL.SB and creates a destination rectangle.
        public void Draw()
        {
            for (int i = 0; i < Count; i++)
            {
                int tilesetIndex = _tiles[i];
                TextureRegion tile = _tileSet.GetTile(tilesetIndex);

                int x = i % Columns;
                int y = i / Columns;

                float worldX = x * TileWidth;
                float worldY = y * TileHeight;

                Rectangle destinationRectangle = new Rectangle(
                    (int)worldX,
                    (int)worldY,
                    (int)TileWidth,
                    (int)TileHeight
                    );

                Vector2 position = new Vector2(x * TileWidth, y * TileHeight);
                GBL.spriteBatch.Draw(tile.Texture, destinationRectangle, tile.SourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0.7f);
            }
        }

        // Sets the TileID of the Tiles that should be Solid.
        public bool IsSolidTileID(int tileID)
        {
            return tileID == 3 || tileID == 4 || tileID == 5;
        }

        // Sets the TileID of the Tiles that should be the Exit.
        public bool IsExitTileID(int tileID)
        {
            return tileID == 0;
        }

        // Gets the ID of the Tiles in the world.
        public int GetTileIDAtWorld(int worldX, int worldY)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);

            if(tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows)
                return 0; // Return 0 for out of bounds

            int index = tileY * Columns + tileX;

            return _tiles[index];
        }

        //This get the world position and checks if that tile at that position is an Exit tile.
        public bool IsExitAtWorld(int worldX, int worldY)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);

            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows)
            {
                return false;
            }

            int index = tileY * Columns + tileX;
            int tileID = _tiles[index];

            return IsExitTileID(tileID);
        }

        // Checks if the tile at the world position is solid.
        public bool IsSolidAtWorld(int worldX, int worldY)
        {
            return IsSolidTileID(GetTileIDAtWorld(worldX, worldY));
        }

        //Creates a new tilemap based on a tilemap xml configuration file.
        public static Tilemap FromFile(string filename)
        {
            string filePath = Path.Combine(GBL.Content.RootDirectory, filename);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            {
                using (XmlReader reader = XmlReader.Create(stream))
                {
                    XDocument doc = XDocument.Load(reader);
                    XElement root = doc.Root;

                    XElement tilesetElement = root.Element("Tileset");

                    int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                    int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                    string contentPath = tilesetElement.Value;

                    // Load the texture 2d at the content path.
                    Texture2D texture = GBL.Content.Load<Texture2D>(contentPath);

                    // Create the texture region from the texture 
                    //TextureRegion textureRegion = new TextureRegion(texture, x, y, width, height);

                    // Create the tileset using the texture region.
                    TileSet tileset = new TileSet(texture, tileWidth, tileHeight);

                    // The element has lines of strings, each representing a tilemap row.
                    // Each line is a space-sperated string with elements as columns,
                    // where the column value is the tile id from the tileset.
                    XElement tilesElement = root.Element("Tiles");

                    // Split the value of the tiles data into rows by splitting on
                    // the new line character.
                    string[] rows = tilesElement.Value.Trim().Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    // Split the Value of the first row to determin the total number of columns.
                    int columnCount = rows[0].Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                    // Create the tilemap.
                    Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                    // Process each row.
                    for (int row = 0; row < rows.Length; row++)
                    {
                        // Split the row into individual columns.
                        string[] columns = rows[row].Trim().Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        // Process each column of the current row.
                        for (int column = 0; column < columnCount; column++)
                        {
                            {
                                // Get the tileset index for this location.
                                int tilesetIndex = int.Parse(columns[column]);

                                // Add that region to the tilemap at the row and column location.
                                tilemap.SetTile(column, row, tilesetIndex);
                            }
                        }
                    }
                    return tilemap;
                }
            }

        }
    }
}
