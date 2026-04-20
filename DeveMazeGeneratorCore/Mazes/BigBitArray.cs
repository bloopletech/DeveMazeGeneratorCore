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

    private readonly BitArrayHolder[] _chunks;
    private readonly long _bitLength;

    public BigBitArray(SafeFileHandle handle, long offset)
    {
        _bitLength = RandomAccess.ReadInt64(handle, ref offset);
        _chunks = InitChunks(handle, offset);
    }

    public BigBitArray(FileStream stream) : this(stream.SafeFileHandle, stream.Position)
    {
    }

    public BigBitArray(SafeFileHandle handle, long offset, long bitLength)
    {
        _bitLength = bitLength;
        RandomAccess.Write(handle, ref offset, bitLength);

        _chunks = InitChunks(handle, offset);
        RandomAccess.EnsureLength(handle, offset, _chunks[^1].End);
    }

    public BigBitArray(FileStream stream, long bitLength) : this(stream.SafeFileHandle, stream.Position, bitLength)
    {
    }

    private BitArrayHolder[] InitChunks(SafeFileHandle handle, long offset)
    {
        var byteLength = GetByteArrayLengthFromBitLength(_bitLength);
        // TODO: Find out why BitArray casts to uint etc
        var (chunkCount, lastChunkSize) = Math.DivRem(byteLength, ChunkByteSize);

        var chunks = new BitArrayHolder[chunkCount + (lastChunkSize > 0 ? 1 : 0)];

        var chunkStart = offset;
        for(var i = 0; i < chunkCount; i++)
        {
            chunks[i] = new BitArrayHolder(handle, chunkStart, ChunkByteSize);
            chunkStart += ChunkByteSize;
        }
        if(lastChunkSize > 0) chunks[^1] = new BitArrayHolder(handle, chunkStart, lastChunkSize);

        return chunks;
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
        EnsureLoaded(chunk);
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
        EnsureLoaded(chunk);
        chunk[(int)chunkBitOffset] = value;
    }

    public long Length => _bitLength;

    private void EnsureLoaded(BitArrayHolder chunk)
    {
        if(chunk.IsPresent) return;
        var toEvict = _chunks.Where(c => c.IsPresent).OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
        chunk.Load();
    }

    public void Write()
    {
        var toEvict = _chunks.Where(c => c.IsPresent);
        foreach(var c in toEvict) c.Evict();
    }

    public async Task WriteAsync()
    {
        // TODO: Figure out some background queue writer async thingo
        Write();
    }

    public static BigBitArray Read(FileStream stream) => new BigBitArray(stream);
    public static async Task<BigBitArray> ReadAsync(FileStream stream) => new BigBitArray(stream);

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