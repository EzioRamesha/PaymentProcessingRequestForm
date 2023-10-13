using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Data
{
    public class Payee
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Fax { get; set; }
        public bool IsEnabled { get; set; }



        public Guid? PreviousDetailsId { get; set; }
        [ForeignKey("PreviousDetailsId")]
        public Payee PreviousDetails { get; set; }
        public virtual List<Payee> PreviousEdits { get; set; }



        public virtual List<PayeeBankAccountDetail> PayeeBankAccounts { get; set; }



        
        public Guid? CountryId { get; set; }
        [ForeignKey("CountryId")]        
        public virtual Country Country { get; set; }
                
        public string HotelName { get; set; }





        public Payee()
        {
            PayeeBankAccounts = new List<PayeeBankAccountDetail>();
            IsEnabled = true;
        }

        public Payee(string name): this()
        {
            Name = name;
        }
    }
}
