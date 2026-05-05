using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGrid : IDisposable, IAsyncDisposable
{
    public readonly int width;
    public readonly int height;
    private readonly BigBitArray array;

    public BigBitGrid(IBinarySerializer serializer, long offset)
    {
        width = serializer.ReadInt32();
        height = serializer.ReadInt32();
        array = new BigBitArray(serializer, serializer.Position, (long)width * height);
    }

    public BigBitGrid(IBinarySerializer serializer, long offset, int width, int height)
    {
        this.width = width;
        this.height = height;
        serializer.Write(width);
        serializer.Write(height);
        array = new BigBitArray(serializer, serializer.Position, (long)width * height);
    }

    public int Width => width;
    public int Height => height;

    //public BigBitGrid Clone() => new(this);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + ((long)y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + ((long)y * height)] = value;
    }

    public void Dispose()
    {
        array.Write();
        GC.SuppressFinalize(this);
    }

    public async ValueTask DisposeAsync()
    {
        await array.WriteAsync();
        GC.SuppressFinalize(this);
    }
}
