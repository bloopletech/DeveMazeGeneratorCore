using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.RNG;

namespace DeveMazeGeneratorCore;

public static class DeveMazeGeneratorCore
{
    public static IMaze Generate(int width, int height, Xoshiro256RandomSeed seed)
    {
        var maze = new ContiguousArrayMaze(width, height);
        //var random = new Xoshiro256Random(seed);
        var random = new Random();
        var rseed = random.GetSeed();

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        return maze;
    }

    public static IMaze Generate(int width, int height, int? seed = null)
    {
        var maze = new ContiguousArrayMaze(width, height);
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        //IRandom random = seed.HasValue ? new NetRandom(seed.Value) : new Xoshiro256Random();
        var rseed = random.GetSeedReflection();

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        return maze;
    }

    public static IMaze BenchmarkBaseline()
    {
        var maze = new ContiguousArrayMaze(BenchmarkSize, BenchmarkSize);
        //var random = new NetRandom(BenchmarkSeed);
        var random = new Random();

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        return maze;
    }

    public static IMaze BenchmarkFast()
    {
        var maze = new ContiguousArrayMaze(BenchmarkSize, BenchmarkSize);
        //var random = new Xoshiro256Random(new Xoshiro256RandomSeed(BenchmarkSeed, 0, 0, 0));
        var random = new Random(BenchmarkSeed);

        var algorithm = new AlgorithmBacktrack2Deluxe2_AsByte(maze, random);
        algorithm.Generate();

        return maze;
    }

    private const int BenchmarkSize = (4096 * 2 * 2 * 2) + 1;
    private const int BenchmarkSeed = 1337;
}
