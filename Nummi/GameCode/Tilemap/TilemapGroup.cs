using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    // This class handles the layers I did layers so you dont have to draw the background tile for an object on top like a chair so you can place it on any tile
    public class TilemapGroup
    {
        private List<Tilemap> _layers = new List<Tilemap>();
        public List<Tilemap> Layers => _layers;
        // This adds a layer to the tilemap group, the first layer added will be drawn first (at the back) and the last layer added will be drawn last (at the front)
        public void AddLayer(Tilemap map)
        {
            _layers.Add(map);
        }
        // This handles the layer depth so that the first layer added is drawn at depth 0.9, the second layer is drawn at depth 0.89, and so on. This ensures that layers are drawn in the correct order without needing to manually set the depth for each layer.
        public void Draw()
        {
            float baseDepth = 0.9f;

            for (int i = 0; i < _layers.Count; i++)
            {
                float depth = baseDepth - (i * 0.01f);
                _layers[i].Draw(depth);
            }
        }
        // These handle the types of tiles they are on both layers
        public bool IsSolidAtWorld(int x, int y)
        {
            foreach (var layer in _layers)
            {
                if (layer.IsSolidAtWorld(x, y))
                    return true;
            }
            return false;
        }

        public bool IsExitAtWorld(int x, int y)
        {
            foreach (var layer in _layers)
            {
                if (layer.IsExitAtWorld(x, y))
                    return true;
            }
            return false;
        }

        public bool IsChestAtWorld(int x, int y)
        {
            foreach (var layer in _layers)
            {
                if (layer.IsChestAtWorld(x, y))
                    return true;
            }
            return false;
        }
        public bool IsTrapDoorAtWorld(int x, int y)
        {
            foreach (var layer in _layers)
            {
                if (layer.IsTrapDoorAtWorld(x, y))
                    return true;
            }
            return false;
        }

        public bool TryGetChestTileAtWorld(int x, int y, out Point tile)
        {
            foreach (var layer in _layers)
            {
                if (layer.TryGetChestTileAtWorld(x, y, out tile))
                    return true;
            }
            tile = Point.Zero;
            return false;
        }

        public bool TryGetTrapDoorTileAtWorld(int x, int y, out Point tile)
        {
            foreach (var layer in _layers)
            {
                if (layer.TryGetTrapDoorTileAtWorld(x, y, out tile))
                    return true;
            }
            tile = Point.Zero;
            return false;
        }
        // This sets the rules for all layers in the tilemap group, so you can set the rules for all layers at once instead of having to set the rules for each layer individually.
        public void SetRules(TilemapRules rules)
        {
            // Apply rules to all layers
            foreach (var layer in _layers)
                layer.SetRules(rules);
        }
    }
}
