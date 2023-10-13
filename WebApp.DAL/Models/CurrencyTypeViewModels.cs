using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebApp.DAL.Models
{
    public class CurrencyType
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal USDValue { get; set; }
        public decimal EuroValue { get; set; }
        public bool IsEnabled { get; set; }
    }

    public class NewCurrency
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal USDValue { get; set; }
        public decimal EuroValue { get; set; }
    }
    public class UpdateCurrencyViewModel
    {
        public string Id { get; set; }
        public string Code { get; set; }
        public decimal USDValue { get; set; }
        public decimal EuroValue { get; set; }
    }
}
