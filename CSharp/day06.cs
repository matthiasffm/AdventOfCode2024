namespace AdventOfCode2024;

using FluentAssertions;
using NUnit.Framework;

using matthiasffm.Common.Math;

// TODO: one order of magnitude too slow
//       walking the guards path has not to be done step by step
//       instead all obstructions could be ordered by x and y and so directly found on the current direction
//       storing only the obstructions would also be easier to parallelize each search because no map copy would be necessary

/// <summary>
/// Theme: Splinter Cell
/// </summary>
[TestFixture]
public class Day06
{
    private static char[,] ParseData(string[] lines)
        => lines.Select(l => l.ToCharArray())
                .ToArray()
                .ConvertToMatrix();

    [Test]
    public void TestSamples()
    {
        var data = new [] {
            "....#.....",
            ".........#",
            "..........",
            "..#.......",
            ".......#..",
            "..........",
            ".#..^.....",
            "........#.",
            "#.........",
            "......#...",
        };
        var map = ParseData(data);
        Puzzle1(map, new Vec2<int>(4, 6), 3).Should().Be(41);
        Puzzle2(map, new Vec2<int>(4, 6), 3).Should().Be(6);
    }

    [Test]
    public void TestAocInput()
    {
        var data = FileUtils.ReadAllLines(this);
        var map  = ParseData(data);

        Puzzle1(map, new Vec2<int>(66, 90), 3).Should().Be(5086);
        Puzzle2(map, new Vec2<int>(66, 90), 3).Should().Be(1770);
    }

    // The Historians search for the Chief. Unfortunately, a single guard is patrolling this part of the lab. The map shows the current position of
    // the guard with ^ (to indicate the guard is currently facing up from the perspective of the map). Any obstructions are shown as #. Lab guards
    // follow a very strict patrol protocol which involves repeatedly following these steps:
    // If there is something directly in front of you, turn right 90 degrees. Otherwise, take a step forward.
    // By predicting the guard's route, you can determine which specific positions in the lab will be in the patrol path.
    //
    // Puzzle == Predict the path of the guard. How many distinct positions will the guard visit before leaving the mapped area?
    private static int Puzzle1(char[,] map, Vec2<int> guardPos, int guardDir)
        => WalkGuard(map, guardPos, guardDir)
               .Path
               .Distinct()
               .Count();

    // The guard's patrol area is simply too large for them to safely search the lab without getting caught. Fortunately, they are pretty sure that adding
    // a single new obstruction won't cause a time paradox. They'd like to place the new obstruction in such a way that the guard will get stuck in a loop,
    // making the rest of the lab safe to search. To have the lowest chance of creating a time paradox, The Historians would like to know all of the possible
    // positions for such an obstruction. The new obstruction can't be placed at the guard's starting position - the guard is there right now and would notice.
    //
    // Puzzle == You need to get the guard stuck in a loop by adding a single new obstruction. How many different positions could you choose for this obstruction?
    private static int Puzzle2(char[,] map, Vec2<int> guardPos, int guardDir)
    {
        // obstructions have to be on the path of the guard or have no effect
        var possibleObstructionPos = WalkGuard(map, guardPos, guardDir)
                                         .Path
                                         .Distinct()
                                         .Where(pos => pos != guardPos)
                                         .ToArray();

        int obstructionsWithLoops = 0;

        foreach(var possibleObstruction in possibleObstructionPos)
        {
            // add obstruction
            map[possibleObstruction.Y, possibleObstruction.X] = '#';

            // test for loop
            if(WalkGuard(map, guardPos, guardDir).Finish == GuardPathFinished.WithLoop)
            {
                obstructionsWithLoops++;
            }

            // restore original map
            map[possibleObstruction.Y, possibleObstruction.X] = '.';
        }

        return obstructionsWithLoops;
    }

    private enum GuardPathFinished { WithLoop, Outside };

    // walk the guard path until it ends outside or in a loop
    private static (GuardPathFinished Finish, IEnumerable<Vec2<int>> Path) WalkGuard(char[,] map, Vec2<int> guardPos, int guardDir)
    {
        var visited = new HashSet<(Vec2<int>, int)>(); // visited == position + direction (crossing != loop)

        do
        {
            visited.Add((guardPos, guardDir));

            var nextPos = guardPos + Offsets[guardDir];
            if(IsOutside(map, nextPos))
            {
                return (GuardPathFinished.Outside, visited.Select(v => v.Item1).Distinct());
            }
            else if(map[nextPos.Y, nextPos.X] == '#')
            {
                while(map[nextPos.Y, nextPos.X] == '#')
                {
                    guardDir = (guardDir + 1) % 4;
                    nextPos = guardPos + Offsets[guardDir];
                }
                guardPos = nextPos;
            }
            else
            {
                guardPos += Offsets[guardDir];
            }
        }
        while(!visited.Contains((guardPos, guardDir)));

        return (GuardPathFinished.WithLoop, visited.Select(v => v.Item1));
    }

    private static bool IsOutside(char[,] map, Vec2<int> nextPos)
        => nextPos.X < 0 ||
           nextPos.Y < 0 ||
           nextPos.X >= map.GetLength(0) ||
           nextPos.Y >= map.GetLength(1);

    private static readonly Vec2<int>[] Offsets = [
        new( 1,  0),
        new( 0,  1),
        new(-1,  0),
        new( 0, -1)
    ];
}
