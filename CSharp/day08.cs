namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;
using matthiasffm.Common.Collections;

/// <summary>
/// Theme: match location ids by similarity score
/// </summary>
[TestFixture]
public class Day08
{
    private static IDictionary<char, List<Vec2<int>>> ParseData(string[] data)
    {
        var antennas = new Dictionary<char, List<Vec2<int>>>();

        for(var y = 0; y < data.Length; y++)
        {
            for(var x = 0; x < data[y].Length; x++)
            {
                var letter = data[y][x];
                if(letter != '.')
                {
                    if(!antennas.TryGetValue(letter, out var antennasForLetter))
                    {
                        antennasForLetter = new List<Vec2<int>>();
                        antennas[letter] = antennasForLetter;
                    }
                    antennasForLetter.Add(new Vec2<int>(x, y));
                }
            }
        }

        return antennas;
    }

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "............",
            "........0...",
            ".....0......",
            ".......0....",
            "....0.......",
            "......A.....",
            "............",
            "............",
            "........A...",
            ".........A..",
            "............",
            "............",
        };
        var antennas = ParseData(data);
        Puzzle1(antennas, data[0].Length - 1, data.Length - 1).Should().Be(14);
        Puzzle2(antennas, data[0].Length - 1, data.Length - 1).Should().Be(34);
    }

    [Test]
    public void TestAocInput()
    {
        var data     = FileUtils.ReadAllLines(this);
        var antennas = ParseData(data);

        Puzzle1(antennas, data[0].Length - 1, data.Length - 1).Should().Be(291);
        Puzzle2(antennas, data[0].Length - 1, data.Length - 1).Should().Be(1015);
    }

    // Scanning across the city, you find that there are many antennas. Each antenna is tuned to a specific frequency indicated by a single lowercase letter, uppercase letter, or
    // digit. You create a map (your puzzle input) of these antennas. The signal only applies its nefarious effect at specific antinodes based on the resonant frequencies of the
    // antennas. In particular, an antinode occurs at any point that is perfectly in line with two antennas of the same frequency - but only when one of the antennas is twice as
    // far away as the other. This means that for any pair of antennas with the same frequency, there are two antinodes, one on either side of them.
    // Antennas with different frequencies don't create antinodes; A and a count as different frequencies. However, antinodes can occur at locations that contain antennas.
    //
    // Puzzle == Calculate the impact of the signal. How many unique locations within the bounds of the map contain an antinode?
    private static long Puzzle1(IDictionary<char, List<Vec2<int>>> map, int maxX, int maxY)
        => map.Values
              .SelectMany(antennas => antennas.Variations()
                                              .SelectMany(antennaPair => AntiNodes(antennaPair.Item1, antennaPair.Item2, maxX, maxY, false, false)))
              .Distinct()
              .Count();

    // After updating your model, it turns out that an antinode occurs at any grid position exactly in line with at least two antennas of the same frequency, regardless of distance.
    // This means that some of the new antinodes will occur at the position of each antenna (unless that antenna is the only one of its frequency).
    //
    // Puzzle == Calculate the impact of the signal using this updated model. How many unique locations within the bounds of the map contain an antinode?
    private static long Puzzle2(IDictionary<char, List<Vec2<int>>> map, int maxX, int maxY)
        => map.Values
              .SelectMany(antennas => antennas.Variations()
                                              .SelectMany(antennaPair => AntiNodes(antennaPair.Item1, antennaPair.Item2, maxX, maxY, true, true)))
              .Distinct()
              .Count();

    // iterator to find all anti nodes a ka nodes with distance d=|antenna1-antenna2| or a multiple of it from antenna1 or antenna2
    private static IEnumerable<Vec2<int>> AntiNodes(Vec2<int> antenna1, Vec2<int> antenna2, int maxX, int maxY, bool repeat, bool includeAntennas)
    {
        if(includeAntennas)
        {
            yield return antenna1;
            yield return antenna2;
        }

        var dist     = antenna2 - antenna1;
        var multiple = 1;

        bool stillInside;
        do
        {
            stillInside = false;

            var antiNode = antenna1 - multiple * dist;
            if(antiNode.X >= 0 && antiNode.Y >= 0 && antiNode.X <= maxX && antiNode.Y <= maxY)
            {
                stillInside = true;
                yield return antiNode;
            }

            antiNode = antenna2 + multiple * dist;
            if(antiNode.X >= 0 && antiNode.Y >= 0 && antiNode.X <= maxX && antiNode.Y <= maxY)
            {
                stillInside = true;
                yield return antiNode;
            }

            multiple++;
        }
        while(repeat && stillInside);
    }
}
