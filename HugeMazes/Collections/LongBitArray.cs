using HugeMazes.Extensions;
using HugeMazes.IO;
using System.Collections;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static HugeMazes.Structures.RenderPalette;

namespace HugeMazes.Collections;

// Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs
public class LongBitArray : Storable, ILongBitArray
{
    private const int ChunkSize = 0x40000000; // (2 ^ 30)

    private Chunk[] chunks = null!;
    private long length;

    public LongBitArray(IStore store, bool leaveOpen = false) : base(store, leaveOpen)
    {
        InitChunks(false);
    }

    public LongBitArray(IStore store, long length, bool leaveOpen = false) : base(store, leaveOpen)
    {
        this.length = length;
        InitChunks(false);
    }

    public override long Extent => chunks[^1].EndOffset;
    public long Length => length;
    public int ChunkCount => chunks.Length;
    public bool IsReadOnly => false;
    public bool IsFixedSize => true;

    public bool this[long index]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var (chunkIndex, chunkOffset) = Index(index);
            return chunks[chunkIndex].Array[chunkOffset];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var (chunkIndex, chunkOffset) = Index(index);
            chunks[chunkIndex].Array[chunkOffset] = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static (int, int) Index(long index)
    {
        var (chunk, chunkOffset) = Math.DivRem((ulong)index, ChunkSize);
        return ((int)chunk, (int)chunkOffset);
    }

    private ChunkRange[] RangeIndexes(long start, long length)
    {
        var end = start + length;
        var ranges = new List<ChunkRange>();

        for(var i = 0; i < chunks.Length; i++)
        {
            var chunk = chunks[i];
            if(start <= chunk.End && end >= chunk.Start)
            {
                var chunkStartIndex = Math.Min(0, start - chunk.Start);
                var chunkEndIndex = Math.Max(chunk.Count, end - chunk.Start);
                var chunkLength = chunkEndIndex - chunkStartIndex;
                ranges.Add(new ChunkRange(i, (int)chunkStartIndex, (int)chunkLength));
            }
        }

        return [..ranges];
    }

    public ChunkBitSpan GetChunkSpan(int index) => new(chunks[index]);

    public ChunkBitSpan[] GetChunkSpans(long start, long length)
    {
        var ranges = RangeIndexes(start, length);
        return [..ranges.Select((range) => chunks[range.ChunkIndex].AsSpan(range.Start, range.Length))];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        foreach(var chunk in chunks) chunk.Array.SetAll(false);
    }

    public IEnumerator<bool> GetEnumerator()
    {
        foreach(var chunk in chunks)
        {
            for(var i = 0; i < chunk.Array.Length; i++) yield return chunk.Array[i];
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public bool Peek() => this[length - 1];

    private void InitChunks(bool read)
    {
        chunks = [..Chunk.Produce(this, length, ChunkSize, sizeof(long), read)];
    }

    public void EvictOldest()
    {
        var toEvict = chunks.OrderByDescending(c => c.LastUsedAt).Skip(3);
        foreach(var c in toEvict) c.Evict();
    }

    public override void Read()
    {
        length = store.Read<long>(0);
        InitChunks(true);
    }

    public override void Write()
    {
        store.Write(0, length);
        foreach(var chunk in chunks) chunk.Evict();
    }

    public ILongBitArray Clone() => Clone(IStore.Create(IsLong));

    public ILongBitArray Clone(IStore destination, bool leaveOpen = false)
    {
        Write();
        store.CopyTo(destination);
        var result = new LongBitArray(destination, leaveOpen);
        result.Read();
        return result;
    }

    public class Chunk(LongBitArray owner, long start, int count, long offset, bool read)
    {
        private BitArray? array;
        public BitArray Array => array ??= Load();

        public long LastUsedAt { get; private set; }

        public long Start => start;
        public int Count => count;
        public long End => start + count;
        public long EndOffset => offset + count.DivCeil(8);

        private BitArray Load()
        {
            LastUsedAt = Environment.TickCount64;
            owner.EvictOldest();

            var array = new BitArray(count);
            if(read) owner.Store.ReadExactly(offset, CollectionsMarshal.AsBytes(array));
            else read = true;
            return array;
        }

        public void Evict()
        {
            if(array == null) return;

            owner.Store.Write(offset, CollectionsMarshal.AsBytes(array));
            array = null;
            LastUsedAt = 0;
        }

        public ChunkBitSpan AsSpan() => new(this);
        public ChunkBitSpan AsSpan(int start) => new(this, start);
        public ChunkBitSpan AsSpan(int start, int length) => new(this, start, length);

        public static IEnumerable<Chunk> Produce(LongBitArray owner, long count, int chunkSize, long offset, bool read)
        {
            if(count == 0)
            {
                yield return new(owner, 0, 0, offset, read);
                yield break;
            }

            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)chunkSize, (uint)System.Array.MaxLength);
            ArgumentOutOfRangeException.ThrowIfGreaterThan((uint)count.DivCeil(chunkSize), (uint)System.Array.MaxLength);

            var chunkByteSize = chunkSize.DivCeil(8);
            for(long start = 0, i = 0; start < count; i++)
            {
                var stride = (int)Math.Min(chunkSize, count - start);
                var chunk = new Chunk(owner, start, stride, (i * chunkByteSize) + offset, read);
                var _ = checked(chunk.EndOffset);
                yield return chunk;
                start += stride;
            }
        }
    }

    public readonly record struct ChunkRange(int ChunkIndex, int Start, int Length);

    // Based on https://github.com/dotnet/runtime/blob/691fd960eb500743b4be71373b11b2263bdbc318/src/libraries/System.Private.CoreLib/src/System/Span.cs
    public struct ChunkBitSpan : IEnumerable<bool>
    {
        private readonly Chunk _chunk;
        private readonly int _start;
        private readonly int _length;

        public ChunkBitSpan(Chunk chunk)
        {
            _chunk = chunk;
            _start = 0;
            _length = chunk.Count;
        }

        public ChunkBitSpan(Chunk chunk, int start)
        {
            if((uint)start > (uint)chunk.Count)
            {
                throw new ArgumentOutOfRangeException(null, "0 <= start <= chunk.Count");
            }

            _chunk = chunk;
            _start = start;
            _length = chunk.Count - start;
        }

        public ChunkBitSpan(Chunk chunk, int start, int length)
        {
            if((uint)start + (uint)length > (uint)chunk.Count)
            {
                throw new ArgumentOutOfRangeException(null, "0 <= start <= length <= chunk.Count");
            }

            _chunk = chunk;
            _start = start;
            _length = length;
        }

        public bool this[int index]
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => _chunk.Array[index + _start];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => _chunk.Array[index + _start] = value;
        }

        public int Length => _length;
        public bool IsEmpty => _length == 0;

        public IEnumerator<bool> GetEnumerator()
        {
            for(var i = 0; i < _length; i++) yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public ChunkBitSpan Slice(int start)
        {
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start, _length);
            return new(_chunk, _start + start, _length - start);
        }

        public ChunkBitSpan Slice(int start, int length)
        {
            if((uint)start + (uint)length > (uint)_length)
            {
                throw new ArgumentOutOfRangeException(null, "start <= length <= Length");
            }

            return new(_chunk, _start + start, length);
        }
    }

    public class LongBitSpan


    /// <summary>Gets an enumerator for this span.</summary>
    public Enumerator GetEnumerator() => new Enumerator(this);

    public struct ChunkSliceEnumerator : IEnumerator<T>
    {
        /// <summary>The span being enumerated.</summary>
        private readonly Span<T> _span;
        /// <summary>The next index to yield.</summary>
        private int _index;

        /// <summary>Initialize the enumerator.</summary>
        /// <param name="span">The span to enumerate.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Enumerator(Span<T> span)
        {
            _span = span;
            _index = -1;
        }

        /// <summary>Advances the enumerator to the next element of the span.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            int index = _index + 1;
            if(index < _span.Length)
            {
                _index = index;
                return true;
            }

            return false;
        }

        /// <summary>Gets the element at the current position of the enumerator.</summary>
        public Span< Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref _span[_index];
        }

        /// <inheritdoc />
        T IEnumerator<T>.Current => Current;

        /// <inheritdoc />
        object IEnumerator.Current => Current!;

        /// <inheritdoc />
        void IEnumerator.Reset() => _index = -1;

        /// <inheritdoc />
        void IDisposable.Dispose() { }
    }
}
