using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class MazePathPos
{
    private IBinarySerializer serializer;
    private MazePointPos[] points;

    public MazePathPos(IBinarySerializer serializer) : this(serializer, [])
    {
    }

    public MazePathPos(IBinarySerializer serializer, MazePointPos[] points)
    {
        this.serializer = serializer;
        this.points = points;
    }

    public MazePointPos[] Points => points;

    public void Read()
    {
        points = serializer.ReadArray<MazePointPos>();
    }

    public void Write()
    {
        serializer.WriteArray(points);
    }

    public async Task WriteAsync()
    {
        using var writer = stream.Writer();
        writer.Write((ushort)MazePathType.MazePath);
        writer.Write(points.Length);
        await stream.WriteAsync(new ReadOnlyMemory<MazePoint>(points));
    }

    public static IMazePath Read(Stream stream)
    {
        using var reader = stream.Reader();
        var length = reader.ReadInt32();
        var points = new MazePoint[length];
        stream.ReadExactly(MemoryMarshal.AsBytes(points.AsSpan()));
        return new MazePath(points);
    }

    public static async Task<IMazePath> ReadAsync(Stream stream)
    {
        using var reader = stream.Reader();
        var length = reader.ReadInt32();
        var points = new MazePoint[length];
        await stream.ReadExactlyAsync(MemoryMarshal.AsBytes(points.AsSpan()));
        return new MazePath(points);
    }
}
