using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGridMaze : IMaze
{
    private readonly IBinarySerializer serializer;
    private readonly int width;
    private readonly int height;
    private readonly BigBitGrid grid;

    //public BigBitGridMaze(BigBitGridMaze source) : this(source.Width, source.Height, new(source.grid))
    //{
    //}

    public BigBitGridMaze(IBinarySerializer serializer)
    {
        this.serializer = serializer;
        width = serializer.ReadInt32();
        height = serializer.ReadInt32();
        grid = new BigBitGrid(serializer, serializer.Position);

        if(width != grid.Width) throw new ArgumentException($"width {width} != grid width {grid.Width}");
        if(height != grid.Height) throw new ArgumentException($"height {height} != grid height {grid.Height}");
    }

    public BigBitGridMaze(IBinarySerializer serializer, int width, int height)
    {
        this.serializer = serializer;
        this.width = width;
        this.height = height;
        grid = new BigBitGrid(serializer, serializer.Position, width, height);
    }

    public MazeType Type => MazeType.BigBitGridMaze;
    public IBinarySerializer Serializer => serializer;
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
        //RandomAccess.Write(handle, ref offset, (ushort)MazeType.BitGridMaze);
        serializer.Write(width);
        serializer.Write(height);
        grid.Dispose();
    }

    public async Task WriteAsync()
    {
        //RandomAccess.Write(handle, ref offset, (ushort)MazeType.BitGridMaze);
        serializer.Write(width);
        serializer.Write(height);
        await grid.DisposeAsync();
    }
}
