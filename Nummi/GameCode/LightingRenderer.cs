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

        // These blend states are used to create the lighting effect. The AlphaSubtract blend state is used to subtract the light from the darkness target, while the ColourOnly blend state is used to add the colour of the light to the darkness target. By using these blend states, we can create a more realistic lighting effect where the light not only reduces the darkness but also adds a warm colour to it.
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
            // The darkness target is a render target that we draw the darkness onto. We then use this render target to apply the lighting effect to the game world. By drawing the darkness onto a separate render target, we can easily manipulate it and apply the lighting effect without affecting the rest of the game's rendering.
            _darknessTarget = new RenderTarget2D(
                GBL.GD,
                GBL.GD.Viewport.Width,
                GBL.GD.Viewport.Height,
                false,
                SurfaceFormat.Color,
                DepthFormat.None,
                0,
                RenderTargetUsage.PreserveContents);
            // The light gradient is a texture that is used to create the light effect
            _lightGradient = CreatePixelatedGradient(GBL.GD, 64);
            // The pixel texture is a simple 1x1 white texture that is used for drawing the darkness and light. By using a single pixel texture, we can easily draw rectangles of any size by scaling it, which is useful for drawing the darkness and light effects.
            _pixel = new Texture2D(GBL.GD, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }

        private Texture2D CreatePixelatedGradient(GraphicsDevice gd, int size)
        {
            Texture2D tex = new Texture2D(gd, size, size);
            Color[] data = new Color[size * size];
            Vector2 centre = new Vector2(size / 2f, size / 2f);
            // This makes it so the light gradient has a pixelated effect, which fits the art style of the game. It does this by snapping the coordinates to a grid defined by PixelSize, and then calculating the alpha value based on the distance from the center of the gradient. The alpha value is then quantized to create distinct steps in the gradient, giving it a pixelated look.
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
        // This draws the lighting effect onto the darkness target first it clears the darkness aka punches a hole through it and then draws the light pixel in its place
        public void DrawLighting(Vector2 playerWorldPos, Vector2[] torchWorldPositions, Matrix cameraTransform)
        {
            GBL.GD.SetRenderTarget(_darknessTarget);
            GBL.GD.Clear(new Color(0, 0, 0, (int)(AmbientDarkness * 255)));

            GBL.spriteBatch.Begin(SpriteSortMode.Immediate, AlphaSubtract,
                SamplerState.PointClamp, null, null, null, cameraTransform);

            DrawLight(playerWorldPos, PlayerLightRadius, Color.White);
            foreach (var torchPos in torchWorldPositions)
                DrawLight(torchPos, TorchLightRadius, Color.White);

            GBL.spriteBatch.End();

            GBL.spriteBatch.Begin(SpriteSortMode.Immediate, ColourOnly,
                SamplerState.PointClamp, null, null, null, cameraTransform);

            foreach (var torchPos in torchWorldPositions)
                DrawLight(torchPos, TorchLightRadius, new Color(60, 25, 0));

            GBL.spriteBatch.End();

            GBL.GD.SetRenderTarget(null);
        }
        // This draws the light
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
        // This applies the lighting effect to the game world by drawing the darkness target over the game world. The darkness target has had the light punched through it, so when we draw it over the game world, it creates the effect of darkness with light areas where the player and torches are. By using a separate render target for the darkness, we can easily manipulate it and apply the lighting effect without affecting the rest of the game's rendering.
        public void ApplyLighting()
        {
            GBL.spriteBatch.Draw(_darknessTarget, Vector2.Zero, null,Color.White, 0f, Vector2.Zero, 1f,SpriteEffects.None, 0.15f);
        }
    }
}