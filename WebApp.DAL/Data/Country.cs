using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class Country
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool IsEnabled { get; set; }

        public Country()
        {
            IsEnabled = true;
        }

        public Country(string name, string code) : this()
        {
            Name = name;
            Code = code;
        }
    }
}
