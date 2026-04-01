using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class BitGrid
{
    private readonly int width;
    private readonly int height;
    private readonly BitList list;

    public BitGrid(int width, int height) : this(width, height, new(width * height))
    {
    }

    public BitGrid(BitGrid source) : this(source.width, source.height, new(source.list))
    {
    }

    private BitGrid(int width, int height, BitList list)
    {
        this.width = width;
        this.height = height;
        this.list = list;
    }

    public BitGrid Clone() => new(this);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => list[x + (y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => list[x + (y * height)] = value;
    }

    public async Task Write(BinaryWriter writer)
    {
        writer.Write(width);
        writer.Write(height);
        await list.Write(writer);
    }

    public static async Task<BitGrid> Read(BinaryReader reader)
    {
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var list = await BitList.Read(reader);

        return new BitGrid(width, height, list);
    }
}
