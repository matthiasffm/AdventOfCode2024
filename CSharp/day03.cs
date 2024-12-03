namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;
using System.Text.RegularExpressions;

/// <summary>
/// Theme: identify and execute multiply instructions
/// </summary>
[TestFixture]
public partial class Day03
{
    [Test]
    public void TestSamples()
    {
        var data = "xmul(2,4)%&mul[3,7]!@^do_not_mul(5,5)+mul(32,64]then(mul(11,8)mul(8,5))";
        Puzzle1(data).Should().Be(2*4 + 5*5 + 11*8 + 8*5);
        data = "xmul(2,4)&mul[3,7]!^don't()_mul(5,5)+mul(32,64](mul(11,8)undo()?mul(8,5))";
        Puzzle2(data).Should().Be(2*4 + 8*5);
    }

    [Test]
    public void TestAocInput()
    {
        var data = FileUtils.ReadAllText(this);
        Puzzle1(data).Should().Be(187194524L);
        Puzzle2(data).Should().Be(127092535L);
    }

    // The shopkeepers computer appears to be trying to run a program, but its memory (your puzzle input) is corrupted. All of the instructions have
    // been jumbled up! It seems like the goal of the program is just to multiply some numbers. It does that with instructions like mul(X,Y), where X
    // and Y are each 1-3 digit numbers. For instance, mul(44,46) multiplies 44 by 46 to get a result of 2024. Similarly, mul(123,4) would multiply 123
    // by 4. However, because the program's memory has been corrupted, there are also many invalid characters that should be ignored, even if they look
    // like part of a mul instruction. Sequences like mul(4*, mul(6,9!, ?(12,34), or mul ( 2 , 4 ) do nothing.
    //
    // Puzzle == Scan the corrupted memory for uncorrupted mul instructions. What do you get if you add up all of the results of the multiplications?
    private static long Puzzle1(string instructions)
        => GeneratedRegexPuzzle1().Matches(instructions)
                                  .Sum(m => long.Parse(m.Groups[2].Value) * long.Parse(m.Groups[3].Value));

    [GeneratedRegex(@"mul\(((\d{1,3}),(\d{1,3}))\)")]
    private static partial Regex GeneratedRegexPuzzle1();

    // As you scan through the corrupted memory, you notice that some of the conditional statements are also still intact. If you handle some of the
    // uncorrupted conditional statements in the program, you might be able to get an even more accurate result.
    // There are two new instructions you'll need to handle: The do() instruction enables future mul instructions.The don't() instruction disables
    // future mul instructions. Only the most recent do() or don't() instruction applies. At the beginning of the program, mul instructions are enabled.
    //
    // Puzzle == Handle the new instructions; what do you get if you add up all of the results of just the enabled multiplications?
    private static long Puzzle2(string instructions)
        => GeneratedRegexPuzzle2().Matches(instructions)
                                  .Aggregate((enabled:true, prod:0L), (aggr, match) => match.Groups[0].Value switch {
                                      "don't"                     => (false, aggr.prod),
                                      "do"                        => (true,  aggr.prod),
                                      var other when aggr.enabled => (true,  aggr.prod + long.Parse(match.Groups[2].Value) *
                                                                                         long.Parse(match.Groups[3].Value)),
                                      _                           => aggr,
                                   }).prod;

    [GeneratedRegex(@"mul\(((\d{1,3}),(\d{1,3}))\)|don't|do")]
    private static partial Regex GeneratedRegexPuzzle2();
}
