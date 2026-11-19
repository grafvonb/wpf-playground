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
                new[] { "CalculationError", "CalculationResult", "CalculatorFacade", "ICalculatorFacade" },
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

        private static void AssertFailure(CalculationError expected, CalculationResult result)
        {
            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(expected, result.Error);
            Assert.AreEqual(0m, result.Value);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result.ErrorMessage));
        }
    }
}
