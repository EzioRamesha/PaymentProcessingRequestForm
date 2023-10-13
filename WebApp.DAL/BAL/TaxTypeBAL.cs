using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;

namespace WebApp.DAL.BAL
{
    public class TaxTypeBAL
    {
        private static readonly TaxTypeDAL _taxTypeDAL = new TaxTypeDAL();
        public List<TaxType> GetActive()
        {
            return _taxTypeDAL.List().Where(w => w.IsEnabled).Select(s => new TaxType
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Description = s.Description,
                PercentageValue = s.PercentageValue,
                IsEnabled = s.IsEnabled
            }).ToList();
        }

        public TaxType FindById(Guid guid)
        {
            return _taxTypeDAL.List().Where(w => w.Id.Equals(guid)).Select(s => new TaxType
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Description = s.Description,
                PercentageValue = s.PercentageValue,
                IsEnabled = s.IsEnabled
            }).FirstOrDefault();
        }
    }
}
