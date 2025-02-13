namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;
using System.Collections.Generic;

using matthiasffm.Common.Algorithms;

/// <summary>
/// Theme: avoid falling RAM blocks forming a maze
/// </summary>
[TestFixture]
public class Day18
{
    private static (int X, int Y)[] ParseData(string[] data) =>
        data.Select(d => d.Split(','))
            .Select(d => (int.Parse(d[0]), int.Parse(d[1])))
            .ToArray();

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "5,4",
            "4,2",
            "4,5",
            "3,0",
            "2,1",
            "6,3",
            "2,4",
            "1,5",
            "0,6",
            "3,3",
            "2,6",
            "5,1",
            "1,2",
            "5,5",
            "2,5",
            "6,5",
            "1,4",
            "0,4",
            "6,4",
            "1,1",
            "6,1",
            "1,0",
            "0,5",
            "1,6",
            "2,0",
        };
        var blocks = ParseData(data);
        Puzzle1(blocks, 12).Should().Be(22);
        Puzzle2(blocks).Should().Be((6, 1));
    }

    [Test]
    public void TestAocInput()
    {
        var data   = FileUtils.ReadAllLines(this);
        var blocks = ParseData(data);

        Puzzle1(blocks, 1024).Should().Be(232);
        Puzzle2(blocks).Should().Be((44, 64));
    }

    // A algorithm is going to cause a byte to fall into your memory space once every nanosecond! Fortunately, you're faster, and
    // by quickly scanning the algorithm, you create a list of which bytes will fall (your puzzle input) in the order they'll land
    // in your memory space.
    // Your memory space is a two-dimensional grid. Each falling byte position is given as an X,Y coordinate, where X is the distance
    // from the left edge of your memory space and Y is the distance from the top edge of your memory space. You and The Historians are
    // currently in the top left corner of the memory space (at 0,0) and need to reach the exit in the bottom right corner.
    // You'll need to simulate the falling bytes to plan out where it will be safe to run; for now, simulate just the first few bytes
    // falling into your memory space.
    // As bytes fall into your memory space, they make that coordinate corrupted. Corrupted memory coordinates cannot be entered by you
    // or The Historians, so you'll need to plan your route carefully. You also cannot leave the boundaries of the memory space; your
    // only hope is to reach the exit. You can take steps up, down, left, or right.
    //
    // Puzzle == Simulate the first n bytes falling. Afterward, what is the minimum number of steps needed to reach the exit?
    private static int Puzzle1((int X, int Y)[] blocks, int count)
    {
        var blockMap = blocks.Take(count).ToHashSet();
        (int X, int Y) endPos = (blocks.Max(b => b.X), blocks.Max(b => b.Y));

        var path = Search.AStar<(int X, int Y), int>((0, 0),
                                                     endPos,
                                                     pos => Neighbors(pos, endPos, blockMap),
                                                     (pos, neighbor) => 1,
                                                     pos => Math.Abs(endPos.X - pos.X) + Math.Abs(endPos.Y - pos.Y),
                                                     int.MaxValue);

        return path.Count() - 1; // steps == path positions - 1 
    }

    // The Historians aren't as used to moving around in this pixelated universe as you are. You're afraid they're not going to be fast
    // enough to make it to the exit before the path is completely blocked. To determine how fast everyone needs to go, you need to
    // determine the first byte that will cut off the path to the exit.
    //
    // Puzzle == What are the coordinates of the first byte that will prevent the exit from being reachable from your starting position?
    private static (int X, int Y) Puzzle2((int X, int Y)[] blocks)
    {
        // do a binary search to find the first block where A* finds no path

        int lowerWorking = 0;
        int upperNotWorking = blocks.Length - 1;

        while(lowerWorking + 1 < upperNotWorking)
        {
            int middle = (lowerWorking + upperNotWorking) / 2;

            int pathLength = Puzzle1(blocks, middle + 1);
            if(pathLength > 0)
            {
                lowerWorking = middle;
            }
            else
            {
                upperNotWorking = middle;
            }
        }

        return blocks[upperNotWorking];
    }

    private static IEnumerable<(int X, int Y)> Neighbors((int X, int Y) pos, (int X, int Y) endPos, HashSet<(int X, int Y)> blockMap)
    {
        if(pos.X > 0 && !blockMap.Contains(pos with { X = pos.X - 1 }))
        {
            yield return pos with { X = pos.X - 1 };
        }
        if(pos.Y > 0 && !blockMap.Contains(pos with { Y = pos.Y - 1 }))
        {
            yield return pos with { Y = pos.Y - 1 };
        }
        if(pos.X < endPos.X && !blockMap.Contains(pos with { X = pos.X + 1 }))
        {
            yield return pos with { X = pos.X + 1 };
        }
        if(pos.Y < endPos.Y && !blockMap.Contains(pos with { Y = pos.Y + 1 }))
        {
            yield return pos with { Y = pos.Y + 1 };
        }
    }
}
