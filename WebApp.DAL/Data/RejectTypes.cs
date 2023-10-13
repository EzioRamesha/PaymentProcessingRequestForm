using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.DAL.Data
{
    public class RejectTypes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }      
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public RejectTypes()
        {
            IsEnabled = true;
        }
        public RejectTypes(string description) : this()
        {
            Description = description;
        }
    }
  
}
