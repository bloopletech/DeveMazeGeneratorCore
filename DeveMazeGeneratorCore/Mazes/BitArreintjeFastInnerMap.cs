using System.Runtime.CompilerServices;
using DeveMazeGeneratorCore.IO;
using DeveMazeGeneratorCore.Mazes.InnerStuff;

namespace DeveMazeGeneratorCore.Mazes;

public class BitArreintjeFastInnerMap : IMaze
{
    private readonly IBinarySerializer serializer;
    private readonly int width;
    private readonly int height;
    private readonly BitArreintjeFastInnerMapArray[] _innerData;

    public BitArreintjeFastInnerMap(IBinarySerializer serializer)
    {
        this.serializer = serializer;

        width = serializer.ReadInt32();
        height = serializer.ReadInt32();

        _innerData = new BitArreintjeFastInnerMapArray[width];
        for(int i = 0; i < width; i++)
        {
            _innerData[i] = new BitArreintjeFastInnerMapArray(height);
        }
    }

    public BitArreintjeFastInnerMap(IBinarySerializer serializer, int width, int height)
    {
        if(width < 3) throw new ArgumentOutOfRangeException(nameof(width), width, "Width must >= 3");
        if(height < 3) throw new ArgumentOutOfRangeException(nameof(height), height, "Height must >= 3");

        this.serializer = serializer;
        this.width = width;
        this.height = height;

        _innerData = new BitArreintjeFastInnerMapArray[width];
        for(int i = 0; i < width; i++)
        {
            _innerData[i] = new BitArreintjeFastInnerMapArray(height);
        }
    }

    public MazeType Type => MazeType.BitArreintjeFastInnerMap;
    public IBinarySerializer Serializer => serializer;
    public int Width => width;
    public int Height => height;

    public IMaze Clone()
    {
        var innerMapTarget = new BitArreintjeFastInnerMap(new BinarySerializer(new MemoryStream()), width, height);
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
        WriteHeader();
        // TODO: Actually write _innerData to the stream
    }

    public async Task WriteAsync()
    {
        WriteHeader();
        // TODO: Actually write _innerData to the stream
    }

    private void WriteHeader()
    {
        //serializer.Write((ushort)MazeType.BitArreintjeFastInnerMap);
        serializer.Write(Width);
        serializer.Write(Height);
    }
}
