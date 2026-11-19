using System;
using Calculator.Service;

namespace Calculator.Tests
{
    internal sealed class FakeCalculatorFacade : ICalculatorFacade
    {
        public CalculationResult NextResult { get; set; } = CalculationResult.Success(0m);

        public string LastOperation { get; private set; }

        public decimal LastLeft { get; private set; }

        public decimal LastRight { get; private set; }

        public int CallCount { get; private set; }

        public CalculationResult Add(decimal left, decimal right)
        {
            return Record("Add", left, right);
        }

        public CalculationResult Subtract(decimal left, decimal right)
        {
            return Record("Subtract", left, right);
        }

        public CalculationResult Multiply(decimal left, decimal right)
        {
            return Record("Multiply", left, right);
        }

        public CalculationResult Divide(decimal dividend, decimal divisor)
        {
            return Record("Divide", dividend, divisor);
        }

        public CalculationResult Power(decimal @base, decimal exponent)
        {
            return Record("Power", @base, exponent);
        }

        private CalculationResult Record(string operation, decimal left, decimal right)
        {
            LastOperation = operation;
            LastLeft = left;
            LastRight = right;
            CallCount++;
            return NextResult;
        }
    }
}
