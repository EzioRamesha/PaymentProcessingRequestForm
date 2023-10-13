using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;

namespace WebApp.DAL.Models
{
    public class PayeeModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Fax { get; set; }
        public bool IsEnabled { get; set; }
        public string CountryName { get; set; }
        public string HotelName { get; set; }

        public Guid? CountryId { get; set; }

        internal static PayeeModel ToModel(Data.Payee entity)
        {
            if (entity == null)
                return null;
            var _payee = new PayeeModel();
            _payee.Id = entity.Id.ToString();
            _payee.Name = entity.Name;
            _payee.AddressLine1 = entity.AddressLine1;
            _payee.AddressLine2 = entity.AddressLine2;
            _payee.AddressLine3 = entity.AddressLine3;
            _payee.CountryId = entity.CountryId;
            _payee.Fax = entity.Fax;
            _payee.HotelName = entity.HotelName;
            _payee.Phone = entity.Phone;

            return _payee;
        }
    }

    public class NewPayeeViewModel
    {
        public string Name { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Fax { get; set; }
        public string CountryId { get; set; }
        public string HotelName { get; set; }
    }

    public class UpdatePayeeViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Fax { get; set; }
        public string CountryId { get; set; }
        public string HotelName { get; set; }
    }
    public class PayeeBankDetails
    {
        public string Id { get; set; }
        public string BankName { get; set; }
        public string BankAddress { get; set; }
        public string AccountName { get; set; }
        public string AccountNumber { get; set; }
        public string IFSC { get; set; }
        public string IBAN { get; set; }
        public string Swift { get; set; }
        public string AccountType { get; set; }
    }

    public class PayeeDetailsViewModel : PayeeModel
    {
        public List<PayeeBankDetails> AccountDetails { get; set; }
    }

    public class PayeeBankViewModel : PayeeModel
    {
        public PayeeBankDetails AccountDetails { get; set; }
    }


    public class ManagePayeeViewModel
    {
        public List<PayeeModel> Payees { get; set; }
        public Paging Pager { get; set; }

        public ManagePayeeViewModel()
        {
            Payees = new List<PayeeModel>();
        }
    }

}
