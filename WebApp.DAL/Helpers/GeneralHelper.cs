using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Helpers
{
    internal class GeneralHelper
    {
        public static DateTime CurrentDate()
        {
            return DateTime.Now;
        }

        public static string _getRequestNumber(string documentType, string payingEntityCode, int month, int year, int number)
        {
            var DocumentNo = documentType + "/"
                                    + payingEntityCode + "/"
                                    + (new DateTime(Convert.ToInt32(year), 01, 01)).ToString("yy") + string.Format("{0:00}", month) + "/"
                                    + string.Format("{0:000000}", number);
            return DocumentNo;
        }
    }


    public class TokenValidation
    {
        public bool Validated { get { return Errors.Count == 0; } }
        public readonly List<TokenValidationStatus> Errors = new List<TokenValidationStatus>();
    }

    public enum TokenValidationStatus
    {
        Expired,
        WrongUser,
        WrongPurpose,
        WrongGuid
    }
}
