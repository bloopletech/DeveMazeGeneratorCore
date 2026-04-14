using System.Collections;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Extensions;

public static class BitArrayExtensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    private extern static ref byte[] GetArrayField(BitArray @this);

    extension(BitArray array)
    {
        private ref byte[] GetArray() => ref GetArrayField(array);

        public int ByteLength() => array.GetArray().Length;

        public void Write(Stream stream)
        {
            using var writer = stream.Writer();
            writer.Write(array.Length);
            writer.BaseStream.Write(array.GetArray());
        }

        public async Task WriteAsync(Stream stream)
        {
            using var writer = stream.Writer();
            writer.Write(array.Length);
            await writer.BaseStream.WriteAsync(array.GetArray());
        }

        public void Write(SafeFileHandle handle, long offset)
        {
            RandomAccess.Write(handle, array.GetArray(), offset);
        }

        public async Task WriteAsync(SafeFileHandle handle, long offset)
        {
            await RandomAccess.WriteAsync(handle, array.GetArray(), offset);
        }

        public static BitArray Read(Stream stream)
        {
            using var reader = stream.Reader();
            var result = new BitArray(reader.ReadInt32());
            reader.BaseStream.ReadExactly(result.GetArray());
            return result;
        }

        public static async Task<BitArray> ReadAsync(Stream stream)
        {
            using var reader = stream.Reader();
            var result = new BitArray(reader.ReadInt32());
            await reader.BaseStream.ReadExactlyAsync(result.GetArray());
            return result;
        }

        public static BitArray Read(int bitLength, SafeFileHandle handle, long offset)
        {
            var result = new BitArray(bitLength);
            RandomAccess.ReadExactly(handle, result.GetArray(), offset);
            return result;
        }

        public static async Task<BitArray> ReadAsync(int bitLength, SafeFileHandle handle, long offset)
        {
            var result = new BitArray(bitLength);
            await RandomAccess.ReadExactlyAsync(handle, result.GetArray(), offset);
            return result;
        }

        // Based on https://github.com/dotnet/runtime/blob/081d220c0a773ffb7c6bea6b48727833576a65ef/src/libraries/System.Private.CoreLib/src/System/Collections/BitArray.cs#L938

        /// <summary>Determines the number of <see cref="byte"/>s required to store <paramref name="bitLength"/> bits.</summary>
        private static int GetByteArrayLengthFromBitLength(int bitLength)
        {
            Debug.Assert(bitLength >= 0);
            return (int)(((uint)bitLength + 7u) >> 3);
        }

        /// <summary>Rounds <paramref name="value"/> up to a multiple of sizeof(int).</summary>
        private static int RoundUpToMultipleSizeOfInt32(int value) =>
            (value + (sizeof(int) - 1)) & ~(sizeof(int) - 1);

        public static int GetAlignedByteArrayLength(int bitLength) =>
            // Always allocate in groups of sizeof(int) bytes so that we can use MemoryMarshal.Cast<byte, int>
            // to manipulate as ints when desired.
            RoundUpToMultipleSizeOfInt32(GetByteArrayLengthFromBitLength(bitLength));
    }
}
