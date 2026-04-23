using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

/// <summary>
/// Info about mazes:
/// 0 = False = Wall = Black
/// 1 = True = Empty = White
/// </summary>
public interface IMaze
{
    MazeType Type { get; }
    Stream Stream { get; }
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IMaze Clone();

    void Read();
    Task ReadAsync();
    void Write();
    Task WriteAsync();

    public static IMaze Read(MazeType type, Stream stream)
    {
        var maze = ReadMazeType(type, stream);
        maze.Read();
        return maze;
    }

    public static async Task<IMaze> ReadAsync(MazeType type, Stream stream)
    {
        var maze = ReadMazeType(type, stream);
        await maze.ReadAsync();
        return maze;
    }

    private static IMaze ReadMazeType(MazeType type, Stream stream) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(stream),
        MazeType.BigBitGridMaze => new BigBitGridMaze((FileStream)stream),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static IMaze Create(MazeType type, Stream stream, int width, int height) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(stream, width, height),
        MazeType.BigBitGridMaze => new BigBitGridMaze((FileStream)stream, width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}