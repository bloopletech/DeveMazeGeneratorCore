namespace DeveMazeGeneratorCore.Mazes;

/// <summary>
/// Info about mazes:
/// 0 = False = Wall = Black
/// 1 = True = Empty = White
/// </summary>
public interface IMaze : IDisposable, IAsyncDisposable
{
    Stream Stream { get; }
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IMaze Clone();

    public static IMaze Read(MazeType type, Stream stream) => type switch
    {
        MazeType.BitGridMaze => BitGridMaze.Read(stream),
        MazeType.BigBitGridMaze => BigBitGridMaze.Read(stream),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static async Task<IMaze> ReadAsync(MazeType type, Stream stream) => type switch
    {
        MazeType.BitGridMaze => await BitGridMaze.ReadAsync(stream),
        MazeType.BigBitGridMaze => await BigBitGridMaze.ReadAsync(stream),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static IMaze Create(MazeType type, Stream stream, int width, int height) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(stream, width, height),
        MazeType.BigBitGridMaze => new BigBitGridMaze(((FileStream)stream).SafeFileHandle, stream.Position, width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}