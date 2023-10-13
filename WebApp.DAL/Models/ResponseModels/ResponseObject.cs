using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models.ResponseModels
{
    public class ResponseObject<T>
    {
        public string ResponseType { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
    }
}
