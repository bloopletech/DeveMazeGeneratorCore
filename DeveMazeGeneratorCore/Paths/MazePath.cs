using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class MazePath(IBinarySerializer serializer, MazePoint[] points) : IBinarySerializable
{
    public MazePath(IBinarySerializer serializer) : this(serializer, [])
    {
    }

    public IBinarySerializer Serializer => serializer;
    public MazePoint[] Points => points;

    public IMazePath Clone() => throw new NotImplementedException();

    public void Read()
    {
        points = serializer.ReadArray<MazePoint>();
    }

    public async Task ReadAsync()
    {
        points = await serializer.ReadArrayAsync<MazePoint>();
    }

    public void Write()
    {
        serializer.WriteArray(points);
    }

    public async Task WriteAsync()
    {
        await serializer.WriteArrayAsync<MazePoint>(points);
    }
}
