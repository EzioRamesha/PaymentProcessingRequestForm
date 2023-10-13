using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models.ResponseModels
{
    public class ChangePasswordResponseModel
    {
        public List<string> Errors { get; set; }

        public ChangePasswordResponseModel()
        {
            Errors = new List<string>();
        }
    }
}
