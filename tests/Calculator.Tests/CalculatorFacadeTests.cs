using System;
using System.Linq;
using System.Reflection;
using Calculator.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class CalculatorFacadeTests
    {
        private readonly ICalculatorFacade _facade = new CalculatorFacade();

        [TestMethod]
        public void Add_ReturnsSum()
        {
            AssertSuccess(9m, _facade.Add(6m, 3m));
            AssertSuccess(-3m, _facade.Add(-6m, 3m));
            AssertSuccess(3.75m, _facade.Add(1.5m, 2.25m));
        }

        [TestMethod]
        public void Subtract_ReturnsDifference()
        {
            AssertSuccess(3m, _facade.Subtract(6m, 3m));
            AssertSuccess(-9m, _facade.Subtract(-6m, 3m));
            AssertSuccess(-0.75m, _facade.Subtract(1.5m, 2.25m));
        }

        [TestMethod]
        public void Multiply_ReturnsProduct()
        {
            AssertSuccess(18m, _facade.Multiply(6m, 3m));
            AssertSuccess(-18m, _facade.Multiply(-6m, 3m));
            AssertSuccess(3.375m, _facade.Multiply(1.5m, 2.25m));
        }

        [TestMethod]
        public void Divide_ReturnsQuotient()
        {
            AssertSuccess(2m, _facade.Divide(6m, 3m));
            AssertSuccess(-2m, _facade.Divide(-6m, 3m));
            AssertSuccess(0.5m, _facade.Divide(1.5m, 3m));
        }

        [TestMethod]
        public void Divide_ByZero_ReturnsDivisionByZeroFailure()
        {
            AssertFailure(CalculationError.DivisionByZero, _facade.Divide(1m, 0m));
            AssertFailure(CalculationError.DivisionByZero, _facade.Divide(0m, 0m));
        }

        [TestMethod]
        public void Power_ReturnsResult()
        {
            AssertSuccess(8m, _facade.Power(2m, 3m));
            AssertSuccess(1024m, _facade.Power(2m, 10m));
            AssertSuccess(0.5m, _facade.Power(2m, -1m));
            AssertSuccess(-8m, _facade.Power(-2m, 3m));
            AssertSuccess(16m, _facade.Power(-2m, 4m));
            AssertSuccess(1m, _facade.Power(1m, 500m));
            AssertSuccess(-1m, _facade.Power(-1m, 501m));
            AssertSuccess(1m, _facade.Power(-1m, 500m));
            AssertSuccess(1073741824m, _facade.Power(2m, 30m));
            AssertSuccess(1000000000000000000m, _facade.Power(10m, 18m));
            AssertClose(3m, _facade.Power(9m, 0.5m));
        }

        [TestMethod]
        public void Power_ExponentZero_ReturnsOne()
        {
            AssertSuccess(1m, _facade.Power(5m, 0m));
            AssertSuccess(1m, _facade.Power(0m, 0m));
            AssertSuccess(1m, _facade.Power(-5m, 0m));
        }

        [TestMethod]
        public void Power_ResultTooSmallToRepresent_ReturnsZero()
        {
            AssertSuccess(0m, _facade.Power(10m, -1000m));
        }

        [TestMethod]
        public void Power_InvalidInputs_ReturnSpecificFailures()
        {
            var zero = _facade.Power(0m, -1m);
            AssertFailure(CalculationError.ZeroToNegativePower, zero);
            Assert.AreEqual("Cannot raise zero to a negative power", zero.ErrorMessage);

            var notReal = _facade.Power(-8m, 0.5m);
            AssertFailure(CalculationError.NotRealNumber, notReal);
            Assert.AreEqual("Result is not a real number", notReal.ErrorMessage);
        }

        [TestMethod]
        public void Power_ResultTooLarge_ReturnsOverflow()
        {
            foreach (var result in new[]
            {
                _facade.Power(10m, 1000m),
                _facade.Power(decimal.MaxValue, 2m),
                _facade.Power(10m, 1000000000m),
                _facade.Power(decimal.MaxValue, 1.5m)
            })
            {
                AssertFailure(CalculationError.Overflow, result);
                Assert.AreEqual("Result is too large", result.ErrorMessage);
            }
        }

        [TestMethod]
        public void Power_MatchingMoreThanOneError_ReportsFirstRule()
        {
            AssertFailure(CalculationError.ZeroToNegativePower, _facade.Power(0m, -0.5m));
            AssertFailure(CalculationError.NotRealNumber, _facade.Power(-1000m, 1000.5m));
        }

        [TestMethod]
        public void Operations_OutsideDecimalRange_ReturnOverflowFailure()
        {
            AssertFailure(CalculationError.Overflow, _facade.Multiply(decimal.MaxValue, 2m));
            AssertFailure(CalculationError.Overflow, _facade.Add(decimal.MaxValue, 1m));
            AssertFailure(CalculationError.Overflow, _facade.Subtract(decimal.MinValue, 1m));
            AssertFailure(CalculationError.Overflow, _facade.Divide(decimal.MaxValue, 0.1m));
        }

        [TestMethod]
        public void Service_DoesNotReferenceWpfOrApp()
        {
            var references = typeof(CalculatorFacade).Assembly.GetReferencedAssemblies()
                .Select(a => a.Name)
                .ToList();

            CollectionAssert.DoesNotContain(references, "PresentationFramework");
            CollectionAssert.DoesNotContain(references, "PresentationCore");
            CollectionAssert.DoesNotContain(references, "WindowsBase");
            CollectionAssert.DoesNotContain(references, "Calculator.App");
        }

        [TestMethod]
        public void Service_ExposesOnlyFacadeAndResultTypesPublicly()
        {
            var publicTypes = typeof(CalculatorFacade).Assembly.GetExportedTypes()
                .Select(t => t.Name)
                .OrderBy(n => n)
                .ToArray();

            CollectionAssert.AreEqual(
                new[] { "CalculationError", "CalculationResult", "CalculatorFacade", "ICalculatorFacade", "SqlCalculatorFacade" },
                publicTypes);
        }

        [TestMethod]
        public void Arithmetic_IsNotPublic()
        {
            var type = typeof(CalculatorFacade).Assembly.GetType("Calculator.Service.Internal.Arithmetic");

            Assert.IsNotNull(type);
            Assert.IsFalse(type.IsPublic);
        }

        private static void AssertSuccess(decimal expected, CalculationResult result)
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(expected, result.Value);
            Assert.AreEqual(CalculationError.None, result.Error);
            Assert.AreEqual(string.Empty, result.ErrorMessage);
        }

        private static void AssertClose(decimal expected, CalculationResult result)
        {
            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(Math.Abs(expected - result.Value) <= Math.Abs(expected) * 0.000000000001m);
        }

        private static void AssertFailure(CalculationError expected, CalculationResult result)
        {
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expected, result.Error);
            Assert.AreEqual(0m, result.Value);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
        }
    }
}
