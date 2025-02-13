namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

// TODO: a bit slow
//       maybe a precalculated dictionary for all pattern combinations is faster

/// <summary>
/// Theme: match towel patterns to designs
/// </summary>
[TestFixture]
public class Day19
{
    private static (string[] Patterns, string[] Designs) ParseData(string[] data)
        => (data[0].Split(", "), data[2..]);

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "r, wr, b, g, bwu, rb, gb, br",
            "",
            "brwrr",
            "bggr",
            "gbbr",
            "rrbgbr",
            "ubwu",
            "bwurrg",
            "brgr",
            "bbrgwb",
        };
        var (patterns, designs) = ParseData(data);

        Puzzle1(patterns, designs).Should().Be(6);
        Puzzle2(patterns, designs).Should().Be(2 + 1 + 4 + 6 + 1 + 2);
    }

    [Test]
    public void TestAocInput()
    {
        var data                = FileUtils.ReadAllLines(this);
        var (patterns, designs) = ParseData(data);

        Puzzle1(patterns, designs).Should().Be(267);
        Puzzle2(patterns, designs).Should().Be(796449099271652L);
    }

    // Every towel at this onsen is marked with a pattern of colored stripes. There are only a few patterns, but for any particular
    // pattern, the staff can get you as many towels with that pattern as you need. Each stripe can be white (w), blue (u), black (b),
    // red (r), or green (g). So, a towel with the pattern ggr would have a green stripe, a green stripe, and then a red stripe, in
    // that order. (You can't reverse a pattern by flipping a towel upside-down, as that would cause the onsen logo to face the wrong way.)
    // The Official Onsen Branding Expert has produced a list of designs - each a long sequence of stripe colors - that they would like
    // to be able to display. You can use any towels you want, but all of the towels' stripes must exactly match the desired design. So,
    // to display the design rgrgr, you could use two rg towels and then an r towel, an rgr towel and then a gr towel, or even a single
    // massive rgrgr towel (assuming such towel patterns were actually available).
    // Not all designs will be possible with the available towels.
    //
    // Puzzle == How many designs are possible?
    private static int Puzzle1(string[] patterns, string[] designs)
        => designs.Count(design => IsPossible(design, patterns.ToHashSet()));

    // just try the patterns by recursion starting from the front
    private static bool IsPossible(string design, HashSet<string> patternSet)
    {
        if(patternSet.Contains(design))
        {
            return true;
        }
        else
        {
            foreach(var pattern in patternSet)
            {
                if(design.StartsWith(pattern))
                {
                    var test = IsPossible(design[pattern.Length..], patternSet);
                    if(test)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    // The staff don't really like some of the towel arrangements you came up with. To avoid an endless cycle of towel
    // rearrangement, maybe you should just give them every possible option. 
    //
    // Puzzle == What do you get if you add up the number of different ways you could make each design?
    private static long Puzzle2(string[] patterns, string[] designs)
    {
        g_nmbrPossibleDesigns.Clear();
        return designs.Sum(design => CountPatternCombinations(design, patterns.ToHashSet()));
    }

    private static Dictionary<string, long> g_nmbrPossibleDesigns = [];

    // solve this by extending the recursion from IsPossible with dynamic programming
    // g_nmbrPossibleDesigns stores all already calculated design pattern combinations for a fast lookup
    private static long CountPatternCombinations(string design, HashSet<string> patternSet)
    {
        if(g_nmbrPossibleDesigns.TryGetValue(design, out var nmbrPossible))
        {
            return nmbrPossible;
        }

        nmbrPossible = patternSet.Contains(design) ? 1 : 0;

        foreach(var pattern in patternSet)
        {
            if(design.StartsWith(pattern))
            {
                nmbrPossible += CountPatternCombinations(design[pattern.Length..], patternSet);
            }
        }

        g_nmbrPossibleDesigns[design] = nmbrPossible;
        return nmbrPossible;
    }
}
