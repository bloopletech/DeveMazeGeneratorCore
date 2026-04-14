using System.Collections;
using DeveMazeGeneratorCore.Extensions;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Mazes;

public class BitArrayHolder(int bitLength, SafeFileHandle handle, long fileOffset)
{
    private BitArray? array;

    public BitArray Array
    {
        get
        {
            array ??= BitArray.Read(bitLength, handle, fileOffset);
            return array;
        }
    }

    public bool IsEmpty => array == null;

    public long LastUsedAt { get; set; }

    public void Evict()
    {
        if(array == null) return;
        var currentLength = RandomAccess.GetLength(handle);
        var requiredLength = fileOffset + array.ByteLength();
        if(currentLength < requiredLength) RandomAccess.SetLength(handle, requiredLength);
        array.Write(handle, fileOffset);
        array = null;
        LastUsedAt = long.MaxValue;
    }
}
