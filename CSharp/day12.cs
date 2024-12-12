namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;
using matthiasffm.Common.Collections;

// TODO:
// at least one order of magnitude too slow
// try to avoid the .Variations() call
// maybe order the fence by adjance and then merge them
// code is also ugly and complicated => simplify

/// <summary>
/// Theme: gardening and fencing
/// </summary>
[TestFixture]
public class Day12
{
    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "AAAA",
            "BBCD",
            "BBCC",
            "EEEC",
        };
        var garden = ParseData(data);
        Puzzle1(garden).Should().Be(4 * 10 + 4 * 8 + 4 * 10 + 1 * 4 + 3 * 8);
        Puzzle2(garden).Should().Be(16 + 16 + 32 + 4 + 12);

        data = [
            "OOOOO",
            "OXOXO",
            "OOOOO",
            "OXOXO",
            "OOOOO",
        ];
        garden = ParseData(data);
        Puzzle1(garden).Should().Be(21 * 36 + 1 * 4 + 1 * 4 + 1 * 4 + 1 * 4);

        data = [
            "RRRRIICCFF",
            "RRRRIICCCF",
            "VVRRRCCFFF",
            "VVRCCCJFFF",
            "VVVVCJJCFE",
            "VVIVCCJJEE",
            "VVIIICJJEE",
            "MIIIIIJJEE",
            "MIIISIJEEE",
            "MMMISSJEEE",
        ];
        garden = ParseData(data);
        Puzzle1(garden).Should().Be(216 + 32 + 392 + 180 + 260 + 220 + 4 + 234 + 308 + 60 + 24);
        Puzzle2(garden).Should().Be(1206);

        data = [
            "EEEEE",
            "EXXXX",
            "EEEEE",
            "EXXXX",
            "EEEEE",
        ];
        garden = ParseData(data);
        Puzzle2(garden).Should().Be(236);

        data = [
            "AAAAAA",
            "AAABBA",
            "AAABBA",
            "ABBAAA",
            "ABBAAA",
            "AAAAAA",
        ];
        garden = ParseData(data);
        Puzzle2(garden).Should().Be(368);
    }

    [Test]
    public void TestAocInput()
    {
        var data    = FileUtils.ReadAllLines(this);
        var garden  = ParseData(data);

        Puzzle1(garden).Should().Be(1361494);
        Puzzle2(garden).Should().Be(830516);
    }

    private static char[,] ParseData(string[] lines)
        => lines.Select(l => l.ToCharArray())
                .ToArray()
                .ConvertToMatrix();

    // The Elves would like to set up fences around each region of garden plots, but they can't figure out how much fence they need to order or how much it
    // will cost. They hand you a map (your puzzle input) of the garden plots. Each garden plot grows only a single type of plant and is indicated by a single
    // letter on your map. When multiple garden plots are growing the same type of plant and are touching (horizontally or vertically), they form a region.
    // In order to accurately calculate the cost of the fence around a single region, you need to know that region's area and perimeter. The area of a region
    // is simply the number of garden plots the region contains. Each garden plot is a square and so has four sides. The perimeter of a region is the number
    // of sides of garden plots in the region that do not touch another garden plot in the same region.
    // Due to "modern" business practices, the price of fence required for a region is found by multiplying that region's area by its perimeter. The total price
    // of fencing all regions on a map is found by adding together the price of fence for every region on the map.
    //
    // Puzzle == What is the total price of fencing all regions on your map?
    private static int Puzzle1(char[,] garden)
        => garden.Select((plantType, row, col) => (plantType, row, col))
                 .ToLookup(g => g.plantType, g => (g.row, g.col))
                 .Sum(plotsByPlantType => MergePlotsForPlantType(garden, plotsByPlantType)
                                              .Sum(int (region) => FenceLength(region, garden) * region.Count));

    // fence length == Sum(4 - nr_of_neighbors(parcel))
    private static int FenceLength(HashSet<(int row, int col)> region, char[,] garden)
        => region.Sum(pos => 4 - Neighbors(pos, garden).Count(n => region.Contains(n)));

    // Fortunately, the Elves are trying to order so much fence that they qualify for a bulk discount! Under the bulk discount, instead of using the perimeter
    // to calculate the price, you need to use the number of sides each region has. Each straight section of fence counts as a side, regardless of how long it is.
    //
    // Puzzle == What is the new total price of fencing all regions on your map?
    private static int Puzzle2(char[,] garden)
        => garden.Select((plantType, row, col) => (plantType, row, col))
                 .ToLookup(g => g.plantType, g => (g.row, g.col))
                 .Sum(plotsByPlantType => MergePlotsForPlantType(garden, plotsByPlantType)
                                              .Sum(int (region) => FenceSides(region, garden) * region.Count));

    private static int FenceSides(HashSet<(int row, int col)> region, char[,] garden)
    {
        // add the 4 fences for a single pos to a hashset, ignore fences added twice (these are adjacent inner positions)

        HashSet<(int startRow, int startCol, int endRow, int endCol)> fenceElements = [];

        foreach(var pos in region)
        {
            TryAddFence(fenceElements, pos.row,     pos.col,     pos.row,     pos.col + 1);
            TryAddFence(fenceElements, pos.row,     pos.col + 1, pos.row + 1, pos.col + 1);
            TryAddFence(fenceElements, pos.row + 1, pos.col + 1, pos.row + 1, pos.col);
            TryAddFence(fenceElements, pos.row + 1, pos.col,     pos.row,     pos.col);
        }

        // try to merge 2 adjacent fences until not longer possible

        bool fenceMerged;

        do
        {
            fenceMerged = false;

            foreach(var (left, right) in fenceElements.Variations())
            {
                if(IsVertical(left) && IsVertical(right) && left.startCol == right.startCol)
                {
                    if(left.endRow == right.startRow)
                    {
                        fenceElements.Add((left.startRow, left.startCol, right.endRow, right.endCol));
                        fenceElements.Remove(left);
                        fenceElements.Remove(right);
                        fenceMerged = true;
                        break;
                    }
                    else if(right.endRow == left.startRow)
                    {
                        fenceElements.Add((right.startRow, right.startCol, left.endRow, left.endCol));
                        fenceElements.Remove(left);
                        fenceElements.Remove(right);
                        fenceMerged = true;
                        break;
                    }
                }
                else if(!IsVertical(left) && !IsVertical(right) && left.startRow == right.startRow)
                {
                    if(left.endCol == right.startCol)
                    {
                        fenceElements.Add((left.startRow, left.startCol, right.endRow, right.endCol));
                        fenceElements.Remove(left);
                        fenceElements.Remove(right);
                        fenceMerged = true;
                        break;
                    }
                    else if(right.endCol == left.startCol)
                    {
                        fenceElements.Add((right.startRow, right.startCol, left.endRow, left.endCol));
                        fenceElements.Remove(left);
                        fenceElements.Remove(right);
                        fenceMerged = true;
                        break;
                    }
                }
            }
        }
        while(fenceMerged);

        return fenceElements.Count;
    }

    private static bool IsVertical((int startRow, int startCol, int endRow, int endCol) fence)
        => fence.startCol == fence.endCol;

    // eliminates inner 'fences' which are not really fences (inner 'fences' == are always added twice)
    private static void TryAddFence(HashSet<(int startRow, int startCol, int endRow, int endCol)> fenceElements, int startRow, int startCol, int endRow, int endCol)
    {
        (int, int, int, int) registeredFence;
        if(fenceElements.TryGetValue((startRow, startCol, endRow, endCol), out registeredFence) ||
           fenceElements.TryGetValue((endRow, endCol, startRow, startCol), out registeredFence))
        {
            fenceElements.Remove(registeredFence);
        }
        else
        {
            fenceElements.Add((startRow, startCol, endRow, endCol));
        }
    }

    // takes all positions for a letter and merges them into the smallest possible number of continous regions
    private static List<HashSet<(int row, int col)>> MergePlotsForPlantType(char[,] garden, IGrouping<char, (int row, int col)> positions)
    {
        List<HashSet<(int row, int col)>> plots = [];

        foreach (var plot in positions)
        {
            var posNeighbors = Neighbors(plot, garden);
            int nmbrNeighboringRegions = 0;
            HashSet<(int row, int col)>? toMergeInto = null;
            foreach (var neighboringRegion in plots.ToList().Where(region => posNeighbors.Any(neighbor => region.Contains(neighbor))))
            {
                nmbrNeighboringRegions++;
                if (nmbrNeighboringRegions == 1)
                {
                    toMergeInto = neighboringRegion;
                }
                else
                {
                    toMergeInto!.AddRange(neighboringRegion);
                    plots.Remove(neighboringRegion);
                }
            }
            if (nmbrNeighboringRegions == 0)
            {
                plots.Add([plot]);
            }
            else
            {
                toMergeInto!.Add(plot);
            }
        }

        return plots;
    }

    private static IEnumerable<(int row, int col)> Neighbors((int row, int col) pos, char[,] garden)
    {
        if(pos.row > 0)
        {
            yield return (pos.row - 1, pos.col);
        }
        if(pos.col > 0)
        {
            yield return (pos.row, pos.col - 1);
        }
        if(pos.row < garden.GetLength(0) - 1)
        {
            yield return (pos.row + 1, pos.col);
        }
        if(pos.col < garden.GetLength(1) - 1)
        {
            yield return (pos.row, pos.col + 1);
        }
    }
}
