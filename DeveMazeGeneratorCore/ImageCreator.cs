#if !BLAZOR
using System.Collections;
using System.Diagnostics;
using DeveMazeGeneratorCore.Mazes;
using DeveMazeGeneratorCore.Paths;
using DeveMazeGeneratorCore.Structures;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.PixelFormats;

namespace DeveMazeGeneratorCore;

public static class ImageCreator
{
    public static Image<Argb32> CreateImage(IMaze maze)
    {
        var image = new Image<Argb32>(maze.Width, maze.Height, Color.Black);

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(maze[x, y])
                    {
                        ref var pixel = ref row[x];
                        pixel = Color.White;
                    }
                }
            }
        });

        return image;
    }

    public static Image<Argb32> CreateImage(IMaze maze, IMazePath path)
    {
        var image = CreateImage(maze);

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(path[x, y])
                    {
                        ref var pixel = ref row[x];
                        pixel = Color.Lime;
                    }
                }
            }
        });

        image[1, 1] = Color.Blue;
        image[maze.Width - 2, maze.Height - 2] = Color.Red;

        return image;
    }

    public static Image<Argb32> CreatePlainImage(IMaze maze, MazePath path)
    {
        var image = CreateImage(maze);

        var points = path.Points;
        foreach(var point in points) image[point.X, point.Y] = Color.Lime;
        image[1, 1] = Color.Blue;
        image[maze.Width - 2, maze.Height - 2] = Color.Red;

        return image;
    }

    public static Image<Argb32> CreateImage(IMaze maze, MazePath path)
    {
        var image = CreateImage(maze);

        var points = path.Points;
        for(var i = 0; i < points.Length; i++)
        {
            ref var point = ref points[i];
            var shade = (byte)(i / (double)points.Length * 255.0);
            image[point.X, point.Y] = new Argb32(shade, (byte)(255 - shade), 0);
        }
        return image;
    }

    public static Image<Argb32> CreatePlainImageSorted(IMaze maze, MazePoint[] points)
    {
        var image = CreateImage(maze);

        var sortedPoints = (MazePoint[])points.Clone();

        sortedPoints.Sort((first, second) =>
        {
            if(first.Y == second.Y) return first.X - second.X;
            return first.Y - second.Y;
        });

        var pointsIndex = 0;

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(pointsIndex >= sortedPoints.Length) return;

                    ref var point = ref sortedPoints[pointsIndex];
                    if(point.X == x && point.Y == y)
                    {
                        ref var pixel = ref row[x];
                        pixel = Color.Lime;
                        pointsIndex++;
                    }
                }
            }
        });

        image[1, 1] = Color.Blue;
        image[maze.Width - 2, maze.Height - 2] = Color.Red;

        return image;
    }

    public static Image<Argb32> CreateImageSorted(IMaze maze, MazePoint[] points)
    {
        var image = CreateImage(maze);

        var sortedPoints = (MazePoint[])points.Clone();

        sortedPoints.Sort((first, second) =>
        {
            if(first.Y == second.Y) return first.X - second.X;
            return first.Y - second.Y;
        });

        var pointsIndex = 0;

        image.ProcessPixelRows(rows =>
        {
            for(int y = 0; y < rows.Height; y++)
            {
                var row = rows.GetRowSpan(y);
                for(int x = 0; x < row.Length; x++)
                {
                    if(pointsIndex >= sortedPoints.Length) return;

                    ref var point = ref sortedPoints[pointsIndex];
                    if(point.X == x && point.Y == y)
                    {
                        ref var pixel = ref row[x];
                        var shade = (byte)(pointsIndex / (double)sortedPoints.Length * 255.0);
                        pixel = new Argb32(shade, (byte)(255 - shade), 0);
                        pointsIndex++;
                    }
                }
            }
        });

        return image;
    }

    public static async Task Serialize(Stream stream, Image image)
    {
        await image.SaveAsync(stream, new PngEncoder() { CompressionLevel = PngCompressionLevel.Level1 });
    }

    public static async Task Save(string fileName, Image image)
    {
        using var fs = File.Open(fileName, FileMode.Create);
        await Serialize(fs, image);
    }
}
#endif