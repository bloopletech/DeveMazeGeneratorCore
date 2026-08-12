using System.Runtime.InteropServices;
using HugeMazes.Extensions;

namespace HugeMazes.Images;

public class TiffBuilder
{
    public static readonly long HeaderLength = 16;

    private readonly Dictionary<Tiff.TagType, TiffTag> tags = [];

    public long DirectoryLength => (tags.Count * 20) + 16;
    public long HeapLength => tags.Values.Where(t => t.IsPointer).Sum(t => t.Value.Length);

    public long Length => HeaderLength + DirectoryLength + HeapLength;

    public void Set(Tiff.TagType type, uint value) => Set(type, [value]);

    public void Set(Tiff.TagType type, uint[] values)
    {
        Set(type, Tiff.ValueType.Int, values);
        //Set(type, Tiff.ValueType.Int, values.Length, [..MemoryMarshal.AsBytes(values)]);
    }

    public void Set(Tiff.TagType type, ulong value) => Set(type, [value]);

    public void Set(Tiff.TagType type, ulong[] values)
    {
        Set(type, Tiff.ValueType.Long, values);
        //Set(type, Tiff.ValueType.Long, values.Length, [..MemoryMarshal.AsBytes(values)]);
    }

    public void Set(Tiff.TagType type, ushort value) => Set(type, [value]);

    public void Set(Tiff.TagType type, ushort[] values)
    {
        Set(type, Tiff.ValueType.Short, values);
        //Set(type, Tiff.ValueType.Short, values.Length, [..MemoryMarshal.AsBytes(values)]);
    }

    public void Set(Tiff.TagType type, string value)
    {
        var bytes = Tiff.GetAsciiBytes(value);
        Set(type, Tiff.ValueType.Ascii, bytes.Length - 1, bytes);
    }

    private void Set(Tiff.TagType type, Tiff.ValueType valueType, int length, byte[] values)
    {
        tags[type] = new TiffTag(type, valueType, length, values);
    }

    private void Set<T>(Tiff.TagType type, Tiff.ValueType valueType, T[] values) where T : struct
    {
        tags[type] = new TiffTag(type, valueType, values.Length, [..MemoryMarshal.AsBytes(values)]);
    }

    public byte[] Build()
    {
        var orderedTags = tags.OrderBy(t => t.Key).Select(t => t.Value);

        var heap = new List<byte>();
        var finalTags = orderedTags.Select(t =>
        {
            if(t.IsPointer)
            {
                var offset = HeaderLength + DirectoryLength + heap.Count;
                heap.AddRange(t.Value);
                return t with { Value = BitConverter.GetBytes(offset) };
            }
            return t;
        });

        var magic = BitConverter.IsLittleEndian ? (byte)0x49 : (byte)0x4D;
        return [
            magic,
            magic,
            ..BitConverter.GetBytes((ushort)0x2B),
            ..BitConverter.GetBytes((ushort)0x08),
            0x00,
            0x00,
            ..BitConverter.GetBytes(16L),
            ..BitConverter.GetBytes((ulong)tags.Count),
            ..finalTags.Select(t => t.Bytes).SelectMany(t => t),
            ..BitConverter.GetBytes(0L),
            ..heap
        ];
    }

    private record struct TiffTag(Tiff.TagType Type, Tiff.ValueType ValueType, long Length, byte[] Value)
    {
        public readonly bool IsPointer => Value.Length > 8;
        public readonly byte[] Bytes => [
            ..BitConverter.GetBytes((ushort)Type),
            ..BitConverter.GetBytes((ushort)ValueType),
            ..BitConverter.GetBytes((ulong)Length),
            ..Value.Extend(8)];
    }
}
