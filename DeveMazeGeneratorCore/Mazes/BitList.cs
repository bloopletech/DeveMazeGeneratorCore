using System.Collections;
using System.Runtime.CompilerServices;

namespace DeveMazeGeneratorCore.Mazes;

public class BitList
{
    private const int ChunkSize = 4096;

    private readonly BitArray array;

    public BitList(int bitLength) : this(new BitArray(bitLength))
    {
    }

    public BitList(BitList source) : this(new BitArray(source.array))
    {
    }

    private BitList(BitArray array)
    {
        this.array = array;
    }

    public bool this[int i]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => array[i];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set => array[i] = value;
    }


    public async Task Write(BinaryWriter writer)
    {
        writer.Write(array.Length);
        await writer.BaseStream.WriteAsync(GetArrayField(array));

        //var rawArray = GetRawArray();
        //await writer.BaseStream.WriteAsync(rawArray);
        //for (var offset = 0; offset < rawArray.Length; offset += ChunkSize)
        //{
        //    var length = Math.Min(ChunkSize, rawArray.Length - 1 - offset);
        //    await writer.BaseStream.WriteAsync(rawArray.AsMemory(offset, length));
        //}
    }

    public static async Task<BitList> Read(BinaryReader reader)
    {
        var array = new BitArray(reader.ReadInt32());

        var rawArray = GetArrayField(array);
        for(var offset = 0; offset < rawArray.Length;)
        {
            var length = Math.Min(ChunkSize, rawArray.Length - 1 - offset);
            if(length == 0) break;
            var bytesRead = await reader.BaseStream.ReadAsync(rawArray.AsMemory(offset, length));
            //if(bytesRead == 0) throw new InvalidOperationException("Stream ended too early");
            offset += bytesRead;
        }

        return new BitList(array);
    }

    //private ref byte[] GetRawArray() => ref GetArrayField(array);

    [UnsafeAccessor(UnsafeAccessorKind.Field, Name = "_array")]
    private extern static ref byte[] GetArrayField(BitArray @this);
}
