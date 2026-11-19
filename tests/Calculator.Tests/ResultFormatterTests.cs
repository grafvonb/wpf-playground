using Calculator.App.Formatting;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class ResultFormatterTests
    {
        [TestMethod]
        public void Format_WholeNumber_HasNoDecimalPoint()
        {
            Assert.AreEqual("2", ResultFormatter.Format(6m / 3m));
            Assert.AreEqual("18", ResultFormatter.Format(18.000m));
        }

        [TestMethod]
        public void Format_RoundsToTenDecimals()
        {
            Assert.AreEqual("0.3333333333", ResultFormatter.Format(1m / 3m));
            Assert.AreEqual("0.6666666667", ResultFormatter.Format(2m / 3m));
        }

        [TestMethod]
        public void Format_DropsTrailingZeros()
        {
            Assert.AreEqual("0.5", ResultFormatter.Format(0.5000m));
            Assert.AreEqual("-3.375", ResultFormatter.Format(-3.37500m));
        }

        [TestMethod]
        public void Format_Zero_ReturnsZero()
        {
            Assert.AreEqual("0", ResultFormatter.Format(0.000m));
            Assert.AreEqual("0", ResultFormatter.Format(0.00000000001m));
        }
    }
}
