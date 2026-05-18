using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Nummi
{
    // This is the class that represents a Tile
    public class TextureRegion
    {
        public Texture2D Texture;
        public Rectangle SourceRectangle;

        public int Width => SourceRectangle.Width;
        public int Height => SourceRectangle.Height;

        public TextureRegion(Texture2D texture, int x, int y, int width, int height)
        {
            Texture = texture;
            SourceRectangle = new Rectangle(x, y, width, height);
        }

    }
}
