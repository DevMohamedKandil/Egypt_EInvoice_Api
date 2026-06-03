using System;

namespace Egypt_EInvoice_Api.Extensions
{
    public static class NumericExtensions
    {
        // Safely convert object that can be double/float/decimal/null to decimal
        public static decimal SafeDoubleToDecimal(this object value)
        {
            if (value == null || value == DBNull.Value)
                return 0m;

            if (value is decimal d)
                return d;

            if (value is double dd)
                return Convert.ToDecimal(dd);

            if (value is float f)
                return Convert.ToDecimal(f);

            // fallback: try parse
            try
            {
                return Convert.ToDecimal(value);
            }
            catch
            {
                return 0m;
            }
        }

        // Null-safe conversion
        public static decimal SafeDoubleToDecimal(this double? value)
        {
            return value.HasValue ? Convert.ToDecimal(value.Value) : 0m;
        }

        public static decimal SafeDecimal(this decimal? value)
        {
            return value ?? 0m;
        }
    }
}
