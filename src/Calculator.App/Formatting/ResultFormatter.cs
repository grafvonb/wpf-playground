using System;
using System.Globalization;

namespace Calculator.App.Formatting
{
    public static class ResultFormatter
    {
        public static string Format(decimal value)
        {
            var rounded = Math.Round(value, 10, MidpointRounding.AwayFromZero);
            // Dividing by 1.000...0 normalizes the scale, dropping trailing zeros.
            var normalized = rounded / 1.0000000000000000000000000000m;
            return normalized.ToString(CultureInfo.InvariantCulture);
        }
    }
}
