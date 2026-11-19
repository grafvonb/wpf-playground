namespace Calculator.Service.Internal
{
    internal static class Arithmetic
    {
        public static decimal Add(decimal left, decimal right)
        {
            return left + right;
        }

        public static decimal Subtract(decimal left, decimal right)
        {
            return left - right;
        }

        public static decimal Multiply(decimal left, decimal right)
        {
            return left * right;
        }

        public static decimal Divide(decimal dividend, decimal divisor)
        {
            return dividend / divisor;
        }
    }
}
