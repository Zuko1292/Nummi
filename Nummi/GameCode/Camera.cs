using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Code_For_Nummi
{
    public class FollowCamera
    {
        public Matrix _transform;
        public Vector2 _position;
        private readonly Viewport _viewport;

        public FollowCamera(Viewport viewport)
        {
            _viewport = viewport;
            _position = Vector2.Zero;
            _transform = Matrix.Identity;
        }
        // Makes it so the camera follows a target position, which is usually the player. It does this by setting the camera's position to be the target position minus half of the viewport size, so that the target is centered on the screen. The Lerp function is used to smoothly transition the camera's position towards the target position, creating a smooth following effect.
        public void Follow(Vector2 targetPosition)
        {
            _position = Vector2.Lerp(_position, targetPosition -
                new Vector2(_viewport.Width / 2f, _viewport.Height
                / 2f), 1f);
        }

        public void Update()
        {
            _transform = Matrix.CreateTranslation(
                new Vector3(-_position, 0f));
        }
        // This converts screen coordinates (like mouse position) to world coordinates, which is useful for things like clicking on the game world. It does this by adding the camera's position to the screen coordinates, effectively translating them into the world space.
        public Point ScreenToWorld(Point screen)
        {
            return screen + _position.ToPoint();
        }

    }
}
