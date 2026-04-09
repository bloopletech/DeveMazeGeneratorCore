namespace DeveMazeGeneratorCore.RNG;

public readonly record struct Xoshiro256RandomSeed(ulong S0, ulong S1, ulong S2, ulong S3)
{
    public void Write(BinaryWriter writer)
    {
        writer.Write(S0);
        writer.Write(S1);
        writer.Write(S2);
        writer.Write(S3);
    }

    public static Xoshiro256RandomSeed Read(BinaryReader reader) => new(
        reader.ReadUInt64(),
        reader.ReadUInt64(),
        reader.ReadUInt64(),
        reader.ReadUInt64());
}