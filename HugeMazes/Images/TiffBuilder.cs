using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace HugeMazes.Images;

public class TiffBuilder
{
    private Dictionary<TiffTag.TagType, TiffTagValue> tags = [];

    //private byte[] FixedHeader()
    //{
    //    var magic = BitConverter.IsLittleEndian ? (byte)0x49 : (byte)0x4D;
    //    return [
    //        magic,
    //        magic,
    //        ..BitConverter.GetBytes((ushort)0x2B),
    //        ..BitConverter.GetBytes((ushort)0x08),
    //        0x00,
    //        0x00,
    //        ..BitConverter.GetBytes(16L)];
    //};

    //public void SetTag<T>(TiffTag.TagType type, T value)
    //{
    //    tags[type] = 
    //}

    public void SetTag(TiffTag.TagType type, uint value) => SetTag(type, [value]);

    public void SetTag(TiffTag.TagType type, Span<uint> values)
    {
        tags[type] = new(TiffTag.ValueType.Int, values.Length, MemoryMarshal.AsBytes(values));
    }

    public void SetTag(TiffTag.TagType type, ulong value) => SetTag(type, [value]);

    public void SetTag(TiffTag.TagType type, Span<ulong> values)
    {
        tags[type] = new(TiffTag.ValueType.Long, values.Length, MemoryMarshal.AsBytes(values));
    }

    public void SetTag(TiffTag.TagType type, ushort value) => SetTag(type, [value]);

    public void SetTag(TiffTag.TagType type, Span<ushort> values)
    {
        tags[type] = new(TiffTag.ValueType.Short, values.Length, MemoryMarshal.AsBytes(values));
    }

    public void SetTag(TiffTag.TagType type, string value)
    {
        tags[type] = new(
            TiffTag.ValueType.Ascii,
            Encoding.ASCII.GetByteCount(value) + 1,
            [..Encoding.ASCII.GetBytes(value), 0x00]);
    }

    //public int HeaderLength()
    //{
    //    16 + 8 + tags.Count + 8;
    //}

    public byte[] Build()
    {
        List<byte> result = new();



        var magic = BitConverter.IsLittleEndian ? (byte)0x49 : (byte)0x4D;
        result.AddRange([
            magic,
            magic,
            ..BitConverter.GetBytes((ushort)0x2B),
            ..BitConverter.GetBytes((ushort)0x08),
            0x00,
            0x00,
            ..BitConverter.GetBytes(16L),
            ..BitConverter.GetBytes((long)tags.Count)]);

        var pointerOffset = result.Count + (tags.Count * 20) + 8

        var tagTypes = tags.Keys.Order()


        result.AddRange(magic, magic);
        result.AddRange(BitConverter.GetBytes((ushort)0x2B));
        result.AddRange(BitConverter.GetBytes((ushort)0x08));
        result.AddRange(0x00, 0x00);

        byte[] header = 
            ..BitConverter.GetBytes(14L),
            ..new TiffTag(TiffTag.TagType.Width, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.Height, [(uint)size.Height]),
            ..new TiffTag(TiffTag.TagType.BitsPerSample, [0x08, 0x08, 0x08]),
            ..new TiffTag(TiffTag.TagType.Compression, [0x08]),
            ..new TiffTag(TiffTag.TagType.PhotometricInterpolation, [0x02]),
            ..new TiffTag(
                TiffTag.TagType.ImageDescription,
                TiffTag.ValueType.Ascii,
                mazeId.ToString().Length,
                BitConverter.GetBytes(MazeIdOffset)),
            ..new TiffTag(TiffTag.TagType.StripOffsets, [ArrayOffset]),
            ..new TiffTag(TiffTag.TagType.SamplesPerPixel, [0x03]),
            ..new TiffTag(TiffTag.TagType.RowsPerStrip, [(uint)size.Width]),
            ..new TiffTag(TiffTag.TagType.StripByteCount, [0L]),
            ..new TiffTag(TiffTag.TagType.MinimumSampleValue, [0, 0, 0]),
            ..new TiffTag(TiffTag.TagType.MaximumSampleValue, [0xFF, 0xFF, 0xFF]),
            ..new TiffTag(TiffTag.TagType.PlanarConfiguration, [0x01]),
            ..new TiffTag(TiffTag.TagType.SampleFormat, [0x01, 0x01, 0x01]),
            ..BitConverter.GetBytes(0L)
        ];
    }

    public struct TiffTagValue(TiffTag.ValueType valueType, long count, Span<byte> value)
    {
        public readonly byte[] Bytes => [
    ..BitConverter.GetBytes((ushort)type),
            ..BitConverter.GetBytes((ushort)valueType),
            ..BitConverter.GetBytes((ulong)length),
            ..value,
            ..new byte[8 - value.Length]
];
    }

    public struct TiffTagSpec<T>(TiffTag.TagType type, TiffTag.ValueType valueType, long length, T value)
    {

    }

    public readonly record struct Mapping(long Offset, byte[] Value);
}
