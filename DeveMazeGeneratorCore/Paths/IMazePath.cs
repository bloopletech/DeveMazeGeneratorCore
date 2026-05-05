using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Paths;

public interface IMazePath : IBinarySerializable
{
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IMazePath Clone();
}