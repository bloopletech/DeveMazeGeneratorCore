using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class MazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    public static void WriteHeader(Stream stream, MazeType type)
    {
        using var writer = stream.Writer();
        writer.Write(MagicHeader);
        writer.Write(Version);
        writer.Write((ushort)type);
    }




    private readonly Stream stream;

    public MazeSerializer(Stream stream)
    {
        this.stream = stream;
    }

    public MazeSerializer(Stream stream, IMaze maze)
    {
        this.stream = stream;


    }

    public static void Serialize(IMaze maze)
    {
        maze.Stream.Position = 0;
        SerializeHeader(maze);
        maze.Write();
    }

    public static async Task SerializeAsync(IMaze maze)
    {
        maze.Stream.Position = 0;
        SerializeHeader(maze);
        await maze.WriteAsync();
    }

    private static void SerializeHeader(IMaze maze)
    {
        using var writer = maze.Stream.Writer();
        writer.Write(MagicHeader);
        writer.Write(Version);
        writer.Write((ushort)maze.Type);
    }

    public IMaze Deserialize()
    {
        var type = DeserializeHeader(stream);
        return IMaze.Read(type, stream);
    }

    public static async Task<IMaze> DeserializeAsync(Stream stream)
    {
        var type = DeserializeHeader(stream);
        var maze = await IMaze.ReadAsync(type, stream);
        stream.EnsureCompleted();
        return maze;
    }

    private static MazeType DeserializeHeader(Stream stream)
    {
        using var reader = stream.Reader();
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Maze version is {version} but we only understand version {Version}");

        return (MazeType)reader.ReadUInt16();
    }
}
