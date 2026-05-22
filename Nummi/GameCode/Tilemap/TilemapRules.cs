using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    // This class is useful so i can have multiple tilemaps with different rules on each tiles type without needing to hardcode the rules into the tilemap class itself. For example, I can have one tilemap that has solid walls and another tilemap that has chests, and I can use the same tilemap class to handle both of them by just giving them different rules. This also makes it easy to change the rules for a tilemap without needing to change the tilemap class itself, which is useful for things like different levels or different types of maps.
    public class TilemapRules
    {
        private HashSet<int> _solidTiles = new HashSet<int>();
        private HashSet<int> _exitTiles = new HashSet<int>();
        private HashSet<int> _chestTiles = new HashSet<int>();
        private HashSet<int> _trapDoorTiles = new HashSet<int>();
        private HashSet<int> _keyTiles = new HashSet<int>();
        private HashSet<int> _lockedDoorTiles = new HashSet<int>();

        // Which collision layers are affected by this tilemap
        public CollisionLayer AffectsLayers { get; private set; } = CollisionLayer.All;

        public TilemapRules SetAffectsLayers(CollisionLayer layers)
        {
            AffectsLayers = layers;
            return this;
        }

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

        public TilemapRules AddKey(params int[] ids)
        {
            foreach (var id in ids) _keyTiles.Add(id);
            return this;
        }

        // Locked door tiles are solid until the player has picked up enough
        // keys; Game1 clears them from the map when _keys.Count >= 2. Add
        // the same IDs to AddSolid(...) so they block movement until unlocked.
        public TilemapRules AddLockedDoor(params int[] ids)
        {
            foreach (var id in ids) _lockedDoorTiles.Add(id);
            return this;
        }

        public bool IsSolid(int id) => _solidTiles.Contains(id);
        public bool IsExit(int id) => _exitTiles.Contains(id);
        public bool IsChest(int id) => _chestTiles.Contains(id);
        public bool IsTrapDoor(int id) => _trapDoorTiles.Contains(id);
        public bool IsKey(int id) => _keyTiles.Contains(id);
        public bool IsLockedDoor(int id) => _lockedDoorTiles.Contains(id);
    }
}
