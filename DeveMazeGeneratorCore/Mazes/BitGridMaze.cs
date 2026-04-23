using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGridMaze : IMaze
{
    private readonly Stream stream;
    private readonly int width;
    private readonly int height;
    private readonly BitGrid grid;

    public BitGridMaze(Stream stream)
    {
        this.stream = stream;

        using var reader = stream.Reader();
        width = reader.ReadInt32();
        height = reader.ReadInt32();

        grid = new BitGrid(stream);

        if(width != grid.Width) throw new ArgumentException($"width {width} != grid width {grid.Width}");
        if(height != grid.Height) throw new ArgumentException($"height {height} != grid height {grid.Height}");
    }

    public BitGridMaze(Stream stream, int width, int height)
    {
        this.stream = stream;
        this.width = width;
        this.height = height;

        MazeSerializer.WriteHeader(stream, MazeType.BitGridMaze);

        using var writer = stream.Writer();
        writer.Write(width);
        writer.Write(height);

        grid = new BitGrid(stream, width, height);
    }

    //public BitGridMaze(BitGridMaze source) : this(new MemoryStream(), source.Width, source.Height, new(source.grid))
    //{
    //}

    //private BitGridMaze(Stream stream, int width, int height, BitGrid grid)
    //{
    //    if(width != grid.Width) throw new ArgumentException($"width {width} != grid width {grid.Width}");
    //    if(height != grid.Height) throw new ArgumentException($"height {height} != grid height {grid.Height}");

    //    this.stream = stream;
    //    this.width = width;
    //    this.height = height;
    //    this.grid = grid;
    //}

    public MazeType Type => MazeType.BitGridMaze;
    public Stream Stream => stream;
    public int Width => width;
    public int Height => height;

    //public IMaze Clone() => new BitGridMaze(this);
    public IMaze Clone() => throw new NotImplementedException();

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
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

    //private void WriteHeader(Stream stream)
    //{
    //    using var writer = stream.Writer();
    //    writer.Write(Width);
    //    writer.Write(Height);
    //}

    public static BitGridMaze Read(Stream stream)
    {
        var maze = new BitGridMaze(stream);
        maze.Read();
        return maze;
    }

    public static async Task<BitGridMaze> ReadAsync(Stream stream)
    {
        var maze = new BitGridMaze(stream);
        await maze.ReadAsync();
        return maze;
    }
}
