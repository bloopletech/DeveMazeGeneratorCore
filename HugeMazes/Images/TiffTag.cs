using System.Runtime.InteropServices;
using System.Text;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public struct TiffTag(TiffTag.TagType type, TiffTag.ValueType valueType, long count, byte[] value)
{
    public const int Length = 20;

    public readonly byte[] Value => value;

    public readonly bool IsPointer => value.Length > 8;

    public readonly byte[] Bytes => [
        ..BitConverter.GetBytes((ushort)type),
        ..BitConverter.GetBytes((ushort)valueType),
        ..BitConverter.GetBytes((ulong)count),
        ..ValueBytes,
        ..new byte[8 - ValueBytes.Length]
    ];

    private readonly byte[] ValueBytes => IsPointer ? [] : value;

    public readonly IEnumerator<byte> GetEnumerator() => Bytes.AsEnumerable().GetEnumerator();

    public enum TagType : ushort
    {
        Width = 0x100,
        Height = 0x101,
        BitsPerSample = 0x102,
        Compression = 0x103,
        PhotometricInterpolation = 0x106,
        ImageDescription = 0x10E,
        StripOffsets = 0x111,
        Orientation = 0x112,
        SamplesPerPixel = 0x115,
        RowsPerStrip = 0x116,
        StripByteCount = 0x117,
        MinimumSampleValue = 0x118,
        MaximumSampleValue = 0x119,
        PlanarConfiguration = 0x11C,
        Software = 0x131,
        ColorMap = 0x140,
        SampleFormat = 0x153
    }

    public enum ValueType : ushort
    {
        Ascii = 0x02,
        Short = 0x03,
        Int = 0x04,
        Long = 0x10
    }
}