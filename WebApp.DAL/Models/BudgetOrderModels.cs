using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;

namespace WebApp.DAL.Models
{
    public class BudgetOrder
    {
        public string BudgetPPRFId { get; set; }
        public string BudgetPPRFNo { get; set; }
        public PayeeBankAccountDetail PayeeBankAccountDetail { get; set; }
        public PayeeModel Payee { get; set; }
        public string PayeeBankAccountDetailId { get; set; }
        public string PayeeName { get; set; }
        public string Description { get; set; }
        public string RestrictedPayeeOnly { get; set; }
        public string BudgetApprovedAmtDesc { get; set; }
        public decimal BudgetApprovedAmt { get; set; }
        public decimal BudgetApprovedAmtUSD { get; set; }
        public decimal BudgetApprovedAmtEuro { get; set; }

    }
}
