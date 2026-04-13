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
    public class BuildingType
    {
        public string Name;
        public Texture2D Texture;
        public Point Size;

        public BuildingType(string name, Texture2D texture, Point size)
        {
            Name = name;
            Texture = texture;
            Size = size;
        }
    }
}
