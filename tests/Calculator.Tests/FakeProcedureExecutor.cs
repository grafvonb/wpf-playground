using System;
using System.Data.SqlClient;
using System.Reflection;
using Calculator.Service.Internal;

namespace Calculator.Tests
{
    internal sealed class FakeProcedureExecutor : IProcedureExecutor
    {
        public Func<string, decimal, decimal, ProcedureOutcome> Handler { get; set; }

        public string LastProcedure { get; private set; }

        public ProcedureOutcome Execute(string procedureName, decimal first, decimal second)
        {
            LastProcedure = procedureName;
            return Handler(procedureName, first, second);
        }

        public static SqlException CreateSqlException()
        {
            var ctor = typeof(SqlException).GetConstructor(
                BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
            if (ctor != null)
            {
                return (SqlException)ctor.Invoke(null);
            }

            return (SqlException)System.Runtime.Serialization.FormatterServices
                .GetUninitializedObject(typeof(SqlException));
        }
    }
}
