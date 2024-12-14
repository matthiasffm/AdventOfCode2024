namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

/// <summary>
/// Theme: bathroom metal gear solid
/// </summary>
[TestFixture]
public class Day14
{
    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "p=0,4 v=3,-3",
            "p=6,3 v=-1,-3",
            "p=10,3 v=-1,2",
            "p=2,0 v=2,-1",
            "p=0,0 v=1,3",
            "p=3,0 v=-2,-2",
            "p=7,6 v=-1,-3",
            "p=3,0 v=-1,-2",
            "p=9,3 v=2,3",
            "p=7,3 v=-1,2",
            "p=2,4 v=2,-3",
            "p=9,5 v=-3,-3",
        };
        var robots = ParseData(data);
        Puzzle1(robots, 11, 7, 100).Should().Be(1L * 3L * 4L * 1L);
    }

    [Test]
    public void TestAocInput()
    {
        var data   = FileUtils.ReadAllLines(this);
        var robots = ParseData(data);

        Puzzle1(robots, 101, 103, 100).Should().Be(208437768L);
        Puzzle2(robots, 101, 103).Should().Be(7492L);
    }

    private record Robot(Vec2<long> Position, Vec2<long> Velocity);

    private static IEnumerable<Robot> ParseData(string[] data) =>
        (data.Select(s => s.Split([',',' ', '='], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
             .Select(split => ParseRobot(split[1], split[2], split[4], split[5]))).ToArray();

    private static Robot ParseRobot(string posX, string posY, string vX, string vY)
        => new(new Vec2<long>(long.Parse(posX), long.Parse(posY)), new Vec2<long>(long.Parse(vX), long.Parse(vY)));

    // One of The Historians needs to use the bathroom. Unfortunately, EBHQ seems to have "improved" bathroom security again after your last visit. The area
    // outside the bathroom is swarming with robots! To get The Historian safely to the bathroom, you'll need a way to predict where the robots will be in the
    // future. Fortunately, they all seem to be moving on the tile floor in predictable straight lines. You make a list (your puzzle input) of all of the
    // robots' current positions (p) and velocities (v), one robot per line. Each robot's position is given as p=x,y where x represents the number of tiles the
    // robot is from the left wall and y represents the number of tiles from the top wall (when viewed from above). Each robot's velocity is given as v=x,y where
    // x and y are given in tiles per second. Positive x means the robot is moving to the right, and positive y means the robot is moving down.
    // The robots outside the actual bathroom are in a space which is 101 tiles wide and 103 tiles tall (when viewed from above). The robots are good at navigating
    // over/under each other (due to a combination of springs, extendable legs, and quadcopters), so they can share the same tile and don't interact with each other.
    // These robots have a unique feature for maximum bathroom security: they can teleport. When a robot would run into an edge of the space they're in, they instead
    // teleport to the other side, effectively wrapping around the edges.
    // To determine the safest area, count the number of robots in each quadrant after 100 seconds. Robots that are exactly in the middle (horizontally or vertically)
    // don't count as being in any quadrant.
    //
    // Puzzle == Predict the motion of the robots in your list within the provided space. What will the safety factor be after exactly n seconds have elapsed?
    private static long Puzzle1(IEnumerable<Robot> robots, int width, int height, int iterations)
    {
        var robotsInQ1 = 0L;
        var robotsInQ2 = 0L;
        var robotsInQ3 = 0L;
        var robotsInQ4 = 0L;

        foreach(var robot in robots)
        {
            var robotEndPos = robot.Position + iterations * robot.Velocity;
            var endPosX = ModWithWrapAround(robotEndPos.X, width);
            var endPosY = ModWithWrapAround(robotEndPos.Y, height);

            if(endPosX < width / 2)
            {
                if(endPosY < height / 2)
                {
                    robotsInQ1++;
                }
                else if(endPosY > height / 2)
                {
                    robotsInQ3++;
                }
            }
            else if(endPosX > width / 2)
            {
                if(endPosY < height / 2)
                {
                    robotsInQ2++;
                }
                else if(endPosY > height / 2)
                {
                    robotsInQ4++;
                }
            }
        }

        return robotsInQ1 * robotsInQ2 * robotsInQ3 * robotsInQ4;
    }

    // During the bathroom break, someone notices that these robots seem awfully similar to ones built and used at the North Pole. If they're the same type of
    // robots, they should have a hard-coded Easter egg: very rarely, most of the robots should arrange themselves into a picture of a Christmas tree.
    //
    // Puzzle == What is the fewest number of seconds that must elapse for the robots to display the Easter egg?
    private static long Puzzle2(IEnumerable<Robot> robots, int width, int height)
    {
        // a picture is probably a iteration where most of the robots form some kind of cluster so variations of robot positions in x and y direction should be
        // at a minimum

        var minVariationX = Enumerable.Range(0, width)
                                      .Select(i => (i, robots.Select(r => ModWithWrapAround((r.Position + i * r.Velocity).X, width)).Variation()))
                                      .MinBy(i => i.Item2)
                                      .i;

        var minVariationY = Enumerable.Range(0, height)
                                      .Select(i => (i, robots.Select(r => ModWithWrapAround((r.Position + i * r.Velocity).Y, height)).Variation()))
                                      .MinBy(i => i.Item2)
                                      .i;

        // so now we know the smallest variations for x and y separately and have just to
        // compute i = minX mod width and i = minY mod height by chinese remainder

        var iterationWithMinVariations = NumberTheory.CalcSimultaneousCongruences(
                                            (minVariationX, width),
                                            (minVariationY, height));

        // PrintRobotsAtIteration(robots, width, height, iterationWithMinVariations.Item1);
        return iterationWithMinVariations.Item1;
    }

    private static long ModWithWrapAround(long n, long mod) => n % mod >= 0 ? n % mod : n % mod + mod;

    private static void PrintRobotsAtIteration(IEnumerable<Robot> robots, long width, long height, long iteration)
    {
        var robotsEndPos = new HashSet<(long x, long y)>();

        foreach(var robot in robots)
        {
            robotsEndPos.Add((ModWithWrapAround((robot.Position + iteration * robot.Velocity).X, width),
                              ModWithWrapAround((robot.Position + iteration * robot.Velocity).Y, height)));
        }

        Console.WriteLine("Iteration = " + iteration);

        for(int y = 0; y < height; y++)
        {
            for(int x = 0; x < width; x++)
            {
                Console.Write(robotsEndPos.Contains((x, y)) ? "X" : ".");
            }
            Console.WriteLine();
        }
    }
}
