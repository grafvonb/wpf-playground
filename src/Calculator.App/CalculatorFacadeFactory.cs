using System;
using Calculator.Service;

namespace Calculator.App
{
    public static class CalculatorFacadeFactory
    {
        public const string InProcess = "InProcess";
        public const string Sql = "Sql";

        /// <summary>Selects the backend; a missing value means in-process, an unknown value is rejected.</summary>
        public static ICalculatorFacade Create(string backend, string connectionString)
        {
            if (string.IsNullOrWhiteSpace(backend) ||
                string.Equals(backend.Trim(), InProcess, StringComparison.OrdinalIgnoreCase))
            {
                return new CalculatorFacade();
            }

            if (string.Equals(backend.Trim(), Sql, StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(connectionString))
                {
                    throw new InvalidOperationException("The CalculatorDb connection string is missing.");
                }

                return new SqlCalculatorFacade(connectionString);
            }

            throw new InvalidOperationException(
                "Unknown CalculatorBackend '" + backend + "'. Use '" + InProcess + "' or '" + Sql + "'.");
        }
    }
}
