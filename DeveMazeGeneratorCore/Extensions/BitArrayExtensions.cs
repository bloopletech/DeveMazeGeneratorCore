using System.Collections;
//using System.IO.MemoryMappedFiles;
using System.Runtime.CompilerServices;
namespace DeveMazeGeneratorCore.Extensions;

public static class BitArrayExtensions
{
    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    private extern static ref byte[] GetArrayField(BitArray @this);

    extension(BitArray array)
    {
        public ref byte[] GetArray() => ref GetArrayField(array);

        //public void Write(MemoryMappedFile file, long offset)
        //{
        //    var rawArray = array.GetArray();
        //    using var accessor = file.CreateViewAccessor(offset, rawArray.Length + 4);
        //    accessor.Write(0, array.Length);
        //    accessor.WriteArray(4, rawArray, 0, rawArray.Length);
        //}

        //public static BitArray Read(MemoryMappedFile file, long offset)
        //{
        //    using var lengthAccessor = file.CreateViewAccessor(offset, 4);
        //    var result = new BitArray(lengthAccessor.ReadInt32(0));
        //    var rawArray = result.GetArray();

        //    using var accessor = file.CreateViewAccessor(offset + 4, rawArray.Length);
        //    accessor.ReadArray(0, rawArray, 0, rawArray.Length);
        //    return result;
        //}
    }
}
