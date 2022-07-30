var width = 800;
var height = 600;

var screen = new Screen(800 / 2, 600 / 2);
var cube = new Mesh(
    new Vertex[]
    {
        new(new(0.5f, 0.5f, 0.5f), Color.RED),
        new(new(-0.5f, 0.5f, 0.5f), Color.RED),
        new(new(-0.5f, 0.5f, -0.5f), Color.RED),
        new(new(0.5f, 0.5f, -0.5f), Color.RED),
        new(new(0.5f, -0.5f, 0.5f), Color.YELLOW),
        new(new(-0.5f, -0.5f, 0.5f), Color.YELLOW),
        new(new(-0.5f, -0.5f, -0.5f), Color.YELLOW),
        new(new(0.5f, -0.5f, -0.5f), Color.YELLOW)
    },
    new int[]
    {
        0, 1, 2, 2, 3, 0,
        4, 5, 6, 6, 7, 4,
        0, 4, 5, 0, 5, 1,
        1, 5, 6, 1, 6, 2,
        2, 6, 7, 2, 7, 3,
        3, 7, 4, 3, 0, 4,
    }
);
var triangle = new Mesh(
    new Vertex[]
    {
        new(new(0, 0.5f, 0), Color.RED),
        new(new(0.5f, -0.5f, 0), Color.GREEN),
        new(new(-0.5f, -0.5f, 0), Color.BLUE)
    },
    new int[] { 0, 1, 2 }
);
var triangle1 = new Mesh(
    new Vertex[]
    {
        new(new(0, 0.5f, 0), Color.RED),
        new(new(0.5f, -0.5f, 0), Color.GREEN),
        new(new(-0.5f, -0.5f, 0), Color.BLUE)
    },
    new int[] { 0, 1, 2 }
);

cube.Scale = new(0.4f, 0.4f, 0.4f);

screen.Meshes.Add(cube);
screen.Meshes.Add(triangle);
screen.Meshes.Add(triangle1);
Raylib.InitWindow(width, height, "Rasterizer");
Raylib.SetTargetFPS(60);

while (!Raylib.WindowShouldClose())
{
    var delta = Raylib.GetFrameTime();
    cube.Rotation *= quat.FromAxisAngle(delta, new(0, 1, 1));
    triangle.Rotation *= quat.FromAxisAngle(delta, new(0, 0, -1));
    triangle1.Rotation *= quat.FromAxisAngle(delta, new(1, 0, 0));
    screen.Draw();
}