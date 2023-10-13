using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;

namespace WebApp.DAL.BAL
{
    public class BudgetOrderBAL
    {
        private static readonly BudgetOrderDAL _budgetOrderDAL = new BudgetOrderDAL();

        public List<BudgetOrder> GetBudgetOrderList(DateTime PPRFDate)
        {
            return _budgetOrderDAL.List().Where(w => w.DocumentType.Equals("BDG", StringComparison.OrdinalIgnoreCase) &&
                                                     w.Status.Equals("Approved", StringComparison.OrdinalIgnoreCase) &&
                                                     w.BudgetValidFrom <= PPRFDate && w.BudgetValidTo >= PPRFDate).Select(s => new BudgetOrder
            {
                BudgetPPRFId = s.Id.ToString(),
                BudgetPPRFNo = s.PPRFNo,
                //PayeeBankAccountDetail = s.PayeeBankAccountDetail,
                PayeeBankAccountDetailId = s.PayeeBankAccountDetailId.ToString(),
                PayeeName = s.PayeeBankAccountDetail.Payee.Name,
                Description = s.Description,
                RestrictedPayeeOnly = s.RestrictedPayeeOnly? "Y" : "N",
                BudgetApprovedAmt = s.GoodsAndServices.Sum(su => su.Amount) + s.GoodsAndServices.Sum(su => su.TaxAmount),
                BudgetApprovedAmtUSD = s.GoodsAndServices.Sum(su => su.AmountUSD) + s.GoodsAndServices.Sum(su => su.TaxAmountUSD),
                BudgetApprovedAmtEuro = s.GoodsAndServices.Sum(su => su.AmountEuro) + +s.GoodsAndServices.Sum(su => su.TaxAmountEuro)

            }).ToList();
        }

    }
}
