namespace DeveMazeGeneratorCore.RNG;

public class NetRandom : IRandom
{
    private readonly Random prng;

    public NetRandom() => prng = new();
    public NetRandom(int seed) => prng = new(seed);

    public int Next() => prng.Next();
    public int Next(int maxValue) => prng.Next(maxValue);
    public int Next(int minValue, int maxValue) => prng.Next(minValue, maxValue);
    public double NextDouble() => prng.NextDouble();
}