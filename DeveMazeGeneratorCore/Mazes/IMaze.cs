using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

/// <summary>
/// Info about mazes:
/// 0 = False = Wall = Black
/// 1 = True = Empty = White
/// </summary>
public interface IMaze
{
    Stream Stream { get; }
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IMaze Clone();

    void Read();
    Task ReadAsync();
    void Write();
    Task WriteAsync();
}