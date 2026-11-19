using System;
using System.Globalization;
using System.Windows.Input;
using Calculator.App.Formatting;
using Calculator.Service;

namespace Calculator.App.ViewModels
{
    public sealed class MainViewModel : ViewModelBase
    {
        private const string InvalidNumberMessage = "Enter a valid number";

        private readonly ICalculatorFacade _facade;
        private string _firstInput = string.Empty;
        private string _secondInput = string.Empty;
        private string _resultText = string.Empty;
        private string _errorText = string.Empty;

        public MainViewModel(ICalculatorFacade facade)
        {
            _facade = facade ?? throw new ArgumentNullException(nameof(facade));
            CalculateCommand = new RelayCommand(p => Calculate((CalculatorOperation)p));
        }

        public string FirstInput
        {
            get => _firstInput;
            set => SetProperty(ref _firstInput, value);
        }

        public string SecondInput
        {
            get => _secondInput;
            set => SetProperty(ref _secondInput, value);
        }

        public string ResultText
        {
            get => _resultText;
            private set => SetProperty(ref _resultText, value);
        }

        public string ErrorText
        {
            get => _errorText;
            private set => SetProperty(ref _errorText, value);
        }

        public ICommand CalculateCommand { get; }

        private void Calculate(CalculatorOperation operation)
        {
            ResultText = string.Empty;
            ErrorText = string.Empty;

            if (!TryParse(FirstInput, out var left) || !TryParse(SecondInput, out var right))
            {
                ErrorText = InvalidNumberMessage;
                return;
            }

            var result = Invoke(operation, left, right);
            if (result.IsSuccess)
            {
                ResultText = ResultFormatter.Format(result.Value);
            }
            else
            {
                ErrorText = result.ErrorMessage;
            }
        }

        private CalculationResult Invoke(CalculatorOperation operation, decimal left, decimal right)
        {
            switch (operation)
            {
                case CalculatorOperation.Add:
                    return _facade.Add(left, right);
                case CalculatorOperation.Subtract:
                    return _facade.Subtract(left, right);
                case CalculatorOperation.Multiply:
                    return _facade.Multiply(left, right);
                case CalculatorOperation.Divide:
                    return _facade.Divide(left, right);
                default:
                    throw new ArgumentOutOfRangeException(nameof(operation));
            }
        }

        private static bool TryParse(string text, out decimal value)
        {
            return decimal.TryParse(
                text,
                NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                CultureInfo.CurrentCulture,
                out value);
        }
    }
}
