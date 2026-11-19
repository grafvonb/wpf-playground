using Calculator.App.ViewModels;
using Calculator.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        private FakeCalculatorFacade _fake;
        private MainViewModel _viewModel;

        [TestInitialize]
        public void Setup()
        {
            _fake = new FakeCalculatorFacade();
            _viewModel = new MainViewModel(_fake);
        }

        [DataTestMethod]
        [DataRow(CalculatorOperation.Add, "Add")]
        [DataRow(CalculatorOperation.Subtract, "Subtract")]
        [DataRow(CalculatorOperation.Multiply, "Multiply")]
        [DataRow(CalculatorOperation.Divide, "Divide")]
        public void Calculate_CallsMatchingFacadeMethodAndShowsResult(CalculatorOperation operation, string expectedCall)
        {
            _fake.NextResult = CalculationResult.Success(9m);
            _viewModel.FirstInput = "6";
            _viewModel.SecondInput = "3";

            _viewModel.CalculateCommand.Execute(operation);

            Assert.AreEqual(expectedCall, _fake.LastOperation);
            Assert.AreEqual(6m, _fake.LastLeft);
            Assert.AreEqual(3m, _fake.LastRight);
            Assert.AreEqual("9", _viewModel.ResultText);
            Assert.AreEqual(string.Empty, _viewModel.ErrorText);
        }

        [TestMethod]
        public void Calculate_AcceptsNegativeAndDecimalOperands()
        {
            _viewModel.FirstInput = "-1.5";
            _viewModel.SecondInput = "2.25";

            _viewModel.CalculateCommand.Execute(CalculatorOperation.Add);

            Assert.AreEqual(-1.5m, _fake.LastLeft);
            Assert.AreEqual(2.25m, _fake.LastRight);
        }

        [TestMethod]
        public void Calculate_SecondCalculation_ReplacesFirstResult()
        {
            _viewModel.FirstInput = "6";
            _viewModel.SecondInput = "3";
            _fake.NextResult = CalculationResult.Success(9m);
            _viewModel.CalculateCommand.Execute(CalculatorOperation.Add);

            _fake.NextResult = CalculationResult.Success(18m);
            _viewModel.CalculateCommand.Execute(CalculatorOperation.Multiply);

            Assert.AreEqual("18", _viewModel.ResultText);
        }

        [DataTestMethod]
        [DataRow("", "3")]
        [DataRow("abc", "3")]
        [DataRow("6", "")]
        [DataRow("6", "1,2,3")]
        public void Calculate_InvalidInput_SetsErrorAndSkipsFacade(string first, string second)
        {
            _viewModel.FirstInput = first;
            _viewModel.SecondInput = second;

            _viewModel.CalculateCommand.Execute(CalculatorOperation.Add);

            Assert.AreEqual(0, _fake.CallCount);
            Assert.AreEqual("Enter a valid number", _viewModel.ErrorText);
            Assert.AreEqual(string.Empty, _viewModel.ResultText);
        }

        [TestMethod]
        public void Calculate_FacadeFailure_SetsErrorAndClearsResult()
        {
            _viewModel.FirstInput = "6";
            _viewModel.SecondInput = "3";
            _fake.NextResult = CalculationResult.Success(9m);
            _viewModel.CalculateCommand.Execute(CalculatorOperation.Add);

            _fake.NextResult = CalculationResult.Failure(CalculationError.DivisionByZero, "Cannot divide by zero");
            _viewModel.CalculateCommand.Execute(CalculatorOperation.Divide);

            Assert.AreEqual("Cannot divide by zero", _viewModel.ErrorText);
            Assert.AreEqual(string.Empty, _viewModel.ResultText);
        }

        [TestMethod]
        public void Calculate_AfterError_ValidCalculationClearsError()
        {
            _viewModel.FirstInput = "abc";
            _viewModel.SecondInput = "3";
            _viewModel.CalculateCommand.Execute(CalculatorOperation.Add);

            _viewModel.FirstInput = "6";
            _fake.NextResult = CalculationResult.Success(9m);
            _viewModel.CalculateCommand.Execute(CalculatorOperation.Add);

            Assert.AreEqual(string.Empty, _viewModel.ErrorText);
            Assert.AreEqual("9", _viewModel.ResultText);
        }
    }
}
