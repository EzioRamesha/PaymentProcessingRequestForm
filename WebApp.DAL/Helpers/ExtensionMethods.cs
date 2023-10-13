using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Helpers
{
    internal static class ExtensionMethods
    {
        public static bool IsNullOrEmpty(this Guid guid)
        {
            return guid == null || guid == Guid.Empty;
        }

        public static Guid ToGuid(this string stringGuid)
        {
            Guid result = Guid.Empty;
            Guid.TryParse(stringGuid, out result);
            return result;
        }

        public static string ToCurrencyString(this decimal value)
        {
            return string.Format("{0:0.00}", value);
        }

        public static string ToCurrencyString(this decimal value, int precision)
        {
            string prec = new string('0', precision);
            return string.Format("{0:0." + prec + "}", value);
        }

        public static DateTime ToStartDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 0, 0, 0, 0);
        }
        public static DateTime ToEndDate(this DateTime value)
        {
            return new DateTime(value.Year, value.Month, value.Day, 23, 59, 59, 999);
        }
    }
}
