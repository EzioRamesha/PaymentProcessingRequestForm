using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace WebApp.DAL.Helpers
{
    internal static class Constants
    {
        public static string PAYING_ENTITY_LOGO_BASE_PATH = Path.Combine(HttpContext.Current.Request.PhysicalApplicationPath, "Logos", "PayingEntityLogos");

        public static string SUCCESS = "success";
        public static string ERROR = "error";
        public static string DATEFORMAT = "dd/MM/yyyy";
    }
}
