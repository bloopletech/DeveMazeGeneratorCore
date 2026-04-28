using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class MazePath : IMazePath
{
    private MazePoint[] points;

    public MazePath(MazePoint[] points)
    {
        this.points = points;
    }

    public IMazePath Clone() => throw new NotImplementedException();

    public void Highlight()
    {
        //var points = new MazePointPos[stack.Count];

        //foreach(var item in stack)
        //{
        //    byte formulathing = (byte)(points.Count / (double)stack.Count * 255.0);
        //    points.Add(new MazePointPos(item.X, item.Y, formulathing));
        //}
    }

    public void Write(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write((ushort)MazePathType.MazePath);
        writer.Write(points.Length);
        stream.Write(MemoryMarshal.AsBytes(points));
    }

    public async Task WriteAsync(Stream stream)
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
