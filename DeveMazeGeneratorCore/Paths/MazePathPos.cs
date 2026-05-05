using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Structures;

namespace DeveMazeGeneratorCore.Paths;

public class MazePathPos(IBinarySerializer serializer, MazePointPos[] points)
{
    public MazePathPos(IBinarySerializer serializer) : this(serializer, [])
    {
    }

    public MazePointPos[] Points => points;

    public void Read()
    {
        points = serializer.ReadArray<MazePointPos>();
    }

    public async Task ReadAsync()
    {
        points = await serializer.ReadArrayAsync<MazePointPos>();
    }

    public void Write()
    {
        serializer.WriteArray(points);
    }

    public async Task WriteAsync()
    {
        await serializer.WriteArrayAsync<MazePointPos>(points);
    }
}
