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

    }
}
