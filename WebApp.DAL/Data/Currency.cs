using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Custom;

namespace WebApp.DAL.Data
{
    public class Currency
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal USDValue { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal EuroValue { get; set; }
        public DateTime CreatedOn { get; set; }

        public virtual string CreatedById { get; set; }
        [ForeignKey("CreatedById")]
        public virtual ApplicationUser CreatedBy { get; set; }

        public Guid? PreviousCurrencyId { get; set; }
        [ForeignKey("PreviousCurrencyId")]
        public virtual Currency PreviousCurrency { get; set; }

        public bool IsEnabled { get; set; }

        public Currency()
        {
            IsEnabled = true;
        }

        public Currency(string name, string code, decimal usdValue, decimal euroValue) : this()
        {
            Name = name;
            Code = code;
            USDValue = usdValue;
            EuroValue = euroValue;
        }
    }
}
