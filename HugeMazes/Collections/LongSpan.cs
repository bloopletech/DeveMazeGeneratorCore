using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using static HugeMazes.Collections.LongBitArray;

namespace HugeMazes.Collections;

public struct LongSpan<T> : IEnumerable<T> where T : struct
{
    private readonly Chunk _chunk;
    private readonly int _start;
    private readonly int _length;

    private LongSpan(Chunk chunk)
    {
        _chunk = chunk;
        _start = 0;
        _length = chunk.Count;
    }

    private LongSpan(Chunk chunk, int start)
    {
        if((uint)start > (uint)chunk.Count)
        {
            throw new ArgumentOutOfRangeException(null, "0 <= start <= chunk.Count");
        }

        _chunk = chunk;
        _start = start;
        _length = chunk.Count - start;
    }

    private LongSpan(Chunk chunk, int start, int length)
    {
        if((uint)start + (uint)length > (uint)chunk.Count)
        {
            throw new ArgumentOutOfRangeException(null, "0 <= start <= length <= chunk.Count");
        }

        _chunk = chunk;
        _start = start;
        _length = length;
    }

    public T this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _chunk.Array[index + _start];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _chunk.Array[index + _start] = value;
    }

    public long Length => _length;
    public bool IsEmpty => _length == 0;

    public IEnumerator<T> GetEnumerator()
    {
        for(var i = 0; i < _length; i++) yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public LongSpan<T> Slice(int start)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, _length);
        return new(_chunk, _start + start, _length - start);
    }

    public LongSpan<T> Slice(int start, int length)
    {
        if((uint)start + (uint)length > (uint)_length)
        {
            throw new ArgumentOutOfRangeException(null, "start <= length <= Length");
        }

        return new(_chunk, _start + start, length);
    }
}
