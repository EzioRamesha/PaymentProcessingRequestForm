using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class Department
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string PayingEntitiesId { get; set; }
        public string PayingEntities { get; set; }
        public virtual string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }
        public bool IsEnabled { get; set; }
        public List<User> Employees { get; set; }


        public Department()
        {
            IsEnabled = true;
        }

        public Department(string name, string code) : this()
        {
            this.Name = name;
            this.Code = code;
        }
    }
}
