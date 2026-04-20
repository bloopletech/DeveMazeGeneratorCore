using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGrid
{
    public readonly int width;
    public readonly int height;
    private readonly BigBitArray array;

    public BigBitGrid(SafeFileHandle handle, long offset)
    {
        //read the width from the handle
        this.width = width;
        //read the height from the handle
        this.height = height;
        this.array = new BigBitArray(handle, offset, (long)width * height);
    }

    public BigBitGrid(SafeFileHandle handle, long offset, int width, int height)
    {
        //write the width to the handle
        this.width = width;
        //write the height to the handle
        this.height = height;
        this.array = new BigBitArray(handle, offset, (long)width * height);
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

    public void Write()
    {
        array.Write();
    }

    public async Task WriteAsync()
    {
        await array.WriteAsync();
    }

    private void WriteHeader(FileStream stream)
    {
        using var writer = stream.Writer();
        writer.Write(width);
        writer.Write(height);
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
