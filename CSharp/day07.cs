namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: insert missing operands
/// </summary>
[TestFixture]
public class Day07
{
    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "190: 10 19",
            "3267: 81 40 27",
            "83: 17 5",
            "156: 15 6",
            "7290: 6 8 6 15",
            "161011: 16 10 13",
            "192: 17 8 14",
            "21037: 9 7 18 13",
            "292: 11 6 16 20",
        };
        var calculations = ParseData(data);

        Puzzle1(calculations).Should().Be(190 + 3267 + 292);
        Puzzle2(calculations).Should().Be(190 + 3267 + 292 + 156 + 7290 + 192);
    }

    [Test]
    public void TestAocInput()
    {
        var data         = FileUtils.ReadAllLines(this);
        var calculations = ParseData(data);

        Puzzle1(calculations).Should().Be(932137732557L);
        Puzzle2(calculations).Should().Be(661823605105500L);
    }

    private static long[][] ParseData(string[] lines)
        => lines.Select(l => l.Split([':', ' '], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                              .Select(n => long.Parse(n))
                              .ToArray())
                .ToArray();

    // The engineers tell you that it only needs final calibrations, but someone stole all the operators from their calibration equations. If only someone could
    // determine which test values could possibly be produced by placing any combination of operators into their calibration equations (your puzzle input).
    // Operators are always evaluated left-to-right, not according to precedence rules. Furthermore, numbers in the equations cannot be rearranged. Only two different
    // types of operators are possible: add (+) and multiply (*).
    //
    // Puzzle == Determine which equations could possibly be true. What is their total calibration result?
    private static long Puzzle1(long[][] calculations)
        => calculations.Where(calculation => CanBeMadeTrue(calculation[0], calculation[1], calculation[2..]))
                       .Sum(calculations => calculations[0]);

    private static bool CanBeMadeTrue(long result, long aggregate, long[] operands)
        => operands switch {
            []                           => result == aggregate,
            [..] when aggregate > result => false,
            [var head, ..]               => CanBeMadeTrue(result, aggregate + head, operands[1..]) ||
                                            CanBeMadeTrue(result, aggregate * head, operands[1..]),
        };

    // You spot your mistake: there is a third type of operator. The concatenation operator (||) combines the digits from its left and right inputs into a single number. For
    // example, 12 || 345 would become 12345. All operators are still evaluated left-to-right.
    //
    // Puzzle == Using the third possible operator, determine which equations could possibly be true. What is their total calibration result?
    private static long Puzzle2(long[][] calculations)
        => calculations.Where(calculation => CanBeMadeTrueWithConcat(calculation[0], calculation[1], calculation[2..]) ||
                                             CanBeMadeTrueWithConcat(calculation[0], Concat(calculation[1], calculation[2]), calculation[3..]))
                       .Sum(calculations => calculations[0]);

    private static bool CanBeMadeTrueWithConcat(long result, long aggregate, long[] operands)
        => operands switch {
            []                           => result == aggregate,
            [..] when aggregate > result => false,
            [var head]                   => (result == aggregate + head) ||
                                            (result == aggregate * head) ||
                                            (result == Concat(aggregate, head)),
            [var head, ..]               => CanBeMadeTrueWithConcat(result, aggregate + head, operands[1..]) ||
                                            CanBeMadeTrueWithConcat(result, aggregate * head, operands[1..]) ||
                                            CanBeMadeTrueWithConcat(result, Concat(aggregate, head), operands[1..]),
        };

    // more than two times faster than
    // Concat(long op1, long op2) => long.Parse(op1 + op2);
    private static long Concat(long op1, long op2)
    {
        if(op2 < 10L) return 10L * op1 + op2;
        if(op2 < 100L) return 100L * op1 + op2;
        if(op2 < 1000L) return 1000L * op1 + op2;
        if(op2 < 10000L) return 10000L * op1 + op2;
        if(op2 < 100000L) return 100000L * op1 + op2;
        if(op2 < 1000000L) return 1000000L * op1 + op2;
        if(op2 < 10000000L) return 10000000L * op1 + op2;
        if(op2 < 100000000L) return 100000000L * op1 + op2;
        if(op2 < 1000000000L) return 1000000000L * op1 + op2;
        if(op2 < 10000000000L) return 10000000000L * op1 + op2;
        if(op2 < 100000000000L) return 100000000000L * op1 + op2;
        if(op2 < 1000000000000L) return 1000000000000L * op1 + op2;
        if(op2 < 10000000000000L) return 10000000000000L * op1 + op2;
        if(op2 < 100000000000000L) return 100000000000000L * op1 + op2;
        if(op2 < 1000000000000000L) return 1000000000000000L * op1 + op2;
        if(op2 < 10000000000000000L) return 10000000000000000L * op1 + op2;
        return 100000000000000000L * op1 + op2;
    }
}
