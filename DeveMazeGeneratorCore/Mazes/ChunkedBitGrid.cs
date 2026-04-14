using System.Collections;
using System.Diagnostics;
using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;

namespace DeveMazeGeneratorCore.Mazes;

public class ChunkedBitGrid
{
    public readonly int width;
    public readonly int height;
    private MemoryMappedFile file;
    private long fileStartOffset;
    private int bitLength;
    private uint chunkBitSize;
    private uint lastChunkBitLength;
    private readonly BitArrayHolder[] arrays;

    //public ChunkedBitGrid(int width, int height) : this(width, height, new(width * height))
    //{
    //}

    //public ChunkedBitGrid(ChunkedBitGrid source) : this(source.width, source.height, new(source.array))
    //{
    //}

    public ChunkedBitGrid(int width, int height, MemoryMappedFile file, long fileStartOffset)
    {
        this.width = width;
        this.height = height;
        this.file = file;
        this.fileStartOffset = fileStartOffset;


        bitLength = width * height; // number of bits

        var chunkByteSize = 64 * 1024 * 1024;
        chunkBitSize = (uint)(8 * chunkByteSize); // bits

        (uint chunks, lastChunkBitLength) = Math.DivRem((uint)bitLength, chunkBitSize);
        var hasExtraChunk = lastChunkBitLength > 0;

        arrays = new BitArrayHolder[chunks + (hasExtraChunk ? 1 : 0)];
        var chunkStartOffset = fileStartOffset;
        for(var i = 0; i < chunks; i++)
        {
            arrays[i] = new BitArrayHolder((int)chunkBitSize, file, chunkStartOffset, BitArrayExtensions.GetAlignedByteArrayLength((int)chunkBitSize));
            chunkStartOffset += chunkByteSize;
        }
        if(hasExtraChunk) arrays[chunks] = new BitArrayHolder((int)lastChunkBitLength, file, chunkStartOffset, BitArrayExtensions.GetAlignedByteArrayLength((int)lastChunkBitLength));

        //if(length != array.Length)
        //{
        //    throw new ArgumentException($"(width {width} * height {height}) {length} != array length {array.Length}");
        //}


    }

    public int Width => width;
    public int Height => height;

    //public ChunkedBitGrid Clone() => new(this);
    public ChunkedBitGrid Clone() => throw new NotImplementedException();

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var index = x + (y * height);

            if((uint)index >= (uint)bitLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "must be less than length");
            }

            (uint chunk, uint chunkOffset) = Math.DivRem((uint)index, chunkBitSize);
            return arrays[chunk].Array[(int)chunkOffset];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var index = x + (y * height);

            if((uint)index >= (uint)bitLength)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, "must be less than length");
            }

            (uint chunk, uint chunkOffset) = Math.DivRem((uint)index, chunkBitSize);
            arrays[chunk].Array[(int)chunkOffset] = value;
        }
    }

    public void Write(Stream stream)
    {
        using var compressor = stream.Compressor();
        WriteHeader(compressor);
        array.Write(compressor);
    }

    public async Task WriteAsync(Stream stream)
    {
        using var compressor = stream.Compressor();
        WriteHeader(compressor);
        await array.WriteAsync(compressor);
    }

    private void WriteHeader(Stream stream)
    {
        using var writer = stream.Writer();
        writer.Write(width);
        writer.Write(height);
    }

    public static ChunkedBitGrid Read(Stream stream)
    {
        using var decompressor = stream.Decompressor();
        using var reader = decompressor.Reader();
        return new ChunkedBitGrid(reader.ReadInt32(), reader.ReadInt32(), BitArray.Read(decompressor));
    }

    public static async Task<ChunkedBitGrid> ReadAsync(Stream stream)
    {
        using var decompressor = stream.Decompressor();
        using var reader = decompressor.Reader();
        return new ChunkedBitGrid(reader.ReadInt32(), reader.ReadInt32(), await BitArray.ReadAsync(decompressor));
    }



}
