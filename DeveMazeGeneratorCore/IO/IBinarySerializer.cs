namespace DeveMazeGeneratorCore.IO
{
    public interface IBinarySerializer
    {
        // Common
        void Close();

        // BinaryReader / BinaryWriter common
        Stream BaseStream { get; }

        // BinaryReader
        int PeekChar();
        int Read();
        int Read(byte[] buffer, int index, int count); // Also Stream
        int Read(char[] buffer, int index, int count);
        int Read(Span<byte> buffer); // Also Stream
        int Read(Span<char> buffer);
        int Read7BitEncodedInt();
        long Read7BitEncodedInt64();
        bool ReadBoolean();
        byte ReadByte(); // Also Stream
        byte[] ReadBytes(int count);
        char ReadChar();
        char[] ReadChars(int count);
        decimal ReadDecimal();
        double ReadDouble();
        void ReadExactly(Span<byte> buffer); // Also Stream
        Half ReadHalf();
        short ReadInt16();
        int ReadInt32();
        long ReadInt64();
        sbyte ReadSByte();
        float ReadSingle();
        string ReadString();
        ushort ReadUInt16();
        uint ReadUInt32();
        ulong ReadUInt64();

        //// BinaryWriter
        //void Flush(); // Also Stream
        //long Seek(int offset, SeekOrigin origin);
        //void Write(bool value);
        //void Write(byte value);
        //void Write(byte[] buffer);
        //void Write(byte[] buffer, int index, int count); // Also Stream
        //void Write(char ch);
        //void Write(char[] chars);
        //void Write(char[] chars, int index, int count);
        //void Write(decimal value);
        //void Write(double value);
        //void Write(float value);
        //void Write(Half value);
        //void Write(int value);
        //void Write(long value);
        //void Write(ReadOnlySpan<byte> buffer); // Also Stream
        //void Write(ReadOnlySpan<char> chars);
        //void Write(sbyte value);
        //void Write(short value);
        //void Write(string value);
        //void Write(uint value);
        //void Write(ulong value);
        //void Write(ushort value);
        //void Write7BitEncodedInt(int value);
        //void Write7BitEncodedInt64(long value);

        //// Stream
        //bool CanRead { get; }
        //bool CanSeek { get; }
        //bool CanTimeout { get; }
        //bool CanWrite { get; }
        //long Length { get; }
        //long Position { get; set; }
        //int ReadTimeout { get; set; }
        //int WriteTimeout { get; set; }

        //IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
        //IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state);
        //void CopyTo(Stream destination);
        //void CopyTo(Stream destination, int bufferSize);
        //Task CopyToAsync(Stream destination);
        //Task CopyToAsync(Stream destination, CancellationToken cancellationToken);
        //Task CopyToAsync(Stream destination, int bufferSize);
        //Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken);
        //int EndRead(IAsyncResult asyncResult);
        //void EndWrite(IAsyncResult asyncResult);
        //Task FlushAsync();
        //Task FlushAsync(CancellationToken cancellationToken);
        //Task<int> ReadAsync(byte[] buffer, int offset, int count);
        //Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
        //ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
        //int ReadAtLeast(Span<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true);
        //ValueTask<int> ReadAtLeastAsync(Memory<byte> buffer, int minimumBytes, bool throwOnEndOfStream = true, CancellationToken cancellationToken = default);
        //void ReadExactly(byte[] buffer, int offset, int count);
        //ValueTask ReadExactlyAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken = default);
        //ValueTask ReadExactlyAsync(Memory<byte> buffer, CancellationToken cancellationToken = default);
        //long Seek(long offset, SeekOrigin origin);
        //void SetLength(long value);
        //Task WriteAsync(byte[] buffer, int offset, int count);
        //Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken);
        //ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default);
        //void WriteByte(byte value);
    }
}