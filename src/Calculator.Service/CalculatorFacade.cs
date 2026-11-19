using System;
using Calculator.Service.Internal;

namespace Calculator.Service
{
    public sealed class CalculatorFacade : ICalculatorFacade
    {
        public CalculationResult Add(decimal left, decimal right)
        {
            return Execute(() => Arithmetic.Add(left, right));
        }

        public CalculationResult Subtract(decimal left, decimal right)
        {
            return Execute(() => Arithmetic.Subtract(left, right));
        }

        public CalculationResult Multiply(decimal left, decimal right)
        {
            return Execute(() => Arithmetic.Multiply(left, right));
        }

        public CalculationResult Divide(decimal dividend, decimal divisor)
        {
            if (divisor == 0m)
            {
                return CalculationResult.Failure(CalculationError.DivisionByZero, "Cannot divide by zero");
            }

            return Execute(() => Arithmetic.Divide(dividend, divisor));
        }

        public CalculationResult Power(decimal @base, decimal exponent)
        {
            switch (Arithmetic.PowerError(@base, exponent))
            {
                case CalculationError.ZeroToNegativePower:
                    return CalculationResult.Failure(CalculationError.ZeroToNegativePower, Arithmetic.ZeroToNegativePowerMessage);
                case CalculationError.NotRealNumber:
                    return CalculationResult.Failure(CalculationError.NotRealNumber, Arithmetic.NotRealNumberMessage);
            }

            try
            {
                return CalculationResult.Success(Arithmetic.Power(@base, exponent));
            }
            catch (OverflowException)
            {
                return CalculationResult.Failure(CalculationError.Overflow, Arithmetic.PowerOverflowMessage);
            }
        }

        private static CalculationResult Execute(Func<decimal> operation)
        {
            try
            {
                return CalculationResult.Success(operation());
            }
            catch (OverflowException)
            {
                return CalculationResult.Failure(CalculationError.Overflow, "The result is too large to calculate");
            }
        }
    }
}
