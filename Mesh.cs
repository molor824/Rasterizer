public class Mesh
{
    public Vertex[] Vertices = Array.Empty<Vertex>();
    public int[] Indices = Array.Empty<int>();
    public vec3 Position;
    public quat Rotation = quat.Identity;
    public vec3 Scale = new(1, 1, 1);

    public Mesh() { }
    public Mesh(IEnumerable<Vertex> vertices, IEnumerable<int> indices)
    {
        Vertices = vertices.ToArray();
        Indices = indices.ToArray();
    }
    static Vertex VertexCalculate(mat4 transform, vec3 size, Vertex vertex)
    {
        var pos = (transform * new vec4(vertex.Position, 1));
        vertex.Position = pos.xyz * pos.w;
        vertex.Position *= size;
        vertex.Position += size;

        return vertex;
    }
    public void Draw(Screen screen, mat4 projection)
    {
        var size = new vec3(screen.Width / 2, screen.Height / 2, 1);
        var matrix = projection * mat4.Scale(Scale) * Rotation.ToMat4 * mat4.Translate(Position);

        for (var i = 0; i < Indices.Length; i += 3)
        {
            var vertex = VertexCalculate(matrix, size, Vertices[Indices[i]]);
            var vertex1 = VertexCalculate(matrix, size, Vertices[Indices[i + 1]]);
            var vertex2 = VertexCalculate(matrix, size, Vertices[Indices[i + 2]]);

            Rasterize(screen, vertex, vertex1, vertex2);
        }
    }
    static bool IsYIn(vec3 pos, vec3 pos1, float y)
    {
        var min = MathF.Min(pos.y, pos1.y);
        var max = MathF.Max(pos.y, pos1.y);

        return min <= y && y <= max;
    }
    static Color GetColorFromY(Vertex vertex, Vertex vertex1, float y)
    {
        return vertex.Color.Lerp(vertex1.Color, vertex.Position.y.InvLerp(vertex1.Position.y, y));
    }
    static vec3? GetYIntersect(vec3 pos, vec3 pos1, float y)
    {
        if (pos.y == pos1.y) return null;
        if (!IsYIn(pos, pos1, y)) return null;

        var diff = pos1 - pos;
        var ratio = diff / diff.y;
        return pos + ratio * (y - pos.y);
    }
    static void DrawLine(Screen screen, Vertex vertex, Vertex vertex1)
    {
        var pos = vertex.Position;
        var pos1 = vertex1.Position;
        var y = (int)MathF.Round(pos.y);
        var min = MathF.Round(MathF.Min(pos.x, pos1.x));
        var max = MathF.Round(MathF.Max(pos.x, pos1.x));
        var depth = pos.z;
        var depth1 = pos1.z;
        var color = vertex.Color;
        var color1 = vertex1.Color;

        if (pos.x > pos1.x)
        {
            var temp = color;
            color = color1;
            color1 = temp;

            var temp2 = depth;
            depth = depth1;
            depth1 = temp2;
        }

        for (var x = (int)min; x <= (int)max; x++)
        {
            if (!(screen.GetDepth(x, y) is float sDepth)) continue;
            var normal = min.InvLerp(max, x);
            var crntDepth = depth.Lerp(depth1, normal);
            // must be in front
            if (sDepth > crntDepth) continue;

            screen.SetDepth(x, y, crntDepth);
            screen.DrawPixel(x, y, color.Lerp(color1, normal));
        }
    }
    static void Rasterize(Screen screen, Vertex vertex, Vertex vertex1, Vertex vertex2)
    {
        var pos = vertex.Position;
        var pos1 = vertex1.Position;
        var pos2 = vertex2.Position;

        var lowest = MathF.Round(MathF.Min(pos.y, MathF.Min(pos1.y, pos2.y)));
        var highest = MathF.Round(MathF.Max(pos.y, MathF.Max(pos1.y, pos2.y)));

        Parallel.For((int)lowest - 1, (int)highest + 2, i =>
        {
            var points = new List<Vertex>(3);

            // pos, pos1
            if (GetYIntersect(pos, pos1, i) is vec3 point)
            {
                points.Add(new(point, GetColorFromY(vertex, vertex1, i)));
            }
            // pos1, pos2
            if (GetYIntersect(pos1, pos2, i) is vec3 point1)
            {
                points.Add(new(point1, GetColorFromY(vertex1, vertex2, i)));
            }
            // pos2, pos
            if (GetYIntersect(pos2, pos, i) is vec3 point2)
            {
                points.Add(new(point2, GetColorFromY(vertex2, vertex, i)));
            }

            if (points.Count < 2)
            {
                return;
            }

            DrawLine(screen, points[0], points[1]);
        });
    }
}
public struct Vertex
{
    public vec3 Position;
    public Color Color;

    public Vertex(vec3 position, Color color)
    {
        Position = position;
        Color = color;
    }
}