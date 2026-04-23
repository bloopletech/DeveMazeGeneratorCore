using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Paths;

public class MazePath : IMazePath
{
    private readonly Stream stream;
    private readonly int width;
    private readonly int height;
    private readonly BitGrid grid;

    public MazePath(Stream stream)
    {
        this.stream = stream;

        using var reader = stream.Reader();
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();

        grid = new BitGrid(stream);

        if(width != grid.Width) throw new ArgumentException($"width {width} != grid width {grid.Width}");
        if(height != grid.Height) throw new ArgumentException($"height {height} != grid height {grid.Height}");
    }

    public MazePath(Stream stream, int width, int height)
    {
        this.stream = stream;
        this.width = width;
        this.height = height;

        using var writer = stream.Writer();
        writer.Write(width);
        writer.Write(height);

        grid = new BitGrid(stream, width, height);
    }

    public Stream Stream => stream;
    public int Width => width;
    public int Height => height;

    public IMazePath Clone() => throw new NotImplementedException();

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public void Highlight()
    {
        //var points = new MazePointPos[stack.Count];

        //foreach(var item in stack)
        //{
        //    byte formulathing = (byte)(points.Count / (double)stack.Count * 255.0);
        //    points.Add(new MazePointPos(item.X, item.Y, formulathing));
        //}
    }

    public void Read()
    {
        grid.Read();
        stream.EnsureCompleted();
    }

    public async Task ReadAsync()
    {
        await grid.ReadAsync();
        stream.EnsureCompleted();
    }

    public void Write()
    {
        //WriteHeader(stream);
        grid.Write();
    }

    public async Task WriteAsync()
    {
        //WriteHeader(stream);
        await grid.WriteAsync();
    }
}
