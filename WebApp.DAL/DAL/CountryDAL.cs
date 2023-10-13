using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class CountryDAL : DALBase
    {
        public IQueryable<Country> List()
        {
            return _dbContext.Countries;
        }

        internal void ChangeActiveStatus(Models.Country country, bool status)
        {
            try
            {
                var idToCompare = country.Id.ToGuid();
                Country existingCountry = List().Where(w => w.Id.Equals(idToCompare)).FirstOrDefault();
                if (existingCountry != null)
                {
                    existingCountry.IsEnabled = status;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

    }
}
