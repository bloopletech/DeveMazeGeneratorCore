using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffImage(IStore store, Guid mazeId, MazeSize size) : Storable(store, false), IImage<MazeColor>
{
    private const long MazeIdOffset = 312;
    private const long MazeIdLength = 37;
    //private const long ArrayOffset = MazeIdOffset + MazeIdLength;
    private const long ArrayOffset = 16;

    private readonly LongArray<MazeColor> array = new(store.Offset(Tiff.HeaderLength - sizeof(long), true), size.Area, true);
    private bool written;

    public override long Extent => array.Extent + Tiff.HeaderLength;
    public Guid MazeId => mazeId;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;

    public MazeColor this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            var index = x + ((long)y * size.Width);
            return array[index];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            var index = x + ((long)y * size.Width);
            array[index] = value;
        }
    }

    public override void Read()
    {
    }

    public override void Write()
    {
        if(written) return;
        written = true;

        array.Write();
        long arrayOffset = Tiff.HeaderLength;
        var arrayLength = StoreDeflater.Deflate(store.Offset(Tiff.HeaderLength, true));

        var mazeIdBytes = Tiff.GetAsciiBytes(mazeId.ToString());
        var mazeIdOffset = arrayOffset + arrayLength;

        store.Write(mazeIdOffset, mazeIdBytes);

        var directoryOffset = mazeIdOffset + mazeIdBytes.Length;

        store.Write<byte>(directoryOffset, [
            ..BitConverter.GetBytes(14L),
            ..Tiff.Tag(Tiff.TagType.Width, (uint)size.Width),
            ..Tiff.Tag(Tiff.TagType.Height, (uint)size.Height),
            ..Tiff.Tag(Tiff.TagType.BitsPerSample, [0x08, 0x08, 0x08]),
            ..Tiff.Tag(Tiff.TagType.Compression, 0x08),
            ..Tiff.Tag(Tiff.TagType.PhotometricInterpolation, 0x02),
            ..Tiff.Tag(Tiff.TagType.ImageDescription, Tiff.ValueType.Ascii, mazeIdBytes.Length - 1, mazeIdOffset),
            ..Tiff.Tag(Tiff.TagType.StripOffsets, (ulong)arrayOffset),
            ..Tiff.Tag(Tiff.TagType.SamplesPerPixel, 0x03),
            ..Tiff.Tag(Tiff.TagType.RowsPerStrip, (uint)size.Width),
            ..Tiff.Tag(Tiff.TagType.StripByteCount, (ulong)arrayLength),
            ..Tiff.Tag(Tiff.TagType.MinimumSampleValue, [0, 0, 0]),
            ..Tiff.Tag(Tiff.TagType.MaximumSampleValue, [0xFF, 0xFF, 0xFF]),
            ..Tiff.Tag(Tiff.TagType.PlanarConfiguration, 0x01),
            ..Tiff.Tag(Tiff.TagType.SampleFormat, [0x01, 0x01, 0x01]),
            ..BitConverter.GetBytes(0L)
        ]);

        store.Write(0, Tiff.Header(directoryOffset));
    }
}
