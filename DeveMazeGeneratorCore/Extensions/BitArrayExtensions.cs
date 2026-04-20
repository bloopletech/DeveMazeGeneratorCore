using System.Collections;
//using System.IO.MemoryMappedFiles;
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

        //public void Write(MemoryMappedFile file, long offset)
        //{
        //    var rawArray = array.GetArray();
        //    using var accessor = file.CreateViewAccessor(offset, rawArray.Length + 4);
        //    accessor.Write(0, array.Length);
        //    accessor.WriteArray(4, rawArray, 0, rawArray.Length);
        //}

        public void Write(SafeFileHandle handle, long offset, long size)
        {
            var rawArray = array.GetArray();
            RandomAccess.Write(handle, rawArray, offset);
        }

        public async Task WriteAsync(SafeFileHandle handle, long offset, long size)
        {
            var rawArray = array.GetArray();
            await RandomAccess.WriteAsync(handle, rawArray, offset);
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

        //public static BitArray Read(MemoryMappedFile file, long offset)
        //{
        //    using var lengthAccessor = file.CreateViewAccessor(offset, 4);
        //    var result = new BitArray(lengthAccessor.ReadInt32(0));
        //    var rawArray = result.GetArray();

        //    using var accessor = file.CreateViewAccessor(offset + 4, rawArray.Length);
        //    accessor.ReadArray(0, rawArray, 0, rawArray.Length);
        //    return result;
        //}

        public static BitArray Read(SafeFileHandle handle, long offset, long size)
        {
            var result = new BitArray((int)size * 8);
            var rawArray = result.GetArray();
            RandomAccess.ReadExactly(handle, rawArray, offset);
            return result;
        }

        public static async Task<BitArray> ReadAsync(SafeFileHandle handle, long offset, long size)
        {
            var result = new BitArray((int)size * 8);
            var rawArray = result.GetArray();
            await RandomAccess.ReadExactlyAsync(handle, rawArray, offset);
            return result;
        }
    }
}
