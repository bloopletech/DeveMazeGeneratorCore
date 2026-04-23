using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class MazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    private static MazeType ReadHeader(Stream stream)
    {
        using var reader = stream.Reader();
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return (MazeType)reader.ReadUInt16();
    }

    private static void WriteHeader(Stream stream, MazeType type)
    {
        using var writer = stream.Writer();
        writer.Write(MagicHeader);
        writer.Write(Version);
        writer.Write((ushort)type);
    }

    public static IMaze Read(Stream stream)
    {
        var type = ReadHeader(stream);
        var maze = InitForRead(type, stream);
        maze.Read();
        return maze;
    }

    public static async Task<IMaze> ReadAsync(Stream stream)
    {
        var type = ReadHeader(stream);
        var maze = InitForRead(type, stream);
        await maze.ReadAsync();
        return maze;
    }

    private static IMaze InitForRead(MazeType type, Stream stream) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(stream),
        MazeType.BigBitGridMaze => new BigBitGridMaze((FileStream)stream),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };

    public static IMaze Create(MazeType type, Stream stream, int width, int height)
    {
        WriteHeader(stream, type);
        return InitForWrite(type, stream, width, height);
    }

    private static IMaze InitForWrite(MazeType type, Stream stream, int width, int height) => type switch
    {
        MazeType.BitGridMaze => new BitGridMaze(stream, width, height),
        MazeType.BigBitGridMaze => new BigBitGridMaze((FileStream)stream, width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
