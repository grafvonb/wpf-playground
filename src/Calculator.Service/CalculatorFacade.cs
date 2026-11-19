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
