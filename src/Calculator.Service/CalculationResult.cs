namespace Calculator.Service
{
    public sealed class CalculationResult
    {
        private CalculationResult(bool isSuccess, decimal value, CalculationError error, string errorMessage)
        {
            IsSuccess = isSuccess;
            Value = value;
            Error = error;
            ErrorMessage = errorMessage;
        }

        public bool IsSuccess { get; }

        public decimal Value { get; }

        public CalculationError Error { get; }

        public string ErrorMessage { get; }

        public static CalculationResult Success(decimal value)
        {
            return new CalculationResult(true, value, CalculationError.None, string.Empty);
        }

        public static CalculationResult Failure(CalculationError error, string message)
        {
            return new CalculationResult(false, 0m, error, message ?? string.Empty);
        }
    }
}
