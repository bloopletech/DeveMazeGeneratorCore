using System.Runtime.InteropServices;

namespace DeveMazeGeneratorCore.IO;

public class BinarySerializer(Stream stream) : IBinarySerializer, IDisposable, IAsyncDisposable
{
    private readonly BinaryReader reader = new BinaryReader(stream);
    private readonly BinaryWriter writer = new BinaryWriter(stream);
    private bool disposed;

    public Stream BaseStream => stream;
    public void Close() => stream.Close();
    public void Dispose() => stream.Dispose();
    public ValueTask DisposeAsync() => stream.DisposeAsync();

    public int PeekChar() => reader.PeekChar();
    public int Read() => reader.Read();
    public byte ReadByte() => reader.ReadByte();
    public int Read(char[] buffer, int index, int count) => reader.Read(buffer, index, count);
    public int Read(Span<char> buffer) => reader.Read(buffer);
    public int Read7BitEncodedInt() => reader.Read7BitEncodedInt();
    public long Read7BitEncodedInt64() => reader.Read7BitEncodedInt64();
    public bool ReadBoolean() => reader.ReadBoolean();
    public byte[] ReadBytes(int count) => reader.ReadBytes(count);
    public char ReadChar() => reader.ReadChar();
    public char[] ReadChars(int count) => reader.ReadChars(count);
    public decimal ReadDecimal() => reader.ReadDecimal();
    public double ReadDouble() => reader.ReadDouble();
    public Half ReadHalf() => reader.ReadHalf();
    public short ReadInt16() => reader.ReadInt16();
    public int ReadInt32() => reader.ReadInt32();
    public long ReadInt64() => reader.ReadInt64();
    public sbyte ReadSByte() => reader.ReadSByte();
    public float ReadSingle() => reader.ReadSingle();
    public string ReadString() => reader.ReadString();
    public ushort ReadUInt16() => reader.ReadUInt16();
    public uint ReadUInt32() => reader.ReadUInt32();
    public ulong ReadUInt64() => reader.ReadUInt64();

    public long Seek(int offset, SeekOrigin origin) => writer.Seek(offset, origin);
    public void Write(bool value) => writer.Write(value);
    public void Write(byte value) => writer.Write(value);
    public void Write(byte[] buffer) => writer.Write(buffer);
    public void Write(char ch) => writer.Write(ch);
    public void Write(char[] chars) => writer.Write(chars);
    public void Write(char[] chars, int index, int count) => writer.Write(chars, index, count);
    public void Write(decimal value) => writer.Write(value);
    public void Write(double value) => writer.Write(value);
    public void Write(float value) => writer.Write(value);
    public void Write(Half value) => writer.Write(value);
    public void Write(int value) => writer.Write(value);
    public void Write(long value) => writer.Write(value);
    public void Write(ReadOnlySpan<char> chars) => writer.Write(chars);
    public void Write(sbyte value) => writer.Write(value);
    public void Write(short value) => writer.Write(value);
    public void Write(string value) => writer.Write(value);
    public void Write(uint value) => writer.Write(value);
    public void Write(ulong value) => writer.Write(value);
    public void Write(ushort value) => writer.Write(value);
    public void Write7BitEncodedInt(int value) => writer.Write(value);
    public void Write7BitEncodedInt64(long value) => writer.Write(value);

    public bool CanRead => stream.CanRead;
    public bool CanSeek => stream.CanSeek;
    public bool CanTimeout => stream.CanTimeout;
    public bool CanWrite => stream.CanWrite;
    public long Length => stream.Length;
    public long Position
    {
        get => stream.Position;
        set => stream.Position = value;
    }
    public int ReadTimeout
    {
        get => stream.ReadTimeout;
        set => stream.ReadTimeout = value;
    }
    public int WriteTimeout
    {
        get => stream.WriteTimeout;
        set => stream.WriteTimeout = value;
    }

    public IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => stream.BeginRead(buffer, offset, count, callback, state);
    public IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state) => stream.BeginWrite(buffer, offset, count, callback, state);
    public void CopyTo(Stream destination) => stream.CopyTo(destination);
    public void CopyTo(Stream destination, int bufferSize) => stream.CopyTo(destination, bufferSize);
    public async Task CopyToAsync(Stream destination) => await stream.CopyToAsync(destination);
    public async Task CopyToAsync(Stream destination, CancellationToken cancellationToken) => await stream.CopyToAsync(destination, cancellationToken);
    public async Task CopyToAsync(Stream destination, int bufferSize) => await stream.CopyToAsync(destination, bufferSize);
    public async Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken) => await stream.CopyToAsync(destination, bufferSize, cancellationToken);
    public int EndRead(IAsyncResult asyncResult) => stream.EndRead(asyncResult);
    public void EndWrite(IAsyncResult asyncResult) => stream.EndWrite(asyncResult);
    public void Flush() => stream.Flush(); // Also BinaryWriter
    public async Task FlushAsync() => await stream.FlushAsync();
    public async Task FlushAsync(CancellationToken cancellationToken) => await stream.FlushAsync(cancellationToken);
    public async Task<int> ReadAsync(byte[] buffer, int offset, int count) => await stream.ReadAsync(buffer, offset, count);
    public async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => await stream.ReadAsync(buffer, offset, count, cancellationToken);
    public async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => await stream.ReadAsync(buffer, cancellationToken);
    public int ReadAtLeast(Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true) => stream.ReadAtLeast(buffer, minimumBytes, throwOnEndOfStream);
    public async ValueTask<int> ReadAtLeastAsync(Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default) => await stream.ReadAtLeastAsync(buffer, minimumBytes, throwOnEndOfStream, cancellationToken);
    public void ReadExactly(byte[] buffer, int offset, int count) => stream.ReadExactly(buffer, offset, count);
    public async ValueTask ReadExactlyAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default) => await stream.ReadExactlyAsync(buffer, offset, count, cancellationToken);
    public async ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default) => await stream.ReadExactlyAsync(buffer, cancellationToken);
    public long Seek(long offset, SeekOrigin origin) => stream.Seek(offset, origin);
    public void SetLength(long value) => stream.SetLength(value);
    public int Read(byte[] buffer, int offset, int count) => stream.Read(buffer, offset, count); // Also BinaryReader
    public int Read(Span<byte> buffer) => stream.Read(buffer); // Also BinaryReader
    public int ReadByteInt() => stream.ReadByte(); // Also BinaryReader
    public void ReadExactly(Span<byte> buffer) => stream.ReadExactly(buffer); // Also BinaryReader
    public async Task WriteAsync(byte[] buffer, int offset, int count) => await stream.WriteAsync(buffer, offset, count);
    public async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken) => await stream.WriteAsync(buffer, offset, count, cancellationToken);
    public async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default) => await stream.WriteAsync(buffer, cancellationToken);
    public void Write(byte[] buffer, int offset, int count) => stream.Write(buffer, offset, count); // Also BinaryWriter
    public void Write(ReadOnlySpan<byte> buffer) => stream.Write(buffer); // Also BinaryWriter
    public void WriteByte(byte value) => stream.WriteByte(value);


    public T Read<T>() where T : struct
    {
        //var size = Unsafe.SizeOf<T>();
        //Span<byte> buffer = stackalloc byte[size];
        //ReadExactly(buffer);
        //return MemoryMarshal.AsRef<T>(buffer);
        Span<T> buffer = new T[1];
        //Span<T> buffer = stackalloc T[1];
        Read(buffer);
        //ReadExactly(MemoryMarshal.AsBytes(buffer));
        return buffer[0];
    }

    //public void Read<T>(T[] array) where T : struct => Read(array, 0, array.Length);

    public void Read<T>(T[] array, int index, int count) where T : struct
    {
        Read(new Span<T>(array, index, count));
    }

    public void Read<T>(Span<T> buffer) where T : struct
    {
        ReadExactly(MemoryMarshal.AsBytes(buffer));
    }

    public T[] ReadArray<T>() where T : struct
    {
        var length = ReadInt32();
        var buffer = new T[length];
        Read(buffer);
        return buffer;
    }

    public void Write<T>(T value) where T : struct
    {
        Span<T> buffer = [value];
        Write(buffer);
    }

    //public void Write<T>(T[] array) where T : struct => Write(array, 0, array.Length);

    public void Write<T>(T[] array, int index, int count) where T : struct
    {
        Write(new ReadOnlySpan<T>(array, index, count));
    }

    public void Write<T>(ReadOnlySpan<T> buffer) where T : struct
    {
        Write(MemoryMarshal.AsBytes(buffer));
    }

    public void WriteArray<T>(ReadOnlySpan<T> buffer) where T : struct
    {
        Write(buffer.Length);
        Write(buffer);
    }


    public async Task WriteAsync<T>(ReadOnlyMemory<T> buffer) where T : struct
    {
        Write(MemoryMarshal.AsBytes(buffer));
    }
}