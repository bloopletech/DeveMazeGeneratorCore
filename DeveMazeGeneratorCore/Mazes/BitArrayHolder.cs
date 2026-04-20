using System.Collections;
using DeveMazeGeneratorCore.Extensions;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Mazes;

public class BitArrayHolder(SafeFileHandle handle, long offset, long size, int bitLength)
{
    private BitArray? array;

    public BitArray Array
    {
        get
        {
            Load();
            return array!;
        }
    }

    public bool IsEmpty => array == null;

    public long LastUsedAt { get; set; } = long.MaxValue;

    public void Load()
    {
        if(array != null) return;
        array = BitArray.Read(handle, offset, size, bitLength);
    }

    public void Save()
    {
        if(array == null) return;
        var currentLength = RandomAccess.GetLength(handle);
        var requiredLength = offset + array.ByteLength();
        if(currentLength < requiredLength) RandomAccess.SetLength(handle, requiredLength);
        array.Write(handle, fileOffset);
        array = null;
        LastUsedAt = long.MaxValue;
    }
}
