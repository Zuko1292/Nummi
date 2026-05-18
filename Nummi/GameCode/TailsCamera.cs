using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Nummi;

public class Camera2D
{
    private Viewport _viewport;

    public Matrix Transform { get; private set; }

    public float Zoom { get; private set; } = 1f;
    public Vector2 Position { get; private set; }

    private int _worldWidth;
    private int _worldHeight;

    private float _minZoom;

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
    // this method calculates the min zoom level needed to fit the entire world within the viewport
    private void CalculateZoomToFit()
    {
        float zoomX = (float)_viewport.Width / _worldWidth;
        float zoomY = (float)_viewport.Height / _worldHeight;

        _minZoom = MathHelper.Min(zoomX, zoomY);
        Zoom = _minZoom;
    }
    // this centers the camera 
    private void CenterCamera()
    {
        Position = new Vector2(_worldWidth / 2f, _worldHeight / 2f);
    }

    public void Update(GameTime gameTime)
    {
        HandleZoom();

        ClampToWorld();

        UpdateTransform();
    }
    // This handles the zoom
    private void HandleZoom()
    {
        MouseState mouse = GBL.mscurr;
        int scrollDelta = mouse.ScrollWheelValue - _previousScroll;

        if (scrollDelta != 0)
        {
            Vector2 mouseScreen = new Vector2(mouse.X, mouse.Y);

            Vector2 worldBefore = Vector2.Transform(mouseScreen, Matrix.Invert(Transform));

            float zoomFactor = 1 + scrollDelta * 0.001f;
            Zoom *= zoomFactor;

            Zoom = MathHelper.Clamp(Zoom, _minZoom, 3f);

            UpdateTransform();

            Vector2 worldAfter = Vector2.Transform(mouseScreen, Matrix.Invert(Transform));

            Position += (worldBefore - worldAfter);

            ClampToWorld();
        }

        _previousScroll = mouse.ScrollWheelValue;
    }
    // This method updates the camera's transformation matrix based on its position and zoom level
    public void UpdateTransform()
    {
        Transform =
            Matrix.CreateTranslation(new Vector3(-Position, 0)) *
            Matrix.CreateScale(Zoom, Zoom, 1f) *
            Matrix.CreateTranslation(new Vector3(_viewport.Width / 2f, _viewport.Height / 2f, 0));
    }
    // This method should be called when the viewport is resized.
    public void OnResize(Viewport newViewport)
    {
        _viewport = newViewport;
        CalculateZoomToFit();
        CenterCamera();
        UpdateTransform();
    }
    // This method ensures that the camera's pos is clamped within the bounds of the world, so you cant scroll past the edges of the tilemap
    private void ClampToWorld()
    {
        float viewWidth = _viewport.Width / Zoom;
        float viewHeight = _viewport.Height / Zoom;

        float halfWidth = viewWidth / 2f;
        float halfHeight = viewHeight / 2f;

        float minX = halfWidth;
        float maxX = _worldWidth - halfWidth;

        float minY = halfHeight;
        float maxY = _worldHeight - halfHeight;

        if (_worldWidth < viewWidth)
        {
            Position = new Vector2(_worldWidth / 2f, Position.Y);
        }
        else
        {
            Position = new Vector2(MathHelper.Clamp(Position.X, minX, maxX), Position.Y);
        }

        if (_worldHeight < viewHeight)
        {
            Position = new Vector2(Position.X, _worldHeight / 2f);
        }
        else
        {
            Position = new Vector2(Position.X, MathHelper.Clamp(Position.Y, minY, maxY));
        }
    }
    // Gets the world position from the screen position, used for example to get the mouse position in the world
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        return Vector2.Transform(screenPosition, Matrix.Invert(Transform));
    }
}