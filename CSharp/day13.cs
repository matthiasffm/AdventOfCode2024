namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: solve claw machines (i could use that in Yakuza)
/// </summary>
[TestFixture]
public class Day13
{
    private record Config((long X, long Y) DiffA, (long X, long Y) DiffB, (long X, long Y) PricePos);

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "Button A: X+94, Y+34",
            "Button B: X+22, Y+67",
            "Prize: X=8400, Y=5400",
            "",
            "Button A: X+26, Y+66",
            "Button B: X+67, Y+21",
            "Prize: X=12748, Y=12176",
            "",
            "Button A: X+17, Y+86",
            "Button B: X+84, Y+37",
            "Prize: X=7870, Y=6450",
            "",
            "Button A: X+69, Y+23",
            "Button B: X+27, Y+71",
            "Prize: X=18641, Y=10279",
        };
        var configs = ParseData(data);
        Puzzle1(configs).Should().Be(280 + 0 + 200 + 0);
    }

    [Test]
    public void TestAocInput()
    {
        var data    = FileUtils.ReadAllLines(this);
        var configs = ParseData(data);

        Puzzle1(configs).Should().Be(29436L);
        Puzzle2(configs).Should().Be(103729094227877L);
    }

    private static IEnumerable<Config> ParseData(string[] lines)
    {
        var parts = string.Join('#', lines).Split("##");

        return parts.Select(part => part.Split('#'))
                    .Select(part => new Config(ParseXY(part[0].Split(['+', ','])), ParseXY(part[1].Split(['+', ','])), ParseXY(part[2].Split(['=', ',']))));
    }

    private static (long X, long Y) ParseXY(string[] xyParts) => new(long.Parse(xyParts[1]), long.Parse(xyParts[3]));

    // You are bored and try to win some prices from the claw machines in the meantime. These machines here are a little unusual. Instead of a joystick or directional
    // buttons to control the claw, these machines have two buttons labeled A and B. Worse, you can't just put in a token and play; it costs 3 tokens to push the A
    // button and 1 token to push the B button. With a little experimentation, you figure out that each machine's buttons are configured to move the claw a specific
    // amount to the right (along the X axis) and a specific amount forward (along the Y axis) each time that button is pressed. Each machine contains one prize; to
    // win the prize, the claw must be positioned exactly above the prize on both the X and Y axes.
    // You wonder: what is the smallest number of tokens you would have to spend to win as many prizes as possible? You assemble a list of every machine's button behavior
    // and prize location (your puzzle input).
    //
    // Puzzle == Figure out how to win as many prizes as possible. What is the fewest tokens you would have to spend to win all possible prizes?
    private static long Puzzle1(IEnumerable<Config> configs)
        => configs.Select(c => Solve(c.PricePos.X, c.DiffA.X, c.DiffB.X, c.PricePos.Y, c.DiffA.Y, c.DiffB.Y))
                  .Where(s => s.solvable)
                  .Sum(s => s.x * 3L + s.y);

    // As you go to win the first prize, you discover that the claw is nowhere near where you expected it would be. Due to a unit conversion error in your measurements, the
    // position of every prize is actually 10000000000000 higher on both the X and Y axis!
    //
    // Puzzle == Using the corrected prize coordinates, figure out how to win as many prizes as possible. What is the fewest tokens you would have to spend to win all possible prizes?
    private static long Puzzle2(IEnumerable<Config> configs)
        => configs.Select(c => Solve(10000000000000L + c.PricePos.X, c.DiffA.X, c.DiffB.X,
                                     10000000000000L + c.PricePos.Y, c.DiffA.Y, c.DiffB.Y))
                  .Where(s => s.solvable)
                  .Sum(s => s.x * 3L + s.y);

    // where do these two linear functions (for X and Y) cross?
    // m = a*X + b*Y and n = c*X + d*Y
    // 
    //     m*d - n*b      m*c - n*a
    // X = ---------, Y = ---------
    //     a*d - c*b      b*c - d*a
    private static (bool solvable, long x, long y) Solve(long m, long a, long b, long n, long c, long d)
    {
        var x = (m * d - n * b) / (a * d - c * b);
        var y = (m * c - n * a) / (b * c - d * a);

        var solvableForInteger = (m == a * x + b * y && n == c * x + d * y);

        return (solvableForInteger, x, y);
    }
}
