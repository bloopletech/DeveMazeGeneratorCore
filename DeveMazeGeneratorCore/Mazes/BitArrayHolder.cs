using System.Collections;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Mazes;

public class BitArrayHolder(SafeFileHandle handle, long offset, long size)
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

    public bool this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => Array[index];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => Array[index] = value;
    }

    public bool IsEmpty => array == null;
    public bool IsPresent => array != null;

    public long LastUsedAt { get; set; } = long.MaxValue;

    public void Load()
    {
        if(array != null) return;
        array = BitArray.Read(handle, offset, size);
    }

    public void Save()
    {
        if(array == null) return;
        array.Write(handle, offset, size);
        array = null;
        LastUsedAt = long.MaxValue;
    }
}
