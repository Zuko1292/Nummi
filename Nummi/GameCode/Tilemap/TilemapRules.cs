using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    public class TilemapRules
    {
        private HashSet<int> _solidTiles = new HashSet<int>();
        private HashSet<int> _exitTiles = new HashSet<int>();
        private HashSet<int> _chestTiles = new HashSet<int>();
        private HashSet<int> _trapDoorTiles = new HashSet<int>();

        public TilemapRules AddSolid(params int[] ids)
        {
            foreach (var id in ids) _solidTiles.Add(id);
            return this; // Return this so you can chain calls
        }

        public TilemapRules AddExit(params int[] ids)
        {
            foreach (var id in ids) _exitTiles.Add(id);
            return this;
        }

        public TilemapRules AddChest(params int[] ids)
        {
            foreach (var id in ids) _chestTiles.Add(id);
            return this;
        }

        public TilemapRules AddTrapDoor(params int[] ids)
        {
            foreach (var id in ids) _trapDoorTiles.Add(id);
            return this;
        }

        public bool IsSolid(int id) => _solidTiles.Contains(id);
        public bool IsExit(int id) => _exitTiles.Contains(id);
        public bool IsChest(int id) => _chestTiles.Contains(id);
        public bool IsTrapDoor(int id) => _trapDoorTiles.Contains(id);
    }
}
