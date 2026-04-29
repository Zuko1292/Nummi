using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Nummi
{
    public class LightingRenderer
    {
        private RenderTarget2D _darknessTarget;
        private Texture2D _lightGradient;
        private Texture2D _pixel;

        public float AmbientDarkness = 0.92f;
        public int PlayerLightRadius = 80;
        public int TorchLightRadius = 50;
        private const int PixelSize = 8;

        private static readonly BlendState AlphaSubtract = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Alpha,
            AlphaBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One
        };

        private static readonly BlendState ColourOnly = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Red | ColorWriteChannels.Green | ColorWriteChannels.Blue,
            ColorBlendFunction = BlendFunction.Add,
            ColorSourceBlend = Blend.One,
            ColorDestinationBlend = Blend.One
        };

        public LightingRenderer()
        {
            _darknessTarget = new RenderTarget2D(
                GBL.GD,
                GBL.GD.Viewport.Width,
                GBL.GD.Viewport.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);

            _lightGradient = CreatePixelatedGradient(GBL.GD, 64);

            _pixel = new Texture2D(GBL.GD, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        private Texture2D CreatePixelatedGradient(GraphicsDevice gd, int size)
        {
            Texture2D tex = new Texture2D(gd, size, size);
            Color[] data = new Color[size * size];
            Vector2 centre = new Vector2(size / 2f, size / 2f);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    int snappedX = (x / PixelSize) * PixelSize;
                    int snappedY = (y / PixelSize) * PixelSize;

                    float dist = Vector2.Distance(new Vector2(snappedX, snappedY), centre);
                    float alpha = 1f - MathHelper.Clamp(dist / (size / 2f), 0f, 1f);
                    alpha = (float)Math.Floor(alpha * 5f) / 5f;

                    data[y * size + x] = new Color(alpha, alpha, alpha, alpha);
                }
            }

            tex.SetData(data);
            return tex;
        }

        public void DrawLighting(Vector2 playerWorldPos, Vector2[] torchWorldPositions, Matrix cameraTransform)
        {
            GBL.GD.SetRenderTarget(_darknessTarget);
            GBL.GD.Clear(new Color(0, 0, 0, (int)(AmbientDarkness * 255)));

            // Pass 1 - punch transparent holes in alpha channel only
            GBL.spriteBatch.Begin(SpriteSortMode.Immediate, AlphaSubtract,
                SamplerState.PointClamp, null, null, null, cameraTransform);

            DrawLight(playerWorldPos, PlayerLightRadius, Color.White);
            foreach (var torchPos in torchWorldPositions)
                DrawLight(torchPos, TorchLightRadius, Color.White);

            GBL.spriteBatch.End();

            // Pass 2 - add colour tint to RGB only, doesnt touch alpha
            GBL.spriteBatch.Begin(SpriteSortMode.Immediate, ColourOnly,
                SamplerState.PointClamp, null, null, null, cameraTransform);

            foreach (var torchPos in torchWorldPositions)
                DrawLight(torchPos, TorchLightRadius, new Color(60, 25, 0));

            GBL.spriteBatch.End();

            GBL.GD.SetRenderTarget(null);
        }

        private void DrawLight(Vector2 worldPos, int radius, Color colour)
        {
            GBL.spriteBatch.Draw(
                _lightGradient,
                new Rectangle(
                    (int)(worldPos.X - radius),
                    (int)(worldPos.Y - radius),
                    radius * 2,
                    radius * 2),
                null,
                colour,
                0f,
                Vector2.Zero,
                SpriteEffects.None,
                0f
            );
        }

        public void ApplyLighting()
        {
            GBL.spriteBatch.Draw(_darknessTarget, Vector2.Zero, Color.White);
        }
    }
}