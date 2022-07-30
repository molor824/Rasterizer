public class Screen
{
    public List<Mesh> Meshes = new();

    float[] _depth;
    Color[] _pixels;
    int _width, _height;
    float Near = 10, Far = -10;

    public int Width => _width;
    public int Height => _height;
    public Screen(int width, int height)
    {
        _width = width;
        _height = height;
        _depth = new float[width * height];
        _pixels = new Color[width * height];
    }
    public void DrawPixel(int x, int y, Color color)
    {
        if (x < 0 || x >= _width) return;
        if (y < 0 || y >= _height) return;

        _pixels[x + y * _width] = color;
    }
    public float? GetDepth(int x, int y)
    {
        if (x >= _width || x < 0) return null;
        if (y >= _height || y < 0) return null;
        return _depth[x + y * _width];
    }
    public void SetDepth(int x, int y, float depth)
    {
        if (x >= _width || x < 0) return;
        if (y >= _height || y < 0) return;
        _depth[x + y * _width] = depth;
    }
    void Clear()
    {
        for (var i = 0; i < _depth.Length; i++)
        {
            _depth[i] = 0;
            _pixels[i] = Color.BLACK;
        }
    }
    void RenderPixels()
    {
        Raylib.BeginDrawing();

        var scale = new vec2(Raylib.GetScreenWidth(), Raylib.GetScreenHeight()) / new vec2(_width, _height);
        for (var i = 0; i < _pixels.Length; i++)
        {
            var pos = new vec2(i % _width, i / _width);
            var color = _pixels[i];

            Raylib.DrawRectangleV((pos * scale).ToNumerics(), scale.ToNumerics(), color);
        }

        Raylib.EndDrawing();
    }
    public void Draw()
    {
        Clear();
        var projection = mat4.Translate(new(0, 0, 0.5f)) * mat4.Scale(new vec3(1, 1, 1 / (Near - Far)))
            * mat4.Translate(new(0, 0, -(Near + Far) / 2));

        for (var i = 0; i < Meshes.Count; i++) Meshes[i].Draw(this, projection);

        RenderPixels();
    }
}