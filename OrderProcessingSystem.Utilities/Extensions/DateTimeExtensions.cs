using System;

namespace OrderProcessingSystem.Utilities.Extensions
{
    public static class DateTimeExtensions
    {
        public static string ToOrderFormat(this DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
        }
    }
}
