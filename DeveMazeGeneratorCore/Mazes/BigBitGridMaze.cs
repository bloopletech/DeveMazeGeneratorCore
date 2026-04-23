using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGridMaze : IMaze
{
    private readonly Stream stream;
    private readonly int width;
    private readonly int height;
    private readonly BigBitGrid grid;

    //public BigBitGridMaze(BigBitGridMaze source) : this(source.Width, source.Height, new(source.grid))
    //{
    //}

    public BigBitGridMaze(FileStream stream)
    {
        this.stream = stream;
        var handle = stream.SafeFileHandle;
        var offset = stream.Position;

        width = RandomAccess.ReadInt32(handle, ref offset);
        height = RandomAccess.ReadInt32(handle, ref offset);
        grid = new BigBitGrid(handle, offset);

        if(width != grid.Width) throw new ArgumentException($"width {width} != grid width {grid.Width}");
        if(height != grid.Height) throw new ArgumentException($"height {height} != grid height {grid.Height}");
    }

    public BigBitGridMaze(FileStream stream, int width, int height)
    {
        this.stream = stream;
        var handle = stream.SafeFileHandle;
        var offset = stream.Position;

        this.width = width;
        this.height = height;

        //RandomAccess.Write(handle, ref offset, (ushort)MazeType.BitGridMaze);
        RandomAccess.Write(handle, ref offset, width);
        RandomAccess.Write(handle, ref offset, height);
        grid = new BigBitGrid(handle, offset, width, height);
    }

    public MazeType Type => MazeType.BigBitGridMaze;
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
    }

    public async Task ReadAsync()
    {
    }

    public void Write()
    {
        grid.Dispose();
    }

    public async Task WriteAsync()
    {
        await grid.DisposeAsync();
    }
}
