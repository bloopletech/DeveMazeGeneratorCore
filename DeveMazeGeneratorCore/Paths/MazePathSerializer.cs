using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Paths;

public static class MazePathSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;

    private static MazePathType ReadHeader(Stream stream)
    {
        using var reader = stream.Reader();
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != Version) throw new InvalidDataException($"Path version is {version} but we only understand version {Version}");

        return (MazePathType)reader.ReadUInt16();
    }

    private static void WriteHeader(Stream stream, MazePathType type)
    {
        using var writer = stream.Writer();
        writer.Write(MagicHeader);
        writer.Write(Version);
        writer.Write((ushort)type);
    }

    public static IMazePath Read(Stream stream)
    {
        var type = ReadHeader(stream);
        var path = InitForRead(type, stream);
        path.Read();
        return path;
    }

    public static async Task<IMazePath> ReadAsync(Stream stream)
    {
        var type = ReadHeader(stream);
        var path = InitForRead(type, stream);
        await path.ReadAsync();
        return path;
    }

    private static IMazePath InitForRead(MazePathType type, Stream stream) => type switch
    {
        MazePathType.MazePath => new MazePath(stream),
        _ => throw new InvalidDataException($"Unknown path type {type}")
    };

    public static IMazePath Create(MazePathType type, Stream stream, int width, int height)
    {
        WriteHeader(stream, type);
        return InitForWrite(type, stream, width, height);
    }

    private static IMazePath InitForWrite(MazePathType type, Stream stream, int width, int height) => type switch
    {
        MazePathType.MazePath => new MazePath(stream, width, height),
        _ => throw new InvalidDataException($"Unknown maze type {type}")
    };
}
