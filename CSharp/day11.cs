namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: stones shift if you blink
/// </summary>
[TestFixture]
public class Day11
{
    [Test]
    public void TestSamples()
    {
        Puzzle(ParseData("0 1 10 99 999"), 1).Should().Be(7);

        var data   = "125 17";
        var stones = ParseData(data);

        Puzzle(stones, 6).Should().Be(22L);
        Puzzle(stones, 25).Should().Be(55312L);
    }

    [Test]
    public void TestAocInput()
    {
        var data   = FileUtils.ReadAllText(this);
        var stones = ParseData(data);

        Puzzle(stones, 25).Should().Be(186424L);
        Puzzle(stones, 75).Should().Be(219838428124832L);
    }

    private static long[] ParseData(string data)
        => data.Split(' ', StringSplitOptions.TrimEntries)
               .Select(s => long.Parse(s))
               .ToArray();

    // Each stone has a number engraved on it. The strange part is that every time you blink, the stones change.
    // Sometimes, the number engraved on a stone changes. Other times, a stone might split in two, causing all the other stones to shift over a bit to make
    // room in their perfectly straight line. As you observe them for a while, you find that the stones have a consistent behavior. Every time you blink, the
    // stones each simultaneously change according to the first applicable rule in this list:
    // - If the stone is engraved with the number 0, it is replaced by a stone engraved with the number 1.
    // - If the stone is engraved with a number that has an even number of digits, it is replaced by two stones. The left half of the digits are engraved on the
    //   new left stone, and the right half of the digits are engraved on the new right stone. (The new numbers don't keep extra leading zeroes: 1000 would
    //   become stones 10 and 0.)
    // - If none of the other rules apply, the stone is replaced by a new stone; the old stone's number multiplied by 2024 is engraved on the new stone.
    // No matter how the stones change, their order is preserved, and they stay on their perfectly straight line.
    //
    // Puzzle == Consider the arrangement of stones in front of you. How many stones will you have after blinking n times?
    private static long Puzzle(long[] stones, int blinks)
    {
        var countCache = new Dictionary<(long stone, int interation), long>();
        return stones.Sum(stone => CountStones(stone, blinks, countCache));
    }

    // solve this by dynamic programming + recursion over the blink iteration
    private static long CountStones(long stone, int timesToBlink, Dictionary<(long stone, int iteration), long> countCache)
    {
        if(countCache.TryGetValue((stone, timesToBlink), out var stoneCount))
        {
            return stoneCount;
        }

        if(timesToBlink == 0)
        {
            stoneCount = 1;
        }
        else
        {
            var digits = stone.Digits();
            stoneCount = digits switch {
                        1 when stone == 0         => CountStones(1, timesToBlink - 1, countCache),
                        > 1 when digits % 2 == 0  => CountStones(SplitLeft(stone, digits / 2), timesToBlink - 1, countCache) + 
                                                     CountStones(SplitRight(stone, digits / 2), timesToBlink - 1, countCache),
                        _                         => CountStones(stone * 2024L, timesToBlink - 1, countCache),
                    };
        }

        countCache.Add((stone, timesToBlink), stoneCount);
        return stoneCount;
    }

    private static long SplitLeft(long stone, int digits)
        => stone / 10.Pow(digits);

    private static long SplitRight(long stone, int digits)
        => stone % 10.Pow(digits);
}
