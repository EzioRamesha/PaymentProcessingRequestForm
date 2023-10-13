using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class Country
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsEnabled { get; set; }
    }
}
