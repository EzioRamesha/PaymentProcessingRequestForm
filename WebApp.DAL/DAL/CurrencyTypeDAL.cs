using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;
using WebApp.DAL.Models;

namespace WebApp.DAL.DAL
{
    internal class CurrencyTypeDAL : DALBase
    {
        public IQueryable<Currency> List()
        {
            return _dbContext.Currencies;
        }

        internal void ChangeActiveStatus(CurrencyType currency, bool status)
        {
            try
            {
                var idToCompare = currency.Id.ToGuid();
                Currency existingCurrency = List().Where(w => w.Id.Equals(idToCompare)).FirstOrDefault();
                if (existingCurrency != null)
                {
                    existingCurrency.IsEnabled = status;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {

            }
        }

        public Guid Save(CurrencyType currency, string userEmail)
        {
            Guid returnVal = Guid.Empty;
            try
            {
                var user = _dbContext.AppUsers.Where(w => w.Email.Equals(userEmail)).FirstOrDefault();
                if (user != null)
                {
                    var newCurrency = new Currency
                    {
                        Code = currency.Code,
                        Description = currency.Description,
                        USDValue = currency.USDValue,
                        EuroValue = currency.EuroValue,
                        CreatedOn = GeneralHelper.CurrentDate(),
                        Name = currency.Name,
                        CreatedById = user.ExternalUserId,
                        PreviousCurrencyId = null
                    };
                    _dbContext.Currencies.Add(newCurrency);
                    _dbContext.SaveChanges();
                    returnVal = newCurrency.Id;
                }
            }
            catch(Exception ex)
            {

            }
            return returnVal;
        }

        public Guid Update(UpdateCurrencyViewModel currency)
        {
            var returnVal = Guid.Empty;
            try
            {
                var idToSearch = currency.Id.ToGuid();
                Currency existingCurrency = List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
                if (existingCurrency != null)
                {
                    existingCurrency.EuroValue = currency.EuroValue;
                    existingCurrency.USDValue = currency.USDValue;
                    existingCurrency.Code = currency.Code;

                    _dbContext.SaveChanges();
                    returnVal = existingCurrency.Id;
                }
            }
            catch (Exception ex)
            {
                
            }
            return returnVal;
        }
    }
}
