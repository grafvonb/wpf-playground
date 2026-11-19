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

        public CalculationResult Power(decimal @base, decimal exponent)
        {
            switch (Arithmetic.PowerError(@base, exponent))
            {
                case CalculationError.ZeroToNegativePower:
                    return ZeroToNegativePower();
                case CalculationError.NotRealNumber:
                    return NotRealNumber();
            }

            return Execute("dbo.Calc_Power", @base, exponent, PowerOverflow());
        }

        private CalculationResult Execute(string procedureName, decimal first, decimal second)
        {
            return Execute(procedureName, first, second, Overflow());
        }

        private CalculationResult Execute(string procedureName, decimal first, decimal second, CalculationResult overflow)
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
                    case ProcedureStatus.ZeroToNegativePower:
                        return ZeroToNegativePower();
                    case ProcedureStatus.NotRealNumber:
                        return NotRealNumber();
                    default:
                        return overflow;
                }
            }
            catch (OverflowException)
            {
                return overflow;
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

        private static CalculationResult ZeroToNegativePower()
        {
            return CalculationResult.Failure(CalculationError.ZeroToNegativePower, Arithmetic.ZeroToNegativePowerMessage);
        }

        private static CalculationResult NotRealNumber()
        {
            return CalculationResult.Failure(CalculationError.NotRealNumber, Arithmetic.NotRealNumberMessage);
        }

        private static CalculationResult PowerOverflow()
        {
            return CalculationResult.Failure(CalculationError.Overflow, Arithmetic.PowerOverflowMessage);
        }

        private static CalculationResult Overflow()
        {
            return CalculationResult.Failure(CalculationError.Overflow, "The result is too large to calculate");
        }
    }
}
