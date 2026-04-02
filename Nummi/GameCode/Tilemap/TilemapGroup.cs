using System;
using System.Collections.Generic;
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
    }
}
