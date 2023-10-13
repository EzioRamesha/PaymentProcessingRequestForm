using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.DAL;
using WebApp.DAL.Models;

namespace WebApp.DAL.BAL
{
    public class CountryBAL
    {
        private static readonly CountryDAL _countryDAL = new CountryDAL();
        public List<Country> GetActive()
        {
            return _countryDAL.List().Where(w => w.IsEnabled).Select(s => new Country
            {
                Id = s.Id.ToString(),
                Name = s.Name,
                Code = s.Code,
                IsEnabled = s.IsEnabled
            }).ToList();
        }

        public List<Country> GetAll()
        {
            return _countryDAL.List()
                                .OrderBy(o => o.Name)
                                .Select(s => new Country
                                {
                                    Id = s.Id.ToString(),
                                    Name = s.Name,
                                    Code = s.Code,
                                    IsEnabled = s.IsEnabled
                                }).ToList();
        }

        public bool Enable(Country country)
        {
            var success = false;
            try
            {
                _countryDAL.ChangeActiveStatus(country, true);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
                throw ex;
            }
            return success;
        }

        public bool Disable(Country country)
        {
            var success = false;
            try
            {
                _countryDAL.ChangeActiveStatus(country, false);
                success = true;
            }
            catch (Exception ex)
            {
                success = false;
            }
            return success;
        }
    }
}
