using System;
using Calculator.App;
using Calculator.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class CalculatorFacadeFactoryTests
    {
        [TestMethod]
        public void MissingOrInProcess_ReturnsInProcessFacade()
        {
            Assert.IsInstanceOfType(CalculatorFacadeFactory.Create(null, null), typeof(CalculatorFacade));
            Assert.IsInstanceOfType(CalculatorFacadeFactory.Create("inprocess", null), typeof(CalculatorFacade));
        }

        [TestMethod]
        public void Sql_ReturnsSqlFacade()
        {
            Assert.IsInstanceOfType(
                CalculatorFacadeFactory.Create("Sql", "Server=.;Database=x"), typeof(SqlCalculatorFacade));
        }

        [TestMethod]
        public void UnknownBackendOrMissingConnectionString_Throws()
        {
            Assert.ThrowsException<InvalidOperationException>(() => CalculatorFacadeFactory.Create("Oracle", null));
            Assert.ThrowsException<InvalidOperationException>(() => CalculatorFacadeFactory.Create("Sql", " "));
        }
    }
}
