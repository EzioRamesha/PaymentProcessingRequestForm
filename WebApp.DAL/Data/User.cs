using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        public Guid? DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public virtual Department Department { get; set; }

        public string Designation { get; set; }


        public string ExternalUserId { get; set; }
        [ForeignKey("ExternalUserId")]
        public virtual ApplicationUser ExternalUser { get; set; }


        public DateTime CreatedOn { get; set; }


        public virtual List<ApplicationUserGroup> UserGroups { get; set; }

        public virtual List<UserPermission> Permissions { get; set; }

        public bool IsEnabled { get; set; }



        public User()
        {
            IsEnabled = true;
            UserGroups = new List<ApplicationUserGroup>();
            Permissions = new List<UserPermission>();
        }
    }
}
