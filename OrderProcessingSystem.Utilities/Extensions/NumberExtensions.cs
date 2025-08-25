using System;
using System.Globalization;

namespace OrderProcessingSystem.Utilities.Extensions
{
    public static class NumberExtensions
    {
        /// <summary>
        /// Formats a double to two decimal places. When currency is true, uses the current culture currency format (C2), otherwise numeric (N2).
        /// </summary>
        public static string ToTwoDecimals(this double value, bool currency = false, CultureInfo? culture = null)
        {
            var ci = culture ?? CultureInfo.CurrentCulture;
            return value.ToString(currency ? "C2" : "N2", ci);
        }

        /// <summary>
        /// Formats a nullable double to two decimal places or returns an empty string when null.
        /// </summary>
        public static string ToTwoDecimals(this double? value, bool currency = false, CultureInfo? culture = null)
        {
            if (!value.HasValue) return string.Empty;
            return value.Value.ToTwoDecimals(currency, culture);
        }
        
        public static string ToTwoDecimals(this decimal value, bool currency = false, CultureInfo? culture = null)
        {
            var ci = culture ?? CultureInfo.CurrentCulture;
            return value.ToString(currency ? "C2" : "N2", ci);
        }

        public static string ToTwoDecimals(this decimal? value, bool currency = false, CultureInfo? culture = null)
        {
            if (!value.HasValue) return string.Empty;
            return value.Value.ToTwoDecimals(currency, culture);
        }
    }
}
