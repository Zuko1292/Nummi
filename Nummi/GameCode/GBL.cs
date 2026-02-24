using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Nummi
{
    static class GBL
    {
        public static ContentManager Content;
        public static GraphicsDeviceManager GDM;
        public static GraphicsDevice GD => GDM.GraphicsDevice;

        public static SpriteBatch spriteBatch;

        public static float DeltaTime { get; private set; }

        public static KeyboardState _kb;
        public static KeyboardState _oldkb;

        public static MouseState mscurr;
        public static MouseState msold;

        public static bool LeftClick => mscurr.LeftButton == ButtonState.Pressed && msold.LeftButton == ButtonState.Released;
        public static bool RightClick => mscurr.RightButton == ButtonState.Pressed && msold.RightButton == ButtonState.Released;
        public static bool MiddleClick => mscurr.MiddleButton == ButtonState.Pressed && msold.MiddleButton == ButtonState.Released;
        public static Point mousePos => mscurr.Position;

        public static bool KeyPress(Keys key) => _kb.IsKeyDown(key) && _oldkb.IsKeyUp(key);
        public static bool KeyHold(Keys key) => _kb.IsKeyDown(key);

        public static readonly Random RNG = new Random();

        public static void Update(GameTime gameTime)
        {
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            msold = mscurr;
            mscurr = Mouse.GetState();

            _oldkb = _kb;
            _kb = Keyboard.GetState();
        }
    }
}
