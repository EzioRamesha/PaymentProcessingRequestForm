using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class PaymentMethodDAL : DALBase
    {
        public IQueryable<PaymentMethod> List()
        {
            return _dbContext.PaymentMethods;
        }

        internal void ChangeActiveStatus(Guid paymentMethodId, bool status)
        {
            try
            {
                PaymentMethod existingPaymentMethod = List().Where(w => w.Id.Equals(paymentMethodId)).FirstOrDefault();
                if (existingPaymentMethod != null)
                {
                    existingPaymentMethod.IsEnabled = status;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal Guid Save(Models.PaymentMethod entity)
        {
            Guid returnValue = Guid.Empty;
            try
            {
                var paymentMethod = new PaymentMethod
                {
                    Name = entity.Name,
                    Description = entity.Description
                };
                _dbContext.PaymentMethods.Add(paymentMethod);
                _dbContext.SaveChanges();
                returnValue = paymentMethod.Id;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return returnValue;
        }

        internal Guid Update(Models.PaymentMethod paymentMethod)
        {
            Guid returnValue = Guid.Empty;
            try
            {
                var idToSearch = paymentMethod.Id.ToGuid();
                var existingEntity = _dbContext.PaymentMethods.Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
                if (existingEntity != null)
                {
                    existingEntity.Name = paymentMethod.Name;
                    existingEntity.Description = paymentMethod.Description;
                    _dbContext.SaveChanges();
                    returnValue = existingEntity.Id;
                }
            }
            catch (Exception ex)
            {                
                throw ex;
            }
            return returnValue;
        }
    }
}
