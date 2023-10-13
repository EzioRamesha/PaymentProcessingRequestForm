using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class PayeeBankAccountDetail
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }

        [ForeignKey("PayeeId")]
        public virtual Payee Payee { get; set; }
        public virtual Guid PayeeId { get; set; }


        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string IBAN { get; set; }
        public string Swift { get; set; }
        public string IFSC { get; set; }



        public Guid? PreviousDetailsId { get; set; }
        [ForeignKey("PreviousDetailsId")]
        public PayeeBankAccountDetail PreviousDetails { get; set; }


        public bool IsEnabled { get; set; }


        public PayeeBankAccountDetail()
        {
            IsEnabled = true;
        }
        public PayeeBankAccountDetail(string bankName, string accountName, string accountNumber, string accountType, string iban, string swift, string ifsc) : this()
        {
            BankName = bankName;
            AccountName = accountName;
            AccountNumber = accountNumber;
            AccountType = accountType;
            IBAN = iban;
            Swift = swift;
            IFSC = ifsc;
        }
    }
}
