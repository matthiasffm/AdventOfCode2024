namespace AdventOfCode2024;

using FluentAssertions;
using matthiasffm.Common.Collections;
using NUnit.Framework;

/// <summary>
/// Theme: safe 
/// </summary>
[TestFixture]
public class Day02
{
    private static int[][] ParseData(string[] data) =>
        data.Select(l => l.Split(' ').Select(level => int.Parse(level)).ToArray())
            .ToArray();

    [Test]
    public void TestSamples()
    {
        var data = new[] {
            "7 6 4 2 1",
            "1 2 7 8 9",
            "9 7 6 2 1",
            "1 3 2 4 5",
            "8 6 4 4 1",
            "1 3 6 7 9",
        };
        var reports = ParseData(data);
        Puzzle1(reports).Should().Be(2);
        Puzzle2(reports).Should().Be(4);
    }

    [Test]
    public void TestAocInput()
    {
        var data = FileUtils.ReadAllLines(this);
        var reports = ParseData(data);

        Puzzle1(reports).Should().Be(660);
        Puzzle2(reports).Should().Be(689);
    }

    // The unusual data (your puzzle input) consists of many reports, one report per line. Each report is a list of numbers called levels that
    // are separated by spaces. The engineers are trying to figure out which reports are safe. The Red-Nosed reactor safety systems can only
    // tolerate levels that are either gradually increasing or gradually decreasing. A report us only safe if both of the following are true:
    // The levels are either all increasing or all decreasing. Any two adjacent levels differ by at least one and at most three.
    //
    // Puzzle == Analyze the unusual data from the engineers. How many reports are safe?
    private static int Puzzle1(int[][] reports)
        => reports.Count(r => IsSafe(Derivative(r)));

    // The engineers are surprised by the low number of safe reports until they realize they forgot to tell you about the Problem Dampener. This dampener
    // is a reactor-mounted module that lets the reactor safety systems tolerate a single bad level in what would otherwise be a safe report. Now, the
    // same rules apply as before, except if removing a single level from an unsafe report would make it safe, the report instead counts as safe.
    //
    // Puzzle == Update your analysis by handling situations where the Problem Dampener can remove a single level from unsafe reports. How many reports are now safe?
    private static int Puzzle2(int[][] reports)
        => reports.Count(r => Enumerable.Range(0, reports.Length)
                                        .Any(i => IsSafe(Derivative(r.Take(i).Concat(r.Skip(i + 1))))));

    private static IEnumerable<int> Derivative(IEnumerable<int> level)
        => level.SkipLast(1)
                .Zip(level.Skip(1))
                .Select(l => l.Second- l.First)
                .ToArray();

    private static bool IsSafe(IEnumerable<int> dL) => dL.All(d => IsSafe(d)) || dL.All(d => IsSafe(-d));
    private static bool IsSafe(int dL) => dL >= 1 && dL <= 3;
}
