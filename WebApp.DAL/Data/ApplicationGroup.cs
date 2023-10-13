using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class ApplicationGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsEnabled { get; set; }

        public virtual List<ApplicationUserGroup> Users { get; set; }
        
        public ApplicationGroup()
        {
            IsEnabled = true;
        }
        public ApplicationGroup(string name) : this()
        {
            Name = name;
        }
        public ApplicationGroup(string name, bool isEnabled) : this(name)
        {
            IsEnabled = isEnabled;
        }
    }
}
