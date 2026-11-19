using System;
using Calculator.Service;
using Calculator.Service.Internal;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class SqlCalculatorFacadeTests
    {
        [TestMethod]
        public void Operations_CallMatchingProcedure()
        {
            var fake = new FakeProcedureExecutor
            {
                Handler = (p, a, b) => new ProcedureOutcome(ProcedureStatus.Ok, 7m)
            };
            var facade = new SqlCalculatorFacade(fake);

            facade.Add(1m, 2m);
            Assert.AreEqual("dbo.Calc_Add", fake.LastProcedure);
            facade.Subtract(1m, 2m);
            Assert.AreEqual("dbo.Calc_Subtract", fake.LastProcedure);
            facade.Multiply(1m, 2m);
            Assert.AreEqual("dbo.Calc_Multiply", fake.LastProcedure);
            var result = facade.Divide(1m, 2m);
            Assert.AreEqual("dbo.Calc_Divide", fake.LastProcedure);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(7m, result.Value);
        }

        [TestMethod]
        public void Divide_ByZero_FailsWithoutCallingDatabase()
        {
            var fake = new FakeProcedureExecutor
            {
                Handler = (p, a, b) => throw new InvalidOperationException("must not be called")
            };

            var result = new SqlCalculatorFacade(fake).Divide(1m, 0m);

            Assert.AreEqual(CalculationError.DivisionByZero, result.Error);
            Assert.AreEqual("Cannot divide by zero", result.ErrorMessage);
        }

        [TestMethod]
        public void OverflowStatusAndException_MapToOverflow()
        {
            var status = new SqlCalculatorFacade(new FakeProcedureExecutor
            {
                Handler = (p, a, b) => new ProcedureOutcome(ProcedureStatus.Overflow, 0m)
            });
            var thrown = new SqlCalculatorFacade(new FakeProcedureExecutor
            {
                Handler = (p, a, b) => throw new OverflowException()
            });

            Assert.AreEqual(CalculationError.Overflow, status.Multiply(1m, 1m).Error);
            Assert.AreEqual(CalculationError.Overflow, thrown.Multiply(1m, 1m).Error);
            Assert.AreEqual("The result is too large to calculate", thrown.Add(1m, 1m).ErrorMessage);
        }

        [TestMethod]
        public void Power_CallsPowerProcedureWithBothOperands()
        {
            var fake = new FakeProcedureExecutor
            {
                Handler = (p, a, b) => new ProcedureOutcome(ProcedureStatus.Ok, 8m)
            };

            var result = new SqlCalculatorFacade(fake).Power(2m, 3m);

            Assert.AreEqual("dbo.Calc_Power", fake.LastProcedure);
            Assert.AreEqual(8m, result.Value);
        }

        [TestMethod]
        public void Power_InvalidInputs_FailWithoutCallingDatabase()
        {
            var facade = new SqlCalculatorFacade(new FakeProcedureExecutor
            {
                Handler = (p, a, b) => throw new InvalidOperationException("must not be called")
            });

            var zero = facade.Power(0m, -1m);
            var notReal = facade.Power(-8m, 0.5m);

            Assert.AreEqual(CalculationError.ZeroToNegativePower, zero.Error);
            Assert.AreEqual("Cannot raise zero to a negative power", zero.ErrorMessage);
            Assert.AreEqual(CalculationError.NotRealNumber, notReal.Error);
            Assert.AreEqual("Result is not a real number", notReal.ErrorMessage);
        }

        [TestMethod]
        public void Power_ProcedureStatuses_MapToSameErrorsAsInProcess()
        {
            var expected = new[]
            {
                new { Status = ProcedureStatus.ZeroToNegativePower, Error = CalculationError.ZeroToNegativePower, Message = "Cannot raise zero to a negative power" },
                new { Status = ProcedureStatus.NotRealNumber, Error = CalculationError.NotRealNumber, Message = "Result is not a real number" },
                new { Status = ProcedureStatus.Overflow, Error = CalculationError.Overflow, Message = "Result is too large" }
            };

            foreach (var item in expected)
            {
                var status = item.Status;
                var facade = new SqlCalculatorFacade(new FakeProcedureExecutor
                {
                    Handler = (p, a, b) => new ProcedureOutcome(status, 0m)
                });

                var result = facade.Power(2m, 3m);

                Assert.AreEqual(item.Error, result.Error);
                Assert.AreEqual(item.Message, result.ErrorMessage);
            }
        }

        [TestMethod]
        public void Power_OperandOutsideDatabaseRange_ReportsPowerOverflowMessage()
        {
            var facade = new SqlCalculatorFacade(new FakeProcedureExecutor
            {
                Handler = (p, a, b) => throw new OverflowException()
            });

            Assert.AreEqual("Result is too large", facade.Power(2m, 3m).ErrorMessage);
        }

        [TestMethod]
        public void InfrastructureFailures_MapToUnavailable()
        {
            Exception[] failures =
            {
                FakeProcedureExecutor.CreateSqlException(),
                new InvalidOperationException(),
                new TimeoutException()
            };

            foreach (var failure in failures)
            {
                var facade = new SqlCalculatorFacade(new FakeProcedureExecutor
                {
                    Handler = (p, a, b) => throw failure
                });

                var result = facade.Add(1m, 1m);

                Assert.AreEqual(CalculationError.Unavailable, result.Error);
                Assert.AreEqual("The calculation service is unavailable", result.ErrorMessage);
            }
        }

        [TestMethod]
        public void Recovers_AfterOutage()
        {
            var down = true;
            var facade = new SqlCalculatorFacade(new FakeProcedureExecutor
            {
                Handler = (p, a, b) =>
                {
                    if (down) throw new TimeoutException();
                    return new ProcedureOutcome(ProcedureStatus.Ok, 3m);
                }
            });

            Assert.AreEqual(CalculationError.Unavailable, facade.Add(1m, 2m).Error);
            down = false;
            Assert.AreEqual(3m, facade.Add(1m, 2m).Value);
        }
    }
}
