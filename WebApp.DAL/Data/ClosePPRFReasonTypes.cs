using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class ClosePPRFReasonTypes
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }
        public ClosePPRFReasonTypes()
        {
            IsEnabled = true;
        }
        public ClosePPRFReasonTypes(string description) : this()
        {
            Description = description;
        }
    }
}
