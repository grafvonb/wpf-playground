using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;

namespace Calculator.Service.Internal
{
    internal sealed class SqlProcedureExecutor : IProcedureExecutor
    {
        private const int CommandTimeoutSeconds = 5;
        private const byte OperandPrecision = 18;
        private const byte OperandScale = 9;
        private const byte ResultPrecision = 38;
        private const byte ResultScale = 18;
        private static readonly decimal OperandLimit = 1000000000m;

        private readonly string _connectionString;

        public SqlProcedureExecutor(string connectionString)
        {
            _connectionString = connectionString;
        }

        public ProcedureOutcome Execute(string procedureName, decimal first, decimal second)
        {
            EnsureRepresentable(first);
            EnsureRepresentable(second);

            using (var connection = new SqlConnection(_connectionString))
            using (var command = new SqlCommand(procedureName, connection))
            {
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = CommandTimeoutSeconds;
                command.Parameters.Add(Operand("@Left", first));
                command.Parameters.Add(Operand("@Right", second));

                var result = new SqlParameter("@Result", SqlDbType.Decimal)
                {
                    Precision = ResultPrecision,
                    Scale = ResultScale,
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(result);

                var returnCode = new SqlParameter("@ReturnCode", SqlDbType.Int)
                {
                    Direction = ParameterDirection.ReturnValue
                };
                command.Parameters.Add(returnCode);

                connection.Open();
                command.ExecuteNonQuery();

                switch ((int)returnCode.Value)
                {
                    case 0:
                        return new ProcedureOutcome(ProcedureStatus.Ok, ReadResult(result));
                    case 1:
                        return new ProcedureOutcome(ProcedureStatus.DivisionByZero, 0m);
                    default:
                        return new ProcedureOutcome(ProcedureStatus.Overflow, 0m);
                }
            }
        }

        private static SqlParameter Operand(string name, decimal value)
        {
            return new SqlParameter(name, SqlDbType.Decimal)
            {
                Precision = OperandPrecision,
                Scale = OperandScale,
                Value = value
            };
        }

        // Operands the procedures cannot hold exactly are rejected instead of being silently rounded.
        private static void EnsureRepresentable(decimal value)
        {
            if (Math.Abs(value) >= OperandLimit || decimal.Round(value, OperandScale) != value)
            {
                throw new OverflowException("The operand is outside the range supported by the database.");
            }
        }

        private static decimal ReadResult(SqlParameter result)
        {
            var sqlValue = (SqlDecimal)result.SqlValue;
            if (sqlValue.IsNull)
            {
                throw new OverflowException("The database returned no result.");
            }

            return sqlValue.Value;
        }
    }
}
