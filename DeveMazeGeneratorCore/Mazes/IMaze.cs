using DeveMazeGeneratorCore.IO;

namespace DeveMazeGeneratorCore.Mazes;

/// <summary>
/// Info about mazes:
/// 0 = False = Wall = Black
/// 1 = True = Empty = White
/// </summary>
public interface IMaze : IBinarySerializable
{
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IMaze Clone();
}