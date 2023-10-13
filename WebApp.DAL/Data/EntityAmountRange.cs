using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class EntityAmountRange
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public Guid PayingEntityId { get; set; }
        [ForeignKey("PayingEntityId")]
        public PayingEntity PayingEntity { get; set; }

        public decimal AmountRangeFrom { get; set; }
        public decimal AmountRangeTo { get; set; }

        //public virtual List<string> EmailAddresses { get; set; }
        public virtual List<AmountRangeEmail> EmailAddresses { get; set; }
    }

    public class AmountRangeEmail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        
        public Guid EntityAmountRangeId { get; set; }
        [ForeignKey("EntityAmountRangeId")]
        public virtual EntityAmountRange EntityAmountRange { get; set; }

        public string Email { get; set; }
        public bool IsDeleted { get; internal set; }
    }
}
