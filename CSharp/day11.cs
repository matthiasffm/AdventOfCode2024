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

    private static string[] ParseData(string data) => data.Split(' ', StringSplitOptions.TrimEntries);

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
    private static long Puzzle(string[] stones, int blinks)
    {
        var countCache = new Dictionary<(string stone, int interation), long>();
        return stones.Sum(s => CountStones(s, blinks, countCache));
    }

    // solve this by dynamic programming + recursion over the blink iteration
    private static long CountStones(string stone, int timesToBlink, Dictionary<(string stone, int iteration), long> countCache)
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
            stoneCount = stone.Length switch {
                        1 when stone == "0"             => CountStones("1", timesToBlink - 1, countCache),
                        > 1 when stone.Length % 2 == 0  => CountStones(stone[..(stone.Length / 2)], timesToBlink - 1, countCache) + 
                                                           CountStones(TrimLeadingZeroes(stone[(stone.Length / 2)..]), timesToBlink - 1, countCache),
                        _                               => CountStones(MultiplyWith2024(stone), timesToBlink - 1, countCache),
                    };
        }

        countCache.Add((stone, timesToBlink), stoneCount);
        return stoneCount;
    }

    // splitting 1000 in two gives 10, 00 but the second one should be a single 0
    private static string TrimLeadingZeroes(string s)
    {
        var trimmed = s.TrimStart('0');
        return trimmed.Length > 0 ? trimmed : "0";
    }

    // result is always lower then long.MaxValue for all input values here
    private static string MultiplyWith2024(string stone)
        => (long.Parse(stone) * 2024L).ToString();
}
