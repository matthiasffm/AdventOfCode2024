namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;
using matthiasffm.Common.Algorithms;
using matthiasffm.Common.Collections;

// TODO: too slow (part 2 takes minutes)
//       maybe Diijkstra from start and from end for all nodes would be faster

/// <summary>
/// Theme: reindeer maze
/// </summary>
[TestFixture]
public class Day16
{
    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "###############",
            "#.......#....E#",
            "#.#.###.#.###.#",
            "#.....#.#...#.#",
            "#.###.#####.#.#",
            "#.#.#.......#.#",
            "#.#.#####.###.#",
            "#...........#.#",
            "###.#.#####.#.#",
            "#...#.....#.#.#",
            "#.#.#.###.#.#.#",
            "#.....#...#.#.#",
            "#.###.#.#.#.#.#",
            "#S..#.....#...#",
            "###############",
        };
        var (map, start, end) = ParseData(data);

        Puzzle1(map, start, end).Should().Be(7036);
        Puzzle2(map, start, end, 7036).Should().Be(45);

        data = new [] {
            "#################",
            "#...#...#...#..E#",
            "#.#.#.#.#.#.#.#.#",
            "#.#.#.#...#...#.#",
            "#.#.#.#.###.#.#.#",
            "#...#.#.#.....#.#",
            "#.#.#.#.#.#####.#",
            "#.#...#.#.#.....#",
            "#.#.#####.#.###.#",
            "#.#.#.......#...#",
            "#.#.###.#####.###",
            "#.#.#...#.....#.#",
            "#.#.#.#####.###.#",
            "#.#.#.........#.#",
            "#.#.#.#########.#",
            "#S#.............#",
            "#################",
        };
        (map, start, end) = ParseData(data);

        Puzzle1(map, start, end).Should().Be(11048);
        Puzzle2(map, start, end, 11048).Should().Be(64);
    }

    [Test]
    public void TestAocInput()
    {
        var data              = FileUtils.ReadAllLines(this);
        var (map, start, end) = ParseData(data);

        Puzzle1(map, start, end).Should().Be(133584);
        Puzzle2(map, start, end, 133584).Should().Be(622);
    }

    private static (char[,] map, (int row, int col) start, (int row, int col) end) ParseData(string[] lines)
    {
        var map = ParseMap(lines);

        var start = map.Where((c, row, col) => c == 'S').First();
        var end   = map.Where((c, row, col) => c == 'E').First();

        return (map, (start.Item2, start.Item3), (end.Item2, end.Item3));
    }

    private static char[,] ParseMap(string[] lines)
        => lines.Select(l => l.ToCharArray())
                .ToArray()
                .ConvertToMatrix();

    // This year, the big event is the Reindeer Maze, where the Reindeer compete for the lowest score. The Reindeer start on the start tile (marked S) facing
    // East and need to reach the end tile (marked E). They can move forward one tile at a time (increasing their score by 1 point), but never into a wall (#).
    // They can also rotate clockwise or counterclockwise 90 degrees at a time (increasing their score by 1000 points).
    //
    // Puzzle == Analyze your map carefully. What is the lowest score a Reindeer could possibly get?
    private static int Puzzle1(char[,] map, (int row, int col) start, (int row, int col) end)
    {
        var path = Search.AStar((start.row, start.col, 'E'),
                                pos => pos.row == end.row && pos.col == end.col,
                                pos => Neighbors(map, pos),
                                Costs,
                                pos => Math.Abs(end.row - pos.row) + Math.Abs(end.col - pos.col),
                                int.MaxValue);

        return CostOfPath(path);
    }

    // Now that you know what the best paths look like, you can figure out the best spot to sit. Every non-wall tile (S, ., or E) is equipped with places to sit
    // along the edges of the tile. While determining which of these tiles would be the best spot to sit depends on a whole bunch of factors, the most important
    // factor is whether the tile is on one of the best paths through the maze. If you sit somewhere else, you'd miss all the action!
    // So, you'll need to determine which tiles are part of any best path through the maze, including the S and E tiles.
    //
    // Puzzle == Analyze your map further. How many tiles are part of at least one of the best paths through the maze?
    private static int Puzzle2(char[,] map, (int row, int col) start, (int row, int col) end, int bestPathCost)
    {
        HashSet<(int row, int col)> allNodesOnBestPaths = [];

        // for all nodes n(x) <> ".ES" find the best paths start-n(x) and n(x)-end so that their sum equals bestPathCost

        for(int row = 1; row < map.GetLength(0); row++)
        {
            for(int col = 1; col < map.GetLength(1); col++)
            {
                if(map[row, col] == '.')
                {
                    var pathFromStartToN = Search.AStar((start.row, start.col, 'E'),
                                                        pos => pos.row == row && pos.col == col,
                                                        pos => Neighbors(map, pos),
                                                        Costs,
                                                        pos => Math.Abs(row - pos.row) + Math.Abs(col - pos.col),
                                                        int.MaxValue);
                    var costFromStartToN = CostOfPath(pathFromStartToN);
                    if(pathFromStartToN.Any() && costFromStartToN < bestPathCost)
                    {
                        var pathFromNToEnd = Search.AStar(pathFromStartToN.Last(),
                                                          pos => pos.row == end.row && pos.col == end.col,
                                                          pos => Neighbors(map, pos),
                                                          Costs,
                                                          pos => Math.Abs(end.row - pos.row) + Math.Abs(end.col - pos.col),
                                                          int.MaxValue);
                        var costFromNToEnd = CostOfPath(pathFromNToEnd);
                        if(costFromStartToN + costFromNToEnd == bestPathCost)
                        {
                            allNodesOnBestPaths.AddRange(pathFromStartToN.Select(pos => (pos.row, pos.col)));
                            allNodesOnBestPaths.AddRange(pathFromNToEnd.Select(pos => (pos.row, pos.col)));
                        }
                    }
                }
            }
        }

        return allNodesOnBestPaths.Count;
    }


    private static IEnumerable<(int row, int col, char dir)> Neighbors(char[,] map, (int row, int col, char dir) pos)
    {
        // north
        if(pos.row > 0 && map[pos.row - 1, pos.col] != '#')
        {
            yield return (pos.row - 1, pos.col, 'N');
        }

        // east
        if(pos.col < map.GetLength(1) - 1 && map[pos.row, pos.col + 1] != '#')
        {
            yield return (pos.row, pos.col + 1, 'E');
        }

        // south
        if(pos.row < map.GetLength(0) - 1 && map[pos.row + 1, pos.col] != '#')
        {
            yield return (pos.row + 1, pos.col, 'S');
        }

        // west
        if(pos.col > 0 && map[pos.row, pos.col - 1] != '#')
        {
            yield return (pos.row, pos.col - 1, 'W');
        }
    }

    private static int Costs((int row, int col, char dir) pos1, (int row, int col, char dir) pos2)
    {
        var cost = 0;

        switch(pos1.dir)
        {
            case 'N':
            case 'S':
                cost += pos2.dir switch {
                    'E' or 'W' => 1000,
                    _          => (pos1.dir == pos2.dir) ? 0 : 2000,
                };
                break;

            case 'E':
            case 'W':
                cost += pos2.dir switch {
                    'N' or 'S' => 1000,
                    _          => (pos1.dir == pos2.dir) ? 0 : 2000,
                };
                break;
        }

        cost += Math.Abs(pos2.row - pos1.row) + Math.Abs(pos2.col - pos1.col);

        return cost;
    }

    private static int CostOfPath(IEnumerable<(int row, int col, char)> path)
        => path.Pairs().Sum(p => Costs(p.Item1, p.Item2));
}
