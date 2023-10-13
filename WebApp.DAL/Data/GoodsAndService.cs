using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Custom;

namespace WebApp.DAL.Data
{
    public class GoodsAndService
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }


        public virtual Guid PaymentRequestFormId { get; set; }
        [ForeignKey("PaymentRequestFormId")]
        public virtual PaymentRequestForm PaymentRequestForm { get; set; }



        public string Description { get; set; }


        public Guid? TaxTypeId { get; set; }
        [ForeignKey("TaxTypeId")]
        public virtual TaxType TaxType { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal TaxAmount { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal TaxAmountUSD { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal TaxAmountEuro { get; set; }



        public int Quantity { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal Amount { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal AmountUSD { get; set; }
        [DecimalPrecision(18, 6)]
        public decimal AmountEuro { get; set; }


        public GoodsAndService()
        {
            Quantity = 0;
            Amount = 0m;
            AmountUSD = 0m;
            AmountEuro = 0m;
        }
    }
}
