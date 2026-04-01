using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class ContiguousArrayMaze : Maze
{
    public const short TypeId = 1;

    private readonly BitGrid grid;

    public ContiguousArrayMaze(int width, int height) : this(width, height, new(width, height))
    {
    }

    public ContiguousArrayMaze(ContiguousArrayMaze source) : this(source.Width, source.Height, new(source.grid))
    {
    }

    private ContiguousArrayMaze(int width, int height, BitGrid grid) : base(width, height)
    {
        this.grid = grid;
    }

    public override Maze Clone() => new ContiguousArrayMaze(this);

    public override bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => grid[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => grid[x, y] = value;
    }

    public override async Task Write(BinaryWriter writer)
    {
        writer.Write(TypeId);
        writer.Write(Width);
        writer.Write(Height);
        await grid.Write(writer);
    }

    public static async Task<ContiguousArrayMaze> Read(BinaryReader reader)
    {
        var width = reader.ReadInt32();
        var height = reader.ReadInt32();
        var grid = await BitGrid.Read(reader);
        return new ContiguousArrayMaze(width, height, grid);
    }
}
