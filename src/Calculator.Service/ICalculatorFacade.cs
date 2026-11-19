namespace Calculator.Service
{
    /// <summary>
    /// The only public entry point of the calculation service.
    /// Implementations are stateless, thread-safe and never throw for expected failures.
    /// </summary>
    public interface ICalculatorFacade
    {
        /// <summary>Adds two numbers.</summary>
        CalculationResult Add(decimal left, decimal right);

        /// <summary>Subtracts <paramref name="right"/> from <paramref name="left"/>.</summary>
        CalculationResult Subtract(decimal left, decimal right);

        /// <summary>Multiplies two numbers.</summary>
        CalculationResult Multiply(decimal left, decimal right);

        /// <summary>Divides <paramref name="dividend"/> by <paramref name="divisor"/>; a zero divisor yields DivisionByZero.</summary>
        CalculationResult Divide(decimal dividend, decimal divisor);
    }
}
