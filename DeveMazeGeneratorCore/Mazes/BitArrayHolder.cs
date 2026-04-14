using System.Collections;
using System.IO.MemoryMappedFiles;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class BitArrayHolder(int bitLength, MemoryMappedFile file, long fileStartOffset, long fileChunkLength)
{
    private BitArray? array;

    public BitArray Array
    {
        get
        {
            using var stream = file.CreateViewStream(fileStartOffset, fileChunkLength);
            array ??= BitArray.ReadRaw(stream, bitLength);
            return array;
        }
    }

    public void Evict()
    {
        if(array == null) return;
        using var stream = file.CreateViewStream(fileStartOffset, fileChunkLength);
        array.WriteRaw(stream);
        array = null;
    }
}
