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
    // This handles the reading of the xml file and the drawing of the tilemap. It also has some helper functions for checking if a tile is solid or an exit tile, and for getting the tile ID at a specific world position. The tilemap is made up of a tileset, which is a collection of texture regions that represent the individual tiles, and an array of tile IDs that specify which tile from the tileset should be drawn at each position in the map. The tilemap also has rules that specify which tile IDs are solid or exit tiles, which are used for collision detection and level progression.
    public class Tilemap
    {
        private readonly TileSet _tileSet;
        private readonly int[] _tiles;
        private TilemapRules _rules;

        public TilemapRules Rules => _rules;

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
            _rules = new TilemapRules();
        }
        //For Setting the rules of the tilemap, such as which tile IDs are solid or exit tiles.
        public void SetRules(TilemapRules rules)
        {
            _rules = rules;
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
        public int GetTileId(int column, int row)
        {
            int index = row * Columns + column;
            if (index < 0 || index >= _tiles.Length) return -1;
            return _tiles[index];
        }
        // Draws this tilemap using the GBL.SpriteBatch and creates a destination rectangle.
        public void Draw(float depth)
        {
            for (int i = 0; i < Count; i++)
            {
                int tilesetIndex = _tiles[i];

                if (tilesetIndex < 0)
                    continue;

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
                GBL.spriteBatch.Draw(tile.Texture, destinationRectangle, tile.SourceRectangle, Color.White, 0f, Vector2.Zero, SpriteEffects.None, depth);
            }
        }

        // Sets the TileID of the Tiles that should be Solid.
        public bool IsSolidTileID(int tileID) => _rules.IsSolid(tileID);
        public bool IsExitTileID(int tileID) => _rules.IsExit(tileID);
        public bool IsChestTileID(int tileID) => _rules.IsChest(tileID);
        public bool IsTrapDoorTileID(int tileID) => _rules.IsTrapDoor(tileID);
        public bool IsKeyTileID(int tileID) => _rules.IsKey(tileID);
        public bool IsLockedDoorTileID(int tileID) => _rules.IsLockedDoor(tileID);

        public bool IsLockedDoorAtWorld(int worldX, int worldY)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);
            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows) return false;
            return IsLockedDoorTileID(_tiles[tileY * Columns + tileX]);
        }

        // Clears every locked door tile on this layer (used when the player
        // collects enough keys to unlock the doors). Returns how many were
        // unlocked.
        public int UnlockAllDoors()
        {
            int count = 0;
            for (int y = 0; y < Rows; y++)
            {
                for (int x = 0; x < Columns; x++)
                {
                    int idx = y * Columns + x;
                    if (IsLockedDoorTileID(_tiles[idx]))
                    {
                        SetTile(x, y, 3);
                        count++;
                    }
                }
            }
            return count;
        }

        // Mirrors IsChestAtWorld / TryGetChestTileAtWorld for keys.
        public bool IsKeyAtWorld(int worldX, int worldY)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);
            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows) return false;
            return IsKeyTileID(_tiles[tileY * Columns + tileX]);
        }

        public bool TryGetKeyTileAtWorld(int worldX, int worldY, out Point tilePos)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);
            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows)
            {
                tilePos = Point.Zero;
                return false;
            }
            int tileID = _tiles[tileY * Columns + tileX];
            if (IsKeyTileID(tileID)) { tilePos = new Point(tileX, tileY); return true; }
            tilePos = Point.Zero;
            return false;
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
        // Checks if the tile at the world position is a chest.
        public bool IsChestAtWorld(int worldX, int worldY)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);

            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows)
            {
                return false;
            }

            int index = tileY * Columns + tileX;
            int tileID = _tiles[index];

            return IsChestTileID(tileID);
        }
        // Checks if the tile at the world position is a trap door.
        public bool IsTrapDoorAtWorld(int worldX, int worldY)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);

            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows)
            {
                return false;
            }

            int index = tileY * Columns + tileX;
            int tileID = _tiles[index];

            return IsTrapDoorTileID(tileID);
        }
        // This tries to get the tile position of a chest tile at the given world coordinates. It returns true if a chest tile is found, and false otherwise. If a chest tile is found, the tile position is output through the out parameter.
        public bool TryGetChestTileAtWorld(int worldX, int worldY, out Point tilePos)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);

            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows)
            {
                tilePos = Point.Zero;
                return false;
            }

            int index = tileY * Columns + tileX;
            int tileID = _tiles[index];

            if (IsChestTileID(tileID))
            {
                tilePos = new Point(tileX, tileY);
                return true;
            }

            tilePos = Point.Zero;
            return false;
        }
        // Works the same as TryGetChestTileAtWorld but for trap doors instead of chests. (brief note: could've done one function that takes a Func<int, bool> as a parameter to check for different tile types but I wanted to keep it simple and straightforward with separate functions for each type.)
        public bool TryGetTrapDoorTileAtWorld(int worldX, int worldY, out Point tilePos)
        {
            int tileX = (int)(worldX / TileWidth);
            int tileY = (int)(worldY / TileHeight);
            if (tileX < 0 || tileY < 0 || tileX >= Columns || tileY >= Rows)
            {
                tilePos = Point.Zero;
                return false;
            }
            int index = tileY * Columns + tileX;
            int tileID = _tiles[index];
            if (IsTrapDoorTileID(tileID))
            {
                tilePos = new Point(tileX, tileY);
                return true;
            }
            tilePos = Point.Zero;
            return false;
        }

        //Creates a new tilemap based on a tilemap xml configuration file.
        // The XML file should have a structure that includes a Tileset element with attributes for tile width and height, and a value that specifies the content path to the tileset texture. It should also have a Layers element that contains one or more Layer elements, each of which has a Tiles element that contains the tile IDs for that layer in a grid format. The function reads the XML file, creates the tileset and tilemap based on the specified configuration, and returns the resulting TilemapGroup.
        public static TilemapGroup FromFile(string filename)
        {
            string filePath = Path.Combine(GBL.Content.RootDirectory, filename);

            using (Stream stream = TitleContainer.OpenStream(filePath))
            using (XmlReader reader = XmlReader.Create(stream))
            {
                XDocument doc = XDocument.Load(reader);
                XElement root = doc.Root;

                XElement tilesetElement = root.Element("Tileset");

                int tileWidth = int.Parse(tilesetElement.Attribute("tileWidth").Value);
                int tileHeight = int.Parse(tilesetElement.Attribute("tileHeight").Value);
                string contentPath = tilesetElement.Value;

                Texture2D texture = GBL.Content.Load<Texture2D>(contentPath);
                TileSet tileset = new TileSet(texture, tileWidth, tileHeight);

                TilemapGroup group = new TilemapGroup();

                var layerElements = root.Element("Layers").Elements("Layer");

                foreach (var layerElement in layerElements)
                {
                    XElement tilesElement = layerElement.Element("Tiles");

                    string[] rows = tilesElement.Value.Trim()
                        .Split('\n', StringSplitOptions.RemoveEmptyEntries);

                    int columnCount = rows[0]
                        .Split(" ", StringSplitOptions.RemoveEmptyEntries).Length;

                    Tilemap tilemap = new Tilemap(tileset, columnCount, rows.Length);

                    for (int row = 0; row < rows.Length; row++)
                    {
                        string[] columns = rows[row]
                            .Trim()
                            .Split(" ", StringSplitOptions.RemoveEmptyEntries);

                        for (int column = 0; column < columnCount; column++)
                        {
                            int tileIndex = int.Parse(columns[column]);
                            tilemap.SetTile(column, row, tileIndex);
                        }
                    }

                    group.AddLayer(tilemap);
                }

                return group;
            }
        }

    }
}
