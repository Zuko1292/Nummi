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
    public class TilemapGroup
    {
        private List<Tilemap> _layers = new List<Tilemap>();
        public List<Tilemap> Layers => _layers;

        public void AddLayer(Tilemap map)
        {
            _layers.Add(map);
        }

        public void Draw()
        {
            float baseDepth = 0.9f;

            for (int i = 0; i < _layers.Count; i++)
            {
                float depth = baseDepth - (i * 0.01f);
                _layers[i].Draw(depth);
            }
        }

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
        public void SetRules(TilemapRules rules)
        {
            // Apply rules to all layers
            foreach (var layer in _layers)
                layer.SetRules(rules);
        }
    }
}
