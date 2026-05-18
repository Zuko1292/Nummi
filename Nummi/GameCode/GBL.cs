using System;
using Code_For_Nummi;
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

        public static Game1 Game;

        public static float DeltaTime { get; private set; }

        public static KeyboardState _kb;
        public static KeyboardState _oldkb;

        public static MouseState mscurr;
        public static MouseState msold;

        public static FollowCamera _camera;

        // Mouse states
        public static bool LeftClick => mscurr.LeftButton == ButtonState.Pressed && msold.LeftButton == ButtonState.Released;
        public static bool RightClick => mscurr.RightButton == ButtonState.Pressed && msold.RightButton == ButtonState.Released;
        public static bool MiddleClick => mscurr.MiddleButton == ButtonState.Pressed && msold.MiddleButton == ButtonState.Released;
        public static Point mousePos => mscurr.Position;

        // Gets Key Press and Key hold, key press is only true for the frame the key was pressed and key hold is true for every frame the key is held down
        public static bool KeyPress(Keys key) => _kb.IsKeyDown(key) && _oldkb.IsKeyUp(key);
        public static bool KeyHold(Keys key) => _kb.IsKeyDown(key);
        // Random Number gen
        public static readonly Random RNG = new Random();

        public static void Update(GameTime gameTime, Game1 gameRoot)
        {
            Game = gameRoot;
            // This makes the camera follow the player. It calculates the center of the player's collision bounds and tells the camera to follow that point. Then it updates the camera's position and transformation matrix. This ensures that as the player moves around the game world, the camera will smoothly follow them, keeping them centered on the screen.
            if (Game._player != null)
            {
                Vector2 playerCentre = new Vector2(Game._player._collisionBounds.X + Game._player._collisionBounds.Width / 2f,
                    Game._player._collisionBounds.Y + Game._player._collisionBounds.Height / 2f);
                _camera.Follow(playerCentre);
                _camera.Update();
            }
            // This is used for timers and making movement frame rate independent. It calculates the time that has passed since the last update by taking the total elapsed game time in seconds. This allows you to use DeltaTime to make sure that things like movement speed are consistent regardless of the frame rate, ensuring smooth gameplay even if the frame rate fluctuates.
            DeltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // Mouse and KB states
            msold = mscurr;
            mscurr = Mouse.GetState();

            _oldkb = _kb;
            _kb = Keyboard.GetState();
            
        }
    }
}
