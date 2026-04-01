using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class ContiguousBitList
{
    private readonly int width;
    private readonly int height;
    private readonly BitList data;

    public ContiguousBitList(int width, int height)
    {
        this.width = width;
        this.height = height;
        data = new(width * height);
    }

    public ContiguousBitList(ContiguousBitList source)
    {
        width = source.width;
        height = source.height;
        data = new(source.data);
    }

    public ContiguousBitList(BinaryReader reader) : this(reader.ReadInt32(), reader.ReadInt32())
    {
    }

    public ContiguousBitList Clone() => new(this);

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => data[x + (y * height)];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => data[x + (y * height)] = value;
    }

    public async Task Read(BinaryReader reader)
    {
        await data.Read(reader.BaseStream);
    }

    public async Task Write(BinaryWriter writer)
    {
        writer.Write(width);
        writer.Write(height);
        await data.Write(writer.BaseStream);
    }
}
