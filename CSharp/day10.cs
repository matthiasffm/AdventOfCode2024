namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;
using matthiasffm.Common.Algorithms;

/// <summary>
/// Theme: topographic map
/// </summary>
[TestFixture]
public class Day10
{
    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "89010123",
            "78121874",
            "87430965",
            "96549874",
            "45678903",
            "32019012",
            "01329801",
            "10456732",
        };
        var map = ParseData(data);

        Puzzle1(map).Should().Be(5 + 6 + 5 + 3 + 1 + 3 + 5 + 3 + 5);
        Puzzle2(map).Should().Be(20 +  24 + 10 + 4 + 1 +4 + 5 + 8 + 5);
    }

    [Test]
    public void TestAocInput()
    {
        var data = FileUtils.ReadAllLines(this);
        var map  = ParseData(data);

        Puzzle1(map).Should().Be(737);
        Puzzle2(map).Should().Be(1619);
    }

    private static int[,] ParseData(string[] lines)
        => lines.Select(l => l.ToCharArray().Select(c => (int)c - 48).ToArray())
                .ToArray()
                .ConvertToMatrix();

    // The topographic map indicates the height at each position using a scale from 0 (lowest) to 9 (highest). Based on un-scorched scraps of the
    // book, you determine that a good hiking trail is as long as possible and has an even, gradual, uphill slope. For all practical purposes, this
    // means that a hiking trail is any path that starts at height 0, ends at height 9, and always increases by a height of exactly 1 at each step.
    // Hiking trails never include diagonal steps - only up, down, left, or right (from the perspective of the map).
    // A trailhead is any position that starts one or more hiking trails - here, these positions will always have height 0. Assembling more fragments
    // of pages, you establish that a trailhead's score is the number of 9-height positions reachable from that trailhead via a hiking trail.
    //
    // Puzzle == What is the sum of the scores of all trailheads on your topographic map?
    private static int Puzzle1(int[,] map)
        => map.Where((height, row, col) => height == 0)
              .Sum(trailhead => TrailheadScore(map, (trailhead.Item2, trailhead.Item3)));

    private static int TrailheadScore(int[,] map, (int row, int col) trailHeadStart)
        => Search.BreadthFirstEnumerate(trailHeadStart,
                                        _ => false,
                                        pos => Neighbors(map, pos.row, pos.col))
                 .Count(t => map[t.row, t.col] == 9);

    // The paper describes a second way to measure a trailhead called its rating. A trailhead's rating is the number of distinct hiking trails which begin at
    // that trailhead.
    //
    // Puzzle == What is the sum of the ratings of all trailheads?
    private static int Puzzle2(int[,] map)
        => map.Where((height, row, col) => height == 0)
              .Sum(trailhead => TrailheadRating(map, (trailhead.Item2, trailhead.Item3)));

    private static int TrailheadRating(int[,] map, (int row, int col) trailHeadStart)
    {
        var rating = 0;

        // basically to a breadth first search without the visited set
        // cycles or depth are not a problem here for the simple algorithm because every path is only including nodes with steadily increasing
        // height from 0-9, so  every path is exactly 9 steps long

        var nextToVisit = new Queue<(int row, int col)>();
        nextToVisit.Enqueue(trailHeadStart);

        do
        {
            var nextNode = nextToVisit.Dequeue();

            if(map[nextNode.row, nextNode.col] == 9)
            {
                rating++;
                continue;
            }

            foreach(var adjacent in Neighbors(map, nextNode.row, nextNode.col))
            {
                nextToVisit.Enqueue(adjacent);
            }
        }
        while(nextToVisit.Any());

        return rating;
    }

    private static IEnumerable<(int row, int col)> Neighbors(int[,] map, int row, int col)
    {
        var currentHeight = map[row, col];

        if(col > 0 && map[row, col - 1] == currentHeight + 1)
        {
            yield return (row, col - 1);
        }
        if(row > 0 && map[row - 1, col] == currentHeight + 1)
        {
            yield return (row - 1, col);
        }
        if(col < map.GetLength(1) - 1 && map[row, col + 1] == currentHeight + 1)
        {
            yield return (row, col + 1);
        }
        if(row < map.GetLength(0) - 1 && map[row + 1, col] == currentHeight + 1)
        {
            yield return (row + 1, col);
        }
    }
}
