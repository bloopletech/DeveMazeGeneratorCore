namespace DeveMazeGeneratorCore.IO;

public interface IBinarySerializable
{
    IBinarySerializer Serializer { get; }

    void Read();
    Task ReadAsync();
    void Write();
    Task WriteAsync();
}
