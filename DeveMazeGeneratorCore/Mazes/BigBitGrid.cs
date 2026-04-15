using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class BigBitGrid
{
    public readonly int width;
    public readonly int height;
    private readonly BigBitArray array;

    public BigBitGrid(int width, int height) : this(width, height, new(width * height))
    {
    }

    public BigBitGrid(BigBitGrid source) : this(source.width, source.height, new(source.array))
    {
    }

    private BigBitGrid(int width, int height, BigBitArray array)
    {
        var length = width * height;
        if(length != array.Length)
        {
            throw new ArgumentException($"(width {width} * height {height}) {length} != array length {array.Length}");
        }

        this.width = width;
        this.height = height;
        this.array = array;
    }

    public int Width => width;
    public int Height => height;

    public BigBitGrid Clone() => new(this);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + (y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + (y * height)] = value;
    }

    public void Write(Stream stream)
    {
        using var compressor = stream.Compressor();
        WriteHeader(compressor);
        array.Write(compressor);
    }

    public async Task WriteAsync(Stream stream)
    {
        using var compressor = stream.Compressor();
        WriteHeader(compressor);
        await array.WriteAsync(compressor);
    }

    private void WriteHeader(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write(width);
        writer.Write(height);
    }

    public static BigBitGrid Read(Stream stream)
    {
        using var decompressor = stream.Decompressor();
        using var reader = decompressor.Reader();
        return new BigBitGrid(reader.ReadInt32(), reader.ReadInt32(), BigBitArray.Read(decompressor));
    }

    public static async Task<BigBitGrid> ReadAsync(Stream stream)
    {
        using var decompressor = stream.Decompressor();
        using var reader = decompressor.Reader();
        return new BigBitGrid(reader.ReadInt32(), reader.ReadInt32(), await BigBitArray.ReadAsync(decompressor));
    }
}
