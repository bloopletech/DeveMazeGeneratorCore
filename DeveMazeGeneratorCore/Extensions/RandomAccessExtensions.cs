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
    }
}
