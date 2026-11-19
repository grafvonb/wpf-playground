using System;

namespace Calculator.Service.Internal
{
    internal static class Arithmetic
    {
        public static decimal Add(decimal left, decimal right)
        {
            return left + right;
        }

        public static decimal Subtract(decimal left, decimal right)
        {
            return left - right;
        }

        public static decimal Multiply(decimal left, decimal right)
        {
            return left * right;
        }

        public static decimal Divide(decimal dividend, decimal divisor)
        {
            return dividend / divisor;
        }

        private const decimal MaxExactExponent = 1000000m;

        public const string ZeroToNegativePowerMessage = "Cannot raise zero to a negative power";
        public const string NotRealNumberMessage = "Result is not a real number";
        public const string PowerOverflowMessage = "Result is too large";

        // Returns None when base ^ exponent has a real result; the first matching rule wins.
        public static CalculationError PowerError(decimal @base, decimal exponent)
        {
            if (@base == 0m && exponent < 0m)
            {
                return CalculationError.ZeroToNegativePower;
            }

            if (@base < 0m && exponent != decimal.Truncate(exponent))
            {
                return CalculationError.NotRealNumber;
            }

            return CalculationError.None;
        }

        // Callers check PowerError first. Throws OverflowException when the result is not representable.
        public static decimal Power(decimal @base, decimal exponent)
        {
            if (exponent == 0m)
            {
                return 1m;
            }

            if (exponent > 0m && exponent <= MaxExactExponent && exponent == decimal.Truncate(exponent))
            {
                return IntegerPower(@base, (int)exponent);
            }

            var value = Math.Pow((double)@base, (double)exponent);
            if (double.IsNaN(value) || double.IsInfinity(value))
            {
                throw new OverflowException();
            }

            return (decimal)value;
        }

        // Exact decimal arithmetic; throws OverflowException when the result exceeds the decimal range.
        private static decimal IntegerPower(decimal @base, int exponent)
        {
            var result = 1m;
            var factor = @base;
            while (exponent > 0)
            {
                if ((exponent & 1) == 1)
                {
                    result *= factor;
                }

                exponent >>= 1;
                if (exponent > 0)
                {
                    factor *= factor;
                }
            }

            return result;
        }
    }
}
