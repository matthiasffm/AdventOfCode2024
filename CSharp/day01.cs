namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: match location ids by similarity score
/// </summary>
[TestFixture]
public class Day01
{
    private static (long[], long[]) ParseData(string[] data) =>
        (data.Select(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
             .Select(split => long.Parse(split[0])).ToArray(),
         data.Select(s => s.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
             .Select(split => long.Parse(split[1])).ToArray());

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "3   4",
            "4   3",
            "2   5",
            "1   3",
            "3   9",
            "3   3",
        };
        var idLists = ParseData(data);
        Puzzle1(idLists.Item1, idLists.Item2).Should().Be(2 + 1 + 0 + 1 + 2 + 5);
        Puzzle2(idLists.Item1, idLists.Item2).Should().Be(9 + 4 + 0 + 0 + 9 + 9);
    }

    [Test]
    public void TestAocInput()
    {
        var data    = FileUtils.ReadAllLines(this);
        var idLists = ParseData(data);

        Puzzle1(idLists.Item1, idLists.Item2).Should().Be(1941353L);
        Puzzle2(idLists.Item1, idLists.Item2).Should().Be(22539317L);
    }

    // Throughout the Chief's office, the historically significant locations are listed not by name but by a unique number called the location ID. To make
    // sure they don't miss anything, The Historians split into two groups, each searching the office and trying to create their own complete list of location
    // IDs. There's just one problem: by holding the two lists up side by side (your puzzle input), it quickly becomes clear that the lists aren't very similar.
    // Maybe the lists are only off by a small amount! To find out, pair up the numbers and measure how far apart they are. Pair up the smallest number in the
    // left list with the smallest number in the right list, then the second-smallest left number with the second-smallest right number, and so on. Within each
    // pair, figure out how far apart the two numbers are; you'll need to add up all of those distances. 
    //
    // Puzzle == Your actual left and right lists contain many location IDs. What is the total distance between your lists?
    private static long Puzzle1(IEnumerable<long> left, IEnumerable<long> right)
        => left.Order()
               .Zip(right.Order())
               .Select(t => Math.Abs(t.First - t.Second))
               .Sum();

    // This time, you'll need to figure out exactly how often each number from the left list appears in the right list. Calculate a total similarity score by
    // adding up each number in the left list after multiplying it by the number of times that number appears in the right list.
    //
    // Puzzle == Once again consider your left and right lists. What is their similarity score?
    private static long Puzzle2(IEnumerable<long> left, IEnumerable<long> right)
    {
        // return right.Select(r => left.Where(l => l == r).Sum()).Sum();
        var rightOccurences = right.GroupBy(r => r).ToDictionary(g => g.Key, g => g.Count());

        return left.Select(l  => rightOccurences.ContainsKey(l) ? l * rightOccurences[l] : 0)
                   .Sum();
    }
}
