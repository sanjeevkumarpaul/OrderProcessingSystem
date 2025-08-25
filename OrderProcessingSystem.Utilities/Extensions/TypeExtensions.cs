using System;

namespace OrderProcessingSystem.Utilities.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsNumericType(this Type t)
        {
            if (t == null) return false;
            var tc = Type.GetTypeCode(t);
            return tc == TypeCode.Decimal || tc == TypeCode.Double || tc == TypeCode.Single ||
                   tc == TypeCode.Int16 || tc == TypeCode.Int32 || tc == TypeCode.Int64 ||
                   tc == TypeCode.UInt16 || tc == TypeCode.UInt32 || tc == TypeCode.UInt64 ||
                   tc == TypeCode.Byte || tc == TypeCode.SByte;
        }
    }
}
