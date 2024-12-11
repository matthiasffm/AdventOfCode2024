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

    public static int Digits(this int n) => n == 0 ? 1 : (int)Math.Log10(n) + 1;

    public static int Digits(this long n) => n == 0L ? 1 : (int)Math.Log10(n) + 1;

    public static int Pow(this int baseVal, int pow)
    {
        var result = 1;
        while(pow != 0)
        {
            if((pow & 1) == 1)
            {
                result *= baseVal;
            }

            baseVal *= baseVal;
            pow >>= 1;
        }

        return result;
    }

    public static long Pow(this long baseVal, long pow)
    {
        var result = 1L;
        while(pow != 0L)
        {
            if((pow & 1L) == 1L)
            {
                result *= baseVal;
            }

            baseVal *= baseVal;
            pow >>= 1;
        }

        return result;
    }
}
