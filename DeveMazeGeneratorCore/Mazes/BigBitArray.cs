using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Mazes;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class BigBitArray
{
    private const int ChunkByteSize = 256 * 1024 * 1024;
    private const int BitsPerByte = 8; // sizeof(byte) * 8

    private BitArrayHolder[] _chunks;
    private long _bitLength;
    private long _offset;

    //public BigBitArray(int length)
    //{
    //    ArgumentOutOfRangeException.ThrowIfNegative(length);

    //    _array = AllocateByteArray(length);
    //    _bitLength = length;
    //}

    public BigBitArray(SafeFileHandle handle, long offset)
    {
        _offset = offset;
        //read the bitlength from the handle
        _bitLength = bitLength;

        var byteLength = GetByteArrayLengthFromBitLength(_bitLength);
        // TODO: Find out why BitArray casts to uint etc
        var (chunkCount, lastChunkSize) = Math.DivRem(byteLength, ChunkByteSize);

        _chunks = new BitArrayHolder[chunkCount + (lastChunkSize > 0 ? 1 : 0)];

        var chunkStart = offset;
        for(var i = 0; i < chunkCount; i++)
        {
            _chunks[i] = new BitArrayHolder(handle, chunkStart, ChunkByteSize);
            chunkStart += ChunkByteSize;
        }
        if(lastChunkSize > 0) _chunks[^1] = new BitArrayHolder(handle, chunkStart, lastChunkSize);

        var currentLength = RandomAccess.GetLength(handle);
        var requiredLength = offset + byteLength;
        if(currentLength < requiredLength) RandomAccess.SetLength(handle, requiredLength);
    }

    public BigBitArray(SafeFileHandle handle, long offset, long bitLength)
    {
        _offset = offset;
        //write the bitlength to the handle
        _bitLength = bitLength;

        var byteLength = GetByteArrayLengthFromBitLength(_bitLength);
        // TODO: Find out why BitArray casts to uint etc
        var (chunkCount, lastChunkSize) = Math.DivRem(byteLength, ChunkByteSize);

        _chunks = new BitArrayHolder[chunkCount + (lastChunkSize > 0 ? 1 : 0)];

        var chunkStart = offset;
        for(var i = 0; i < chunkCount; i++)
        {
            _chunks[i] = new BitArrayHolder(handle, chunkStart, ChunkByteSize);
            chunkStart += ChunkByteSize;
        }
        if(lastChunkSize > 0) _chunks[^1] = new BitArrayHolder(handle, chunkStart, lastChunkSize);

        var currentLength = RandomAccess.GetLength(handle);
        var requiredLength = offset + byteLength;
        if(currentLength < requiredLength) RandomAccess.SetLength(handle, requiredLength);
    }

    public bool this[long index]
    {
        get => Get(index);
        set => Set(index, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Get(long index)
    {
        if((ulong)index >= (ulong)_bitLength)
        {
            ThrowArgumentOutOfRangeException(index);
        }

        var (byteIndex, bitOffset) = Math.DivRem((ulong)index, BitsPerByte);
        var (chunksIndex, chunkOffset) = Math.DivRem(byteIndex, ChunkByteSize);
        var chunkBitOffset = (chunkOffset * BitsPerByte) + bitOffset;

        var chunk = _chunks[chunksIndex];
        Sync(chunk);
        return chunk[(int)chunkBitOffset];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Set(long index, bool value)
    {
        if((ulong)index >= (ulong)_bitLength)
        {
            ThrowArgumentOutOfRangeException(index);
        }

        var (byteIndex, bitOffset) = Math.DivRem((ulong)index, BitsPerByte);
        var (chunksIndex, chunkOffset) = Math.DivRem(byteIndex, ChunkByteSize);
        var chunkBitOffset = (chunkOffset * BitsPerByte) + bitOffset;

        var chunk = _chunks[chunksIndex];
        Sync(chunk);
        chunk[(int)chunkBitOffset] = value;
    }

    public long Length => _bitLength;

    /// <summary>Creates a shallow copy of the <see cref="BitArray"/>.</summary>
    //public object Clone() => new BitArray(this);

    private void Sync(BitArrayHolder chunk)
    {
        chunk.LastUsedAt = Environment.TickCount64;
        if(chunk.IsPresent) return;
        do
        {
            var inUse = _chunks.Where(c => c.IsPresent);
            if(inUse.Count() < 10) break;

            var oldest = inUse.OrderBy(c => c.LastUsedAt);
            oldest.First().Save();
        }
        while(true);
        chunk.Load();
    }

    public void Write()
    {
        foreach(var chunk in _chunks) if(chunk.IsPresent) chunk.Save();
    }

    public async Task WriteAsync()
    {
        foreach(var chunk in _chunks) if(chunk.IsPresent) chunk.Save();
    }

    public static BigBitArray Read(FileStream stream)
    {
        return new BigBitArray(stream.SafeFileHandle, stream.Position);
    }

    public static async Task<BigBitArray> ReadAsync(FileStream stream)
    {
        return new BigBitArray(stream.SafeFileHandle, stream.Position);
    }

    /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
    private static long GetByteArrayLengthFromBitLength(long bitLength)
    {
        Debug.Assert(bitLength >= 0);
        return (long)(((ulong)bitLength + 7u) >> 3);
    }

    private static void ThrowArgumentOutOfRangeException(long index) => throw new ArgumentOutOfRangeException(
        nameof(index),
        index,
        "Index was out of range. Must be non-negative and less than the size of the collection");
}