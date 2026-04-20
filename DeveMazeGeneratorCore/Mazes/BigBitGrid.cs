using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGrid : IDisposable, IAsyncDisposable
{
    public readonly int width;
    public readonly int height;
    private readonly BigBitArray array;

    public BigBitGrid(SafeFileHandle handle, long offset)
    {
        width = RandomAccess.ReadInt32(handle, ref offset);
        height = RandomAccess.ReadInt32(handle, ref offset);
        array = new BigBitArray(handle, offset, (long)width * height);
    }

    public BigBitGrid(FileStream stream) : this(stream.SafeFileHandle, stream.Position)
    {
    }

    public BigBitGrid(SafeFileHandle handle, long offset, int width, int height)
    {
        this.width = width;
        this.height = height;
        RandomAccess.Write(handle, ref offset, width);
        RandomAccess.Write(handle, ref offset, height);
        array = new BigBitArray(handle, offset, (long)width * height);
    }

    public BigBitGrid(FileStream stream, int width, int height) : this(stream.SafeFileHandle, stream.Position, width, height)
    {
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

    public static BigBitGrid Read(FileStream stream)
    {
        return new BigBitGrid(stream.SafeFileHandle, stream.Position);
    }

    public static async Task<BigBitGrid> ReadAsync(FileStream stream)
    {
        return new BigBitGrid(stream.SafeFileHandle, stream.Position);
    }
}
