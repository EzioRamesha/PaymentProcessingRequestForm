using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Exceptions
{
    public class UnableToParseSaveRequestException : Exception
    {
        public UnableToParseSaveRequestException(string message) : base(message)
        {
           
        }
        public override string ToString()
        {
            return base.ToString();
        }
    }
}
