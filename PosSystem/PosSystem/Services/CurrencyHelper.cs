using System.Globalization;

namespace PosSystem.Services
{
    public static class CurrencyHelper
    {
        // Change "en-IN" to "en-US" if you want Dollar formatting ($)
        private static readonly CultureInfo _culture = new CultureInfo("en-IN");

        public static string Format(decimal amount)
        {
            return amount.ToString("C", _culture);
        }
    }
}