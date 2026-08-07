using System.Runtime.CompilerServices;
using System.Text;
using HugeMazes.Collections;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffImage(IStore store, Guid mazeId, MazeSize size) : Storable(store, false), IImage<MazeColor>
{
    private const long MazeIdOffset = 2000;
    private const long ArrayOffset = 4096;

    private readonly LongArray<MazeColor> array = new(store.Offset(ArrayOffset - sizeof(long), true), size.Area, true);
    private bool written;

    public override long Extent => array.Extent + ArrayOffset;
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

        var builder = new TiffBuilder();
        builder.SetTag(TiffTag.TagType.Width, (uint)size.Width);
        builder.SetTag(TiffTag.TagType.Height, (uint)size.Height);
        builder.SetTag(TiffTag.TagType.BitsPerSample, [0x08, 0x08, 0x08]);
        builder.SetTag(TiffTag.TagType.Compression, 0x08);
        builder.SetTag(TiffTag.TagType.PhotometricInterpolation, 0x02);
        builder.SetTag(TiffTag.TagType.ImageDescription, mazeId.ToString());
        builder.SetTag(TiffTag.TagType.StripOffsets, 0L);
        builder.SetTag(TiffTag.TagType.SamplesPerPixel, 0x03);
        builder.SetTag(TiffTag.TagType.RowsPerStrip, (uint)size.Width);
        builder.SetTag(TiffTag.TagType.StripByteCount, 0L);
        builder.SetTag(TiffTag.TagType.MinimumSampleValue, [0, 0, 0]);
        builder.SetTag(TiffTag.TagType.MaximumSampleValue, [0xFF, 0xFF, 0xFF]);
        builder.SetTag(TiffTag.TagType.PlanarConfiguration, 0x01);
        builder.SetTag(TiffTag.TagType.SampleFormat, [0x01, 0x01, 0x01]);
        var header = builder.Build();

        store.Write(0, header);
        store.Write<byte>(MazeIdOffset, [..Encoding.ASCII.GetBytes(mazeId.ToString()), 0x00]);
        array.Write();

        var deflateStore = store.Offset(ArrayOffset, true);
        StoreDeflater.Deflate(deflateStore);
        store.Write(196L, deflateStore.Length);
    }
}
