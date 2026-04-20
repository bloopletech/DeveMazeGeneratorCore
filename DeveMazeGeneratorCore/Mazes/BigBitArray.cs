using System.Collections;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class BigBitArray : IDisposable
{
    private const int ChunkByteSize = 256 * 1024 * 1024;
    private const int BitsPerByte = 8; // sizeof(byte) * 8

    private BitArrayHolder[] _arrays;

    private long _bitLength;
    private bool _disposed;

    //public BigBitArray(int length)
    //{
    //    ArgumentOutOfRangeException.ThrowIfNegative(length);

    //    _array = AllocateByteArray(length);
    //    _bitLength = length;
    //}

    public BigBitArray(MemoryMappedFile file, long offset)
    {
        //read own length from first 4 bytes
        using var lengthAccessor = file.CreateViewAccessor(offset, offset + sizeof(int));
        _bitLength = lengthAccessor.ReadInt32(0);

        var byteLength = GetByteArrayLengthFromBitLength(_bitLength);
        // TODO: Find out why BitArray casts to uint etc
        var (chunkCount, lastChunkSize) = Math.DivRem(byteLength, ChunkByteSize);

        _arrays = new Overlay[chunkCount + (lastChunkSize > 0 ? 1 : 0)];

        var chunkStart = offset + sizeof(int);
        for(var i = 0; i < chunkCount; i++)
        {
            _arrays[i] = new Overlay(file.CreateViewAccessor(chunkStart, ChunkByteSize));
            chunkStart += ChunkByteSize;
        }
        if(lastChunkSize > 0) _arrays[^1] = new Overlay(file.CreateViewAccessor(chunkStart, lastChunkSize));
    }

    public bool this[int index]
    {
        get => Get(index);
        set => Set(index, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Get(long index)
    {
        if((uint)index >= (uint)_bitLength)
        {
            ThrowArgumentOutOfRangeException(index);
        }

        (uint byteIndex, uint bitOffset) = Math.DivRem((uint)index, BitsPerByte);
        return (_array[byteIndex] & (1 << (int)bitOffset)) != 0;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(int index, bool value)
    {
        if((uint)index >= (uint)_bitLength)
        {
            ThrowArgumentOutOfRangeException(index);
        }

        (uint byteIndex, uint bitOffset) = Math.DivRem((uint)index, BitsPerByte);

        ref byte segment = ref _array[byteIndex];
        byte bitMask = (byte)(1 << (int)bitOffset);
        if(value)
        {
            segment |= bitMask;
        }
        else
        {
            segment &= (byte)~bitMask;
        }
    }

    public int Length => _bitLength;

    /// <summary>Creates a shallow copy of the <see cref="BitArray"/>.</summary>
    //public object Clone() => new BitArray(this);

    protected virtual void Dispose(bool disposing)
    {
        if(!_disposed)
        {
            if(disposing)
            {
                foreach(var array in _arrays) array.Dispose();
            }

            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public void Write(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write(_bitLength);
        writer.BaseStream.Write(_array);
    }

    public async Task WriteAsync(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write(_bitLength);
        await writer.BaseStream.WriteAsync(_array);
    }

    public void Write(MemoryMappedFile file, long offset)
    {
        using var accessor = file.CreateViewAccessor(offset, _array.Length + 4);
        accessor.Write(0, _bitLength);
        accessor.WriteArray(4, _array, 0, _array.Length);
    }

    public static BigBitArray Read(Stream stream)
    {
        using var reader = stream.Reader();
        var result = new BigBitArray(reader.ReadInt32());
        reader.BaseStream.ReadExactly(result._array);
        return result;
    }

    public static async Task<BigBitArray> ReadAsync(Stream stream)
    {
        using var reader = stream.Reader();
        var result = new BigBitArray(reader.ReadInt32());
        await reader.BaseStream.ReadExactlyAsync(result._array);
        return result;
    }

    public static BigBitArray Read(MemoryMappedFile file, long offset)
    {
        using var lengthAccessor = file.CreateViewAccessor(offset, 4);
        var result = new BigBitArray(lengthAccessor.ReadInt32(0));

        using var accessor = file.CreateViewAccessor(offset + 4, result._array.Length);
        accessor.ReadArray(0, result._array, 0, result._array.Length);
        return result;
    }

    /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
    private static int GetByteArrayLengthFromBitLength(int bitLength)
    {
        Debug.Assert(bitLength >= 0);
        return (int)(((uint)bitLength + 7u) >> 3);
    }

    /// <summary>Allocates a new byte array of the specified bit length, rounded up to the nearest multiple of sizeof(int).</summary>
    private static byte[] AllocateByteArray(int bitLength)
    {
        int byteLength = GetByteArrayLengthFromBitLength(bitLength);
        Debug.Assert(byteLength >= 0, "byteLength should be non-negative.");
        Debug.Assert(byteLength % sizeof(int) == 0, "byteLength should be a multiple of sizeof(int).");
        return bitLength != 0 ? new byte[byteLength] : [];
    }

    private static void ThrowArgumentOutOfRangeException(int index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");
}