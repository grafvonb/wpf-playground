using System;
using System.Data.SqlClient;
using Calculator.Service.Internal;

namespace Calculator.Service
{
    /// <summary>Calculator facade that performs every operation in a SQL Server stored procedure.</summary>
    public sealed class SqlCalculatorFacade : ICalculatorFacade
    {
        private readonly IProcedureExecutor _executor;

        public SqlCalculatorFacade(string connectionString)
            : this(new SqlProcedureExecutor(connectionString))
        {
        }

        internal SqlCalculatorFacade(IProcedureExecutor executor)
        {
            _executor = executor;
        }

        public CalculationResult Add(decimal left, decimal right)
        {
            return Execute("dbo.Calc_Add", left, right);
        }

        public CalculationResult Subtract(decimal left, decimal right)
        {
            return Execute("dbo.Calc_Subtract", left, right);
        }

        public CalculationResult Multiply(decimal left, decimal right)
        {
            return Execute("dbo.Calc_Multiply", left, right);
        }

        public CalculationResult Divide(decimal dividend, decimal divisor)
        {
            if (divisor == 0m)
            {
                return DivisionByZero();
            }

            return Execute("dbo.Calc_Divide", dividend, divisor);
        }

        private CalculationResult Execute(string procedureName, decimal first, decimal second)
        {
            try
            {
                var outcome = _executor.Execute(procedureName, first, second);
                switch (outcome.Status)
                {
                    case ProcedureStatus.Ok:
                        return CalculationResult.Success(outcome.Value);
                    case ProcedureStatus.DivisionByZero:
                        return DivisionByZero();
                    default:
                        return Overflow();
                }
            }
            catch (OverflowException)
            {
                return Overflow();
            }
            catch (Exception ex) when (ex is SqlException || ex is InvalidOperationException || ex is TimeoutException)
            {
                return CalculationResult.Failure(CalculationError.Unavailable, "The calculation service is unavailable");
            }
        }

        private static CalculationResult DivisionByZero()
        {
            return CalculationResult.Failure(CalculationError.DivisionByZero, "Cannot divide by zero");
        }

        private static CalculationResult Overflow()
        {
            return CalculationResult.Failure(CalculationError.Overflow, "The result is too large to calculate");
        }
    }
}
