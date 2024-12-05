namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

/// <summary>
/// Theme: find XMAS word
/// </summary>
[TestFixture]
public partial class Day04
{
    private static char[,] ParseData(string[] lines)
        => lines.Select(l => l.ToCharArray())
                .ToArray()
                .ConvertToMatrix();

    [Test]
    public void TestSamples()
    {
        var data = new[] {
            "MMMSXXMASM",
            "MSAMXMSMSA",
            "AMXSXMAAMM",
            "MSAMASMSMX",
            "XMASAMXAMM",
            "XXAMMXXAMA",
            "SMSMSASXSS",
            "SAXAMASAAA",
            "MAMMMXMMMM",
            "MXMXAXMASX",
        };
        var matrix = ParseData(data);

        Puzzle1(matrix).Should().Be(18);
        Puzzle2(matrix).Should().Be(9);
    }

    [Test]
    public void TestAocInput()
    {
        var data   = FileUtils.ReadAllLines(this);
        var matrix = ParseData(data);

        Puzzle1(matrix).Should().Be(2500);
        Puzzle2(matrix).Should().Be(1933);
    }

    private static int Puzzle1(char[,] charMatrix)
    {
        var matches = 0;

        for(var row = 0; row < charMatrix.GetLength(0); row++)
        {
            for(var col = 0; col < charMatrix.GetLength(1); col++)
            {
                if(col >= 3)
                {
                    // test hor -
                    if(MatchesXmas(charMatrix[row, col - 3], charMatrix[row, col - 2], charMatrix[row, col - 1], charMatrix[row, col]))
                    {
                        matches++;
                    }

                    if(row >= 3)
                    {
                        // test diag \
                        if(MatchesXmas(charMatrix[row - 3, col - 3], charMatrix[row - 2, col - 2], charMatrix[row - 1, col - 1], charMatrix[row, col]))
                        {
                            matches++;
                        }
                    }
                }
                if(row >= 3)
                {
                    // test vert |
                    if(MatchesXmas(charMatrix[row - 3, col], charMatrix[row - 2, col], charMatrix[row - 1, col], charMatrix[row, col]))
                    {
                        matches++;
                    }

                    if(col < charMatrix.GetLength(1) - 3)
                    {
                        // test diag /
                        if(MatchesXmas(charMatrix[row - 3, col + 3], charMatrix[row - 2, col + 2], charMatrix[row - 1, col + 1], charMatrix[row, col]))
                        {
                            matches++;
                        }
                    }
                }
            }
        }

        return matches;
    }

    private static bool MatchesXmas(char c1, char c2, char c3, char c4)
        => (c1 == 'X' && c2 == 'M' && c3 == 'A' && c4 == 'S') ||
           (c1 == 'S' && c2 == 'A' && c3 == 'M' && c4 == 'X');

    private static int Puzzle2(char[,] charMatrix)
    {
        var matches = 0;

        for(var row = 1; row < charMatrix.GetLength(0) - 1; row++)
        {
            for(var col = 1; col < charMatrix.GetLength(1) - 1; col++)
            {
                if(charMatrix[row, col] != 'A')
                {
                    continue;
                }

                // // test +
                // if(MatchesMas(charMatrix[row - 1, col], charMatrix[row, col], charMatrix[row + 1, col]) &&
                //    MatchesMas(charMatrix[row, col - 1], charMatrix[row, col], charMatrix[row, col + 1]))
                // {
                //     matches++;
                // }

                // test x
                if(MatchesMas(charMatrix[row - 1, col - 1], charMatrix[row, col], charMatrix[row + 1, col + 1]) &&
                   MatchesMas(charMatrix[row - 1, col + 1], charMatrix[row, col], charMatrix[row + 1, col - 1]))
                {
                    matches++;
                }
            }
        }

        return matches;
    }

    private static bool MatchesMas(char c1, char c2, char c3)
        => (c1 == 'M' && c2 == 'A' && c3 == 'S') ||
           (c1 == 'S' && c2 == 'A' && c3 == 'M');

}
