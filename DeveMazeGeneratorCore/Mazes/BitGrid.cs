using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid
{
    private readonly IBinarySerializer serializer;
    public readonly int width;
    public readonly int height;
    private BitArray array;

    public BitGrid(IBinarySerializer serializer)
    {
        this.serializer = serializer;

        width = serializer.ReadInt32();
        height = serializer.ReadInt32();

        //array = new BitArray(0);
        array = new BitArray(serializer.ReadInt32());
    }

    public BitGrid(IBinarySerializer serializer, int width, int height)
    {
        this.serializer = serializer;
        this.width = width;
        this.height = height;
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
       // array = new BitArray(serializer.ReadInt32());
        serializer.ReadExactly(array.GetArray());
    }

    public async Task ReadAsync()
    {
        //array = new BitArray(serializer.ReadInt32());
        await serializer.ReadExactlyAsync(array.GetArray());
    }

    public void Write()
    {
        serializer.Write(width);
        serializer.Write(height);
        serializer.Write(array.Length);
        serializer.Write(array.GetArray());
        //serializer.WriteArray(array.GetArray());
    }

    public async Task WriteAsync()
    {
        serializer.Write(width);
        serializer.Write(height);
        serializer.Write(array.Length);
        await serializer.WriteAsync(array.GetArray());
        //await serializer.WriteArrayAsync(array.GetArray());
    }
}
