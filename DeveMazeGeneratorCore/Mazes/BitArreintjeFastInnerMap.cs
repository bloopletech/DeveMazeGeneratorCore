using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Mazes.InnerStuff;

namespace DeveMazeGeneratorCore.Mazes;

public class BitArreintjeFastInnerMap : IMaze
{
    private readonly Stream stream;
    private readonly int width;
    private readonly int height;
    private readonly BitArreintjeFastInnerMapArray[] _innerData;

    public BitArreintjeFastInnerMap(Stream stream)
    {
        this.stream = stream;

        using var reader = stream.Reader();
        width = reader.ReadInt32();
        height = reader.ReadInt32();

        _innerData = new BitArreintjeFastInnerMapArray[width];
        for(int i = 0; i < width; i++)
        {
            _innerData[i] = new BitArreintjeFastInnerMapArray(height);
        }
    }

    public BitArreintjeFastInnerMap(Stream stream, int width, int height)
    {
        if(width < 3) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must >= 3");
        if(height < 3) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must >= 3");

        this.stream = stream;
        this.width = width;
        this.height = height;

        using var writer = stream.Writer();
        writer.Write(width);
        writer.Write(height);

        _innerData = new BitArreintjeFastInnerMapArray[width];
        for(int i = 0; i < width; i++)
        {
            _innerData[i] = new BitArreintjeFastInnerMapArray(height);
        }
    }

    public MazeType Type => MazeType.BitArreintjeFastInnerMap;
    public Stream Stream => stream;
    public int Width => width;
    public int Height => height;

    public IMaze Clone()
    {
        var innerMapTarget = new BitArreintjeFastInnerMap(new MemoryStream(), width, height);
        for(int i = 0; i < _innerData.Length; i++)
        {
            innerMapTarget._innerData[i] = _innerData[i].Clone();
        }
        return innerMapTarget;
    }

    public bool this[int x, int y]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return _innerData[x][y];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            _innerData[x][y] = value;
        }
    }

    public void Read()
    {
        // TODO: Actually read _innerData from the stream
    }

    public async Task ReadAsync()
    {
        // TODO: Actually read _innerData from the stream
    }

    public void Write()
    {
        //WriteHeader(stream);
        // TODO: Actually write _innerData to the stream
    }

    public async Task WriteAsync()
    {
        //WriteHeader(stream);
        // TODO: Actually write _innerData to the stream
    }

    //private void WriteHeader(Stream stream)
    //{
    //    using var writer = stream.Writer();
    //    writer.Write((ushort)MazeType.BitArreintjeFastInnerMap);
    //    writer.Write(Width);
    //    writer.Write(Height);
    //}
}
