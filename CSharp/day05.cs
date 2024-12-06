namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: correctly ordered safety manuals
/// </summary>
[TestFixture]
public class Day05
{
    private static (ILookup<int, int> before, int[][] updates) ParseData(string[] lines)
    {
        var parts = string.Join('#', lines).Split("##");
        return (parts[0].Split('#')
                        .Select(l => l.Split('|'))
                        .ToLookup(order => int.Parse(order[1]), order => int.Parse(order[0])),
                parts[1].Split('#')
                        .Select(l => l.Split(',').Select(n => int.Parse(n)).ToArray())
                        .ToArray());
    }

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "47|53",
            "97|13",
            "97|61",
            "97|47",
            "75|29",
            "61|13",
            "75|53",
            "29|13",
            "97|29",
            "53|29",
            "61|53",
            "97|53",
            "61|29",
            "47|13",
            "75|47",
            "97|75",
            "47|61",
            "75|61",
            "47|29",
            "75|13",
            "53|13",
            "",
            "75,47,61,53,29",
            "97,61,53,29,13",
            "75,29,13",
            "75,97,47,61,53",
            "61,13,29",
            "97,13,75,29,47",
        };
        var (order, updates) = ParseData(data);
        Puzzle1(order, updates).Should().Be(61 + 53 + 29);
        Puzzle2(order, updates).Should().Be(47 + 29 + 47);
    }

    [Test]
    public void TestAocInput()
    {
        var data             = FileUtils.ReadAllLines(this);
        var (order, updates) = ParseData(data);

        Puzzle1(order, updates).Should().Be(4996);
        Puzzle2(order, updates).Should().Be(6311);
    }

    // The new sleigh launch safety manual updates won't print correctly. Safety protocols clearly indicate that new pages for the safety
    // manuals must be printed in a very specific order. The notation X|Y means that if both page number X and page number Y are to be
    // produced as part of an update, page number X must be printed at some point before page number Y.
    // The Elf has for you both the page ordering rules and the pages to produce in each update (your puzzle input), but can't figure out
    // whether each update has the pages in the right order. To get the printers going as soon as possible, start by identifying which updates
    // are already in the right order.
    // For some reason, the Elves also need to know the middle page number of each update being printed. Because you are currently only printing
    // the correctly-ordered updates, you will need to find the middle page number of each correctly-ordered update.
    //
    // Puzzle == Determine which updates are already in the correct order. What do you get if you add up the middle page number from those
    //           correctly-ordered updates?
    private static int Puzzle1(ILookup<int, int> before, int[][] updates)
        => updates.Where(update => IsOrdered(update, new BeforeComparer(before)))
                  .Sum(update => update[update.Length / 2]);

    private static bool IsOrdered<T>(IEnumerable<T> list, IComparer<T> comparer)
        => list.Zip(list.Skip(1))
               .All(t => comparer.Compare(t.First, t.Second) < 0);

    // For each of the incorrectly-ordered updates, use the page ordering rules to put the page numbers in the right order.
    //
    // Puzzle == Find the updates which are not in the correct order. What do you get if you add up the middle page numbers after correctly ordering
    //           just those updates?
    private static int Puzzle2(ILookup<int, int> before, int[][] updates)
        => updates.Where(update => !IsOrdered(update, new BeforeComparer(before)))
                  .Select(iu => iu.OrderBy(x => x, new BeforeComparer(before))
                                  .ToArray())
                  .Sum(update => update[update.Length / 2]);

    // uses the informationen in the before lookup to implement the Compare method
    internal class BeforeComparer : IComparer<int>
    {
        private readonly ILookup<int, int> _before;

        internal BeforeComparer(ILookup<int, int> before)
        {
            _before = before;
        }

        public int Compare(int x, int y)
        {
            if(_before.Contains(y) && _before[y].Contains(x))
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }
}
