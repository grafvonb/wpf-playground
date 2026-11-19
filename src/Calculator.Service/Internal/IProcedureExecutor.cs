namespace Calculator.Service.Internal
{
    internal interface IProcedureExecutor
    {
        /// <summary>
        /// Runs a two-operand calculation procedure. Throws <see cref="System.OverflowException"/> when an
        /// operand or the result cannot be represented, and database exceptions for infrastructure failures.
        /// </summary>
        ProcedureOutcome Execute(string procedureName, decimal first, decimal second);
    }
}
