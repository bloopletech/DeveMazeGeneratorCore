namespace DeveMazeGeneratorCore.Paths;

public interface IMazePath
{
    Stream Stream { get; }
    int Height { get; }
    int Width { get; }

    bool this[int x, int y] { get; set; }

    IMazePath Clone();
    void Highlight();

    void Read();
    Task ReadAsync();
    void Write();
    Task WriteAsync();
}