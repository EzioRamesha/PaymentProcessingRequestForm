using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Custom;

namespace WebApp.DAL.Data
{
    public class TaxType
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal PercentageValue { get; set; }



        //public virtual List<PaymentRequestForm> FormsByTax2 { get; }
        //public virtual List<PaymentRequestForm> FormsByTax3 { get; }


        public bool IsEnabled { get; set; }

        public TaxType()
        {
            IsEnabled = true;
        }
        public TaxType(string name, decimal percentageValue) : this()
        {
            this.Name = name;         
            this.PercentageValue = percentageValue;
        }
        public TaxType(string name, decimal percentageValue, Guid Id, string Description) : this()
        {
            this.Name = name;
            this.Id = Id;
            this.Description = Description;
            this.PercentageValue = percentageValue;
        }
    }
}
