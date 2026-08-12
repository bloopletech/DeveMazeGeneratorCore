using System.Runtime.CompilerServices;
using HugeMazes.Collections;
using HugeMazes.Extensions;
using HugeMazes.IO;
using HugeMazes.Structures;

namespace HugeMazes.Images;

// Based on https://paulbourke.net/dataformats/tiff/
public class TiffIndexedImage : Storable, IImage<byte>
{
    public const int PaletteSize = 256;

    private readonly LongArray<byte> array;
    private readonly Guid mazeId;
    private readonly MazeSize size;
    private readonly MazeColor[] palette;
    private TiffBuilder builder;
    private bool written;

    public TiffIndexedImage(IStore store, Guid mazeId, MazeSize size, MazeColor[] palette) : base(store, false)
    {
        this.mazeId = mazeId;
        this.size = size;
        this.palette = palette;
        builder = Build();
        array = new(store.Offset(builder.Length - sizeof(long), true), size.Area, true);
    }

    public override long Extent => array.Extent + builder.Length;
    public Guid MazeId => mazeId;
    public MazeSize Size => size;
    public int Width => size.Width;
    public int Height => size.Height;
    public MazeColor[] Palette => palette;

    public byte this[int x, int y]
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

        var deflatedLength = StoreDeflater.Deflate(store.Offset(builder.Length, true));

        builder.Set(Tiff.TagType.StripByteCount, (ulong)deflatedLength);
        store.Write<byte>(0, builder.Build());
    }

    private TiffBuilder Build()
    {
        var builder = new TiffBuilder();
        builder.Set(Tiff.TagType.Width, (uint)size.Width);
        builder.Set(Tiff.TagType.Height, (uint)size.Height);
        builder.Set(Tiff.TagType.BitsPerSample, 0x08);
        builder.Set(Tiff.TagType.Compression, 0x08);
        builder.Set(Tiff.TagType.PhotometricInterpolation, 0x03);
        builder.Set(Tiff.TagType.ImageDescription, mazeId.ToString());
        builder.Set(Tiff.TagType.StripOffsets, (ulong)0);
        builder.Set(Tiff.TagType.RowsPerStrip, (uint)size.Width);
        builder.Set(Tiff.TagType.StripByteCount, (ulong)0);
        builder.Set(Tiff.TagType.PlanarConfiguration, 0x01);
        //XResolution
        //YResolution
        //ResolutionUnit
        builder.Set(Tiff.TagType.ColorMap, Tiff.GetColorMap(palette.Extend(PaletteSize)));
        builder.Set(Tiff.TagType.StripOffsets, (ulong)builder.Length);
        return builder;
    }

}
