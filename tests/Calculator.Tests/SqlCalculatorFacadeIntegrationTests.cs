using System;
using Calculator.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class SqlCalculatorFacadeIntegrationTests
    {
        private const string DefaultConnection =
            "Server=.;Database=CalculatorDb;Integrated Security=true;Connect Timeout=5";

        private ICalculatorFacade _sql;
        private readonly ICalculatorFacade _reference = new CalculatorFacade();

        [TestInitialize]
        public void Init()
        {
            var connection = Environment.GetEnvironmentVariable("CALCULATOR_TEST_CONNECTION") ?? DefaultConnection;
            _sql = new SqlCalculatorFacade(connection);
            if (_sql.Add(1m, 1m).Error == CalculationError.Unavailable)
            {
                Assert.Inconclusive("CalculatorDb is not reachable or the stored procedures are not installed.");
            }
        }

        private static readonly decimal[][] Pairs =
        {
            new[] { 6m, 3m }, new[] { -6m, 3m }, new[] { 1.5m, 2.25m }, new[] { 0m, 0m },
            new[] { 10m, 4m }, new[] { 1m, 3m }, new[] { 123456.789m, -0.001m }
        };

        [TestMethod]
        public void MatchesInProcessFacade()
        {
            foreach (var p in Pairs)
            {
                AssertSame(_reference.Add(p[0], p[1]), _sql.Add(p[0], p[1]));
                AssertSame(_reference.Subtract(p[0], p[1]), _sql.Subtract(p[0], p[1]));
                AssertSame(_reference.Multiply(p[0], p[1]), _sql.Multiply(p[0], p[1]));
                AssertSame(_reference.Divide(p[0], p[1]), _sql.Divide(p[0], p[1]));
            }
        }

        private static readonly decimal[][] PowerPairs =
        {
            new[] { 2m, 3m }, new[] { 2m, 10m }, new[] { 5m, 0m }, new[] { 0m, 0m }, new[] { 2m, -1m },
            new[] { 9m, 0.5m }, new[] { -2m, 3m }, new[] { 2.5m, 2.5m }, new[] { 10m, -3m },
            new[] { 0m, -1m }, new[] { -8m, 0.5m }, new[] { 10m, 30m }, new[] { 10m, -1000m }
        };

        [TestMethod]
        public void Power_MatchesInProcessFacade()
        {
            foreach (var p in PowerPairs)
            {
                var expected = _reference.Power(p[0], p[1]);
                var actual = _sql.Power(p[0], p[1]);

                Assert.AreEqual(expected.Error, actual.Error, p[0] + " ^ " + p[1]);
                Assert.AreEqual(expected.ErrorMessage, actual.ErrorMessage, p[0] + " ^ " + p[1]);
                Assert.IsTrue(
                    Math.Abs(expected.Value - actual.Value) <= Math.Abs(expected.Value) * 0.000000000001m,
                    p[0] + " ^ " + p[1]);
            }
        }

        [TestMethod]
        public void Power_ResultBeyondDatabaseRange_FailsWithTooLarge()
        {
            var result = _sql.Power(10m, 25m);

            Assert.AreEqual(CalculationError.Overflow, result.Error);
            Assert.AreEqual("Result is too large", result.ErrorMessage);
        }

        [TestMethod]
        public void LargeOperands_FailWithOverflow()
        {
            Assert.AreEqual(CalculationError.Overflow, _sql.Add(decimal.MaxValue, 1m).Error);
            Assert.AreEqual(CalculationError.Overflow, _sql.Multiply(decimal.MinValue, 2m).Error);
        }

        [TestMethod]
        public void RepresentativeCalls_FinishWithinFiveSeconds()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            _sql.Multiply(12.5m, 4m);
            Assert.IsTrue(watch.Elapsed < TimeSpan.FromSeconds(5));
        }

        // The database keeps 18 decimal places; the in-process decimal keeps up to 28 significant digits.
        private static void AssertSame(CalculationResult expected, CalculationResult actual)
        {
            Assert.AreEqual(expected.Error, actual.Error);
            Assert.IsTrue(Math.Abs(expected.Value - actual.Value) <= 0.000000000000000001m);
        }
    }
}
