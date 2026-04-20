using System.Buffers.Binary;
using System.Diagnostics;
using Microsoft.Win32.SafeHandles;

namespace DeveMazeGeneratorCore.Extensions;

public static class RandomAccessExtensions
{
    extension(RandomAccess)
    {
        // Based on https://stackoverflow.com/a/24412022
        public static void ReadExactly(SafeFileHandle handle, Span<byte> buffer, long fileOffset)
        {
            var offset = 0;
            while(offset < buffer.Length)
            {
                var read = RandomAccess.Read(handle, buffer.Slice(offset), fileOffset + offset);
                if(read == 0) throw new IOException("File ended prematurely");
                offset += read;
            }
            Debug.Assert(offset == buffer.Length);
        }

        // Based on https://stackoverflow.com/a/24412022
        public static async Task ReadExactlyAsync(SafeFileHandle handle, Memory<byte> buffer, long fileOffset)
        {
            var offset = 0;
            while(offset < buffer.Length)
            {
                var read = await RandomAccess.ReadAsync(handle, buffer.Slice(offset), fileOffset + offset);
                if(read == 0) throw new IOException("File ended prematurely");
                offset += read;
            }
            Debug.Assert(offset == buffer.Length);
        }

        public static int ReadInt32(SafeFileHandle handle, long fileOffset)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            RandomAccess.ReadExactly(handle, buffer, fileOffset);
            return BinaryPrimitives.ReadInt32LittleEndian(buffer);
        }

        public static int ReadInt32(SafeFileHandle handle, ref long fileOffset)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            RandomAccess.ReadExactly(handle, buffer, fileOffset);
            fileOffset += buffer.Length;
            return BinaryPrimitives.ReadInt32LittleEndian(buffer);
        }

        public static long ReadInt64(SafeFileHandle handle, long fileOffset)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            RandomAccess.ReadExactly(handle, buffer, fileOffset);
            return BinaryPrimitives.ReadInt64LittleEndian(buffer);
        }

        public static long ReadInt64(SafeFileHandle handle, ref long fileOffset)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            RandomAccess.ReadExactly(handle, buffer, fileOffset);
            fileOffset += buffer.Length;
            return BinaryPrimitives.ReadInt64LittleEndian(buffer);
        }

        public static void Write(SafeFileHandle handle, long fileOffset, int value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            RandomAccess.EnsureLength(handle, fileOffset, buffer.Length);
            RandomAccess.Write(handle, buffer, fileOffset);
        }

        public static void Write(SafeFileHandle handle, ref long fileOffset, int value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(int)];
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
            RandomAccess.EnsureLength(handle, fileOffset, buffer.Length);
            RandomAccess.Write(handle, buffer, fileOffset);
            fileOffset += buffer.Length;
        }

        public static void Write(SafeFileHandle handle, long fileOffset, long value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
            RandomAccess.EnsureLength(handle, fileOffset, buffer.Length);
            RandomAccess.Write(handle, buffer, fileOffset);
        }

        public static void Write(SafeFileHandle handle, ref long fileOffset, long value)
        {
            Span<byte> buffer = stackalloc byte[sizeof(long)];
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
            RandomAccess.EnsureLength(handle, fileOffset, buffer.Length);
            RandomAccess.Write(handle, buffer, fileOffset);
            fileOffset += buffer.Length;
        }

        public static void EnsureLength(SafeFileHandle handle, long fileOffset, long size)
        {
            var currentLength = RandomAccess.GetLength(handle);
            var requiredLength = fileOffset + size;
            if(currentLength < requiredLength) RandomAccess.SetLength(handle, requiredLength);
        }
    }
}
