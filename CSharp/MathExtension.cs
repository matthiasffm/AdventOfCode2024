namespace AdventOfCode2024;

using System.Numerics;

/// <summary>
/// Provides extension methods for basic math functions.
/// </summary>
public static class MathExtensions
{
    public static T SumNumbers<T>(T start, T length) where T : INumberBase<T>
        => (start - T.One) * length +
           length * (length + T.One) / (T.One + T.One);
}
