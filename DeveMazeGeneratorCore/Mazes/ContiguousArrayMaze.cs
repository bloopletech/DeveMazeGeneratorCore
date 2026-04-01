using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class ContiguousArrayMaze : Maze
{
    public const short TypeId = 1;

    private readonly ContiguousBitList list;

    public ContiguousArrayMaze(int width, int height) : base(width, height)
    {
        list = new(width, height);
    }

    public ContiguousArrayMaze(ContiguousArrayMaze source) : base(source.Width, source.Height)
    {
        list = new(source.list);
    }

    public ContiguousArrayMaze(BinaryReader reader) : this(reader.ReadInt32(), reader.ReadInt32())
    {
    }

    public override Maze Clone() => new ContiguousArrayMaze(this);

    public override bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => list[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => list[x, y] = value;
    }

    protected override async Task Read(BinaryReader reader)
    {
        await list.Read(reader);
    }

    protected override async Task Write(BinaryWriter writer)
    {
        writer.Write(TypeId);
        writer.Write(Width);
        writer.Write(Height);
        await list.Write(writer);
    }
}
