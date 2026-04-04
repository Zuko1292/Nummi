using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class Camera2D
{
    private Viewport _viewport;

    public Matrix Transform { get; private set; }

    public float Zoom { get; private set; } = 1f;
    public Vector2 Position { get; private set; }

    private int _worldWidth;
    private int _worldHeight;

    private int _previousScroll;

    public Camera2D(Viewport viewport, int mapTilesX, int mapTilesY, int tileSize)
    {
        _viewport = viewport;

        _worldWidth = mapTilesX * tileSize;
        _worldHeight = mapTilesY * tileSize;

        CalculateZoomToFit();
        CenterCamera();
        UpdateTransform();

        _previousScroll = Mouse.GetState().ScrollWheelValue;
    }

    private void CalculateZoomToFit()
    {
        float zoomX = (float)_viewport.Width / _worldWidth;
        float zoomY = (float)_viewport.Height / _worldHeight;

        Zoom = MathHelper.Min(zoomX, zoomY);
    }

    private void CenterCamera()
    {
        Position = new Vector2(_worldWidth / 2f, _worldHeight / 2f);
    }

    public void Update(GameTime gameTime)
    {
        HandleZoom();
        UpdateTransform();;
    }

    private void HandleZoom()
    {
        MouseState mouse = Mouse.GetState();

        int scrollDelta = mouse.ScrollWheelValue - _previousScroll;

        if (scrollDelta != 0)
        {
            float zoomSpeed = 0.001f;

            Zoom += scrollDelta * zoomSpeed;

            // Clamp zoom so it doesn't go crazy
            Zoom = MathHelper.Clamp(Zoom, 0.234375f, 1f);
        }

        _previousScroll = mouse.ScrollWheelValue;
    }

    public void UpdateTransform()
    {
        Transform =
            Matrix.CreateTranslation(new Vector3(-Position, 0)) *
            Matrix.CreateScale(Zoom, Zoom, 1f) *
            Matrix.CreateTranslation(new Vector3(_viewport.Width / 2f, _viewport.Height / 2f, 0));
    }

    public void OnResize(Viewport newViewport)
    {
        _viewport = newViewport;
        CalculateZoomToFit();
        CenterCamera();
        UpdateTransform();
    }
}
