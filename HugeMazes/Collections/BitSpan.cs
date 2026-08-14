using System.Collections;
using System.Runtime.CompilerServices;

namespace HugeMazes.Collections;

// Based on https://github.com/dotnet/runtime/blob/691fd960eb500743b4be71373b11b2263bdbc318/src/libraries/System.Private.CoreLib/src/System/Span.cs
public readonly struct BitSpan : IEnumerable<bool>
{
    private readonly BitArray _array;
    private readonly int _start;
    private readonly int _length;

    public BitSpan(BitArray array)
    {
        _array = array;
        _start = 0;
        _length = array.Length;
    }

    public BitSpan(BitArray array, int start)
    {
        if((uint)start > (uint)array.Length)
        {
            throw new ArgumentOutOfRangeException(null, "0 <= start <= array.Length");
        }

        _array = array;
        _start = start;
        _length = array.Length - start;
    }

    public BitSpan(BitArray array, int start, int length)
    {
        if((uint)start + (uint)length > (uint)array.Length)
        {
            throw new ArgumentOutOfRangeException(null, "0 <= start <= length <= array.Length");
        }

        _array = array;
        _start = start;
        _length = length;
    }

    public bool this[int index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _array[index + _start];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => _array[index + _start] = value;
    }

    public int Length => _length;
    public bool IsEmpty => _length == 0;

    public IEnumerator<bool> GetEnumerator()
    {
        for(var i = 0; i < _length; i++) yield return this[i];
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public BitSpan Slice(int start)
    {
        ArgumentOutOfRangeException.ThrowIfGreaterThan(start, _length);
        return new(_array, _start + start, _length - start);
    }

    public BitSpan Slice(int start, int length)
    {
        if((uint)start + (uint)length > (uint)_length)
        {
            throw new ArgumentOutOfRangeException(null, "start <= length <= Length");
        }

        return new(_array, _start + start, length);
    }
}