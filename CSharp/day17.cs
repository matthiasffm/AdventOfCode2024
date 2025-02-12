namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

/// <summary>
/// Theme: simulate a chronospatial computer in a vm
/// </summary>
[TestFixture]
public class Day17
{
    private static (long valRegisterA, long valRegisterB, long valRegisterC, int[] instructions) ParseData(string[] data)
        => (long.Parse(data[0][11..]),
            long.Parse(data[1][11..]),
            long.Parse(data[2][11..]),
            data[4][8..].Split(',').Select(s => int.Parse(s)).ToArray());

    [Test]
    public void TestSamples()
    {
        Puzzle1(   0,    0,     9, [2, 6]).b.Should().Be(1);
        Puzzle1(  10,    0,     0, [5,0,5,1,5,4]).output.Should().BeEquivalentTo([0,1,2]);
        Puzzle1(   0,   29,     0, [1, 7]).b.Should().Be(26);
        Puzzle1(   0, 2024, 43690, [4, 0]).b.Should().Be(44354);
        Puzzle1(2024,    0,     0, [0,1,5,4,3,0]).output.Should().BeEquivalentTo([4,2,5,6,7,7,7,7,3,1,0]);
        Puzzle1(2024,    0,     0, [0,1,5,4,3,0]).a.Should().Be(0);

        Puzzle1(729, 0, 0, [0,1,5,4,3,0]).output.Should().BeEquivalentTo([4,6,3,5,6,3,5,2,1,0]);

        Puzzle2([0,3,5,4,3,0], 0, 0).Should().Be(117440);
    }

    [Test]
    public void TestAocInput()
    {
        var data = FileUtils.ReadAllLines(this);
        var (regA, regB, regC, instructions) = ParseData(data);

        Puzzle1(regA, regB, regC, instructions).output.Should().BeEquivalentTo([3,7,1,7,2,1,0,6,3]);
        Puzzle2(instructions, 0, 0).Should().Be(37221334433268L);
    }

    // The Chronospatial Computer seems to be a 3-bit computer: its program is a list of 3-bit numbers (0 through 7), like
    // 0,1,2,3. The computer also has three registers named A, B, and C, but these registers aren't limited to 3 bits and
    // can instead hold any integer.
    // The computer knows eight instructions, each identified by a 3-bit number (called the instruction's opcode). Each
    // instruction also reads the 3-bit number after it as an input; this is called its operand.
    // The instruction pointer (ip) starts at 0, pointing at the first 3-bit number in the program. Except for jump instructions,
    // the ip increases by 2 after each instruction is processed. If the computer tries to read an opcode past the end of the
    // program, it instead halts.
    // There are two types of operands. The value of a literal operand is the operand itself. The value of a combo operand can be
    // found as follows:
    // - Combo operands 0 through 3 represent literal values 0 through 3.
    // - Combo operand 4, 5 or 6 represents the value of register A, B or C.
    // The eight instructions are as follows:
    // - Opcode 0 performs division and stores the result in the A register. The numerator is the value in the A register. The denominator
    //   is found by raising 2 to the power of the instruction's combo operand.
    // - Opcode 1 calculates the bitwise XOR of register B and the instruction's literal operand, then stores the result in register B.
    // - Opcode 2 calculates the value of its combo operand modulo 8, then writes that value to the B register.
    // - Opcode 3 does nothing if A is 0. If the A register is not 0, it jumps by setting the ip to the value of its literal operand.
    // - Opcode 4 calculates the bitwise XOR of register B and register C, then stores the result in register B.
    // - Opcode 5 calculates the value of its combo operand modulo 8, then outputs that value.
    // - Opcode 6 performs division and stores the result in the B register. The numerator is the value in the A register. The denominator
    //   is found by raising 2 to the power of the instruction's combo operand.
    // - Opcode 7 performs division and stores the result in the C register. The numerator is the value in the A register. The denominator
    //   is found by raising 2 to the power of the instruction's combo operand.
    //
    // Puzzle == Your first task is to determine what the program is trying to output.
    private static (long a, long b, long c, int[] output) Puzzle1(long regA, long regB, long regC, int[] instructions)
    {
        var ip = 0;
        var outputList = new List<int>();

        do
        {
            (regA, regB, regC, ip, int output) = instructions[ip] switch {
                0               => (regA >> (int)Combo(instructions[ip + 1], regA, regB, regC), regB, regC, ip + 2, -1),
                1               => (regA, regB ^ (long)instructions[ip + 1], regC, ip + 2, -1),
                2               => (regA, Combo(instructions[ip + 1], regA, regB, regC) % 8, regC, ip + 2, -1),
                3 when regA > 0 => (regA, regB, regC, instructions[ip + 1], -1),
                4               => (regA, regB ^ regC, regC, ip + 2, -1),
                5               => (regA, regB, regC, ip + 2, (int)(Combo(instructions[ip + 1], regA, regB, regC) % 8)),
                6               => (regA, regA >> (int)Combo(instructions[ip + 1], regA, regB, regC), regC, ip + 2, -1),
                7               => (regA, regB, regA >> (int)Combo(instructions[ip + 1], regA, regB, regC), ip + 2, -1),
                _               => (regA, regB, regC, ip + 2, -1),
            };

            if(output >= 0)
            {
                 outputList.Add(output);
            }
        }
        while(ip < instructions.Length);

        return (regA, regB, regC, outputList.ToArray());
    }

    // Digging deeper in the device's manual, you discover the problem: this program is supposed to output another copy of the program!
    // Unfortunately, the value in register A seems to have been corrupted. You'll need to find a new value to which you can initialize
    // register A so that the program's output instructions produce an exact copy of the program itself.
    //
    // Puzzle == What is the lowest positive initial value for register A that causes the program to output a copy of itself?
    private static long? Puzzle2(int[] instructions, long aReg, int digit)
    {
        if(digit == instructions.Length)
        {
            return aReg;
        }

        long? res = null;
        for(var i = 0L; i < 8L; i++)
        {
            var resultForNextDigit = Puzzle1(aReg * 8L + i, 0L, 0L, instructions).output;

            if(resultForNextDigit.Length == digit + 1 &&
               resultForNextDigit.SequenceEqual(instructions.Reverse().Take(digit + 1).Reverse()))
            {
                res = Puzzle2(instructions, aReg * 8L + i, digit + 1);
                if(res.HasValue)
                {
                    return res;
                }
            }
        }

        return res;
    }

    private static long Combo(int op, long regA, long regB, long regC) => op switch {
        4 => regA,
        5 => regB,
        6 => regC,
        _ => op,
    };
}
