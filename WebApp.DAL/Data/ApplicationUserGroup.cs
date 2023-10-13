using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class ApplicationUserGroup
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User AppUser { get; set; }


        public Guid ApplicationGroupId { get; set; }
        [ForeignKey("ApplicationGroupId")]
        public virtual ApplicationGroup ApplicationGroup { get; set; }

        public bool IsSelected { get; set; }
    }
}
