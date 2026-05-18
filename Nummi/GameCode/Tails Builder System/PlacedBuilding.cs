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
    // literally just a struct to hold the position and type of a building that has been placed in the world. This is used by the BuildingSystem to keep track of what buildings are where, and is also used by the rendering code to draw the buildings at the correct positions.
    public class PlacedBuilding
    {
        public Point Position;
        public BuildingType Type;
    }
}
