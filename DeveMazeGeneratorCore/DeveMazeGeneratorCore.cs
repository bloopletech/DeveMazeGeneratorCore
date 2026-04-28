using DeveMazeGeneratorCore.Extensions;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;

namespace DeveMazeGeneratorCore;

public static class DeveMazeGeneratorCore
{
    public static IMaze Generate(int width, int height, int? seed = null) => Generate(
        new MemoryStream(),
        width,
        height,
        seed);

    public static IMaze Generate(Stream stream, int width, int height, int? seed = null) => Generate(
        DetermineMazeType(width, height),
        AlgorithmType.Backtrack,
        stream,
        width,
        height,
        seed);

    public static IMaze Generate(MazeType mazeType, AlgorithmType algorithmType, int width, int height, int? seed = null) => Generate(
        mazeType,
        algorithmType,
        new MemoryStream(),
        width,
        height,
        seed);

    public static IMaze Generate(
        MazeType mazeType,
        AlgorithmType algorithmType,
        Stream stream,
        int width,
        int height,
        int? seed = null)
    {
        var maze = MazeSerializer.Create(mazeType, stream, width, height);
        var random = seed.HasValue ? new Random(seed.Value) : new Random();
        var realSeed = random.GetSeed();

        var algorithm = IAlgorithm.Create(algorithmType, maze, random);
        algorithm.Generate();

        return maze;
    }

    public static MazeType DetermineMazeType(int width, int height)
    {
        var size = (long)width * height;
        var byteSize = size.DivCeil(8);
        return byteSize > int.MaxValue ? MazeType.BigBitGridMaze : MazeType.BitGridMaze;
    }

    public static IMaze BenchmarkBaseline()
    {
        return Generate(MazeType.BitGridMaze, AlgorithmType.Backtrack, BenchmarkSize, BenchmarkSize, BenchmarkSeed);
    }

    public static IMaze BenchmarkFast()
    {
        return Generate(
            MazeType.BitGridMaze,
            AlgorithmType.Backtrack2_Deluxe2_AsByte,
            BenchmarkSize,
            BenchmarkSize,
            BenchmarkSeed);
    }

    public static IMazePath Solve(IMaze maze) => Solve(new MemoryStream(), maze);

    public static IMazePath Solve(Stream stream, IMaze maze)
    {
        var points = PathFinder.Find(maze);
        var path = new MazePath(points.ToArray());
        path.Write(stream);
        return path;
    }

    private const int BenchmarkSize = (4096 * 2 * 2 * 2) + 1;
    private const int BenchmarkSeed = 1337;
}
