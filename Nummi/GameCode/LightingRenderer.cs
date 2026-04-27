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

        // This blend subtracts the light texture's alpha from the darkness
        private static readonly BlendState SubtractAlphaBlend = new BlendState
        {
            ColorWriteChannels = ColorWriteChannels.Alpha,
            AlphaBlendFunction = BlendFunction.ReverseSubtract,
            AlphaSourceBlend = Blend.One,
            AlphaDestinationBlend = Blend.One,
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

                    data[y * size + x] = new Color(1f, 1f, 1f, alpha);
                }
            }

            tex.SetData(data);
            return tex;
        }

        public void DrawLighting(Vector2 playerWorldPos, Vector2[] torchWorldPositions, Matrix cameraTransform)
        {
            GBL.GD.SetRenderTarget(_darknessTarget);
            GBL.GD.Clear(new Color(0, 0, 0, (int)(AmbientDarkness * 255)));

            // Punch transparent holes in the alpha channel
            GBL.spriteBatch.Begin(SpriteSortMode.Immediate, SubtractAlphaBlend,
                SamplerState.PointClamp,
                null, null, null,
                cameraTransform);

            DrawLight(playerWorldPos, PlayerLightRadius, Color.White);
            foreach (var torchPos in torchWorldPositions)
                DrawLight(torchPos, TorchLightRadius, Color.White);

            GBL.spriteBatch.End();

            // Add colour tint on top additively
            GBL.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive,
                SamplerState.PointClamp,
                null, null, null,
                cameraTransform);

            // Torches get a subtle orange tint added
            foreach (var torchPos in torchWorldPositions)
                DrawLight(torchPos, TorchLightRadius, new Color(80, 40, 0)); // Dark orange, subtle

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