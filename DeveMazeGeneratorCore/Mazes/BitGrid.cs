using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid
{
    private readonly Stream stream;
    public readonly int width;
    public readonly int height;
    private readonly BitArray array;

    public BitGrid(Stream stream)
    {
        this.stream = stream;

        using var reader = stream.Reader();
        width = reader.ReadInt32();
        height = reader.ReadInt32();

        //array = new BitArray(0);
        array = new BitArray(reader.ReadInt32());
    }

    public BitGrid(Stream stream, int width, int height)
    {
        this.stream = stream;
        this.width = width;
        this.height = height;

        using var writer = stream.Writer();
        writer.Write(width);
        writer.Write(height);

        array = new BitArray(width * height);
    }

    //public BitGrid(BitGrid source) : this(source.width, source.height, new(source.array))
    //{
    //}

    public int Width => width;
    public int Height => height;

    //public BitGrid Clone() => new(this);
    public BitGrid Clone() => throw new NotImplementedException();

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[x + (y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[x + (y * height)] = value;
    }

    public void Read()
    {
        stream.ReadExactly(array.GetArray());
        //stream.PreservePosition(() => stream.ReadExactly(array.GetArray()));
        //array = BitArray.Read(stream);
    }

    public async Task ReadAsync()
    {
        await stream.ReadExactlyAsync(array.GetArray());
        //await stream.PreservePosition(async () => await stream.ReadExactlyAsync(array.GetArray()));
        //array = await BitArray.ReadAsync(stream);
    }

    public void Write()
    {
        //WriteHeader();
        array.Write(stream);
    }

    public async Task WriteAsync()
    {
        //WriteHeader();
        await array.WriteAsync(stream);
    }

    //private void WriteHeader()
    //{
    //    using var writer = stream.Writer();
    //    writer.Write(width);
    //    writer.Write(height);
    //}
}
