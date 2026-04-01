using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore.Serializers;

public class MazeSerializer
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'M', 'A', 'Z', 'E'];
    public const short Version = 1;

    public static async Task Write(Stream stream, Maze maze)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(MagicHeader);
        writer.Write(Version);
        await maze.Write(writer);
    }

    public static async Task Save(string fileName, Maze maze)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Write(fs, maze);
    }

    public static async Task<Maze> Read(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != 1) throw new InvalidDataException($"Maze version is {version} but we only understand version 1");

        var type = reader.ReadInt16();
        if(type == ContiguousArrayMaze.TypeId)
        {
            return await ContiguousArrayMaze.Read(reader);
        }
        else
        {
            throw new InvalidDataException($"Unknown maze type {type}");
        }
    }

    public static async Task<Maze> Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await Read(fs);
    }
}
