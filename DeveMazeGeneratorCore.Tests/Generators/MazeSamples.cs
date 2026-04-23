using System;
using System.IO;
using System.Threading.Tasks;
using DeveMazeGeneratorCore.Generators;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DeveMazeGeneratorCore.Tests.Generators;

[TestClass]
public class MazeSamples
{
    [TestMethod]
    public async Task GeneratingAMazeWithABlockInTheMiddleWorks()
    {
        var maze = new BitArreintjeFastInnerMap(new MemoryStream(), 129, 129);

        for(int y = 33; y < 96; y++)
        {
            for(int x = 33; x < 96; x++)
            {
                maze[x, y] = true;
            }
        }

        var random = new Random(1337);

        var algorithm = new AlgorithmBacktrack(maze, random);
        algorithm.Generate();

        var path = new MazePath(new MemoryStream(), maze.Width, maze.Height);
        PathFinder.Find(maze, path);

        var image = ImageCreator.CreateImage(maze, path);

        using var fs = new FileStream("GeneratingAMazeWithABlockInTheMiddleWorks.png", FileMode.Create);
        await ImageCreator.Serialize(fs, image);
    }
}
