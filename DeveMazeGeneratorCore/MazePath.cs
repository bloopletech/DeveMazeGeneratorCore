using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Mazes;

namespace DeveMazeGeneratorCore;

public class MazePath
{
    public static readonly char[] MagicHeader = ['D', 'E', 'V', 'E', 'P', 'A', 'T', 'H'];
    public const short Version = 1;
    public const short TypeId = 1;

    private readonly ContiguousBitList list;

    public MazePath(int width, int height)
    {
        list = new(width, height);
    }

    public MazePath(BinaryReader reader)
    {
        list = new(reader);
    }

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => list[x, y];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => list[x, y] = value;
    }

    public void Highlight()
    {
        //var points = new MazePointPos[stack.Count];

        //foreach(var item in stack)
        //{
        //    byte formulathing = (byte)(points.Count / (double)stack.Count * 255.0);
        //    points.Add(new MazePointPos(item.X, item.Y, formulathing));
        //}
    }

    public async Task Read(BinaryReader reader)
    {
        await list.Read(reader);
    }

    public async Task Write(Stream stream)
    {
        using var writer = new BinaryWriter(stream);
        writer.Write(MagicHeader);
        writer.Write(Version);
        writer.Write(TypeId);
        await list.Write(writer);
    }

    public async Task Save(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Write(fs);
    }

    public static async Task<MazePath> Read(Stream stream)
    {
        using var reader = new BinaryReader(stream);
        var magic = reader.ReadChars(8);
        if(!magic.SequenceEqual(MagicHeader)) throw new InvalidDataException("Magic header not present");

        var version = reader.ReadInt16();
        if(version != 1) throw new InvalidDataException($"Path version is {version} but we only understand version 1");

        var type = reader.ReadInt16();
        if(type != TypeId) throw new InvalidDataException($"Unknown path type {type}");

        var path = new MazePath(reader);
        await path.Read(reader);
        return path;
    }

    public static async Task<MazePath> Load(string fileName)
    {
        using var fs = File.Open(fileName, FileMode.Open);
        return await Read(fs);
    }
}
