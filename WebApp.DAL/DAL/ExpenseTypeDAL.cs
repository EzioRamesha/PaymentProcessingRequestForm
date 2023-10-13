using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApp.DAL.Data;
using WebApp.DAL.Helpers;

namespace WebApp.DAL.DAL
{
    internal class ExpenseTypeDAL : DALBase
    {
        public IQueryable<ExpenseType> List()
        {
            return _dbContext.ExpenseTypes;
        }

        internal void ChangeActiveStatus(Guid guid, bool p)
        {
            try
            {
                ExpenseType existingExpenseType = List().Where(w => w.Id.Equals(guid)).FirstOrDefault();
                if (existingExpenseType != null)
                {
                    existingExpenseType.IsEnabled = p;
                }
                _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        internal Guid Save(Models.ExpenseType entity)
        {
            Guid returnValue = Guid.Empty;
            try
            {
                var expenseType = new ExpenseType
                {
                    Name = entity.Name,
                    Description = entity.Description
                };
                _dbContext.ExpenseTypes.Add(expenseType);
                _dbContext.SaveChanges();
                returnValue = expenseType.Id;
            }
            catch (Exception ex)
            {
                // Save log
            }
            return returnValue;
        }

        internal Guid Update(Models.ExpenseType entity)
        {
            Guid returnId = Guid.Empty;
            var idToSearch = entity.Id.ToGuid();
            var existingEntity = List().Where(w => w.Id.Equals(idToSearch)).FirstOrDefault();
            if (existingEntity != null)
            {
                existingEntity.Name = entity.Name;
                existingEntity.Description = entity.Description;
                _dbContext.SaveChanges();
                returnId = existingEntity.Id;
            }
            return returnId;
        }
    }
}
